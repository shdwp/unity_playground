using BlockGame.Scripts.Controllers;
using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using BlockGame.Scripts.Signals;
using BlockGame.Scripts.Signals.ToGridView;
using BlockGame.Scripts.Views.Block;
using BlockGame.Scripts.Views.Control;
using BlockGame.Scripts.Views.Grid;
using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using UnityEngine;

namespace BlockGame.Scripts
{
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

        private void BindModels()
        {
            injectionBinder.Bind<IPartialGrid>().To<PartialGridImpl>();
            injectionBinder.Bind<IGameState>().To<GameStateImpl>().ToSingleton();
            injectionBinder.Bind<IGridTransform>().To<GridTransformImpl>().ToSingleton();
        }

        private void BindSignals()
        {
            injectionBinder.Bind<SetupInitialGameStateSignal>().ToSingleton();
            injectionBinder.Bind<UpdateGridModelPositionSignal>().ToSingleton();
            injectionBinder.Bind<ReplaceGridInViewSignal>().ToSingleton();
            injectionBinder.Bind<SetupGridTransformSignal>().ToSingleton();
            injectionBinder.Bind<PlayerMoveDetachedGridSignal>().ToSingleton();
        }

        private void BindMediators()
        {
            mediationBinder.BindView<KeyboardControlView>().ToMediator<KeyboardControlMediator>();
            
            mediationBinder.BindView<BlockView>().ToMediator<BlockViewMediator>();

            mediationBinder.BindView<GridTransformView>().ToMediator<GridTransformViewMediator>();
            
            mediationBinder.BindView<DetachedGridView>().ToMediator<DetachedGridViewMediator>();
            mediationBinder.BindView<AttachedGridView>().ToMediator<GridViewMediator>();
        }

        private void BindCommands()
        {
            commandBinder.Bind<SetupInitialGameStateSignal>().To<InitialGameSetupCommand>();
            commandBinder.Bind<SetupGridTransformSignal>().To<SetupGridTransformCommand>();
            commandBinder.Bind<UpdateGridModelPositionSignal>().To<UpdateGridModelPositionCommand>();
        }
    }
}