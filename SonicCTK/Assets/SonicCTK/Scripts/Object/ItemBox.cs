using System.Collections;
using System.Collections.Generic;
using SonicCTK.Core;
using SonicCTK.Physical;
using SonicCTK.World;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SonicCTK.Gameplay
{
    public enum ItemBoxType
    {
        Ring, Shield, ElectricShield, FireShield, SpeedShoes, Invincibility
    };

    [RequireComponent(typeof(SolidBox), typeof(HitBox))]
    public class ItemBox : MonoBehaviour
    {
        public ItemBoxType BoxType;

        private SolidBox solidBox;
        private HitBox hitBox;
        private Player interactedPlayer;

        public bool IsFalling;
        public float Ysp;

        public bool IsMonitorInactive;

        [Header("Visual Stuff")]
        public Sprite DefaultMonitor;
        public Sprite InactiveMonitor;

        public SpriteRenderer MonitorIcon;
        public SpriteRenderer MonitorIconStatic;

        private Sprite MonitorIconDefault;

        private new SpriteRenderer renderer;

        private void Start()
        {
            solidBox = GetComponent<SolidBox>();
            hitBox = GetComponent<HitBox>();
            renderer = GetComponent<SpriteRenderer>();

            hitBox.OnOverlap += OnItemBoxOverlap;

            MonitorIconDefault = MonitorIcon.sprite;
        }

        private void Update()
        {
            if (!IsMonitorInactive)
            {
                MonitorIconStatic.enabled = (int)(Time.time * 8) % 2 == 0;
            }

            Player p = Stage.GetClosestPlayer((Vector2)transform.position);
            if (p.Common.IsGrounded)
            {
                solidBox.enabled = (!p.Is<PStateRoll>() && !p.Is<PStateJump>());
            }
            else
            {
                if (p.Common.Ysp >= 0) solidBox.enabled = !p.Common.IsCurledIntoBall;
                else solidBox.enabled = true;
            }

            if (IsMonitorInactive) solidBox.enabled = false;

            if (IsFalling)
            {
                Ysp += 0.21875f;
                transform.position += Vector3.down * Ysp.PToW();

                var FloorCheck = Physics2D.Raycast(transform.position, Vector2.down, 16f.PToW());
                if (FloorCheck)
                {
                    IsFalling = false;
                    Ysp = 0;
                    transform.position = FloorCheck.point + Vector2.up * 16f.PToW();
                }
            }
        }

        private IEnumerator IconSequence()
        {
            var frameCounter = 0;

            while(frameCounter < 24)
            {
                if (frameCounter == 4)
                {
                    renderer.sprite = IsMonitorInactive ? InactiveMonitor : DefaultMonitor;

                    transform.position += Vector3.up * 16f.PToW();
                    IsFalling = true;
                    Destroy(MonitorIconStatic.gameObject);
                }

                MonitorIcon.transform.position += Vector3.up * 0.02f;
                frameCounter++;
                yield return null;
            }

            // TODO: Apply monitor effect to interacted Player here
            switch(BoxType)
            {
                case ItemBoxType.Ring:
                    interactedPlayer.Storage.Rings += 10;
                    CAudio.Play("Ring");
                    break;
                case ItemBoxType.Shield:
                    interactedPlayer.Interactions.SetShield(PlayerShieldType.Normal);
                    break;
                case ItemBoxType.ElectricShield:
                    interactedPlayer.Interactions.SetShield(PlayerShieldType.Electric);
                    break;
            }
            
            yield return new WaitForSeconds(0.5f);

            Destroy(MonitorIcon.gameObject);
            yield return null;

        }

        public void DestroyItemBox()
        {
            if (solidBox.enabled) return;
            Addressables.InstantiateAsync("S_FX_Explosion.prefab", transform.position, Quaternion.identity);
            MonitorIcon.transform.SetParent(null);
            IsMonitorInactive = true;

            CAudio.Play("Break");

            StartCoroutine(IconSequence());
        }

        private void OnItemBoxOverlap(Player Player)
        {
            var PixelPlayerPos = ((Vector2)Player.transform.position).WToP();
            var PixelObjectPos = ((Vector2)transform.position).WToP() + hitBox.BoxOffset;

            if (IsMonitorInactive) return;

            if (!Player.Common.IsGrounded)
            {
                if (Player.Common.Ysp >= 0)
                {
                    interactedPlayer = Player;
                    Player.Common.Ysp *= -1;
                    DestroyItemBox();
                    return;
                }

                if (PixelPlayerPos.y >= PixelObjectPos.y + 16)
                {
                    if (Player.Common.Ysp < 0)
                    {
                        IsFalling = true;
                        Player.Common.Ysp *= -1;
                        Ysp = -1.5f;
                    }
                }
            }
            else
            {
                interactedPlayer = Player;

                DestroyItemBox();
            }
        }
    }

}