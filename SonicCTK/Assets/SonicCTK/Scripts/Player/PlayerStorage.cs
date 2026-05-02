using System;
using UnityEngine;
using SonicCTK.Core;

namespace SonicCTK.Physical
{
    /// <summary>
    /// Per player storage, use to store some useful info
    /// </summary>
    [Serializable]
    public class PlayerStorage
    {
        public int Rings;
        public int Score;

        public int PlayerBounceScoreGainIndex = 0;
    }
}