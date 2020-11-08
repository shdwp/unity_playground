using System;
using BlockGame.Scripts.Controllers;
using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.GridSpawners;
using BlockGame.Scripts.Model.Interfaces;
using BlockGame.Scripts.Signals;
using BlockGame.Scripts.Signals.FromView;
using BlockGame.Scripts.Views.ScriptableObjects;
using Lib;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using UnityEngine;

namespace BlockGame.Scripts
{
    public class BlockGameContextView : ContextView
    {
        public GameObject prefab;
        public GameStateScriptableObject stateScriptableObject;

        [Inject] public SetupInitialGameStateSignal setupInitialGameState { get; set; }
        [Inject] public RestoreGameStateSignal restoreGameState { get; set; }
        [Inject] public SaveStateBeforeQuitSignal saveStateBeforeQuit { get; set; }
        
        private void Awake()
        {
            var blockGameContext = new BlockGameContext(this, ContextStartupFlags.MANUAL_LAUNCH | ContextStartupFlags.MANUAL_MAPPING);
            blockGameContext.BindGameObjectPoolInstance(new GameObjectPool(prefab));
            blockGameContext.BindGameStateScriptableObjectInstance(stateScriptableObject);

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
            
            context = blockGameContext;
            context.Start();
            context.Launch();
        }

        private void Start()
        {
            if (stateScriptableObject.hasData)
            {
                restoreGameState.Dispatch();
            }
            else
            {
                setupInitialGameState.Dispatch();
            }
        }

        private void OnApplicationQuit()
        {
            saveStateBeforeQuit.Dispatch();
        }
    }
}