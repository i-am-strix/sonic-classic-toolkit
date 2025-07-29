using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SonicCTK.Physical
{
    public sealed class Sonic : Player
    {
        public PStateDropdash DropdashState = new PStateDropdash();
        public override void AddStates()
        {
            base.AddStates();
            Add(DropdashState);
        }

        public override void PlayerStart()
        {
            
        }
    }
}