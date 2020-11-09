using System.Collections.Generic;
using System.Linq;
using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using BlockGame.Scripts.Signals.ToView;

namespace BlockGame.Scripts.Controllers.ToView
{
    /// <summary>
    /// Helper class injected in some commands, contains methods to send signals to views
    /// </summary>
    public class ToGridViewComponent
    {
        [Inject] public ReplaceGridInViewSignal<CellDataModel> replaceGridInView { get; set; }
        [Inject] public MergeGridInViewSignal<CellDataModel> mergeGridInView { get; set; }
        
        [Inject] public IGridTransform transform { get; set; }

        /// <summary>
        /// Send replace grid command to the view. Fully refreshes the view.
        /// Will transform grid based on injected IGridTransform.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="grid"></param>
        public void DispatchReplaceGrid(GridType type, IPartialGrid<CellDataModel> grid)
        {
            replaceGridInView.Dispatch(type, transform.GridToWorld(grid.pos), MapToViewData(grid));
        }
        
        /// <summary>
        /// Send merge command to the view. Will add parameter grid to the existing view structure of specific grid type.
        /// Will transform grid based on injected IGridTransform.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="grid"></param>
        public void DispatchMergeGrid(GridType type, IPartialGrid<CellDataModel> grid)
        {
            mergeGridInView.Dispatch(type, transform.GridToWorld(grid.pos), MapToViewData(grid));
        }

        /// <summary>
        /// Map model-level data class to view-level data class
        /// </summary>
        /// <param name="input"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private IEnumerable<BaseToGridViewData<T>> MapToViewData<T>(IEnumerable<GridCell<T>> input)
        {
            return input.Select(a => new BaseToGridViewData<T>(transform.GridToWorld(a.pos), a.data));
        }
    }
}