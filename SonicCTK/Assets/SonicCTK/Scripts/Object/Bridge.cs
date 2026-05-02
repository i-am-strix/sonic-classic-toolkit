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
        [Header("Rendering")]
        public Sprite BridgeLogSprite;
        public Sprite BridgeLimitSprite;

        public SortingLayer BridgeRenderLayer;
        
        [Min(1)] 
        public int BridgeLength;

        private List<Transform> generatedBridgeLogs;
        private float[] generatedBridgeLogDepressionMax;

        private float logSpriteWidth => BridgeLogSprite.rect.width;
        private bool isAnyPlayerStandingOnBridge = false;

        private Vector2 bridgeLogSpriteSize;

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

            bridgeLogSpriteSize = BridgeLogSprite.rect.size;
            
            for (int i = 0; i < BridgeLength; i++)
            {
                var newLog = new GameObject($"Bridge Log {i}");
                newLog.transform.SetParent(transform);
                newLog.transform.localPosition = origin + Vector3.right * (bridgeLogSpriteSize.x.PToW() * i);

                var sprite = newLog.AddComponent<SpriteRenderer>();
                sprite.sprite = BridgeLogSprite;
                
                generatedBridgeLogs.Add(newLog.transform);
                generatedBridgeLogDepressionMax[i] = i < BridgeLength / 2 ? (i + 1) * 2 : (BridgeLength - i) * 2;
            }

            radius.x = bridgeLogSpriteSize.x * BridgeLength / 2;
            
            radius.y = boxOffset.y = bridgeLogSpriteSize.y / 2;
            boxOffset.x = BridgeLength / 2 * bridgeLogSpriteSize.x;
            
            OnCollision += WatchForCollision;

            var bridgeLimitLeft = new GameObject("Bridge Limit Left");
            bridgeLimitLeft.transform.position = transform.position;
            var limitLeftSprite = bridgeLimitLeft.AddComponent<SpriteRenderer>();
            limitLeftSprite.sprite = BridgeLimitSprite;
            
            var bridgeLimitRight = new GameObject("Bridge Limit Right");
            var limitRightSprite = bridgeLimitRight.AddComponent<SpriteRenderer>();
            limitRightSprite.sprite = BridgeLimitSprite;
            limitRightSprite.flipX = true;
            
            bridgeLimitRight.transform.position = transform.position + Vector3.right * bridgeLogSpriteSize.x.PToW() * BridgeLength;
        }

        private float depressionAmp;
        private int curSeg;

        private Player attachedPlayer;

        private void WatchForCollision(Player player, SolidBoxCollisionDirection direction)
        {
            attachedPlayer = player;
        }

        public override void Update()
        {
            base.Update();

            var playerStanding = IsAnyPlayerStandingOnThis();
            depressionAmp = Mathf.Clamp(depressionAmp + (playerStanding ? 5.625f : -5.625f), 0,90);

            if (depressionAmp == 0) return;
            
            isAnyPlayerStandingOnBridge = true;
            var origin = (Vector3)BridgeLogSprite.rect.size.PToW() / 2;
            if (playerStanding)
                curSeg = Mathf.Clamp((int)Mathf.Floor((attachedPlayer.transform.position.x.WToP() - transform.position.x.WToP()) / 16) + 1, 1, BridgeLength);

            for (int i = 0; i < BridgeLength; i++)
            {
                float logDistance = 0;
                float difference = Mathf.Abs(i + 1 - curSeg);
                if (i < curSeg) 
                    logDistance = 1 - difference / curSeg;
                else 
                    logDistance = 1 - difference / (BridgeLength - curSeg + 1);
                
                generatedBridgeLogs[i].localPosition = 
                    new Vector3(generatedBridgeLogs[i].localPosition.x, origin.y - Mathf.Floor(generatedBridgeLogDepressionMax[curSeg - 1] * Mathf.Sin((depressionAmp * logDistance) * Mathf.Deg2Rad)).PToW());

            }
            
            if (attachedPlayer && attachedPlayer.Common.IsGrounded)
                attachedPlayer.transform.position -= new Vector3(0, (generatedBridgeLogDepressionMax[curSeg - 1]).PToW() * Mathf.Sin(depressionAmp * Mathf.Deg2Rad));
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
