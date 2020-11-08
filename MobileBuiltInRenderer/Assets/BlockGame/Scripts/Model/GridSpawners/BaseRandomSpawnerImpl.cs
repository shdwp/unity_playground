using BlockGame.Scripts.Model.Interfaces;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using strange.framework.api;
using UnityEngine;

namespace BlockGame.Scripts.Model.GridSpawners
{
    public abstract class BaseRandomSpawnerImpl: IGridSpawner<BlockDataModel>
    {
        [Inject] public IInjectionBinder binder { get; set; }
        
        protected abstract int SeedMin { get; }
        protected abstract int SeedMax { get; }

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
        
        public IPartialGrid<BlockDataModel> SpawnRandomGrid()
        {
            var grid = binder.GetInstance<IPartialGrid<BlockDataModel>>();

            var colors = BlockDataModel.ALL_COLORS;
            _colorIdx = _colorIdx + 1 >= colors.Length ? 0 : _colorIdx + 1;
            
            var figureSeed = Random.Range(SeedMin, SeedMax - 1);
            var figure = NextFigureWithSeed(figureSeed);
            
            grid.Setup3x3(new BlockDataModel(colors[_colorIdx]), figure);
            return grid;
        }

        protected abstract string NextFigureWithSeed(int seed);
    }
}