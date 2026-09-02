using Assets.Scripts.Public.Data.Enums;
using Assets.Scripts.Public.Data.Signals;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Views
{
    public class MineVisuals : MonoBehaviour
    {
        private Renderer mineRenderer;

        private Material mineMaterial;

        private Color initialColor;

        private CancellationTokenSource cts;

        private MineController _controller;

        [Inject]
        private SignalBus _signalBus;

        private void Awake()
        {
            mineRenderer = GetComponent<Renderer>();

            mineMaterial = mineRenderer.material;

            initialColor = mineMaterial.color;

            _controller = GetComponent<MineController>();
        }
        private void OnEnable()
        {
            cts = new CancellationTokenSource();

            if (_controller.IsActive.Value)
                SetAlpha(1f);

            SetAlpha(0f);
        }
        public void Start()
        {
            _controller.IsActive.OnValueChanged += OnActivityChanged;

            Debug.Log($"Мина активна? - {_controller.IsActive.Value}");
        }
        private void OnDisable()
        {
            cts.Cancel();
            cts.Dispose();
        }

        private void SetAlpha(float alpha)
        {
            if (mineMaterial == null) return;

            Color newColor = initialColor;

            newColor.a = alpha;

            mineMaterial.color = newColor;
        }
        private async UniTaskVoid FadeInAsync(CancellationToken token)
        {
            float elapsedTime = 0f;

            while (elapsedTime < _controller.GetActivationTime())
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Clamp01(elapsedTime / _controller.GetActivationTime());
                SetAlpha(alpha);

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
            SetAlpha(1f);
        }
        private void OnActivityChanged(bool previousValue, bool newValue)
        {
            if (newValue)
            {
                FadeInAsync(cts.Token).Forget();
            }
        }
        public void PlayExplosionVFX()
        {
            _signalBus.Fire(new EffectSignal
            {
                ExplosionType= EffectType.BigExplosion,
                Position= transform.position
            });
        }

    }
}
