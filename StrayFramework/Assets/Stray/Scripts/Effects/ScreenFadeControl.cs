using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stray.Core
{
    [ExecuteAlways]
    public class ScreenFadeControl : MonoBehaviour
    {
        public Shader shader;
        private Material curMaterial;

        [Range(0, 1)]
        public float Amount;

        Material material
        {
            // material getter checks for a material, creates one if it doesn't find one.
            get
            {
                if (curMaterial == null)
                {
                    curMaterial = new Material(shader);
                    curMaterial.hideFlags = HideFlags.HideAndDontSave;
                }

                return curMaterial;
            }
        }

        void Start()
        {

        }

        void Update()
        {

        }

        public void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            // access the render texture from the Unity Renderer.
            if (shader != null)
            {
                material.SetFloat("_Amount", Amount);
                Graphics.Blit(source, destination, material);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }
    }
}
