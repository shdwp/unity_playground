using BlockGame.Scripts.Model.Interfaces;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using UnityEngine;

namespace BlockGame.Scripts.Model.GridSpawners
{
    public class SpecificFiguresSpawnerImpl: IGridSpawner<BlockDataModel>
    {
        [Inject] public IInjectionBinder binder { get; set; }

        private int _figureIdx = 0;
        
        private string[] _figures =
        {
            "xx " +
            "xx " +
            "   " ,
        };

        public IPartialGrid<BlockDataModel> SpawnRandomGrid()
        {
            var grid = binder.GetInstance<IPartialGrid<BlockDataModel>>();
            var colors = BlockDataModel.ALL_COLORS;
            var colorIdx = Random.Range(0, colors.Length);

            grid.Setup3x3(new BlockDataModel(colors[colorIdx]), _figures[_figureIdx]);
            _figureIdx = _figureIdx + 1 >= _figures.Length ? 0 : _figureIdx + 1;
            
            return grid;
        }
    }
}