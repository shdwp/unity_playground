using BlockGame.Scripts.Controllers;
using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using BlockGame.Scripts.Signals;
using BlockGame.Scripts.Signals.ToGridView;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace BlockGame.Scripts.Views.Grid
{
    public class GridViewMediator : Mediator
    {
        [Inject] public DetachedGridView view { get; set; }
        
        [Inject] public ReplaceGridInViewSignal replaceGridView { get; set; }
        [Inject] public UpdateGridModelPositionSignal updateGridModelPosition { get; set; }

        public override void OnRegister()
        {
            view.positionUpdate.AddListener(pos =>
            {
                updateGridModelPosition.Dispatch(GridType.Detached, pos);
            });
            
            replaceGridView.AddListener((type, points, color) =>
            {
                if (type == view.GridType)
                {
                    view.Setup(points, Color.blue);
                }
            });
        }
    }
}