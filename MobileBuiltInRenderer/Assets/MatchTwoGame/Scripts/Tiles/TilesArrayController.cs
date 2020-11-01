using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchTwoGame.Scripts.Tiles
{
    /// <summary>
    /// Controller that provides control over the array of tiles on the screen
    /// </summary>
    public class TilesArrayController : MonoBehaviour
    {
        /// <summary>
        /// Tiles in the scene
        /// </summary>
        public GameObject[] tiles;
        
        /// <summary>
        /// Mapping array (tile index -> pictogram index), used to load correct pictogram textures for tiles
        /// </summary>
        public int[] tilePictogramMapping;
        
        /// <summary>
        /// Amount of tiles
        /// </summary>
        public int tileCount => tiles.Length;

        // TileBehaviour components of each tile
        private TileBehaviour[] _tileBehaviours;

        private void Awake()
        {
            _tileBehaviours = new TileBehaviour[tiles.Length];
            
            // get behaviours from tiles
            for (int i = 0; i < tiles.Length; i++)
            {
                _tileBehaviours[i] = tiles[i].GetComponentInChildren<TileBehaviour>();
            }
        }

        /// <summary>
        /// Show tiles in unloaded state
        /// </summary>
        public void ShowUnloadedTiles()
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                _tileBehaviours[i].SetUnloadedState(i * 0.25f);
            }
        }

        /// <summary>
        /// Load specific tile with pictogram texture, transitioning it into loaded state
        /// </summary>
        /// <param name="pictogramIdx">pictogram idx</param>
        /// <param name="pictogramTex">texture</param>
        /// <param name="animateLoad">whether load will be animated (fade-in)</param>
        public void LoadTile(int pictogramIdx, Texture2D pictogramTex, bool animateLoad = true)
        {
            // find tiles matching with this pictogram idx
            for (int i = 0; i < tilePictogramMapping.Length; i++)
            {
                if (tilePictogramMapping[i] == pictogramIdx)
                {
                    // set correct state with provided texture
                    _tileBehaviours[i].SetLoadedState(pictogramTex, animateLoad);
                }
            }
        }

        /// <summary>
        /// Flip tile at index (flips around no matter the state)
        /// </summary>
        /// <param name="idx"></param>
        /// <returns>coroutine, which finishes when animations are complete</returns>
        public IEnumerator FlipTile(int idx)
        {
            return _tileBehaviours[idx].Flip();
        }

        /// <summary>
        /// Set flipped state of all tiles to specific value
        /// </summary>
        /// <param name="value">true - pictograms invisible, false - pictograms visible</param>
        /// <returns>coroutine, which finishes when animations are complete</returns>
        public IEnumerator SetAllTilesFlipped(bool value)
        {
            // start coroutines that flip each tile
            var coroutines = new List<Coroutine>();
            foreach (var animator in _tileBehaviours)
            {
                coroutines.Add(StartCoroutine(animator.SetFlipped(value)));
            }

            // wait on those coroutines
            foreach (var coroutine in coroutines)
            {
                yield return coroutine;
            }
        }

        /// <summary>
        /// Find out tile index on the world-space position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>index or null</returns>
        public int? TileIndexAtWorldspacePosition(Vector3 pos)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                if (_tileBehaviours[i].BoundsContainPosition(pos))
                {
                    return i;
                }
            }

            return null;
        }
    }
}