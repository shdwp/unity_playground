using System;
using BlockGame.Scripts.Model.Interfaces;
using strange.extensions.signal.impl;
using UnityEngine;

namespace BlockGame.Scripts.Signals.FromView
{
    public struct AttemptGridModelMoveData
    {
        public GridType type;
        public Vector3 position;
        public Vector3 translation;

        public AttemptGridModelMoveData(GridType type, Vector3 position, Vector3 translation)
        {
            this.type = type;
            this.position = position;
            this.translation = translation;
        }
    }
    
    public class AttemptGridModelMoveSignal: Signal<AttemptGridModelMoveData, Action<bool>>
    {
        
    }
}