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
        [Header("Solid Boxes")]
        public bool IsStandingOnSolidBox;
        public SolidBox CurrentlyStandingSolidBox;
    }
}