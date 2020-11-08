using System.Collections.Generic;
using BlockGame.Scripts.Model.Interfaces;

namespace BlockGame.Scripts.Model.GridSpawners
{
    public class GrabBagSpawnerImpl: BaseRandomSpawnerImpl
    {
        protected override int SeedMin => 0;
        protected override int SeedMax => _grabBag.Count - 1;

        public override GridSpawnerType type => GridSpawnerType.GrabBag;

        private List<string> _grabBag;
        
        public GrabBagSpawnerImpl()
        {
            _grabBag = new List<string>(AllFigures);
        }
        
        protected override string NextFigureWithSeed(int seed)
        {
            var figure = _grabBag[seed];
            _grabBag.RemoveAt(seed);
            if (_grabBag.Count == 0)
            {
                _grabBag = new List<string>(AllFigures);
            }

            return figure;
        }
    }
}