using BlockGame.Scripts.Model.Interfaces;
using BlockGame.Scripts.Signals;
using BlockGame.Scripts.Signals.FromView;
using strange.extensions.command.impl;
using UnityEngine;

namespace BlockGame.Scripts.Controllers.FromView
{
    /// <summary>
    /// Command to setup IGridTransform with values from editor
    /// </summary>
    public class SetupGridTransformCommand: Command
    {
        [Inject] public Bounds bounds { get; set; }
        [Inject] public SetupGridTransformSignal.GridSize size { get; set; }
        
        [Inject] public IGridTransform transform { get; set; }

        public override void Execute()
        {
            transform.Setup(bounds, size.rows, size.cols);
        }
    }
}