using System.Collections.Generic;
using System.Linq;
using BlockGame.Scripts.Controllers;
using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using BlockGame.Scripts.Signals;
using BlockGame.Scripts.Signals.FromView;
using BlockGame.Scripts.Signals.ToGridView;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace BlockGame.Scripts.Views.Grid
{
    public class AttachedGridViewMediator : Mediator
    {
        [Inject] public AttachedGridView view { get; set; }
        
        [Inject] public ReplaceGridInViewSignal<BlockDataModel> replaceGridInView { get; set; }
        [Inject] public MergeGridInViewSignal<BlockDataModel> mergeGridInView { get; set; }

        [Inject] public AttemptGridModelMoveSignal attemptGridModelMove { get; set; }

        public override void OnRegister()
        {
            replaceGridInView.AddListener((type, centroid, list) =>
            {
                if (type == view.GridType)
                {
                    view.Setup(centroid, list.Select(a => new GridView.BlockViewItem(a)));
                }
            });
            
            mergeGridInView.AddListener((type, centroid, enumerable) =>
            {
                if (type == view.GridType)
                {
                    view.Merge(centroid, enumerable.Select(a => new GridView.BlockViewItem(a)));
                }
            });
        }
    }
}