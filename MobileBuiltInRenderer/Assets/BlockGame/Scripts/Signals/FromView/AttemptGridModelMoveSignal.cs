using System;
using BlockGame.Scripts.Model.Interfaces;
using strange.extensions.signal.impl;
using UnityEngine;

namespace BlockGame.Scripts.Signals.FromView
{
    /// <summary>
    /// Data struct for the signal
    /// </summary>
    public struct AttemptGridModelMoveData
    {
        /// <summary>
        /// Type of the grid
        /// </summary>
        public GridType type;
        
        /// <summary>
        /// Current world-space position
        /// </summary>
        public Vector3 position;
        
        /// <summary>
        /// Translation that is being applied to the grid
        /// </summary>
        public Vector3 translation;

        public AttemptGridModelMoveData(GridType type, Vector3 position, Vector3 translation)
        {
            this.type = type;
            this.position = position;
            this.translation = translation;
        }
    }
    
    /// <summary>
    /// Signal model classes that view attempts to move the grid.
    /// Besides data also accepts callback. Model class will respond to it indicating whether this
    /// move can actually be performed. View should always follow this callback and do not change its contents until
    /// move has been confirmed with `true`.
    /// </summary>
    public class AttemptGridModelMoveSignal: Signal<AttemptGridModelMoveData, Action<bool>>
    {
        
    }
}