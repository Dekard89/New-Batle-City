using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Public.Data.Stats
{
    [Serializable]
    public struct ShootStats
    {
        public float ReloadTime;

        public float FireRate;

        public int Ammo;

        public float DamageFactor;

        public float SpeedFactor;
    }
}
