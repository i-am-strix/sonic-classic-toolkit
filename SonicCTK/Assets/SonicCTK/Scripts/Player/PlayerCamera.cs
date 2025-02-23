using SonicCTK.Core;
using SonicCTK.World;
using UnityEngine;
using UnityEngine.U2D;

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
        private Vector2 _currentLimit;
        private Vector2 _focalPoint;

        private int _freezeTicks;

        public void FreezeCamera(int TickCount)
        {
            _freezeTicks = TickCount;
        }

        public void Start()
        {
            PixelPosition = Player.transform.position.WToP();
        }

        public void Follow()
        {
            if (!Player) return;

            if (_freezeTicks > 0)
            {
                _freezeTicks--;
                return;
            }

            var pixelPlayerPosition = ((Vector2)Player.transform.position).WToP() -
                                      new Vector2(0, (Player.Collision.DefaultCollisionSize.y - Player.Collision.PlayerCollisionSize.y) / 2);

            _focalPoint = (pixelPlayerPosition + new Vector2(0, 16)) - PixelPosition;
            _currentLimit = new Vector2(0, 0);
            
            if (_focalPoint.x <= PPCam.refResolutionX / 2 - 16) 
                _currentLimit.x = -Mathf.Clamp(PPCam.refResolutionX / 2 - 16 - _focalPoint.x, 0, CameraSpeedLimit.x);
            else if (_focalPoint.x >= PPCam.refResolutionX / 2) 
                _currentLimit.x = Mathf.Clamp(_focalPoint.x - PPCam.refResolutionX / 2, 0, CameraSpeedLimit.x);
            else _currentLimit.x = 0;

            if (Player.Common.IsGrounded)
            {
                if (Mathf.Abs(Player.Common.Gsp) >= 8) CameraSpeedLimit.y = 16;
                else CameraSpeedLimit.y = 6;

                _currentLimit.y = 
                    Mathf.Clamp(_focalPoint.y - PPCam.refResolutionY / 2, 
                        -CameraSpeedLimit.y, CameraSpeedLimit.y);
            }

            else
            {
                CameraSpeedLimit.y = 16;
                if (_focalPoint.y <= PPCam.refResolutionY / 2 - 16)
                    _currentLimit.y = -Mathf.Clamp(PPCam.refResolutionY / 2 - 16 - _focalPoint.y, 0, CameraSpeedLimit.y);
                else if (_focalPoint.y > -(PPCam.refResolutionY / 2 + 48))
                    _currentLimit.y = Mathf.Clamp(_focalPoint.y - (PPCam.refResolutionY / 2 + 48), 0, CameraSpeedLimit.y);
                else _currentLimit.y = 0;
            }

            PixelPosition += _currentLimit;
            
            PixelPosition.x = Mathf.Clamp(PixelPosition.x, 0, Stage.Current.StageSize.x - PPCam.refResolutionX);
            PixelPosition.y = Mathf.Clamp(PixelPosition.y, 0, Stage.Current.StageSize.y - PPCam.refResolutionY);
            
            transform.position = (Vector3)(PixelPosition + new Vector2(PPCam.refResolutionX / 2, PPCam.refResolutionY / 2)).PToW() + new Vector3(0, 0, -10);}
    }
}