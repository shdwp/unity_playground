using strange.extensions.signal.impl;
using UnityEngine;

namespace BlockGame.Scripts.Signals.FromView
{
    /// <summary>
    /// Fired when grid parameters has been received from the editor
    /// </summary>
    public class SetupGridTransformSignal: Signal<Bounds, SetupGridTransformSignal.GridSize>
    {
        public struct GridSize
        {
            public int rows, cols;
        }
    }
}