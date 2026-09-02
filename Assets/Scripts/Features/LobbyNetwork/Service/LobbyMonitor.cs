using System;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Features.LobbyNetwork.Service
{
    public class LobbyMonitor : IDisposable
    {
        private SignalBus _bus;

        private LobbyStateManager _stateManager;

        private ILobbyEvents _lobbyEvents;

        [Inject]
        public void Construct(SignalBus signalBus, LobbyStateManager stateManager)
        {
            _bus = signalBus;
            _stateManager = stateManager;
        }
        public async void Dispose()
        {
            if (_lobbyEvents != null)
            {
                await _lobbyEvents.UnsubscribeAsync();

                _lobbyEvents = null;
            }
        }
        public async void SubscribeToCurrentLobby()
        {
            if (_stateManager.CurrentLobby == null) return;

            var callbacks = new LobbyEventCallbacks();
            callbacks.LobbyChanged += OnLobbyChanged;
            callbacks.PlayerDataChanged += OnPlayerDataChanged;
            callbacks.KickedFromLobby += OnKickedFromLobby;

            try
            {
                _lobbyEvents = await LobbyService.Instance.SubscribeToLobbyEventsAsync(
                    _stateManager.CurrentLobby.Id,
                    callbacks
                    );

                NotifyClients();
            }
            catch(LobbyServiceException ex)
            {
                Debug.LogError($"[LobbyMonitor] Subscription failed: {ex.Message}");
            }
        }

        private void OnKickedFromLobby()
        {
            _stateManager.Clear();
            _bus.Fire<LocalPlayerKickedSignal>();
        }

        private void OnPlayerDataChanged(Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> playersDataChanges)
        {
            if(_stateManager.CurrentLobby == null || _stateManager.CurrentLobby.Players==null) return;

            foreach(var playerChange in playersDataChanges)
            {
                int index = playerChange.Key;
                var changedData = playerChange.Value;

                if(index>=0 && index < _stateManager.CurrentLobby.Players.Count)
                {
                    var player = _stateManager.CurrentLobby.Players[index];

                    if(player.Data == null)
                    {
                        player.Data = new Dictionary<string, PlayerDataObject>();
                    }

                    foreach(var dataField in changedData)
                    {
                        string key = dataField.Key;

                        var changedValue = dataField.Value;

                        if (changedValue.Changed)
                        {
                            player.Data[key] = changedValue.Value;
                        }
                        else if (changedValue.Removed)
                        {
                            player.Data.Remove(key);
                        }
                    }
                }
            }
            NotifyClients();
        }

        private void OnLobbyChanged(ILobbyChanges changes)
        {
            changes.ApplyToLobby(_stateManager.CurrentLobby);
            NotifyClients();
        }

        private void NotifyClients()
        {
            if(_stateManager.CurrentLobby==null) return;

            var uiPlayers= new List<PlayerModel>();

            foreach(var player in _stateManager.CurrentLobby.Players)
            {
                var displayName = player.Data != null && player.Data.TryGetValue("DisplayName", out var nameData)
                    ? nameData.Value : "Connecting...";
                    

                var character = player.Data != null && player.Data.TryGetValue("CharacterId", out var charName)
                    ? charName.Value : "Not Selected";

                var isReady = player.Data != null && player.Data.TryGetValue("IsReady", out var readyData)
                    && bool.TryParse(readyData.Value, out var ready) && ready;

                uiPlayers.Add(new PlayerModel(player.Id, displayName,character, isReady));

            }

            _bus.Fire(new LobbyPlayerChangedSignal(uiPlayers));
        }
        public void InvokeLocalUpdate()
        {
            NotifyClients();
        }
    }
}
