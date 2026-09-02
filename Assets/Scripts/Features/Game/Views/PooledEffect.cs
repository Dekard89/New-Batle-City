using Assets.Scripts.Public.Infrasructure;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Views
{
    public class PooledEffect : MonoBehaviour
    {
        private ILocalPool  _pool;

        private AssetReferenceGameObject _myKey;

        public void Setup(ILocalPool pool, AssetReferenceGameObject key)
        {
            _myKey = key;
            _pool = pool;
        }
        private void OnParticleSystemStopped()
        {
            _pool.Return(_myKey,gameObject);
        }
    }
}
