using BlockGame.Scripts.Controllers;
using BlockGame.Scripts.Controllers.FromView;
using BlockGame.Scripts.Controllers.ToView;
using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.GridSpawners;
using BlockGame.Scripts.Model.Interfaces;
using BlockGame.Scripts.Signals;
using BlockGame.Scripts.Signals.FromView;
using BlockGame.Scripts.Signals.ToGridView;
using BlockGame.Scripts.Views.Block;
using BlockGame.Scripts.Views.Control;
using BlockGame.Scripts.Views.Grid;
using BlockGame.Scripts.Views.GridTransform;
using BlockGame.Scripts.Views.Signals;
using Lib;
using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using UnityEngine;

namespace BlockGame.Scripts
{
    public enum BlockGameGridSpawner
    {
        GrabBag,
        SpecificFigures,
        TrueRandom
    }
    
    public class BlockGameContext: MVCSContext
    {
        public BlockGameContext(MonoBehaviour view, ContextStartupFlags flags) : base(view, flags)
        {
        }

        protected override void addCoreComponents()
        {
            base.addCoreComponents();
            
            injectionBinder.Unbind<ICommandBinder>();
            injectionBinder.Bind<ICommandBinder>().To<SignalCommandBinder>().ToSingleton();
        }

        protected override void mapBindings()
        {
            base.mapBindings();
            
            BindModels();
            BindSignals();
            BindMediators();
            BindCommands();
        }

        public void BindGameObjectPoolInstance(GameObjectPool instance)
        {
            injectionBinder.Bind<GameObjectPool>().To(instance);
        }

        public void BindSpawnerClass<T>()
        {
            injectionBinder.Bind<IGridSpawner<BlockDataModel>>().To<T>().ToSingleton();
        }

        private void BindModels()
        {
            injectionBinder.Bind<IPartialGrid<BlockDataModel>>().To<PartialGridImpl<BlockDataModel>>();

            /*
            injectionBinder.Bind<IGridSpawner<BlockDataModel>>().To<GrabBagSpawnerImpl>().Named(BlockGameGridSpawner.GrabBag).ToSingleton();
            injectionBinder.Bind<IGridSpawner<BlockDataModel>>().To<SpecificFiguresSpawnerImpl>().Named(BlockGameGridSpawner.SpecificFigures).ToSingleton();
            injectionBinder.Bind<IGridSpawner<BlockDataModel>>().To<TrueRandomSpawnerImpl>().Named(BlockGameGridSpawner.TrueRandom).ToSingleton();
            */
            
            injectionBinder.Bind<IGameState>().To<GameStateImpl>().ToSingleton();
            injectionBinder.Bind<IGridTransform>().To<GridTransformImpl>().ToSingleton();
            injectionBinder.Bind<ToGridViewComponent>().ToSingleton();
        }

        private void BindSignals()
        {
            injectionBinder.Bind<ReplaceGridInViewSignal<BlockDataModel>>().ToSingleton();
            injectionBinder.Bind<MergeGridInViewSignal<BlockDataModel>>().ToSingleton();
            injectionBinder.Bind<PlayerMoveDetachedGridSignal>().ToSingleton();
        }

        private void BindMediators()
        {
            mediationBinder.BindView<KeyboardControlView>().ToMediator<KeyboardControlMediator>();
            
            mediationBinder.BindView<BlockView>().ToMediator<BlockViewMediator>();

            mediationBinder.BindView<GridTransformView>().ToMediator<GridTransformViewMediator>();
            
            mediationBinder.BindView<DetachedGridView>().ToMediator<DetachedGridViewMediator>();
            mediationBinder.BindView<AttachedGridView>().ToMediator<AttachedGridViewMediator>();
        }

        private void BindCommands()
        {
            commandBinder.Bind<SetupInitialGameStateSignal>().To<InitialGameSetupCommand>();
            commandBinder.Bind<SetupGridTransformSignal>().To<SetupGridTransformCommand>();
            commandBinder.Bind<AttemptGridModelMoveSignal>().To<AttemptGridModelMoveCommand>();
        }
    }
}