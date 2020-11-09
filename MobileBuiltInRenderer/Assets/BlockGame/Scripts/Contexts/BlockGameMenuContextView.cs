using BlockGame.Scripts.Views.ScriptableObjects;
using strange.extensions.context.api;
using strange.extensions.context.impl;

namespace BlockGame.Scripts.Contexts
{
    /// <summary>
    /// Context view for menu scene
    /// </summary>
    public class BlockGameMenuContextView: ContextView
    {
        /// <summary>
        /// Scriptable object to store game persistent data in
        /// </summary>
        public GameStateScriptableObject scriptableObject;
        
        private void Awake()
        {
            var menuContext = new BlockGameMenuContext(this, ContextStartupFlags.MANUAL_LAUNCH | ContextStartupFlags.MANUAL_MAPPING);
            
            // bind scriptable data instance
            menuContext.injectionBinder.Bind<GameStateScriptableObject>().To(scriptableObject);

            // setup and start context
            context = menuContext;
            context.Start();
            context.Launch();
        }
    }
}