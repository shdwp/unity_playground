﻿using System.Collections.Generic;
using System.Linq;
using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using BlockGame.Scripts.Signals;
using BlockGame.Scripts.Signals.ToGridView;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace BlockGame.Scripts.Views.Grid
{
    public class DetachedGridViewMediator: Mediator
    {
        [Inject] public DetachedGridView view { get; set; }
        [Inject] public PlayerMoveDetachedGridSignal playerMove { get; set; }
        
        [Inject] public ReplaceGridInViewSignal<BlockDataModel> replaceGridInView { get; set; }
        [Inject] public MergeGridInViewSignal<BlockDataModel> mergeGridInView { get; set; }
        [Inject] public UpdateGridModelPositionSignal updateGridModelPosition { get; set; }

        public override void OnRegister()
        {
            view.positionUpdate.AddListener(pos =>
            {
                updateGridModelPosition.Dispatch(GridType.Detached, pos);
            });
            
            replaceGridInView.AddListener((type, centroid, list) =>
            {
                if (type == view.GridType)
                {
                    view.Setup(centroid, list.Select(a => new GridView.BlockViewItem(a.worldspacePos, Color.red)));
                }
            });
            
            mergeGridInView.AddListener((type, centroid, enumerable) =>
            {
                if (type == view.GridType)
                {
                    view.Merge(centroid, enumerable.Select(a => new GridView.BlockViewItem(a.worldspacePos, Color.red)));
                }
            });
            
            playerMove.AddListener(dir =>
            {
                switch (dir)
                {
                    case PlayerMoveDetachedGridSignal.Direction.Left:
                        view.Move(Vector3.left);
                        break;
                    
                    case PlayerMoveDetachedGridSignal.Direction.Right:
                        view.Move(Vector3.right);
                        break;
                    
                    case PlayerMoveDetachedGridSignal.Direction.Down:
                        view.Push();
                        break;
                }
            });
        }
    }
}