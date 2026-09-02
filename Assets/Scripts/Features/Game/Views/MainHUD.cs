using Assets.Scripts.Public.Data.Signals;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Views
{
    public class MainHUD : MonoBehaviour
    {
        [SerializeField]
        private Slider _slider;

        [SerializeField]
        private Image _fill;

        [SerializeField]
        private Color mainColor = Color.green;

        [SerializeField]
        private Color dangerColor = Color.red;

        [Inject]
        private SignalBus _signalBus;

        private CancellationTokenSource _pulseCts;

        private void Start()
        {
           
        }

        private void OnEnable() => _signalBus.Subscribe<LocalHealthChangedSignal>(OnHealthChanged);

        private void OnDisable() => _signalBus.Unsubscribe<LocalHealthChangedSignal>(OnHealthChanged);
      
        private void OnHealthChanged(LocalHealthChangedSignal signal)
        {
            var ratio = signal.Current / signal.Max;
            _slider.value = ratio;
            if(ratio < 0.3) StartPulsing();
            else StopPulsing();
        }
        private void StartPulsing()
        {
            if (_pulseCts != null) return;
            _pulseCts= new CancellationTokenSource();
            PulseAsync(_pulseCts.Token).Forget();
        }
        private void StopPulsing()
        {
            if(_pulseCts != null)
            {
                _pulseCts?.Cancel();
                _pulseCts?.Dispose();
                _pulseCts = null;
                _fill.color = mainColor;
            }
        }
        private async UniTaskVoid PulseAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                float t = (Mathf.Sin(Time.time * 5f) + 1f) / 2f;

                _fill.color = Color.Lerp(dangerColor, Color.white, t);

                await UniTask.Yield(token);
            }
        }
        private void OnDestroy()
        {
            StopPulsing();
        }
    }
}
