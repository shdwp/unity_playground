using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;

namespace BlockGame.Scripts.Views.Grid
{
    public class AttachedGridView: GridView
    {
        public override GridType GridType => GridType.Attached;
    }
}