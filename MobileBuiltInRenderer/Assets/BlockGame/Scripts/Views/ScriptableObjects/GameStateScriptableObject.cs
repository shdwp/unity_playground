using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using UnityEngine;

namespace BlockGame.Scripts.Views.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Game State Data", menuName = "ScriptableObjects/GameStateData")]
    public class GameStateScriptableObject: ScriptableObject
    {
        public bool hasData;
        
        public GridSpawnerType spawnerType;

        public BlockDataModel[] attachedGridData;
        public int attachedGridRows, attachedGridCols;
        public GridPosition attachedGridPos;
        
        public BlockDataModel[] detachedGridData;
        public int detachedGridRows, detachedGridCols;
        public GridPosition detachedGridPos;
    }
}