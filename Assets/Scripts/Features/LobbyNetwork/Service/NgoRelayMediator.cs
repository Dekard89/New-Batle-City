using Assets.Scripts.Public.Data.Model;
using Cysharp.Threading.Tasks;
using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Assets.Scripts.Features.LobbyNetwork.Service;

namespace Assets.Scripts.Features.LobbyNetworking.Service
{
    public class NgoRelayMediator
    {
        private readonly LobbyConnectionService _connectionService;

        private readonly LobbyStateManager _stateManager;

        public NgoRelayMediator(LobbyConnectionService lobbyConnectionService, LobbyStateManager stateManager)
        {
            _connectionService = lobbyConnectionService;
            _stateManager = stateManager;
        }
        public async UniTask<string> HostGameAsync(string lobbyName, GameMode gameMode)
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(gameMode.MaxPlayer);

                string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                SetupTransport(AllocationUtils.ToRelayServerData(allocation, "dtls"));

                NetworkManager.Singleton.StartHost();

                var lobbyCode = await _connectionService.CreateLobbyAsync(lobbyName, gameMode,relayJoinCode);

                return lobbyCode;
            }
            catch(Exception ex) 
            {
                Debug.LogError($"[NgoRelayMediator] Hosting failed: {ex.Message}");
                
                if (NetworkManager.Singleton.IsHost) NetworkManager.Singleton.Shutdown();
                throw;
            }
        }
        public async UniTask JoinGameByIDAsync(string lobbyID)
        {
            try
            {
                var lobby = await _connectionService.JoinLobbyByIdAsync(lobbyID);

                await ConnectRelayAndStartClient(lobby);
            }
            catch(Exception ex)
            {
                Debug.LogError($"[NgoRelayMediator] Joining by code failed: {ex.Message}");
                throw;
            }
        }
        private void SetupTransport(RelayServerData serverData)
        {
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            if(transport == null)
            {
                throw new NullReferenceException("UnityTransport component missing from NetworkManager GameObject.");
            }
            transport.SetRelayServerData(serverData);
        }
        private async UniTask ConnectRelayAndStartClient(Lobby lobby)
        {
            if(!lobby.Data.TryGetValue(LobbyConnectionService.RelayKey,out var relayData))
                throw new Exception("Lobby data does not contain a Relay Join Code.");

            string relayJoinCode = relayData.Value;

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);

            SetupTransport(AllocationUtils.ToRelayServerData(joinAllocation, "dtls"));

            NetworkManager.Singleton.StartClient();
        }
        public async UniTask StartMatchAsync()
        {
            var currentLobby = _stateManager.CurrentLobby;


            if (currentLobby == null || currentLobby.HostId != _stateManager.LocalPlayerId)
            {
                Debug.LogWarning("[NgoRelayMediator] Only the host can start the match.");
                return;
            }

            try
            {
                Debug.Log("[NgoRelayMediator] Starting match... Locking lobby.");


                var updateOptions = new Unity.Services.Lobbies.UpdateLobbyOptions
                {
                    IsPrivate = true,
                    IsLocked = true
                };

                await Unity.Services.Lobbies.LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, updateOptions);


                if (NetworkManager.Singleton.IsServer)
                {
                    NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[NgoRelayMediator] Failed to start match: {e.Message}");
            }
        }

    }

}
