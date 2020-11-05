using System;
using System.Collections.Generic;
using System.Linq;
using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using BlockGame.Scripts.Signals.ToGridView;
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

            public BlockViewItem(Vector3 pos, Color color)
            {
                this.pos = pos;
                this.color = color;
            }
        }
        
        public abstract GridType GridType { get; }

        public Signal<Vector3> positionUpdate = new Signal<Vector3>();

        [Inject] public GameObjectPool pool { get; set; }

        public void Setup(Vector3 centroid, IEnumerable<BlockViewItem> enumerable)
        {
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
            var localPos = item.pos - transform.position;
            instance.transform.position = new Vector3(localPos.x, localPos.y, 0f);
        }
    }
}