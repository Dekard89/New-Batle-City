using Assets.Scripts.Public.Data.Enums;
using Assets.Scripts.Public.Data.Signals;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Assets.Scripts.Public.Infrasructure
{
    public class EffectManager : IInitializable, IDisposable
    {
        private SignalBus _signalBus;

        private ILocalPool _localPool;

        private List<LocalPoolConfig> _configs;

        private Dictionary<EffectType, Queue<GameObject>> _limetedEffects = new();

        public EffectManager(SignalBus signalBus, ILocalPool localPool, List<LocalPoolConfig> configs)
        {
                _localPool = localPool;
            _signalBus = signalBus;
            _configs= configs;
        }
        public void Dispose()
        {
            _signalBus.Unsubscribe<EffectSignal>(HandleEffect);
        }

        public void Initialize()
        {
            _signalBus.Subscribe<EffectSignal>(HandleEffect);
        }
        private void  HandleEffect(EffectSignal signal)
        {
            var config = GetConfigById(signal.ExplosionType);
            if (config.IsCycle)
            {
                HandleLimetedEffect(signal, config);
            }
           
            _localPool.Get(config.ReferenceGameObject, signal.Position, UnityEngine.Quaternion.identity);
        }
        private void HandleLimetedEffect(EffectSignal signal, LocalPoolConfig config)
        {
            EffectType type = signal.ExplosionType;

            if(!_limetedEffects.ContainsKey(type))
                _limetedEffects[type] = new Queue<GameObject>();

            var queue= _limetedEffects[type];

            if(queue.Count>= config.Capacity && queue.Count > 0)
            {
                var oldest= queue.Dequeue();
                if (oldest != null)
                {
                    _localPool.Return(config.ReferenceGameObject, oldest);
                }
            }
            var newEffect= _localPool.Get(config.ReferenceGameObject, signal.Position, Quaternion.identity);

            queue.Enqueue(newEffect);
        }
        private LocalPoolConfig GetConfigById(EffectType type)
        {
            
            string id = type switch
            {
                EffectType.SmallExplosion => "small",
                EffectType.BigExplosion => "medium",
                EffectType.WallDamage => "wallDamage",
                EffectType.WallDestoy => "wallDestroy",
                EffectType.Crator => "crator",
                EffectType.Smoke => "smoke",
                EffectType.Fire => "fire",
                _ => null
            };

            return _configs.FirstOrDefault(x=>x.Id==id);
        }
       
    }
}
