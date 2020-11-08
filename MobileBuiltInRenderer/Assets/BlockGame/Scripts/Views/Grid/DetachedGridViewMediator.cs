using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using BlockGame.Scripts.Signals;
using BlockGame.Scripts.Signals.FromView;
using BlockGame.Scripts.Signals.ToGridView;
using BlockGame.Scripts.Views.Signals;
using strange.extensions.mediation.impl;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace BlockGame.Scripts.Views.Grid
{
    public class DetachedGridViewMediator: Mediator
    {
        [Inject] public DetachedGridView view { get; set; }
        
        [Inject] public PlayerMoveDetachedGridSignal playerMove { get; set; }
        
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
            
            playerMove.AddListener(dir =>
            {
                var translation = Vector3.zero;
                switch (dir)
                {
                    case PlayerMoveDetachedGridSignal.Direction.Left:
                        translation = Vector3.left * (view.moveSpeed * Time.deltaTime);
                        break;
                    
                    case PlayerMoveDetachedGridSignal.Direction.Right:
                        translation = Vector3.right * (view.moveSpeed * Time.deltaTime);
                        break;
                    
                    case PlayerMoveDetachedGridSignal.Direction.Down:
                        translation = Vector3.down * (view.pushSpeed * Time.deltaTime);
                        break;
                }

                var data = new AttemptGridModelMoveData(view.GridType, view.transform.position, translation);
                attemptGridModelMove.Dispatch(data, result =>
                {
                    if (result)
                    {
                        view.transform.position += translation;
                    }
                });
            });
        }

        private void Update()
        {
            var gravityTranslation = Vector3.down * (view.fallSpeed * Time.deltaTime);
            var data = new AttemptGridModelMoveData(view.GridType, view.transform.position, gravityTranslation);
            attemptGridModelMove.Dispatch(data, result =>
            {
                if (result)
                {
                    view.transform.position += gravityTranslation;
                }
            });
        }
    }
}