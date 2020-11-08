using System.Collections.Generic;
using System.Linq;
using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using BlockGame.Scripts.Signals.ToGridView;
using BlockGame.Scripts.Signals.ToView;

namespace BlockGame.Scripts.Controllers.ToView
{
    public class ToGridViewComponent
    {
        [Inject] public ReplaceGridInViewSignal<BlockDataModel> replaceGridInView { get; set; }
        [Inject] public MergeGridInViewSignal<BlockDataModel> mergeGridInView { get; set; }
        
        [Inject] public IGridTransform transform { get; set; }

        public void DispatchReplaceGrid(GridType type, IPartialGrid<BlockDataModel> grid)
        {
            replaceGridInView.Dispatch(type, transform.GridToWorld(grid.pos), MapToViewData(grid));
        }
        
        public void DispatchMergeGrid(GridType type, IPartialGrid<BlockDataModel> grid)
        {
            mergeGridInView.Dispatch(type, transform.GridToWorld(grid.pos), MapToViewData(grid));
        }

        private IEnumerable<BaseToGridViewData<T>> MapToViewData<T>(IEnumerable<GridCell<T>> input)
        {
            return input.Select(a => new BaseToGridViewData<T>(transform.GridToWorld(a.pos), a.data));
        }
    }
}