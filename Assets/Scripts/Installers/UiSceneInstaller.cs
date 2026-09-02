using Assets.Scripts.Abstraction;
using Assets.Scripts.Features.LobbyNetwork.Service;
using Assets.Scripts.Features.LobbyNetwork.UICards;
using Assets.Scripts.Features.LobbyNetworking.Service;
using Assets.Scripts.Public.Data.Signals;
using Assets.Scripts.Public.DataBase;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Public
{
    public class UiSceneInstaller : MonoInstaller
    {
        [SerializeField] private ConnectionWindow connectionWindow;
        [SerializeField] private ChangeCharacterWindow changeCharacterWindow;
        [SerializeField] private CreateLobbyWindow createLobbyWindow;
        [SerializeField] private OptionsWindow optionsWindow;

        public override void InstallBindings()
        {
           

            Container.BindInterfacesAndSelfTo<UIWindowManager>().AsSingle();

            Container.BindInstance(connectionWindow).AsSingle();
            Container.BindInstance(changeCharacterWindow).AsSingle();
            Container.BindInstance(createLobbyWindow).AsSingle();
            Container.BindInstance(optionsWindow).AsSingle();
           
            Container.BindInterfacesAndSelfTo<LobbyActionService>().AsSingle();

            Container.BindInterfacesAndSelfTo<LobbyMonitor>().AsSingle();

            Container.BindInterfacesAndSelfTo<LobbyConnectionService>().AsSingle();

            Container.Bind<NgoRelayMediator>().AsSingle();
        }
    }
}
