using System;
using SonicCTK.Physical;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Stage = SonicCTK.World.Stage;

namespace SonicCTK.Core
{
    [RequireComponent(typeof(HitBox), typeof(HazardBox))]
    public class EnemyBase : MonoBehaviour
    {
        /// <summary>
        /// The hitbox that the player can attack to kill the enemy
        /// </summary>
        public HitBox PlayerDamageableHitBox;
        
        /// <summary>
        /// The hitbox that the player will take damage from
        /// </summary>
        public HazardBox PlayerHazardousHitbox;

        /// <summary>
        /// Some enemies may not be currently defeatable when rolling into or jumping on them.
        /// Use this flag to define that behavior 
        /// </summary>
        public bool CanAttackWhileSpinning;

        public virtual void Reset()
        {
            TryGetComponent(out PlayerDamageableHitBox);
            TryGetComponent(out PlayerHazardousHitbox);
        }

        public virtual void Start()
        {
            PlayerDamageableHitBox.OnOverlap += DestroyEnemy;
        }

        public virtual void Update()
        {
            foreach (var player in Stage.ConnectedPlayers)
                PlayerHazardousHitbox.enabled = !player.Common.IsCurledIntoBall;
        }

        public virtual void DestroyEnemy(Player player = null)
        {
            if (!player.Common.IsCurledIntoBall) return;
            
            if (player)
                player.Common.Ysp = -Mathf.Abs(player.Common.Ysp);
            
            Addressables.InstantiateAsync("S_FX_Explosion.prefab", transform.position, Quaternion.identity);
            Destroy(gameObject); // You could also just disable them and add some distance check to re-enable later, but whatever
        }
    }
}