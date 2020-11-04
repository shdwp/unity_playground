using BlockGame.Scripts.Model.Interfaces;
using UnityEngine;

namespace BlockGame.Scripts.Model
{
    public class GameStateImpl: IGameState
    {
        [Inject] public IPartialGrid attachedGrid { get; set; }
        [Inject] public IPartialGrid detachedGrid { get; set; }

        public bool TestAndApplyDetachedGridCollisions()
        {
            if (detachedGrid.DoesCollideWith(attachedGrid))
            {
                attachedGrid.Merge(detachedGrid);
                return true;
            }
            else
            {
                return false;
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