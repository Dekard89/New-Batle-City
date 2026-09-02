using Assets.Scripts.Features.LobbyNetworking.Service;
using Assets.Scripts.Public.Data.Model;
using Assets.Scripts.Public.DataBase;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Features.LobbyNetwork.Presentation
{
    public class CreateGamePresenter : MonoBehaviour
    {
        [SerializeField] private CreateGameView view;

        
        private GameModeDataBase _dataBase;

        private NgoRelayMediator _mediator;

        private List<GameMode> _gameModes;

        [Inject]
        public void Construct(GameModeDataBase dataBase, NgoRelayMediator mediator)
        {
            _dataBase = dataBase;
            _mediator = mediator;
        }
        private void Start()
        {
            _gameModes = _dataBase.GetAll();

            view.GenerateGameModeDropDpwn(_gameModes.Select(x => x.Name).ToList());
        }
        private void OnEnable()
        {
            view.OnFinalizezeEvent += HandleFinalizeClicked;
        }
        private void OnDisable()
        {
            view.OnFinalizezeEvent -= HandleFinalizeClicked;
        }

        private async void HandleFinalizeClicked(int index, string lobbyName)
        {

            await _mediator.HostGameAsync(lobbyName, _gameModes[index]);
        }
    }
}
