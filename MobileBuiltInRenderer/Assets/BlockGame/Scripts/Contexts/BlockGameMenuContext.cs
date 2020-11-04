using BlockGame.Scripts.Controllers.FromView;
using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using BlockGame.Scripts.Signals.FromView;
using BlockGame.Scripts.Views.Menu;
using BlockGame.Scripts.Views.Signals;
using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using UnityEngine;

namespace BlockGame.Scripts.Contexts
{
    /// <summary>
    /// Context for menu scene
    /// </summary>
    public class BlockGameMenuContext: MVCSContext
    {
        public BlockGameMenuContext(MonoBehaviour view, ContextStartupFlags flags) : base(view, flags)
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

            commandBinder.Bind<TransitionToGameSignal>().To<TransitionToGameCommand>();
            injectionBinder.Bind<IIGamePersistentState>().To<ScriptableObjectPersistentStateImpl>().ToSingleton();
            mediationBinder.BindView<MainMenuButtonsView>().ToMediator<MainMenuButtonsMediator>();
        }
    }
}