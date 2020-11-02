using System;
using System.Collections;
using UnityEngine;

namespace MatchTwoGame.Scripts.Tiles
{
    /// <summary>
    /// Behaviour for each tile, provides API to control it's appearance and animations
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Renderer))]
    public class TileBehaviour : MonoBehaviour
    {
        private Animator _animator;
        private bool _flipped = false;
        
        // whether behaviour is currently waiting for an animation event (used in outgoing coroutines)
        // @TODO: transition to bitflags
        private int _waitingForEvent;

        private Material _mat;
        private Renderer _renderer;

        // related shader property ids
        private int _sidAppearAnimation = Shader.PropertyToID("_AppearAnim");
        private int _sidLoadingAnimation = Shader.PropertyToID("_LoadingAnim");
        private int _sidPictogramTex = Shader.PropertyToID("_PictogramTex");

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _renderer = GetComponentInChildren<Renderer>();
            
            // create copy of material, for simplicity sake each tile will use separate material
            // @TODO: move to per-render properties?
            _mat = _renderer.material;
        }

        private void Start()
        {
            // by default tiles are not visible until scheme is loaded
            _renderer.enabled = false;
        }

        /// <summary>
        /// Set state to unloaded (tile is visible, but unloaded animation is playing)
        /// </summary>
        /// <param name="appearAnimationOffset">offset for its appear animation</param>
        public void SetUnloadedState(float appearAnimationOffset)
        {
            _mat.SetFloat(_sidAppearAnimation, Time.timeSinceLevelLoad + appearAnimationOffset);
            _renderer.enabled = true;
        }

        /// <summary>
        /// Set state to loaded (tile is visible, pictogram is visible)
        /// </summary>
        /// <param name="pictogramTex"></param>
        /// <param name="animateLoad">whether this transition will be animated (fade)</param>
        public void SetLoadedState(Texture2D pictogramTex, bool animateLoad)
        {
            _mat.SetTexture(_sidPictogramTex, pictogramTex);

            if (animateLoad)
            {
                _mat.SetFloat(_sidLoadingAnimation, Time.timeSinceLevelLoad);
            }
        }

        /// <summary>
        /// Flip tile (flips around no matter the state)
        /// </summary>
        /// <returns>coroutine which finishes when animation is finished</returns>
        public IEnumerator Flip()
        {
            return SetFlipped(!_flipped);
        }

        /// <summary>
        /// Set specific flipped value.
        /// </summary>
        /// <param name="value">true - pictogram not visible, false - visible</param>
        /// <returns></returns>
        public IEnumerator SetFlipped(bool value)
        {
            if (value != _flipped)
            {
                // update value
                _flipped = value;
                
                // setup variables for `FlipAnimationCompleted`
                var eventFlag = value ? 1 : 0;
                _waitingForEvent |= 1 << eventFlag;
                
                // start animation and wait for its completion event
                _animator.SetBool("Flipped", value);
                while ((_waitingForEvent & (1 << eventFlag)) > 0)
                {
                    yield return null;
                }
            }
        }

        /// <summary>
        /// Check if tile bounds contain world-space position
        /// </summary>
        /// <param name="worldspacePosition"></param>
        /// <returns></returns>
        public bool BoundsContainPosition(Vector3 worldspacePosition)
        {
            return _renderer.bounds.Contains(worldspacePosition);
        }

        /// <summary>
        /// Used in animation event. Will be fired in the start and end of the flip animation, with 0 and 1 as an argument.
        /// </summary>
        /// <param name="value"></param>
        public void FlipAnimationCompleted(int value)
        {
            _waitingForEvent &= ~(1 << value);
        }
    }
}