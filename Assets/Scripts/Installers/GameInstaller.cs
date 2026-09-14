using Assets.Scripts.Abstraction;
using Assets.Scripts.Public;
using Assets.Scripts.Public.Data;
using Assets.Scripts.Public.Data.Model;
using Assets.Scripts.Public.Data.Signals;
using Assets.Scripts.Public.Infrasructure;
using Assets.Scripts.Services;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] CinemachineCamera virtualCamera;
    [SerializeField] PlayerTankConfig playerTankConfig;
    [SerializeField] NetworkPool networkPool;
    [SerializeField] EffectsPool effectsPool;
    [SerializeField] AssetReferenceGameObject projectileReferance;
    [SerializeField] AssetReferenceGameObject mineReferance;
    [SerializeField] private List<NetworkPoolConfig> configs;
    [SerializeField] private List<LocalPoolConfig> localConfigs;
    public override void InstallBindings()
    {
        
        Container.DeclareSignal<EffectSignal>();

        Container.DeclareSignal<LocalHealthChangedSignal>();

        Container.BindInterfacesAndSelfTo<EffectManager>()
            .AsSingle();


        Container.Bind<IInputService>()
            .To<InputService>()
            .AsSingle()
            .WithArguments(inputActions);

        
        Container.BindInterfacesAndSelfTo<PlayerSpawner>()
            .AsSingle()
            .NonLazy();

   

        Container.Bind<CinemachineCamera>()
            .FromInstance(virtualCamera)
            .AsSingle();

        Container.Bind<ICameraTarget>()
            .To<CameraTargetService>()
            .AsSingle();

        Container.Bind<INetworkPool>()
            .To<NetworkPool>()
            .FromComponentInHierarchy(networkPool)
            .AsSingle();

        Container.Bind<ILocalPool>()
            .To<EffectsPool>()
            .FromComponentInHierarchy(effectsPool)
            .AsSingle();
          
        Container.Bind<AssetReferenceGameObject>()
            .WithId("projectile")
             .FromInstance(projectileReferance)
             .AsCached();

        Container.Bind<AssetReferenceGameObject>()
            .WithId("mine")
             .FromInstance(mineReferance)
             .AsCached();

        Container.Bind<List<NetworkPoolConfig>>()
            .FromInstance(configs);

        Container.Bind<List<LocalPoolConfig>>()
            .FromInstance(localConfigs);

        Container.Bind<IBalancer>()
            .To<TeamBalancer>()
            .AsSingle();

        Container.BindInterfacesAndSelfTo<LocalSessionModel>().AsSingle();
    }
}