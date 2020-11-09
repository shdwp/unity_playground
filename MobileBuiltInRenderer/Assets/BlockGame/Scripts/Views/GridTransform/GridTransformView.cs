using strange.extensions.mediation.impl;
using UnityEngine;

namespace BlockGame.Scripts.Views.GridTransform
{
    /// <summary>
    /// Simple view used to setup grid size and bounds in the editor
    /// </summary>
    public class GridTransformView: View
    {
        public Bounds bounds;
        public int rows, cols;
    }
}