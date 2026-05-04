using System.Collections;
using System.Collections.Generic;
using SonicCTK.Core;
using SonicCTK.Physical;
using SonicCTK.World;
using UnityEngine;

namespace SonicCTK
{
    public class ScoreCard : MonoBehaviour
    {
        // v Pro Tip, never do this. I just got lazy...
        public Sprite Score100Sprite, Score200Sprite, Score500Sprite, Score1000Sprite, Score10000Sprite;
        private SpriteRenderer renderer;
        
        private IEnumerator Start()
        {
            renderer = GetComponent<SpriteRenderer>();
            
            renderer.sprite = Stage.LatestGrantedScore switch
            {
                ScoreGrantAmount.Score100 => Score100Sprite,
                ScoreGrantAmount.Score200 => Score200Sprite,
                ScoreGrantAmount.Score500 => Score500Sprite,
                ScoreGrantAmount.Score1000 => Score1000Sprite,
                ScoreGrantAmount.Score10000 => Score10000Sprite,
                _ => null
            };
            
            var frameCounter = 0;

            while(frameCounter < 24)
            {
                transform.position += Vector3.up * 0.02f;
                frameCounter++;
                yield return null;
            }
            
            Destroy(gameObject);
            yield return null;
        }
    }
}
