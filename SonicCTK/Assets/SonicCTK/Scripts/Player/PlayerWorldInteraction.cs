using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicCTK.Core;

namespace SonicCTK.Physical
{
    /// <summary>
    /// Contains info about interactivity with other objects aswell as functions to process incoming info and constraints
    /// from stuff like <b>Solid Boxes</b>
    /// </summary>
    [Serializable]
    public class PlayerWorldInteraction
    {
        public Player Player;
        public PlayerCommon Cmn => Player.Common;

        public int IgnoreObjectsDuration;

        [Header("Common Flags")] 
        public bool IsSTunnelMode;
        
        [Header("Shields")]
        public PlayerShieldType ShieldType;
        public GameObject CurrentShieldInstance;
        
        [Header("Solid Boxes")]
        public bool IsStandingOnSolidBox;
        public SolidBox CurrentlyStandingSolidBox;

        [Header("Collision")] 
        public CLayerMode CurrentPlayerLayer;

        public void InteractionUpdate()
        {
            IgnoreObjectsDuration = Mathf.Max(IgnoreObjectsDuration - 1, 0);

            if (Player.Is<PStateDead>()) return;

            if (IsStandingOnSolidBox)
            {
                var pixelObjectPos = CurrentlyStandingSolidBox.transform.position.WToP();

                Player.transform.position = new Vector3(Player.transform.position.x,
                    -(pixelObjectPos.y - CurrentlyStandingSolidBox.Radius.y - Player.Collision.HeightRadius - 1).PToW(),
                    Player.transform.position.z);
            }
            
            if (IsSTunnelMode)
            {
                if (!Cmn.IsGrounded) return;
                if (!Player.Is<PStateRoll>())
                {
                    if (Player.Set<PStateRoll>())
                    {
                        CAudio.Play("Spin");
                        Player.Cam.Follow();
                    }
                }
            }

            if (CurrentShieldInstance)
                CurrentShieldInstance.transform.position = ((Vector2)Player.transform.position).WToP().Round().PToW();
        }

        public void SetShield(PlayerShieldType NewShieldType)
        {
            if (NewShieldType != ShieldType)
            {
                if (CurrentShieldInstance) GameObject.Destroy(CurrentShieldInstance);
                switch (NewShieldType)
                {
                    case PlayerShieldType.None:
                        ShieldType = PlayerShieldType.None;
                        break;
                    
                    case PlayerShieldType.Normal:
                        CurrentShieldInstance =
                            ObjectReferenceContainer.Spawn("Normal Shield", Player.transform.position);
                        ShieldType = NewShieldType;
                        break;
                }
            }
        }
    }
}