using Stray.Core;
using UnityEngine;
using UnityEngine.U2D;

namespace Stray.Physical
{
    public class PlayerCamera : MonoBehaviour
    {
        public Player Player;

        public PixelPerfectCamera PPCam => GetComponent<PixelPerfectCamera>();
        
        public Vector4 CameraBorders = new Vector4(
            144, 160, // Horizontal
            64, 128 // Vertical
        );

        public Vector2 CameraSpeedLimit = new Vector2(12, 12);

        private Vector2 PixelPosition;
        private Vector2 CurrentLimit;
        private Vector2 FocalPoint;

        private int FreezeTicks;

        public void FreezeCamera(int TickCount)
        {
            FreezeTicks = TickCount;
        }
        
        public void Follow()
        {
            if (Player == null) return;

            if (FreezeTicks > 0)
            {
                FreezeTicks--;
                return;
            }

            var PixelPlayerPosition = ((Vector2)Player.transform.position).WToP() -
                                      new Vector2(0, (Player.Col.DefaultCollisionSize.y - Player.Col.PlayerCollisionSize.y) / 2);

            FocalPoint = 
                (PixelPlayerPosition + new Vector2(0, 16)) - PixelPosition;
            CurrentLimit = new Vector2(0, 0);
            
            if (FocalPoint.x <= PPCam.refResolutionX / 2 - 16) 
                CurrentLimit.x = -Mathf.Clamp(PPCam.refResolutionX / 2 - 16 - FocalPoint.x, 0, CameraSpeedLimit.x);
            else if (FocalPoint.x >= PPCam.refResolutionX / 2) 
                CurrentLimit.x = Mathf.Clamp(FocalPoint.x - PPCam.refResolutionX / 2, 0, CameraSpeedLimit.x);
            else CurrentLimit.x = 0;

            if (Player.Cmn.IsGrounded)
            {
                if (Mathf.Abs(Player.Cmn.Gsp) >= 8) CameraSpeedLimit.y = 16;
                else CameraSpeedLimit.y = 6;

                CurrentLimit.y = 
                    Mathf.Clamp(FocalPoint.y - PPCam.refResolutionY / 2, 
                        -CameraSpeedLimit.y, CameraSpeedLimit.y);
            }

            else
            {
                CameraSpeedLimit.y = 16;
                if (FocalPoint.y <= PPCam.refResolutionY / 2 - 16)
                    CurrentLimit.y = -Mathf.Clamp(PPCam.refResolutionY / 2 - 16 - FocalPoint.y, 0, CameraSpeedLimit.y);
                else if (FocalPoint.y > -(PPCam.refResolutionY / 2 + 48))
                    CurrentLimit.y = Mathf.Clamp(FocalPoint.y - (PPCam.refResolutionY / 2 + 48), 0, CameraSpeedLimit.y);
                else CurrentLimit.y = 0;
            }

            PixelPosition += CurrentLimit;
            
            transform.position = (Vector3)(PixelPosition + new Vector2(PPCam.refResolutionX / 2, PPCam.refResolutionY / 2)).PToW() + new Vector3(0, 0, -10);

            /*
            if (freezeTimer > 0)
            {
                currentLimit.x = 0;
                freezeTimer--;
            }
            if (player.grounded)
            {
                if (Mathf.Abs(player.gsp) >= 8) speedLimit.y = 16;
                else speedLimit.y = 6;

                currentLimit.y = Mathf.Clamp(focalPoint.y - (ppc.refResolutionY / 2), -speedLimit.y, speedLimit.y);
            }
            else
            {
                speedLimit.y = 16;
                if (focalPoint.y <= (ppc.refResolutionY / 2 - 16)) currentLimit.y = Mathf.Clamp(focalPoint.y - (ppc.refResolutionY / 2 - 16), -speedLimit.y, 0);
                else if (focalPoint.y > -(ppc.refResolutionY / 2 + 48)) currentLimit.y = Mathf.Clamp(focalPoint.y - (ppc.refResolutionY / 2 + 48), 0, speedLimit.y);
                else currentLimit.y = 0;
            }

            pos += currentLimit;

            pos.x = Mathf.Clamp(pos.x, 0, StrayGame.ActiveLoop.roomSize.x - ppc.refResolutionX);
            if (StrayGame.ActiveLoop.activeSignPost != null) pos.x = Mathf.Clamp(pos.x, 0, StrayGame.ActiveLoop.activeSignPost.transform.position.WTP().x - ppc.refResolutionX / 2);
            pos.y = Mathf.Clamp(pos.y, 0, StrayGame.ActiveLoop.roomSize.y - ppc.refResolutionY);
            transform.position = (Vector3)(pos + new Vector2(ppc.refResolutionX/2, ppc.refResolutionY/2)).PTW() + new Vector3(0,0,zOffset);*/
        }
    }
}