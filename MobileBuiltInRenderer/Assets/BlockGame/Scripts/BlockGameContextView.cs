using System;
using BlockGame.Scripts.Controllers;
using BlockGame.Scripts.Signals;
using Lib;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using UnityEngine;

namespace BlockGame.Scripts
{
    public class BlockGameContextView : ContextView
    {
        public GameObject prefab;

        [Inject] public SetupInitialGameStateSignal SetupInitialGameState { get; set; }
        
        private void Awake()
        {
            var blockGameContext = new BlockGameContext(this, ContextStartupFlags.MANUAL_LAUNCH | ContextStartupFlags.MANUAL_MAPPING);
            blockGameContext.injectionBinder.Bind<GameObjectPool>().To(new GameObjectPool(prefab));

            context = blockGameContext;
            context.Start();
            context.Launch();
        }

        private void Start()
        {
            SetupInitialGameState.Dispatch();
        }
    }
}