using Assets.Scripts.Public.Data.Model;
using Cysharp.Threading.Tasks;
using System;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Assets.Scripts.Features.LobbyNetwork.Service
{
    public class LobbyActionService
    {
        private readonly LobbyStateManager _stateManager;


        public LobbyActionService(LobbyStateManager lobbyStateManager)
        {
            _stateManager = lobbyStateManager;
        }  
        public async UniTask UpdateLocalPlayerDataAsync(string characterId, bool IsReady)
        {
            if(_stateManager.CurrentLobby==null) return;

            try
            {
                var options = new UpdatePlayerOptions
                {
                    Data = new()
                    {
                        {"DisplayName",new (PlayerDataObject.VisibilityOptions.Member, _stateManager.LocalPlayerName) },
                        {"CharacterId", new (PlayerDataObject.VisibilityOptions.Member,characterId) },
                        {"IsReady", new(PlayerDataObject.VisibilityOptions.Member, IsReady.ToString().ToLower() )}
                    }
                };
                var lobby = await LobbyService.Instance.UpdatePlayerAsync(
                    _stateManager.CurrentLobby.Id,
                    _stateManager.LocalPlayerId,
                    options
                    );

              
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LobbyService] Failed to update player data: {ex.Message}");
            }
        }
        public async UniTask UpdateLobbyDataAsync(GameMode gameMode, string name)
        {
            if(_stateManager.CurrentLobby== null) return;

            try
            {
                var options = new UpdateLobbyOptions
                {
                    Name = name,
                    MaxPlayers=gameMode.MaxPlayer,
                    Data= new()
                    {
                        {"GameMode", new (DataObject.VisibilityOptions.Public, gameMode.Name) },
                        {"TeamCount", new (DataObject.VisibilityOptions.Public, gameMode.TeamCount.ToString()) }
                    }
                    
                };

                await LobbyService.Instance.UpdateLobbyAsync(
                    _stateManager.CurrentLobby.Id,
                    options
                    );
            }
            catch(Exception ex)
            {
                Debug.LogError($"[LobbyService] Failed to update lobby data: {ex.Message}");
            }
        }
    }
}
