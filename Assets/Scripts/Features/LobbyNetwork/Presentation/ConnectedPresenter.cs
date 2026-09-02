using Assets.Scripts.Features.LobbyNetwork.Service;
using Assets.Scripts.Features.LobbyNetwork.UI;
using Assets.Scripts.Features.LobbyNetworking.Service;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Features.LobbyNetwork.Presentation
{
    public class ConnectedPresenter : MonoBehaviour
    {
        [SerializeField] private ConnectedView view;
        
        private UIWindowManager _windowManager;
        private NgoRelayMediator _mediator;
        private LobbyConnectionService _connectionService;

        [Inject]
        public void Construct(LobbyConnectionService connectionService,
            NgoRelayMediator mediator,
            UIWindowManager windowManager)
        {
            _connectionService = connectionService;
            _mediator = mediator;
            _windowManager = windowManager;
        }
        private void OnEnable()
        {
            view.OnCreateLobbyEvent += HandleCreateLobbyClicked;
            view.OnRefreshLobbyListEvent += HandleRefreshLobbyClicked;
            view.OnServerSelectedEvent += HandleSelectedServer;
            view.OnOptionsEvent += HandleOptiosClicked;
        }

        private void HandleOptiosClicked()
        {
            _windowManager.OpenOptionsWindow();
        }

        private void OnDisable()
        {
            view.OnCreateLobbyEvent -= HandleCreateLobbyClicked;
            view.OnRefreshLobbyListEvent -= HandleRefreshLobbyClicked;
            view.OnServerSelectedEvent -= HandleSelectedServer;
            view.OnOptionsEvent -= HandleOptiosClicked;
        }
        private void Start()
        {
            HandleRefreshLobbyClicked();
        }

        private async void HandleSelectedServer(string lobbyID)
        {
            view.SetStatusText("Подключение к выбранному серверу...");
            view.SetControlsInteractable(false);

            try
            {
                await _mediator.JoinGameByIDAsync(lobbyID);
            }
            catch(Exception ex)
            {
                view.SetStatusText($"Не удалось войти: {ex.Message}");
                view.SetControlsInteractable(true);
            }
        }

        private async void HandleRefreshLobbyClicked()
        {
            view.SetStatusText("Обновление списка серверов.....");
            view.SetControlsInteractable(false);

            try
            {
                var activeLobbies= await _connectionService.QueryLobbiesListAsync();
                var uiModels= new List<LobbyModel>();

                foreach(var lobby in activeLobbies)
                {
                    var gameMode = lobby.Data.TryGetValue(LobbyConnectionService.GameModeKey, out var modeObject)
                        ? modeObject.Value
                        : "Unknown";
                    var currentPlayers = lobby.MaxPlayers - lobby.AvailableSlots;

                    uiModels.Add(new(
                        lobby.Id,
                        lobby.Name,
                        gameMode,
                        currentPlayers,
                        lobby.MaxPlayers
                        ));
                }
                view.PopulatrServerList(uiModels);

                string resultText = uiModels.Count == 0
                    ? "Доступных комнат не найдено."
                    : "Список обновлен.";
                view.SetStatusText(resultText);
            }
            catch(Exception ex)
            {
                view.SetStatusText($"Ошибка обновления: {ex.Message}");
                view.ClearServerList();
            }
            finally
            {
                view.SetControlsInteractable(true);
            }
        }

        private void HandleCreateLobbyClicked()
        {
            view.SetStatusText("Сздание комнаты.....");
            _windowManager.OpenCreateLobbyWindow();
        }
    }
}
