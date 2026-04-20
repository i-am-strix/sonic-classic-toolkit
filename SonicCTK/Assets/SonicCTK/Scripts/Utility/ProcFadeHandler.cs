using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SonicCTK.Core
{
    /// <summary>
    /// On the fly fade-in-fade-out system. Useful for sequencing stuff or for stalling things out (totally didn't yoink from hoyo)
    /// </summary>
    public class ProcFadeHandler : MonoBehaviour
    {
        private struct ProcFadeEventPassthroughData
        {
            public float fadeInStrength;
            public float holdFadeDuration;
            public float fadeOutStrength;
            public Action fadeOutEvent;
            public Color fadeColor;
        }
        
        private static bool createdInstance = false;
        public static bool alreadyHasEnqueued = false;

        public static ProcFadeHandler Instance { get; set;  }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Main()
        {
            if (!createdInstance)
            {
                Instance = new GameObject("GenProcFadeHandler").AddComponent<ProcFadeHandler>();
                DontDestroyOnLoad(Instance.gameObject);
                createdInstance = true;
            }
        }

        public static void RequestFade(float fadeInStrength, float holdFadeDuration, float fadeOutStrength,
            Action fadeOutEvent)
        {
            RequestFade(fadeInStrength, holdFadeDuration, fadeOutStrength, fadeOutEvent, Color.black);
        }

        public static void RequestFade(float fadeInStrength, float holdFadeDuration, float fadeOutStrength,
            Action fadeOutEvent, Color fadeColor)
        {
            if (alreadyHasEnqueued) return;

            var container = Instance.GenerateFadeCanvasTuple();
            ProcFadeEventPassthroughData data = new()
            {
                fadeInStrength = fadeInStrength,
                holdFadeDuration = holdFadeDuration,
                fadeColor = fadeColor,
                fadeOutStrength = fadeOutStrength,
                fadeOutEvent = fadeOutEvent
            };
            alreadyHasEnqueued = true;
            Instance.StartCoroutine(Instance.EventSequence(container, data));
            
        }

        private IEnumerator EventSequence((GameObject, Canvas, CanvasGroup, Image) input, ProcFadeEventPassthroughData data)
        {
            input.Item3.alpha = 0;
            input.Item4.color = data.fadeColor;

            while (input.Item3.alpha < 1)
            {
                input.Item3.alpha = Mathf.MoveTowards(input.Item3.alpha, 1, data.fadeInStrength * Time.unscaledDeltaTime);
                yield return null;
            }

            yield return new WaitForSecondsRealtime(data.holdFadeDuration);
            data.fadeOutEvent();
            yield return new WaitForEndOfFrame();
            
            while (input.Item3.alpha > 0)
            {
                input.Item3.alpha = Mathf.MoveTowards(input.Item3.alpha, 0, data.fadeOutStrength * Time.unscaledDeltaTime);
                yield return null;
            }
            
            Destroy(input.Item1);
            
            alreadyHasEnqueued = false;
        }

        private (GameObject, Canvas, CanvasGroup, Image) GenerateFadeCanvasTuple()
        {
            GameObject canvasRoot = new GameObject("Root");
            canvasRoot.transform.SetParent(transform);
            
            Canvas canvas = canvasRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 512;
            
            var canvasScaler = canvasRoot.AddComponent<CanvasScaler>();

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);

            CanvasGroup canvasGroup = canvasRoot.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0;

            GameObject imageRoot = new GameObject("Fade", typeof(RectTransform));
            Image image = imageRoot.AddComponent<Image>();
            image.color = Color.black;
            
            imageRoot.transform.SetParent(canvasRoot.transform);

            imageRoot.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            imageRoot.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            imageRoot.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            imageRoot.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            imageRoot.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

            
            return (canvasRoot, canvas, canvasGroup, image);
        }
    }
}