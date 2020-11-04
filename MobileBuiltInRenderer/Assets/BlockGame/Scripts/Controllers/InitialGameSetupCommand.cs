using System.Collections.Generic;
using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using BlockGame.Scripts.Signals;
using BlockGame.Scripts.Signals.ToGridView;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace BlockGame.Scripts.Controllers
{
    public class InitialGameSetupCommand : Command
    {
        [Inject] public ReplaceGridInViewSignal replaceGridInView { get; set; }
        [Inject] public MergeGridInViewSignal mergeGridInView { get; set; }
        
        [Inject] public IGameState gameState { get; set; }
        [Inject] public IGridTransform transform { get; set; }
        
        public override void Execute()
        {
            gameState.attachedGrid = injectionBinder.GetInstance<IPartialGrid>();
            gameState.attachedGrid.SetupFloor();
            
            gameState.detachedGrid = injectionBinder.GetInstance<IPartialGrid>();
            gameState.detachedGrid.Setup3x3(" x " +
                                            "xx " +
                                            "x  ");

            replaceGridInView.Dispatch(GridType.Attached, transform.TransformGridToWorldPoints(gameState.attachedGrid));
            replaceGridInView.Dispatch(GridType.Detached, transform.TransformGridToWorldPoints(gameState.detachedGrid));
        }
    }
}