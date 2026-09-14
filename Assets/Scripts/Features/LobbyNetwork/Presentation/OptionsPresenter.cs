using Assets.Scripts.Features.LobbyNetwork.Data.Signals;
using Assets.Scripts.Features.LobbyNetwork.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Features.LobbyNetwork.Presentation
{
    public class OptionsPresenter : MonoBehaviour
    {
        [SerializeField] private OptionsView _view;

        private LobbyStateManager stateManager;
        private LobbyActionService actionService;
        private UIWindowManager windowManager;
        private SignalBus bus;

        [Inject]
        public void Construct(LobbyStateManager lobbyStateManager, LobbyActionService LobbyActionService,
            UIWindowManager uiWindowManager, SignalBus signalBus)
        {
            stateManager = lobbyStateManager;
            actionService = LobbyActionService;
            windowManager = uiWindowManager;
            bus = signalBus;
        }
        private void OnEnable()
        {
            bus.Subscribe<ServiceInitializeSignal>(OnServiceReady);
            _view.ReturnEvent += OnReturnClicked;
            _view.InputEvent += OnInputNameEntered;

            UpdatePlaceholderUI();
 
        }

        private void OnServiceReady()
        {
            UpdatePlaceholderUI();
        }
        private void UpdatePlaceholderUI()
        {
            _view.SetPlaceholder(stateManager.LocalPlayerName);
        }

        private async void OnInputNameEntered(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;

            stateManager.SavePlayerNameToPrefs(name);

            UpdatePlaceholderUI();

            try
            {
                if(UnityServices.State == ServicesInitializationState.Initialized &&
                    AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.UpdatePlayerNameAsync(name);
                    Debug.Log($"[OptionsPresenter] Имя синхронизировано с профилем UGS.");
                }
            }
            catch(Exception ex)
            {
                Debug.LogWarning($"[OptionsPresenter] Ошибка синхронизации с сервером: {ex.Message}");
            }
        }

        private void OnReturnClicked()
        {
            windowManager.OpenConnectionWindow();
        }
        private void OnDisable()
        {
            _view.ReturnEvent -= OnReturnClicked;
            _view.InputEvent -= OnInputNameEntered;
            bus.Unsubscribe<ServiceInitializeSignal>(OnServiceReady);
        }
    }
}
