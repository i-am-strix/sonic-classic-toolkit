using UnityEngine;

namespace SonicCTK.Physical
{
    public class PlayerElectricShield : PlayerShield
    {
        public bool Triggered = false;
        public override void OnPlayerShieldTriggerAbility(Player player, PlayerState triggeringState)
        {
            // Tails and Knuckles don't get the privilege of a Double Jump, screw those guys...
            if (player is not Sonic sonic) return;

            // ... but then Sonic doesn't get to do HIS own abilities!
            sonic.Common.CanCharacterPerformSpecialAbilities = false;
            sonic.DropdashState.IsDisabled = true;
            
            if (Triggered) return;

            if (sonic.Common.IsCurledIntoBall)
            {
                sonic.Common.Ysp = -5.5f;
                Triggered = true;
            }
        }

        public override void OnShieldUpdate(Player player)
        {
            if (player.Common.IsGrounded)
                Triggered = false;
        }

        public override void OnShieldGather(Player player)
        {
            
        }

        public override void OnShieldLost(Player player)
        {
            if (player is not Sonic sonic) return;
            
            Debug.Log("Electric Shield lost");
            
            sonic.Common.CanCharacterPerformSpecialAbilities = true;
            sonic.DropdashState.IsDisabled = false;
        }
    }
}