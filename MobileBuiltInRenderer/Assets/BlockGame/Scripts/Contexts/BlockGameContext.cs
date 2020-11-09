using BlockGame.Scripts.Controllers.FromView;
using BlockGame.Scripts.Controllers.ToView;
using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using BlockGame.Scripts.Signals;
using BlockGame.Scripts.Signals.FromView;
using BlockGame.Scripts.Signals.ToView;
using BlockGame.Scripts.Views.Block;
using BlockGame.Scripts.Views.Control;
using BlockGame.Scripts.Views.Grid;
using BlockGame.Scripts.Views.GridTransform;
using BlockGame.Scripts.Views.Menu;
using BlockGame.Scripts.Views.Signals;
using Lib;
using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using UnityEngine;

namespace BlockGame.Scripts.Contexts
{
    /// <summary>
    /// Context for game scene.
    /// </summary>
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

        /// <summary>
        /// Binds instance of GameObjectPool
        /// </summary>
        /// <param name="instance"></param>
        public void BindGameObjectPoolInstance(GameObjectPool instance)
        {
            injectionBinder.Bind<GameObjectPool>().To(instance);
        }
        
        /// <summary>
        /// Bind game state scriptable object instance
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="T"></typeparam>
        public void BindGameStateScriptableObjectInstance<T>(T instance)
        {
            injectionBinder.Bind<T>().To(instance);
        }

        /// <summary>
        /// Bind spawner class T to IGridSpawner interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void BindSpawnerClass<T>()
        {
            injectionBinder.Bind<IGridSpawner<CellDataModel>>().To<T>().ToSingleton();
        }

        private void BindModels()
        {
            injectionBinder.Bind<IPartialGrid<CellDataModel>>().To<PartialGridImpl<CellDataModel>>();
            
            injectionBinder.Bind<IGameFieldState>().To<GameFieldStateImpl>().ToSingleton();
            injectionBinder.Bind<IIGamePersistentState>().To<ScriptableObjectPersistentStateImpl>().ToSingleton();
            injectionBinder.Bind<IGridTransform>().To<GridTransformImpl>().ToSingleton();
            injectionBinder.Bind<ToGridViewComponent>().ToSingleton();
        }

        private void BindSignals()
        {
            injectionBinder.Bind<ReplaceGridInViewSignal<CellDataModel>>().ToSingleton();
            injectionBinder.Bind<MergeGridInViewSignal<CellDataModel>>().ToSingleton();
            injectionBinder.Bind<PlayerMoveDetachedGridSignal>().ToSingleton();
        }

        private void BindMediators()
        {
            mediationBinder.BindView<KeyboardControlView>().ToMediator<KeyboardControlMediator>();
            mediationBinder.BindView<GameBackButtonView>().ToMediator<GameBackButtonMediator>();
            mediationBinder.BindView<GridTransformView>().ToMediator<GridTransformViewMediator>();
            
            mediationBinder.BindView<DetachedGridView>().ToMediator<DetachedGridViewMediator>();
            mediationBinder.BindView<AttachedGridView>().ToMediator<AttachedGridViewMediator>();
        }

        private void BindCommands()
        {
            commandBinder.Bind<SetupInitialGameStateSignal>().To<InitialGameSetupCommand>();
            commandBinder.Bind<SetupGridTransformSignal>().To<SetupGridTransformCommand>();
            commandBinder.Bind<AttemptGridModelMoveSignal>().To<AttemptGridModelMoveCommand>();
            
            commandBinder.Bind<TransitionToMenuSignal>().To<StoreStateCommand>().To<TransitionToMenuCommand>();
            commandBinder.Bind<SaveStateBeforeQuitSignal>().To<StoreStateCommand>();
            commandBinder.Bind<RestoreGameStateSignal>().To<RestoredGameSetupCommand>();
        }
    }
}