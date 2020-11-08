using System;
using System.Collections.Generic;
using System.Linq;
using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using BlockGame.Scripts.Signals.ToGridView;
using BlockGame.Scripts.Signals.ToView;
using BlockGame.Scripts.Views.Block;
using Lib;
using SnowplowGame.Scripts;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace BlockGame.Scripts.Views.Grid
{
    public abstract class GridView : View
    {
        public struct BlockViewItem
        {
            public Vector3 pos;
            public Color color;

            public BlockViewItem(BaseToGridViewData<BlockDataModel> data)
            {
                pos = data.worldspacePos;
                color = ColorFromModelDataColor(data.data.color);
            }
            
            private static Color ColorFromModelDataColor(BlockDataModel.Color color)
            {
                switch (color)
                {
                    case BlockDataModel.Color.Blue:
                        return BrightenColorForLight(Color.blue);
                    case BlockDataModel.Color.Cyan:
                        return BrightenColorForLight(Color.cyan);
                    case BlockDataModel.Color.Green:
                        return BrightenColorForLight(Color.green);
                    case BlockDataModel.Color.Orange:
                        return BrightenColorForLight(new Color(1f, 0.64f, 0f));
                    case BlockDataModel.Color.Purple:
                        return BrightenColorForLight(new Color(0.5f, 0f, 0.5f));
                    case BlockDataModel.Color.Red:
                        return BrightenColorForLight(Color.red);
                    case BlockDataModel.Color.Yellow:
                        return BrightenColorForLight(Color.yellow);
                    case BlockDataModel.Color.Empty:
                        throw new ArgumentException("Method only accept defined block data!", "color");
                }
            
                return Color.clear;
            }

            private static Color BrightenColorForLight(Color c)
            {
                return new Color(
                    Mathf.Min(c.r + 0.2f, 1f),
                    Mathf.Min(c.g + 0.2f, 1f),
                    Mathf.Min(c.b + 0.2f, 1f)
                );
            }
        }
        
        public abstract GridType GridType { get; }

        public Signal<Vector3> positionUpdate = new Signal<Vector3>();

        [Inject] public GameObjectPool pool { get; set; }

        public void Setup(Vector3 centroid, IEnumerable<BlockViewItem> enumerable)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                pool.Free(transform.GetChild(i).gameObject);
            }
            
            transform.position = centroid;
            foreach (var item in enumerable)
            {
                SetupBlockViewWithItem(item);
            }
        }

        public void Merge(Vector3 centroid, IEnumerable<BlockViewItem> enumerable)
        {
            foreach (var item in enumerable)
            {
                SetupBlockViewWithItem(item);
            }
        }

        private void SetupBlockViewWithItem(BlockViewItem item)
        {
            var instance = pool.Dequeue(transform);
            instance.transform.position = item.pos;

            var blockView = instance.GetComponentInChildren<BlockView>();
            blockView.Setup(item.color);
        }
    }
}