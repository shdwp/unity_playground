using BlockGame.Scripts.Contexts;
using BlockGame.Scripts.Model.Interfaces;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using strange.framework.api;
using UnityEngine;

namespace BlockGame.Scripts.Model.GridSpawners
{
    /// <summary>
    /// Base class for random grid spawner. Yields IPartialGrid instances of random nature, used to spawn new
    /// tetrominos in the game.
    /// </summary>
    public abstract class BaseRandomSpawnerImpl: IGridSpawner<CellDataModel>
    {
        [Inject] public IInjectionBinder binder { get; set; }
        
        /// <summary>
        /// Each spawner has a distinctive type (one per class)
        /// </summary>
        public abstract GridSpawnerType type { get; }
        
        /// <summary>
        /// Min value for random seed passed to implementation class
        /// </summary>
        protected abstract int SeedMin { get; }
        
        /// <summary>
        /// Max value for random seed passed to implementation class
        /// </summary>
        protected abstract int SeedMax { get; }

        /// <summary>
        /// String representations of all tetrominos in game
        /// </summary>
        protected string[] AllFigures =
        {
            "   " +
            "   " +
            "xxx",
            
            "x  " +
            "x  " +
            "x  ",
            
            "   " +
            "  x" +
            "xxx",
            
            "xxx" +
            "x  " +
            "   ",
            
            "   " +
            " xx" +
            "xx ",
            
            "xx " +
            " xx" +
            "   ",
            
            " xx" +
            " xx" +
            "   ",
            
            "xx " +
            "xx " +
            "   ",
            
            "xxx" +
            " x " +
            "   ",
            
            "x  " +
            "xx " +
            "x  ",
        };
        
        private int _colorIdx;
        
        /// <summary>
        /// Spawns random grid based on scheme provided by child class interface
        /// </summary>
        /// <returns></returns>
        public IPartialGrid<CellDataModel> SpawnRandomGrid()
        {
            var grid = binder.GetInstance<IPartialGrid<CellDataModel>>();

            var colors = CellDataModel.ALL_COLORS;
            _colorIdx = _colorIdx + 1 >= colors.Length ? 0 : _colorIdx + 1;
            
            var figureSeed = Random.Range(SeedMin, SeedMax - 1);
            var figure = NextFigureWithSeed(figureSeed);
            
            grid.Setup3x3(new CellDataModel(colors[_colorIdx]), figure);
            return grid;
        }

        /// <summary>
        /// Get random string scheme of a figure based on seed
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        protected abstract string NextFigureWithSeed(int seed);
    }
}