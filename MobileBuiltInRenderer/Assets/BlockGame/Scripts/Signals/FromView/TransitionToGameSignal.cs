using BlockGame.Scripts.Contexts;
using BlockGame.Scripts.Model.Interfaces;
using strange.extensions.signal.impl;

namespace BlockGame.Scripts.Signals.FromView
{
    /// <summary>
    /// Fired when transition to game scene should be performed
    /// </summary>
    public class TransitionToGameSignal: Signal<bool, GridSpawnerType>
    {
        
    }
}