using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using strange.extensions.command.impl;
using strange.extensions.context.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace BlockGame.Scripts.Controllers
{
    public class UpdateGridModelPositionCommand : Command
    {
        [Inject] public GridType type { get; set; }
        [Inject] public Vector3 pos { get; set; }
        
        [Inject] public IGameState state { get; set; }
        [Inject] public IGridTransform transform { get; set; }
        
        public override void Execute()
        {
            var gridPos = transform.WorldToGrid(pos);
            Debug.Log(gridPos);
        }
    }
}