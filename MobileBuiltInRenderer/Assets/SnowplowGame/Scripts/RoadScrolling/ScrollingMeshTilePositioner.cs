using UnityEngine;

namespace SnowplowGame.Scripts.RoadScrolling
{
    /// <summary>
    /// Class that positions two mesh tiles so it appears as they are moving with constant speed.
    /// Tiles are in constant movement, and as one goes out of the visible area it is moved to the top of the visible one.
    /// </summary>
    public class ScrollingMeshTilePositioner : MonoBehaviour
    {
        public GameObject tileA, tileB;
        private GameObject _visibleTile;

        private void Start()
        {
            _visibleTile = tileA;
        }

        /// <summary>
        /// Advance tiles by distance. Moves the tiles, swapping them around if needed
        /// </summary>
        /// <param name="distance"></param>
        public void Advance(float distance)
        {
            tileA.transform.position += Vector3.right * (-distance * Time.deltaTime);
            tileB.transform.position += Vector3.right * (-distance * Time.deltaTime);
            
            if (_visibleTile.transform.position.x <= 0f)
            {
                // swap visible tile
                _visibleTile = _visibleTile == tileA ? tileB : tileA;
                
                // @TODO: refactor to use mesh bounds 
                _visibleTile.transform.position = new Vector3(170f, 0f, 0f);
            }
        }
    }
}