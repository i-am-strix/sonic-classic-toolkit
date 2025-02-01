using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stray.Core;
using Stray.Physical;
using Stray.World;

namespace Stray.Gameplay
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
            if (p.Cmn.IsGrounded)
            {
                solidBox.enabled = (!p.Is<WPStateRoll>() && !p.Is<WPStateJump>());
            }
            else
            {
                if (p.Cmn.Ysp >= 0) solidBox.enabled = p.Visual.CurrentAnimation != "Spin";
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

            // TODO: Apply monitor effect to interactedPlayer here
            switch(BoxType)
            {
                case ItemBoxType.Ring:
                    Stage.Current.RingsCollected += 10;
                    WAudio.Play("Ring");
                    break;
            }
            
            yield return new WaitForSeconds(0.5f);

            Destroy(MonitorIcon.gameObject);
            yield return null;

        }

        public void DestroyItemBox()
        {
            if (solidBox.enabled) return;
            ObjectReferenceContainer.Spawn("Explosion", transform.position);
            MonitorIcon.transform.SetParent(null);
            IsMonitorInactive = true;

            WAudio.Play("Break");

            StartCoroutine(IconSequence());
        }

        private void OnItemBoxOverlap(Player Player)
        {
            var PixelPlayerPos = ((Vector2)Player.transform.position).WToP();
            var PixelObjectPos = ((Vector2)transform.position).WToP() + hitBox.boxOffset;

            if (IsMonitorInactive) return;

            if (!Player.Cmn.IsGrounded)
            {
                if (Player.Cmn.Ysp >= 0)
                {
                    interactedPlayer = Player;
                    Player.Cmn.Ysp *= -1;
                    DestroyItemBox();
                    return;
                }

                if (PixelPlayerPos.y >= PixelObjectPos.y + 16)
                {
                    if (Player.Cmn.Ysp < 0)
                    {
                        IsFalling = true;
                        Player.Cmn.Ysp *= -1;
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