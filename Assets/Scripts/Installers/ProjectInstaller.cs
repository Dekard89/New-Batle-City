using Assets.Scripts.Abstraction;
using Assets.Scripts.Features.LobbyNetwork.Data.Signals;
using Assets.Scripts.Features.LobbyNetwork.Service;
using Assets.Scripts.Public;
using Assets.Scripts.Public.Data.Signals;
using Assets.Scripts.Public.DataBase;
using Assets.Scripts.Services;
using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private CharacterDataBase characterDB;
    [SerializeField] private GameModeDataBase gameModeDataBase;
    public override void InstallBindings()
    {

        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<ServiceInitializeSignal>();

        Container.DeclareSignal<LobbyJoinedSignal>();

        Container.DeclareSignal<LobbyPlayerChangedSignal>();

        Container.DeclareSignal<LocalPlayerKickedSignal>();

        Container.DeclareSignal<ShowLoadingSignal>();

        Container.DeclareSignal<HideLoadingSignal>();

        Container.DeclareSignal<UpdateLoadingProgressSignal>();

        Container.Bind<IPrefabLoad>()
            .To<PrefabLoader>()
            .AsSingle();

        Container.BindInstance(characterDB).AsSingle();
        Container.BindInstance(gameModeDataBase).AsSingle().NonLazy();

        Container.Bind<ISelectorService>()
            .To<CharacterSelectorService>()
            .AsSingle();

       
        Container.BindInterfacesAndSelfTo<CharactersHandlerRegistratonService>()
            .AsSingle();

        Container.BindInterfacesAndSelfTo<ApplicationInitializer>().AsSingle().NonLazy();

        Container.Bind<LobbyStateManager>().AsSingle();
    }
}
