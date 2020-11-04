using System;
using System.Collections.Generic;
using System.Linq;
using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using BlockGame.Scripts.Signals.ToView;
using BlockGame.Scripts.Views.Block;
using Lib;
using SnowplowGame.Scripts;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace BlockGame.Scripts.Views.Grid
{
    /// <summary>
    /// Base view class for displaying grids.
    /// Provides API to setup blocks on the screen.
    /// </summary>
    public abstract class GridView : View
    {
        /// <summary>
        /// View level model struct for single block
        /// </summary>
        public struct BlockViewItem
        {
            /// <summary>
            /// Position
            /// </summary>
            public Vector3 pos;
            
            /// <summary>
            /// Color
            /// </summary>
            public Color color;

            /// <summary>
            /// Create view level model from controller-level model
            /// </summary>
            /// <param name="data"></param>
            public BlockViewItem(BaseToGridViewData<CellDataModel> data)
            {
                pos = data.worldspacePos;
                color = ColorFromModelDataColor(data.data.color);
            }
            
            /// <summary>
            /// Map color from model-layer to view-layer
            /// </summary>
            /// <param name="color"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentException"></exception>
            private static Color ColorFromModelDataColor(CellDataModel.Color color)
            {
                switch (color)
                {
                    case CellDataModel.Color.Blue:
                        return CorrelateColorForLight(Color.blue);
                    case CellDataModel.Color.Cyan:
                        return CorrelateColorForLight(Color.cyan);
                    case CellDataModel.Color.Green:
                        return CorrelateColorForLight(Color.green);
                    case CellDataModel.Color.Orange:
                        return CorrelateColorForLight(new Color(1f, 0.64f, 0f));
                    case CellDataModel.Color.Purple:
                        return CorrelateColorForLight(new Color(0.5f, 0f, 0.5f));
                    case CellDataModel.Color.Red:
                        return CorrelateColorForLight(Color.red);
                    case CellDataModel.Color.Yellow:
                        return CorrelateColorForLight(Color.yellow);
                    case CellDataModel.Color.Empty:
                        throw new ArgumentException("Method only accept defined block data!", "color");
                }
            
                return Color.clear;
            }

            /// <summary>
            /// In order for lighting to look better default colors should be correlated
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            private static Color CorrelateColorForLight(Color c)
            {
                return new Color(
                    Mathf.Min(c.r + 0.2f, 1f),
                    Mathf.Min(c.g + 0.2f, 1f),
                    Mathf.Min(c.b + 0.2f, 1f)
                );
            }
        }
        
        /// <summary>
        /// This view's grid type
        /// </summary>
        public abstract GridType GridType { get; }

        /// <summary>
        /// GameObjectPool instance from which block prefab instances will be dequeued. Shared between all grids.
        /// </summary>
        [Inject] public GameObjectPool pool { get; set; }

        /// <summary>
        /// Setup view with new set of blocks.
        /// Frees every existing view then sets up everything anew.
        /// </summary>
        /// <param name="centroid"></param>
        /// <param name="enumerable"></param>
        public void Setup(Vector3 centroid, IEnumerable<BlockViewItem> enumerable)
        {
            // free pool instances
            for (int i = 0; i < transform.childCount; i++)
            {
                pool.Free(transform.GetChild(i).gameObject);
            }
            
            // setup position and blocks
            transform.position = centroid;
            foreach (var item in enumerable)
            {
                SetupBlockViewWithItem(item);
            }
        }

        /// <summary>
        /// Merge view with another list of blocks.
        /// </summary>
        /// <param name="centroid"></param>
        /// <param name="enumerable"></param>
        public void Merge(Vector3 centroid, IEnumerable<BlockViewItem> enumerable)
        {
            // setup additional blocks
            foreach (var item in enumerable)
            {
                SetupBlockViewWithItem(item);
            }
        }

        /// <summary>
        /// Setup block view.
        ///
        /// Dequeues instance from object pool, sets it's parent to current view and
        /// setups its variables to match required cell.
        /// </summary>
        /// <param name="item"></param>
        private void SetupBlockViewWithItem(BlockViewItem item)
        {
            // get and re-parent the block prefab
            var instance = pool.Dequeue(transform);
            instance.transform.position = item.pos;

            // setup variables in BlockView
            var blockView = instance.GetComponentInChildren<BlockView>();
            blockView.Setup(item.color);
        }
    }
}