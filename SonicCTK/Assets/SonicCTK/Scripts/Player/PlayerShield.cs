using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SonicCTK.Physical
{
    /// <summary>
    /// Simple API for implementing shields
    /// </summary>
    public abstract class PlayerShield : MonoBehaviour
    {
        public abstract void OnPlayerShieldTriggerAbility(Player player, PlayerState triggeringState);
        public abstract void OnShieldUpdate(Player player);
        public abstract void OnShieldGather(Player player);
        public abstract void OnShieldLost(Player player);
    }
}
