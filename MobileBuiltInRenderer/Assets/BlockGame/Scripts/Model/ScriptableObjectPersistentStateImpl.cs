using BlockGame.Scripts.Contexts;
using BlockGame.Scripts.Model.Interfaces;
using BlockGame.Scripts.Views.ScriptableObjects;

namespace BlockGame.Scripts.Model
{
    /// <summary>
    /// Default persistency implementation
    /// </summary>
    public class ScriptableObjectPersistentStateImpl: IIGamePersistentState
    {
        /// <summary>
        /// Spawner type 
        /// </summary>
        public GridSpawnerType spawnerType { get; set; }
        
        public bool canContinue => scriptableObject.hasData;
        
        [Inject] public GameStateScriptableObject scriptableObject { get; set; }

        public void StoreState(IGameFieldState state, GridSpawnerType spawnerType)
        {
            // store overall game state
            scriptableObject.hasData = true;
            scriptableObject.spawnerType = spawnerType;
            
            // store attached grid
            state.attachedGrid.StoreData(
                out scriptableObject.attachedGridData, 
                out scriptableObject.attachedGridRows, 
                out scriptableObject.attachedGridCols, 
                out scriptableObject.attachedGridPos
            );
            
            // store detached grid
            state.detachedGrid.StoreData(
                out scriptableObject.detachedGridData, 
                out scriptableObject.detachedGridRows, 
                out scriptableObject.detachedGridCols, 
                out scriptableObject.detachedGridPos
            );
        }

        public void StoreSpawnerType(GridSpawnerType type)
        {
            spawnerType = type;
            scriptableObject.spawnerType = type;
        }

        public void RestoreState(IGameFieldState state)
        {
            // @NOTE: since spawner type should already be mapped in DI it's
            // "restored" as a part of context startup and not here
            // @TODO: research possibility of this class binding the spawner instead of context
            
            // restore attached grid
            state.attachedGrid.RestoreData(
                scriptableObject.attachedGridData,
                scriptableObject.attachedGridRows,
                scriptableObject.attachedGridCols,
                scriptableObject.attachedGridPos
            );
            
            // restore detached grid
            state.detachedGrid.RestoreData(
                scriptableObject.detachedGridData,
                scriptableObject.detachedGridRows,
                scriptableObject.detachedGridCols,
                scriptableObject.detachedGridPos
            );
        }

        public void ClearState()
        {
            // data is not actually removed, only flag is set
            scriptableObject.hasData = false;
        }
    }
}