using Assets.Scripts.Abstraction;
using Assets.Scripts.Features.LobbyNetwork.Service;
using Assets.Scripts.Features.LobbyNetworking.Service;
using UnityEngine;
using Zenject;

public class LobbyUIPresenter : MonoBehaviour
{
    [SerializeField] private LobbyReadyView view;

    private ISelectorService _selectorService;
    private LobbyStateManager _stateManager;
    private SignalBus _signalBus;
    private LobbyActionService _actionService;
    private NgoRelayMediator _relayMediator;
    private UIWindowManager _windowManager;
    

    private string _charterId;
    private string _playerName;
    private bool _isReady = false;

    [Inject]
    public void Construct(
        ISelectorService selectorService,
        SignalBus signalBus,
        LobbyActionService lobbyActionService,
        LobbyStateManager stateManager,
        NgoRelayMediator ngoRelayMediator,
        UIWindowManager windowManager)
    {
        _selectorService = selectorService;
        _stateManager = stateManager;
        _signalBus = signalBus;
        _actionService=lobbyActionService;
        _relayMediator = ngoRelayMediator;
        _windowManager = windowManager;
    }
    private void Start()
    {
        
       _charterId = _selectorService.SelectedCharacter;

        var allCharacters = _selectorService.GetSelectedAllCharacters();
        view.GenerateCharacterMenu(allCharacters, _charterId);

        bool isHost = _stateManager.CurrentLobby != null && _stateManager.CurrentLobby.HostId == _stateManager.LocalPlayerId;

        view.SetStartButtonVisibility(isHost);
        view.SetStartButtonInteractable(false);
    }
    private void OnEnable()
    {
        view.OnReadyClicked += HandleReadyPressed;
        view.OnStartClicked += HandleStartPressed;
        view.OnCharacterSelected += HandleCharacterSelected;

        _signalBus.Subscribe<LobbyPlayerChangedSignal>(OnPlayersUpdated);
        _signalBus.Subscribe<LocalPlayerKickedSignal>(OnIWasKicked);
    }
    private void OnDisable()
    {
        view.OnReadyClicked -= HandleReadyPressed;
        view.OnStartClicked -= HandleStartPressed;
        view.OnCharacterSelected -= HandleCharacterSelected;

        _signalBus.Unsubscribe<LobbyPlayerChangedSignal>(OnPlayersUpdated);
        _signalBus.Unsubscribe<LocalPlayerKickedSignal>(OnIWasKicked);
    }

    private void OnIWasKicked()
    {
        Debug.LogWarning("[Presenter] Local player was kicked or lobby was deleted.");
        _windowManager.OpenConnectionWindow();
    }

    private void OnPlayersUpdated(LobbyPlayerChangedSignal signal)
    {
        view.ClearPlayersList();
        view.PopulatePlayersList(signal.Players);

        bool allReady = true;
        bool isHost = _stateManager.CurrentLobby != null && _stateManager.CurrentLobby.HostId == _stateManager.LocalPlayerId;
        foreach (var player in signal.Players)
        {
            if (!player.IsReady)
            {
                allReady = false;
                break;
            }

        }
        view.SetStartButtonVisibility(isHost);
        view.SetStartButtonInteractable(allReady);
    }
    

    private async void HandleCharacterSelected(string id)
    {
        _charterId = id;
        view.UpdateCardVisualSelection(id);

        _selectorService.SelectedCharacter = _charterId;

        _isReady = false;

        await _actionService.UpdateLocalPlayerDataAsync(_charterId,_isReady);
    }

    private async void HandleStartPressed()
    {
        await _relayMediator.StartMatchAsync();
    }

    private async void HandleReadyPressed()
    {
        _isReady = !_isReady;
        await _actionService.UpdateLocalPlayerDataAsync( _charterId, _isReady);
    }
}
