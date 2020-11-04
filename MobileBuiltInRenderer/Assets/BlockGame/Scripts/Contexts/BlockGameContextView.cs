using BlockGame.Scripts.Model.GridSpawners;
using BlockGame.Scripts.Model.Interfaces;
using BlockGame.Scripts.Signals;
using BlockGame.Scripts.Signals.FromView;
using BlockGame.Scripts.Views.ScriptableObjects;
using Lib;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using UnityEngine;

namespace BlockGame.Scripts.Contexts
{
    /// <summary>
    /// Context view for game scene
    /// </summary>
    public class BlockGameContextView : ContextView
    {
        /// <summary>
        /// Single block prefab
        /// </summary>
        public GameObject blockPrefab;
        
        /// <summary>
        /// Scriptable object to store game persistent data in
        /// </summary>
        public GameStateScriptableObject stateScriptableObject;

        [Inject] public SetupInitialGameStateSignal setupInitialGameState { get; set; }
        [Inject] public RestoreGameStateSignal restoreGameState { get; set; }
        [Inject] public SaveStateBeforeQuitSignal saveStateBeforeQuit { get; set; }
        
        private void Awake()
        {
            // create context and bind classes not available in the context itself
            var blockGameContext = new BlockGameContext(this, ContextStartupFlags.MANUAL_LAUNCH | ContextStartupFlags.MANUAL_MAPPING);
            blockGameContext.BindGameObjectPoolInstance(new GameObjectPool(blockPrefab));
            blockGameContext.BindGameStateScriptableObjectInstance(stateScriptableObject);

            // bind spawner class depending on the type set in persistent storage
            switch (stateScriptableObject.spawnerType)
            {
                case GridSpawnerType.GrabBag:
                    blockGameContext.BindSpawnerClass<GrabBagSpawnerImpl>();
                    break;
                
                case GridSpawnerType.SpecificFigures:
                    blockGameContext.BindSpawnerClass<SpecificFiguresSpawnerImpl>();
                    break;
                
                case GridSpawnerType.TrueRandom:
                    blockGameContext.BindSpawnerClass<TrueRandomSpawnerImpl>();
                    break;
            }
            
            // setup and start context
            context = blockGameContext;
            context.Start();
            context.Launch();
        }

        private void Start()
        {
            if (stateScriptableObject.hasData)
            {
                // there is persistent game data available, meaning that game state should be restored
                restoreGameState.Dispatch();
            }
            else
            {
                // no persistent game data, session should be start anew
                setupInitialGameState.Dispatch();
            }
        }

        private void OnApplicationQuit()
        {
            // store game persistent data before quit
            saveStateBeforeQuit.Dispatch();
        }
    }
}