using Assets.Scripts.Abstraction;
using Assets.Scripts.Features.LobbyNetwork.Service;
using Assets.Scripts.Public.Data.Model;
using Assets.Scripts.Public.DataBase;
using Assets.Scripts.Public.Infrasructure;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class PlayerSpawner : IInitializable
{
    private  DiContainer _container;
    private ICameraTarget _cameraTarget;
    private IPrefabLoad _loader;
    private CharacterDataBase _characterDB;
    private IBalancer _balancer;
    private LobbyStateManager _stateManager;
    private GameModeDataBase _gameModeDB;

    
    [Inject]
    public void Construct(DiContainer container, ICameraTarget cameraTarget,
        IPrefabLoad prefabLoader, CharacterDataBase characterDataBase, LobbyStateManager lobbyStateManager,
        IBalancer balancer, GameModeDataBase gameModeDataBase)
    {
         _container = container;
        _cameraTarget = cameraTarget;
        _characterDB = characterDataBase;
        _gameModeDB = gameModeDataBase;
        _loader = prefabLoader;
        _stateManager=lobbyStateManager;
        _balancer = balancer;
    }
    
    public async UniTask SpawnPlayer(ulong playerId, string characterId, int teamId)
    {
        Debug.Log($"[Spawner_Step 1] Начало спавна для игрока {playerId}, ID танка: {characterId}");

        var characterData = _characterDB.GetById(characterId);

        Debug.Log( $"[Spawner] спавним персонжа {characterData.Name}");

        if (characterData == null)
        {
            Debug.LogError($"[Spawner] Персонаж с ID {characterId} не найден");
        }
        Debug.Log($"[Spawner_Step 2] Найдена запись в БД: {characterData.Name}. Запрашиваем префаб...");

        GameObject prefab = await _loader.GetPrefab(characterData.referenceGameObject);

        if (prefab == null) return;

        GameObject tankObj = UnityEngine.GameObject.Instantiate(prefab, Vector2.zero, Quaternion.identity, null);

        _container.InjectGameObject(tankObj);

        if(tankObj.TryGetComponent<HealthController>(out var health))
        {
            health.TeamId.Value = teamId;
        }

        var networkObj = tankObj.GetComponent<NetworkObject>();

        networkObj.SpawnAsPlayerObject(playerId, true);

        if (NetworkManager.Singleton.LocalClientId == playerId)
        {
            _container.Resolve<LocalSessionModel>().LocalTeamId = teamId;

            await _cameraTarget.SetCameraTargetAsync(networkObj);
        }
    }


    public void Initialize()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += HandleSceneLoadComplete;
    }

    private void HandleSceneLoadComplete(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        var currentLobby= _stateManager.CurrentLobby;

        if (currentLobby == null)
        {
            Debug.LogError("[PlayerSpawner] Критическая ошибка: Данные лобби в LobbyStateManager отсутствуют!");
            return;
        }
        var gameModeName = "Unknown";

        if (currentLobby.Data.TryGetValue(LobbyConnectionService.GameModeKey, out var modeObject))
        {
            gameModeName = modeObject.Value;
        }
        var gameMode = _gameModeDB.GetByName(gameModeName);

        var clientList = NetworkManager.Singleton.ConnectedClientsList;

        var balancedTeam = _balancer.BalanceTeam(clientList, gameMode);

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            ulong clientId =client.ClientId;

            var characterId = GetGharacterIdFromLobby(clientId, currentLobby);

            int assignedTeam = balancedTeam[clientId];

            SpawnPlayer(clientId, characterId, assignedTeam).Forget();
        }
        
    }
    private string GetGharacterIdFromLobby(ulong clientId, Lobby lobby)
    {
        int index = (int)clientId;

        if(index < lobby.Players.Count)
        {
            var player = lobby.Players[index];
            if(player.Data != null && player.Data.TryGetValue("CharacterName", out var charName))
            {
                return charName.Value;
            }
        }
        Debug.LogWarning($"[PlayerSpawner] Не удалось найти данные лобби для clientId {clientId}. Применен дефолтный персонаж.");
        return "Medium";
    }
}
