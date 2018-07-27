using System;
using UnityEngine;

namespace svtz.Tanks.Camera
{
    public class DisplacementEffect : MonoBehaviour
    {
#pragma warning disable 0649
        public float DisplacementPower;
        public RenderTexture DisplacementTex;
#pragma warning restore 0649

        private Material _material;

        private void Awake()
        {
            _material = new Material(Shader.Find("Hidden/DisplacementEffect"));
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (Math.Abs(DisplacementPower) < float.Epsilon)
            {
                Graphics.Blit(source, destination);
                return;
            }

            _material.SetTexture("_DisplacementTex", DisplacementTex);
            _material.SetFloat("_DisplacementPower", DisplacementPower);
            Graphics.Blit(source, destination, _material);
        }
    }
}