using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Public.Data.Stats
{
    [Serializable]
    public struct HealthStats
    {
        public float MaxHealth;

        public float SelfHealDelay;

        public float SelfHealPercent;

        public float HealingTick;
    }
}
