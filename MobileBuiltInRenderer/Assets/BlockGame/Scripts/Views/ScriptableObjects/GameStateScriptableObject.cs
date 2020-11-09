using BlockGame.Scripts.Contexts;
using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using UnityEngine;

namespace BlockGame.Scripts.Views.ScriptableObjects
{
    /// <summary>
    /// Scriptable object used with `ScriptableObjectPersistentStateimpl`.
    /// </summary>
    [CreateAssetMenu(fileName = "Game State Data", menuName = "ScriptableObjects/GameStateData")]
    public class GameStateScriptableObject: ScriptableObject
    {
        /// <summary>
        /// Whether object has any stored data
        /// </summary>
        public bool hasData;
        
        /// <summary>
        /// Spawner type that was previously used
        /// </summary>
        public GridSpawnerType spawnerType;

        // attached grid data
        public CellDataModel[] attachedGridData;
        public int attachedGridRows, attachedGridCols;
        public GridPosition attachedGridPos;
        
        // detached grid data
        public CellDataModel[] detachedGridData;
        public int detachedGridRows, detachedGridCols;
        public GridPosition detachedGridPos;
    }
}