using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Rendering;

namespace Assets.Scripts.Abstraction
{
    public interface IHealable
    {
        void TakeHeal(float heal);
    }
}
