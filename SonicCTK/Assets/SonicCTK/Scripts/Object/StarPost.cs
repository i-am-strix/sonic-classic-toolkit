using SonicCTK.Core;
using SonicCTK.Physical;
using SonicCTK.World;
using UnityEngine;

namespace SonicCTK
{
    public class StarPost : PointBox
    {
        private Animator animator;
        private bool wasTriggered = false;
        
        private void Start()
        {
            animator = GetComponent<Animator>();
            OnBeginOverlap += WatchForOverlap;
        }

        private void WatchForOverlap(Player player)
        {
            if (!wasTriggered)
            {
                CAudio.Play("Checkpoint");
                wasTriggered = true;
                animator.Play("Trigger", 0, 0);

                Stage.HasCheckpointSpawn = true;
                Stage.CheckpointSpawnPosition = transform.position;
            }
        }
    }
}