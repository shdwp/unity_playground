using BlockGame.Scripts.Model.Interfaces;

namespace BlockGame.Scripts.Model.GridSpawners
{
    public class TrueRandomSpawnerImpl: BaseRandomSpawnerImpl
    {
        public override GridSpawnerType type => GridSpawnerType.TrueRandom;
        
        protected override int SeedMin => 0;
        protected override int SeedMax => AllFigures.Length;
        
        protected override string NextFigureWithSeed(int seed)
        {
            return AllFigures[seed];
        }
    }
}