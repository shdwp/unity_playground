using strange.extensions.signal.impl;
using UnityEngine;

namespace BlockGame.Scripts.Signals
{
    public class SetupGridTransformSignal: Signal<Bounds, SetupGridTransformSignal.GridSize>
    {
        public struct GridSize
        {
            public int rows, cols;
        }
    }
}