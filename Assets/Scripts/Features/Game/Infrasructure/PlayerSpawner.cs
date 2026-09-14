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
using UnityEngine.TextCore.Text;
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
    
    public async UniTask SpawnPlayer(ulong playerId, string characterId, string playerName, int teamId)
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

        NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;

        Debug.Log($"[Spawner] запущен");

        if (NetworkManager.Singleton.ConnectedClientsList.Count==1)
        {
            Debug.Log("[PlayerSpawner] В сети обнаружен только Хост. Запускаем мгновенный спавн.");
            ExecuteSpawnProcess().Forget();
        }
    }
    private async UniTaskVoid ExecuteSpawnProcess()
    {
        var currentLobby = _stateManager.CurrentLobby;

        if(currentLobby == null)
        {
            Debug.LogError("[PlayerSpawner] Критическая ошибка: Данные лобби в LobbyStateManager отсутствуют!");
            return;
        }
        string gameModeName = "Unknown";
        if (currentLobby.Data.TryGetValue(LobbyConnectionService.GameModeKey, out var modeObject))
        {
            gameModeName = modeObject.Value;
        }
        var gameMode = _gameModeDB.GetByName(gameModeName);
        var clientList = NetworkManager.Singleton.ConnectedClientsList;
        var balancedTeam = _balancer.BalanceTeam(clientList, gameMode);

        foreach ( var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            ulong clientId = client.ClientId;

            var (characterID, playerName) = GetPlayerDataFromLobby(clientId, currentLobby);

            var assignedTeam = balancedTeam[clientId];

            await SpawnPlayer(clientId, characterID, playerName, assignedTeam);
        }
    }

    private void OnSceneEvent(SceneEvent sceneEvent)
    {
        Debug.Log($"[PlayerSpawner] Получено сетевое событие сцены: {sceneEvent.SceneEventType}");

        if (sceneEvent.SceneEventType== SceneEventType.LoadEventCompleted)
        {

            Debug.Log($"[PlayerSpawner] Условие спавна выполнено по событию {sceneEvent.SceneEventType}. Запускаем ExecuteSpawnProcess.");
            ExecuteSpawnProcess().Forget();
        }
    }

    
    private (string characterId, string playerName) GetPlayerDataFromLobby(ulong clientId, Lobby lobby)
    {
        int index = (int)clientId;

        if(index>=0 &&  index < lobby.Players.Count)
        {
            var player = lobby.Players[index];
            string charId = "Medium";
            string displayName = "Player";
            if (player.Data != null )
            {
                if (player.Data.TryGetValue("CharacterId", out var charData)) charId = charData.Value;
                if (player.Data.TryGetValue("DisplayName", out var nameData)) displayName = nameData.Value;
            } 

            return (charId, displayName);
        }
        Debug.LogWarning($"[PlayerSpawner] Не удалось найти данные лобби для clientId {clientId}. Применен дефолтный персонаж.");
        return ("Medium", $"Player_{clientId}");
    }
}
