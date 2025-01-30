using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stray.Core;

namespace Stray.Physical
{
    [Serializable]
    public class PlayerWorldInteraction
    {
        public Player Player;
        public PlayerCommon Cmn => Player.Cmn;

        [Header("Common Flags")] 
        public bool IsSTunnelMode;

        [Header("Solid Boxes")]
        public bool IsStandingOnSolidBox;
        public SolidBox CurrentlyStandingSolidBox;

        public void InteractionUpdate()
        {
            if (IsStandingOnSolidBox)
            {
                var PixelObjectPos = CurrentlyStandingSolidBox.transform.position.WToP();

                Player.transform.position = new Vector3(Player.transform.position.x,
                    -(PixelObjectPos.y - CurrentlyStandingSolidBox.Radius.y - Player.Col.HeightRadius - 1).PToW(),
                    Player.transform.position.z);
            }
            
            if (IsSTunnelMode)
            {
                if (!Cmn.IsGrounded) return;
                if (!Player.Is<WPStateRoll>())
                {
                    if (Player.Set<WPStateRoll>())
                    {
                        WAudio.Play("Spin");
                        Player.Cam.Follow();
                    }
                }
            }
        }
    }
}