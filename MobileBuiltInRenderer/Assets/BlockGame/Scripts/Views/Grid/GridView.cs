using System;
using System.Collections.Generic;
using System.Linq;
using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using Lib;
using SnowplowGame.Scripts;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace BlockGame.Scripts.Views.Grid
{
    public abstract class GridView : View
    {
        public float moveSpeed;
        public float pushSpeed;
        public float fallSpeed;
        
        public abstract GridType GridType { get; }

        public Signal<Vector3> positionUpdate = new Signal<Vector3>();

        [Inject] public GameObjectPool pool { get; set; }

        public void Setup(IEnumerable<Vector3> positionsEnumerable, Color color)
        {
            var positions = positionsEnumerable.ToArray();
            var center = positions.Aggregate((a, b) => a + b) / positions.Length;
            
            foreach (var pos in positions)
            {
                var instance = pool.Dequeue(transform);
                var localPos = pos - center;
                instance.transform.position = new Vector3(localPos.x, localPos.y, 0f);
            }

            transform.position = new Vector3(center.x, center.y, transform.position.z);
        }

        public void Move(Vector3 direction)
        {
            transform.position += direction * (moveSpeed * Time.deltaTime);
        }

        public void Push()
        {
            transform.position += Vector3.down * (pushSpeed * Time.deltaTime);
        }

        private void Update()
        {
            transform.position += Vector3.down * (Time.deltaTime * fallSpeed);
            positionUpdate.Dispatch(transform.position);
        }
    }
}