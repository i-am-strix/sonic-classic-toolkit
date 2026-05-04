using System.Collections;
using System.Collections.Generic;
using SonicCTK.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SonicCTK.Physical
{
    public class PlayerElectricShield : PlayerShield
    {
        private Player attachedPlayer;
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
                CAudio.PlayOneShot("Electric Shield Trigger");
                sonic.Common.Ysp = -5.5f;
                Triggered = true;
                attachedPlayer.StartCoroutine(HandleSparks());
            }
        }

        public override void OnShieldUpdate(Player player)
        {
            if (player.Common.IsGrounded)
                Triggered = false;
        }

        public override void OnShieldGather(Player player)
        {
            attachedPlayer = player;
        }

        public override void OnShieldLost(Player player)
        {
            if (player is not Sonic sonic) return;
            
            Debug.Log("Electric Shield lost");
            
            sonic.Common.CanCharacterPerformSpecialAbilities = true;
            sonic.DropdashState.IsDisabled = false;
        }

        public IEnumerator HandleSparks()
        {
            var sparks = new List<GameObject>();
            
            for(int i = 0; i < 4; i++)
                sparks.Add(Addressables.InstantiateAsync("/Common/FX/Shield/Electric Shield Spark", attachedPlayer.transform.position, Quaternion.identity).WaitForCompletion());

            int frameCount = 24;
            while (frameCount > 0)
            {
                for (int i = 0; i < 4; i++)
                    sparks[i].transform.position += new Vector3(i % 2 == 0 ? -2 : 2, i >= 2 ? 2 : -2).PToW();

                frameCount--;
                yield return null;
            }
            
            foreach(var obj in sparks) Destroy(obj);
        }
    }
}