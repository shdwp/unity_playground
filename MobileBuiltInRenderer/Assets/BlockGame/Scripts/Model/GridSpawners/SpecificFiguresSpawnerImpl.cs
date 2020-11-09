using BlockGame.Scripts.Contexts;
using BlockGame.Scripts.Model.Interfaces;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using UnityEngine;

namespace BlockGame.Scripts.Model.GridSpawners
{
    /// <summary>
    /// Class that returns specific figures in sequential order.
    /// </summary>
    public class SpecificFiguresSpawnerImpl: IGridSpawner<CellDataModel>
    {
        [Inject] public IInjectionBinder binder { get; set; }

        public GridSpawnerType type => GridSpawnerType.SpecificFigures;

        private int _figureIdx = 0;
        
        private string[] _figures =
        {
            "xx " +
            "xx " +
            "   " ,
        };

        public IPartialGrid<CellDataModel> SpawnRandomGrid()
        {
            // instantiate partial grid
            var grid = binder.GetInstance<IPartialGrid<CellDataModel>>();
            
            // get random color
            var colors = CellDataModel.ALL_COLORS;
            var colorIdx = Random.Range(0, colors.Length);

            // setup grid based on current index figure
            grid.Setup3x3(new CellDataModel(colors[colorIdx]), _figures[_figureIdx]);
            
            // bump and wrap figure index
            _figureIdx = _figureIdx + 1 >= _figures.Length ? 0 : _figureIdx + 1;
            
            return grid;
        }
    }
}