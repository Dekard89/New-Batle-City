using Assets.Scripts.Public.Data.Enums;
using Assets.Scripts.Public.Data.Signals;
using Assets.Scripts.Public.Infrasructure;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Assets.Scripts.Views
{
    public class ProjectileVisuals : MonoBehaviour
    {

        [Inject]
        private SignalBus _signalBus;

        private void Awake()
        {
            
        }

        public void PlayExplosionFX()
        {
            _signalBus.Fire(new EffectSignal { Position = transform.position, ExplosionType = EffectType.SmallExplosion });
        }
        
    }
}
