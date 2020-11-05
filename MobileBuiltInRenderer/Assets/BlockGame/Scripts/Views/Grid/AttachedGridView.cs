using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using UnityEngine;

namespace BlockGame.Scripts.Views.Grid
{
    public class AttachedGridView: GridView
    {
        public override GridType GridType => GridType.Attached;

    }
}