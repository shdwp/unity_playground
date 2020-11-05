using BlockGame.Scripts.Model.Interfaces;
using UnityEngine;

namespace BlockGame.Scripts.Model
{
    public class GameStateImpl: IGameState
    {
        [Inject] public IPartialGrid<BlockDataModel> attachedGrid { get; set; }
        [Inject] public IPartialGrid<BlockDataModel> detachedGrid { get; set; }

        public IPartialGrid<BlockDataModel> TestAndApplyDetachedGridCollisions()
        {
            if (detachedGrid.DoesCollideWith(attachedGrid))
            {
                var grid = detachedGrid;
                grid.Translate(2, 0);
                attachedGrid.Merge(grid);

                return grid;
            }
            else
            {
                return null;
            }
        }
        
        /*
        private void TestCollisions()
        {
            for (int row = 0; row < transform.rows; row++)
            {
                for (int cell = 0; cell < transform.cells; cell++)
                {
                    if (_attachedGrid[row][cell] > 0 && _detachedGrid[row][cell] > 0)
                    {
                        
                        break;
                    }
                }
            }
        }
        */
    }
}