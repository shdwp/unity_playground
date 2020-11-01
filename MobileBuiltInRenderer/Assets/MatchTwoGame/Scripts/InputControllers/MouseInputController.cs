using System;
using MatchTwoGame.Scripts.Tiles;
using UnityEngine;

namespace MatchTwoGame.Scripts.InputControllers
{
    /// <summary>
    /// Very simple mouse controller class, provides events to scene controller when user clicks tiles
    /// </summary>
    public class MouseInputController : MonoBehaviour
    {
        // DI
        public MatchTwoGameController gameController;
        public TilesArrayController tilesArrayController;
        
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // get world-space point
                var pos = _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
                
                // figure out tile index from tiles controller
                var index = tilesArrayController.TileIndexAtWorldspacePosition(pos);

                if (index.HasValue)
                {
                    // send actual event if user hit the tile
                    gameController.UserTouchedTile(index.Value);
                }
            }
        }
    }
}