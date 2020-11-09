using System.Collections.Generic;
using BlockGame.Scripts.Model.Interfaces;
using strange.extensions.signal.impl;
using UnityEngine;

namespace BlockGame.Scripts.Signals.ToView
{
    /// <summary>
    /// Struct containing information about individual cell (called block in view layer) at controller-level (passed to view-level)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct BaseToGridViewData<T>
    {
        /// <summary>
        /// World-space cell position
        /// </summary>
        public Vector3 worldspacePos;
        
        /// <summary>
        /// Cell specific data
        /// </summary>
        public T data;

        public BaseToGridViewData(Vector3 worldspacePos, T data)
        {
            this.worldspacePos = worldspacePos;
            this.data = data;
        }
    }
    
    /// <summary>
    /// Base class for signal providing updated information about the grid to the views.
    /// First parameter is grid type, second is world-space grid centroid, and the third is
    /// enumerable containing individual cells data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseToGridViewSignal<T>: Signal<GridType, Vector3, IEnumerable<BaseToGridViewData<T>>>
    {
        
    }
}