using Assets.Scripts.Features.LobbyNetwork.Data.Signals;
using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Features.LobbyNetwork.Service
{
    public class ApplicationInitializer : IInitializable
    {
        private readonly SignalBus _signalBus;

        public ApplicationInitializer(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            InitializeAsync().Forget();
        }

        public async UniTask InitializeAsync()
        {
            try
            {
                if(UnityServices.State != ServicesInitializationState.Initialized)
                {
                    var options = new InitializationOptions();

                    options.SetProfile($"Player_{UnityEngine.Random.Range(0, 10000)}");

                    await UnityServices.InitializeAsync(options);

                    
                }
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    Debug.Log($"[AppInit] Инициализация успешна. ID Игрока: {AuthenticationService.Instance.PlayerId}");
                }
                _signalBus.Fire<ServiceInitializeSignal>();
            }
            catch(Exception ex)
            {
                Debug.LogError($"[AppInit] Критическая ошибка инициализации: {ex.Message}");
            }
        }
    }
}
