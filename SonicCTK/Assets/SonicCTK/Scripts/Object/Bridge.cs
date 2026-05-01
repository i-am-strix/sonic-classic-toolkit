using System;
using System.Collections;
using System.Collections.Generic;
using SonicCTK.Core;
using SonicCTK.Physical;
using UnityEngine;

namespace SonicCTK
{
    public class Bridge : SolidBox
    {
        public Sprite BridgeLogSprite;
        
        [Min(1)] 
        public int BridgeLength;

        private List<Transform> generatedBridgeLogs;
        private float[] generatedBridgeLogDepressionMax;

        private float logSpriteWidth => BridgeLogSprite.rect.width;
        private bool isAnyPlayerStandingOnBridge = false;

        private void Reset()
        {
            BridgeLength = 10;
            AllowSolidityFrom.Bottom = AllowSolidityFrom.Left = AllowSolidityFrom.Right = false;
        }
        
        private void Start()
        {
            generatedBridgeLogs = new();
            generatedBridgeLogDepressionMax = new float[BridgeLength];
            Vector3 origin = (Vector3)BridgeLogSprite.rect.size.PToW() / 2;
            
            for (int i = 0; i < BridgeLength; i++)
            {
                var newLog = new GameObject($"Bridge Log {i}");
                newLog.transform.SetParent(transform);
                newLog.transform.localPosition = origin + Vector3.right * (BridgeLogSprite.rect.width.PToW() * i);

                var sprite = newLog.AddComponent<SpriteRenderer>();
                sprite.sprite = BridgeLogSprite;
                
                generatedBridgeLogs.Add(newLog.transform);
                generatedBridgeLogDepressionMax[i] = (i < BridgeLength / 2 ? i : BridgeLength - i - 1) * 2 + 2;
            }

            radius.x = BridgeLogSprite.rect.width * BridgeLength / 2;
            
            radius.y = boxOffset.y = BridgeLogSprite.rect.height / 2;
            boxOffset.x = BridgeLength / 2 * BridgeLogSprite.rect.width;
            
            OnCollision += WatchForCollision;
        }

        private float depressionAmp;
        private float curSeg;

        private void WatchForCollision(Player player, SolidBoxCollisionDirection direction)
        {
            depressionAmp = Mathf.Clamp(depressionAmp + (player.Interactions.CurrentlyStandingSolidBox == this ? 5.625f : -5.625f), 0,90);
            if (player.Interactions.CurrentlyStandingSolidBox == this)
            {
                isAnyPlayerStandingOnBridge = true;
                var origin = (Vector3)BridgeLogSprite.rect.size.PToW() / 2;
                curSeg = Mathf.Clamp((int)Mathf.Floor((player.transform.position.x.WToP() - transform.position.x.WToP()) / 16) + 1, 1, BridgeLength);

                for (int i = 0; i < BridgeLength; i++)
                {
                    float logDistance = 0;
                    float difference = Mathf.Abs((i + 1) - curSeg);
                    if (i < curSeg) 
                        logDistance = 1 - (difference / curSeg);
                    else 
                        logDistance = 1 - (difference / ((BridgeLength - curSeg) + 1));
                    
                    generatedBridgeLogs[i].localPosition = 
                        new Vector3(generatedBridgeLogs[i].localPosition.x, origin.y - Mathf.Floor(generatedBridgeLogDepressionMax[(int)curSeg - 1] * Mathf.Sin((depressionAmp * logDistance) * Mathf.Deg2Rad)).PToW());

                    player.transform.position = new Vector3(player.transform.position.x,
                        UpperLimit + player.Collision.HeightRadius.PToW());
                }
            }
            else
            {
                
            }
        }

        public override void Update()
        {
            boxOffset.y = -generatedBridgeLogs[BridgeLength / 2].localPosition.y.WToP();
            
            base.Update();
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            if (BridgeLogSprite)
            {
                Gizmos.color = new Color(1f, 0.6f, 0.19f);
                Vector3 origin = transform.position + (Vector3)BridgeLogSprite.rect.size.PToW() / 2;
                for (int i = 0; i < BridgeLength; i++)
                {
                    Gizmos.DrawWireSphere(origin + Vector3.right * (BridgeLogSprite.rect.width.PToW() * i), BridgeLogSprite.rect.width.PToW() / 2);
                }
            }
        }
    }
}
