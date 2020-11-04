using System.Collections.Generic;
using BlockGame.Scripts.Contexts;
using BlockGame.Scripts.Model.Interfaces;

namespace BlockGame.Scripts.Model.GridSpawners
{
    /// <summary>
    /// Implementation of "grab bag" spawner, persumably original tetris algorithm for tetromino spawning.
    /// Keeps track of yielded figures, ensuring that every figure will eventually appear.
    ///
    /// Seed will change based on internal grab bag count.
    /// </summary>
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
            // grab random figure from the bag
            var figure = _grabBag[seed];
            
            // remove it from the bag 
            _grabBag.RemoveAt(seed);
            
            if (_grabBag.Count == 0)
            {
                // if bag is empty as a result - repopulate it with all figures
                // starting the cycle anew
                _grabBag = new List<string>(AllFigures);
            }

            return figure;
        }
    }
}