using BlockGame.Scripts.Model.Interfaces;
using BlockGame.Scripts.Views.ScriptableObjects;

namespace BlockGame.Scripts.Model
{
    public class GamePersistentStateImpl: IIGamePersistentState
    {
        public GridSpawnerType spawnerType { get; set; }
        public bool canContinue => scriptableObject.hasData;
        
        [Inject] public GameStateScriptableObject scriptableObject { get; set; }

        public void StoreState(IGameState state, GridSpawnerType spawnerType)
        {
            scriptableObject.hasData = true;
            scriptableObject.spawnerType = spawnerType;
            
            state.attachedGrid.StoreData(
                out scriptableObject.attachedGridData, 
                out scriptableObject.attachedGridRows, 
                out scriptableObject.attachedGridCols, 
                out scriptableObject.attachedGridPos
            );
            
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
        }

        public void RestoreState(IGameState state)
        {
            state.attachedGrid.RestoreData(
                scriptableObject.attachedGridData,
                scriptableObject.attachedGridRows,
                scriptableObject.attachedGridCols,
                scriptableObject.attachedGridPos
            );
            
            state.detachedGrid.RestoreData(
                scriptableObject.detachedGridData,
                scriptableObject.detachedGridRows,
                scriptableObject.detachedGridCols,
                scriptableObject.detachedGridPos
            );
        }

        public void ClearState()
        {
            scriptableObject.hasData = false;
        }
    }
}