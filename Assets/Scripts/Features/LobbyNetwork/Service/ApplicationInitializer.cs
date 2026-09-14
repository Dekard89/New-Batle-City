using Assets.Scripts.Features.LobbyNetwork.Data.Signals;
using Cysharp.Threading.Tasks;
using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Zenject;

#if UNITY_EDITOR
using Unity.Multiplayer.PlayMode;
#endif

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

#if UNITY_EDITOR
                    options.SetProfile("Main_Editor");
                    Debug.Log("[AppInit] Инициализация в Редакторе. Профиль: Main_Editor");
#else
                    string randomProfile = $"Build_Player_{UnityEngine.Random.Range(1000, 9999)}";
                    options.SetProfile(randomProfile);
                    Debug.Log($"[AppInit] Инициализация в Билде. Создан уникальный профиль: {randomProfile}");
#endif
                    await UnityServices.InitializeAsync(options);  
                }
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    Debug.Log($"[AppInit] Инициализация успешна. ID Игрока: {AuthenticationService.Instance.PlayerId}");
                }
                else
                {
                    Debug.Log($"[AppInit] Сессия уже активна. ID Игрока: {AuthenticationService.Instance.PlayerId}");
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
