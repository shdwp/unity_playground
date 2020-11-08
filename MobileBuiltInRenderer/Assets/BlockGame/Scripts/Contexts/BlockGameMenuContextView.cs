using BlockGame.Scripts.Views.ScriptableObjects;
using strange.extensions.context.api;
using strange.extensions.context.impl;

namespace BlockGame.Scripts
{
    public class BlockGameMenuContextView: ContextView
    {
        public GameStateScriptableObject scriptableObject;
        
        private void Awake()
        {
            var menuContext = new BlockGameMenuContext(this, ContextStartupFlags.MANUAL_LAUNCH | ContextStartupFlags.MANUAL_MAPPING);
            menuContext.injectionBinder.Bind<GameStateScriptableObject>().To(scriptableObject);

            context = menuContext;
            context.Start();
            context.Launch();
        }
    }
}