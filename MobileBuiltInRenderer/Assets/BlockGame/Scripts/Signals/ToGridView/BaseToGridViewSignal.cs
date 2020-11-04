using System.Collections.Generic;
using BlockGame.Scripts.Model.Interfaces;
using strange.extensions.signal.impl;
using UnityEngine;

namespace BlockGame.Scripts.Signals.ToGridView
{
    public class BaseToGridViewSignal: Signal<GridType, IEnumerable<Vector3>, BlockColor>
    {
        
    }
}