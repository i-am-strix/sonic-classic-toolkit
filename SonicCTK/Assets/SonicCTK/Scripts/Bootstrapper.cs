using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SonicCTK
{
    /// <summary>
    /// This is where a lot of "startup" stuff happens behind the scenes (hence the title of Bootstrapper)
    /// </summary>
    public static class Bootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BeginGame()
        {
            // Although the 60FPS hard limit is harsh, the Classic games **were** built with this limit in mind,
            // so I decided to stick by with it aswell
            
            // You can kinda?? get around this by running every script in FixedUpdate() instead of normal Update(), which will force it to run at the 
            // fixed timestamp that's defined to be 60FPS by default, but that introduces a lot of inconsistencies with input detection, so I recommend
            // you stick to this
            Application.targetFrameRate = 60;
        }
    }
}
