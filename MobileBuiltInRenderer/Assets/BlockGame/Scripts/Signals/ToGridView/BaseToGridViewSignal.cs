using System.Collections.Generic;
using BlockGame.Scripts.Model.Interfaces;
using strange.extensions.signal.impl;
using UnityEngine;

namespace BlockGame.Scripts.Signals.ToGridView
{
    public struct BaseToGridViewData<T>
    {
        public Vector3 worldspacePos;
        public T data;

        public BaseToGridViewData(Vector3 worldspacePos, T data)
        {
            this.worldspacePos = worldspacePos;
            this.data = data;
        }
    }
    
    public class BaseToGridViewSignal<T>: Signal<GridType, Vector3, IEnumerable<BaseToGridViewData<T>>>
    {
        
    }
}