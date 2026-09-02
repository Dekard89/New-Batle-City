using Assets.Scripts.Public.Data.Enums;
using Assets.Scripts.Public.Data.Signals;
using System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Views
{
    public class WallVisuals : MonoBehaviour
    {
        private static readonly int CrackedPercent = Shader.PropertyToID("_DamageAmount");

        private SpriteRenderer _spriteRenderer;

        private Material _material;

        private WallHealtController _controller;

        [Inject]
        private SignalBus _signalBus;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

            _material= _spriteRenderer.material;

            _controller = GetComponent<WallHealtController>();
        }
        
        
   
        public void SyncTakeDamageHandler(float previousValue, float newValue)
        {
            float normalizedDamage = 1f - (newValue / _controller.GetMaxHealth());

            _material.SetFloat(CrackedPercent, normalizedDamage);

           if(newValue < _controller.GetMaxHealth())
            {
                _signalBus.Fire(new EffectSignal
                {
                    Position = transform.position,

                    ExplosionType = EffectType.WallDamage
                });
            }

        }
        public void SyncDeath()
        {
            _signalBus.Fire(new EffectSignal
            {
                Position = transform.position,
                ExplosionType = EffectType.WallDestoy
            });
        }
       
    }
}
