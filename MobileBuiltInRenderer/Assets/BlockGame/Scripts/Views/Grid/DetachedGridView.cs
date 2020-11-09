using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using UnityEngine;

namespace BlockGame.Scripts.Views.Grid
{
    /// <summary>
    /// View for detached grid, provides movement parameters editable from the editor
    /// </summary>
    public class DetachedGridView: GridView
    {
        public float moveSpeed;
        public float pushSpeed;
        public float fallSpeed;
        
        public override GridType GridType => GridType.Detached;
    }
}