using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using UnityEngine;

namespace BlockGame.Scripts.Views.Grid
{
    public class DetachedGridView: GridView
    {
        public float moveSpeed;
        public float pushSpeed;
        public float fallSpeed;
        
        public override GridType GridType => GridType.Detached;
    }
}