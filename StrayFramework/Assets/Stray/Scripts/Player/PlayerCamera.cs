using Stray.Core;
using Stray.World;
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

        public Vector2 PixelPosition;
        private Vector2 CurrentLimit;
        private Vector2 FocalPoint;

        private int FreezeTicks;

        public void FreezeCamera(int TickCount)
        {
            FreezeTicks = TickCount;
        }

        public void Start()
        {
            PixelPosition = Player.transform.position.WToP();
        }

        public void Follow()
        {
            if (!Player) return;

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
            
            PixelPosition.x = Mathf.Clamp(PixelPosition.x, 0, Stage.Current.StageSize.x - PPCam.refResolutionX);
            PixelPosition.y = Mathf.Clamp(PixelPosition.y, 0, Stage.Current.StageSize.y - PPCam.refResolutionY);
            
            transform.position = (Vector3)(PixelPosition + new Vector2(PPCam.refResolutionX / 2, PPCam.refResolutionY / 2)).PToW() + new Vector3(0, 0, -10);}
    }
}