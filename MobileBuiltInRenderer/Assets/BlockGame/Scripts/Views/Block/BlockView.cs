using strange.extensions.mediation.impl;
using UnityEngine;

namespace BlockGame.Scripts.Views.Block
{
    /// <summary>
    /// View for each individual block (cells on model level).
    /// </summary>
    public class BlockView : View
    {
        /// <summary>
        /// UV offset for normal and smoothness maps.
        /// </summary>
        public float uvOffset
        {
            set
            {
                _uvOffset = value;
                
                _matBlock.SetFloat(_sidUvOffset, _uvOffset);
                _renderer.SetPropertyBlock(_matBlock);
            }
        }

        private Renderer _renderer;
        private Light _light;
        private float _uvOffset;
        private MaterialPropertyBlock _matBlock;

        private int _sidUvOffset = Shader.PropertyToID("_UVOffset");

        protected override void Awake()
        {
            _renderer = GetComponentInChildren<Renderer>();
            _light = GetComponentInChildren<Light>();
            _matBlock = new MaterialPropertyBlock();
        }

        /// <summary>
        /// Setup block with specific color and random UV offset
        /// </summary>
        /// <param name="color"></param>
        public void Setup(Color color)
        {
            uvOffset = Random.Range(0f, 1f);
            _light.color = color;
        }
    }
}