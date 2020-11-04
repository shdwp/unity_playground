using strange.extensions.mediation.impl;
using UnityEngine;

namespace BlockGame.Scripts.Views.Block
{
    [RequireComponent(typeof(Renderer))]
    public class BlockView : View
    {
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
        private float _uvOffset;
        private MaterialPropertyBlock _matBlock;

        private int _sidUvOffset = Shader.PropertyToID("_UVOffset");

        protected override void Awake()
        {
            _renderer = GetComponentInChildren<Renderer>();
            _matBlock = new MaterialPropertyBlock();
        }
    }
}