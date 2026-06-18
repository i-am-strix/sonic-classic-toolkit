using SonicCTK.Core;
using SonicCTK.Physical;
using SonicCTK.World;
using UnityEngine;

namespace SonicCTK
{
    public class GoalPost : PointBox
    {
        private Animator animator;
        private bool wasTriggered = false;
        
        private void Start()
        {
            foreach (var player in Stage.ConnectedPlayers)
            {
                player.Cam.CameraBorders.y = transform.position.x.WToP();
            }
        }

        private void WatchForOverlap(Player player)
        {

        }
    }
}