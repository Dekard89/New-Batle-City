using Assets.Scripts.Features.LobbyNetwork.Data.Signals;
using Assets.Scripts.Features.LobbyNetwork.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
 
        }

        private void OnServiceReady()
        {
            _view.SetPlaceholder(stateManager.LocalPlayerName);
        }

        private async void OnInputNameEntered(string name)
        {
            await actionService.UpdateLocalPlayerDataAsync(name, "Medium", false);
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
