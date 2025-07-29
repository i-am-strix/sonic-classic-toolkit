using SonicCTK.Core;
using SonicCTK.World;
using UnityEngine;

using UnityEngine.Experimental.Rendering.Universal;

namespace SonicCTK.Physical
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
        public Vector2 CameraOffset;
        
        private Vector2 currentLimit;
        private Vector2 focalPoint;
        private int freezeTicks;

        public void FreezeCamera(int TickCount)
        {
            freezeTicks = TickCount;
        }

        public void Start()
        {
            PixelPosition = Player.transform.position.WToP();
        }

        public void Follow()
        {
            if (!Player) return;

            if (freezeTicks > 0)
            {
                freezeTicks--;
                return;
            }

            var pixelPlayerPosition = ((Vector2)Player.transform.position).WToP() -
                                      new Vector2(0, (Player.Collision.DefaultCollisionSize.y - Player.Collision.PlayerCollisionSize.y) / 2);

            focalPoint = (pixelPlayerPosition + new Vector2(0, 16)) - PixelPosition;
            currentLimit = new Vector2(0, 0);
            
            if (focalPoint.x <= PPCam.refResolutionX / 2 - 16) 
                currentLimit.x = -Mathf.Clamp(PPCam.refResolutionX / 2 - 16 - focalPoint.x, 0, CameraSpeedLimit.x);
            else if (focalPoint.x >= PPCam.refResolutionX / 2) 
                currentLimit.x = Mathf.Clamp(focalPoint.x - PPCam.refResolutionX / 2, 0, CameraSpeedLimit.x);
            else currentLimit.x = 0;

            if (Player.Common.IsGrounded)
            {
                if (Mathf.Abs(Player.Common.Gsp) >= 8) CameraSpeedLimit.y = 16;
                else CameraSpeedLimit.y = 6;

                currentLimit.y = 
                    Mathf.Clamp(focalPoint.y - PPCam.refResolutionY / 2, 
                        -CameraSpeedLimit.y, CameraSpeedLimit.y);
            }

            else
            {
                CameraSpeedLimit.y = 16;
                if (focalPoint.y <= PPCam.refResolutionY / 2 - 16)
                    currentLimit.y = -Mathf.Clamp(PPCam.refResolutionY / 2 - 16 - focalPoint.y, 0, CameraSpeedLimit.y);
                else if (focalPoint.y > -(PPCam.refResolutionY / 2 + 48))
                    currentLimit.y = Mathf.Clamp(focalPoint.y - (PPCam.refResolutionY / 2 + 48), 0, CameraSpeedLimit.y);
                else currentLimit.y = 0;
            }

            PixelPosition += currentLimit;

            var targetCameraPoint = PixelPosition + CameraOffset.Round(); 
            
            targetCameraPoint.x = Mathf.Clamp(targetCameraPoint.x, 0, Stage.Current.StageSize.x - PPCam.refResolutionX);
            targetCameraPoint.y = Mathf.Clamp(targetCameraPoint.y, 0, Stage.Current.StageSize.y - PPCam.refResolutionY);
            
            transform.position = (Vector3)(targetCameraPoint + new Vector2(PPCam.refResolutionX / 2, PPCam.refResolutionY / 2)).PToW() + new Vector3(0, 0, -10);

            PixelPosition = targetCameraPoint - CameraOffset.Round();
        }
    }
}