using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using UnityEngine;

namespace BlockGame.Scripts.Views.Grid
{
    /// <summary>
    /// View class for attached grid. Attached (ground) grid is always static and lacking any move methods.
    /// </summary>
    public class AttachedGridView: GridView
    {
        public override GridType GridType => GridType.Attached;

    }
}