using UnityEngine;

namespace SonicCTK.Physical
{
    /// <summary>
    /// Dummy shield, doesn't do anything (since it's the basic one, duh)
    /// </summary>
    public class PlayerBasicShield : PlayerShield
    {
        public override void OnPlayerShieldTriggerAbility(Player player, PlayerState triggeringState)
        {
            
        }

        public override void OnShieldUpdate(Player player)
        {
            
        }

        public override void OnShieldGather(Player player)
        {
            
        }

        public override void OnShieldLost(Player player)
        {
            
        }
    }
}