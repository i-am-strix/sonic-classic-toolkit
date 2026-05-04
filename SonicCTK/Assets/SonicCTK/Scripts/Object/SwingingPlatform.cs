using System.Collections;
using System.Collections.Generic;
using SonicCTK.Core;
using UnityEngine;

namespace SonicCTK.Gameplay
{
    public class SwingingPlatform : MonoBehaviour
    {
        [Min(1)] 
        public int ChainLength;
        public Sprite ChainSprite;
        public SolidBox Platform;

        [Range(0, 180)]
        public float SwingMaxAngle = 90f;
        public float SwingSpeed = 1;

        private float swingTime => Time.time * SwingSpeed;
        private List<Transform> chainArray = new();

        private void Start()
        {
            for (int i = 0; i < ChainLength; i++)
            {
                var newChain = new GameObject("Chain");
                newChain.transform.SetParent(transform);

                var sprite = newChain.AddComponent<SpriteRenderer>();
                sprite.sprite = ChainSprite;
            
                chainArray.Add(newChain.transform);
            }
        }

        private void Update()
        {
            float arcRadius = ChainSprite.rect.width.PToW() / 2;
            var arcRoot = transform.position + Quaternion.AngleAxis(Mathf.Sin(swingTime) * SwingMaxAngle, Vector3.forward) * Vector3.down * 
                (arcRadius * 2 * (ChainLength + 1));
            
            Platform.transform.position = arcRoot;

            for (int i = 0; i < ChainLength; i++)
            {
                chainArray[i].position = Vector3.Lerp(transform.position, arcRoot, (i + 1) * 1.0f / (ChainLength + 1));
            }
        }

        private void OnDrawGizmos()
        {
            if (ChainSprite)
            {
                Gizmos.color = new Color(0.53f, 0.53f, 0.53f);
                Vector3 origin = transform.position;
                for (int i = 1; i <= ChainLength; i++)
                {
                    Gizmos.DrawWireSphere(origin - transform.up * (ChainSprite.rect.height.PToW() * i), ChainSprite.rect.width.PToW() / 2);
                }
            }
        }
    }
}
