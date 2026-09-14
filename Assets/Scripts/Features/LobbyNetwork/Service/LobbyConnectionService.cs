using Assets.Scripts.Features.LobbyNetwork.Data.Signals;
using Assets.Scripts.Public.Data.Model;
using Assets.Scripts.Public.Data.Signals;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Features.LobbyNetwork.Service
{
    public class LobbyConnectionService : IDisposable,IInitializable
    {
        private readonly LobbyStateManager _stateManager;
        private readonly LobbyMonitor _lobbyMonitor;
        private readonly SignalBus _signalBus;

        private bool _isAuthComlete = false;

        private CancellationTokenSource _heartBeatCTS;

        public const string RelayKey = "RelayJoinKey";

        public const string GameModeKey = "GameMode";

        public LobbyConnectionService(LobbyStateManager stateManager, LobbyMonitor lobbyMonitor,
            SignalBus signalBus)
        {
            _lobbyMonitor = lobbyMonitor;
            _stateManager = stateManager;
            _signalBus = signalBus;
        }
        public async UniTask<string> CreateLobbyAsync(string lobbyName, GameMode gameMode, string relayJoinCode)
        {
            StopHeartbeat();

            try
            {
                var lobbyOptions = new CreateLobbyOptions
                {
                    IsPrivate = false,
                    Player= GetLocalPlayerForLobby(),
                    Data = new Dictionary<string, DataObject>
                    {
                        {RelayKey, new( DataObject.VisibilityOptions.Member, relayJoinCode) },
                        {GameModeKey, new (DataObject.VisibilityOptions.Public,gameMode.Name) }
                    }
                };

                var lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, gameMode.MaxPlayer, lobbyOptions);

                _stateManager.SetLobby(lobby);

                _lobbyMonitor.SubscribeToCurrentLobby();

                StartHeartbeatLoop(lobby.Id);

                _signalBus.Fire<LobbyJoinedSignal>();

                return lobby.LobbyCode;
            }

            catch(Exception ex)
            {
                Debug.LogError($"[LobbyConnection] Failed to create lobby: {ex.Message}");
                throw;
            }
        }

        public async UniTask<Lobby> JoinLobbyByCodeAsync(string lobbyCode)
        {
            try
            {
                var options = new JoinLobbyByCodeOptions
                {
                    Player = GetLocalPlayerForLobby()
                };

                var lobby =  await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, options);

                _stateManager.SetLobby(lobby);
                _lobbyMonitor.SubscribeToCurrentLobby();
                

                return lobby;
            }
            catch(Exception ex)
            {
                Debug.LogError($"[LobbyConnection] Failed to join lobby by code: {ex.Message}");
                throw;
            }
        }
        public async UniTask<Lobby> JoinLobbyByIdAsync(string id)
        {
            try
            {
                var options = new JoinLobbyByIdOptions
                {
                    Player = GetLocalPlayerForLobby()
                };

                var lobby = await LobbyService.Instance.JoinLobbyByIdAsync(id, options);

                _stateManager.SetLobby(lobby);
                _lobbyMonitor.SubscribeToCurrentLobby();
                
                return lobby;
            }
            catch(Exception ex )
            {
                Debug.LogError($"[LobbyConnection] Failed to join lobby by ID: {ex.Message}");
                throw;
            }
        }
        public async UniTask<List<Lobby>> QueryLobbiesListAsync()
        {
            if(!_isAuthComlete)
            {
                Debug.LogWarning("[Lobby] Запрос заблокирован: UGS еще не авторизован.");
                return new List<Lobby>();
            }

            try
            {
                var options = new QueryLobbiesOptions
                {
                    Count = 25,
                    Filters = new()
                    {
                        new(QueryFilter.FieldOptions.AvailableSlots,"0",QueryFilter.OpOptions.GT)
                    }
                };

                var response =  await LobbyService.Instance.QueryLobbiesAsync(options);

                return response.Results;
            }
            catch(Exception ex)
            {
                Debug.LogError($"[LobbyConnection] Failed to query lobbies: {ex.Message}");
                return new List<Lobby>();
            }
        }
        public async UniTask UpdateGameMode(GameMode gameMode)
        {
            var currentLobby = _stateManager.CurrentLobby;
            if (currentLobby == null || currentLobby.HostId != _stateManager.LocalPlayerId) return;

            try
            {
                var options = new UpdateLobbyOptions
                {
                    MaxPlayers = gameMode.MaxPlayer,

                    Data = new()
                {
                    {GameModeKey,new(DataObject.VisibilityOptions.Public, gameMode.Name) }
                }
                };

                await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, options);
            }
            catch(Exception ex)
            {
                Debug.LogError($"[LobbyConnection] Failed to update game mode: {ex.Message}");
            }
        }
        

        public void Dispose()
        {
            StopHeartbeat();
            _signalBus.Unsubscribe<ServiceInitializeSignal>(OnServiceReady);
        }
        private void StopHeartbeat()
        {
            _heartBeatCTS?.Cancel();
            _heartBeatCTS?.Dispose();
            _heartBeatCTS = null;
        }
        private void StartHeartbeatLoop(string lobbyId)
        {
            _heartBeatCTS = new CancellationTokenSource();
            HeartbeatLoopAsync(lobbyId, _heartBeatCTS.Token).Forget();
        }

        private async UniTaskVoid HeartbeatLoopAsync(string lobbyId, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(15), cancellationToken: token);
            }
        }

        public void Initialize()
        {
            _signalBus.Subscribe<ServiceInitializeSignal>(OnServiceReady);
        }

        private void OnServiceReady()
        {
            _isAuthComlete = true;

            QueryLobbiesListAsync().Forget();
        }
        private Player GetLocalPlayerForLobby()
        {
            return new Player
            {
                
                Data = new Dictionary<string, PlayerDataObject>
                {
                    
                    { "DisplayName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, _stateManager.LocalPlayerName) },
                    { "CharacterId", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "Medium") },
                    { "IsReady", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "false") }
                }
            };
        }
    }
}
