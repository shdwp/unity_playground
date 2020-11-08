using strange.extensions.signal.impl;

namespace BlockGame.Scripts.Views.Signals
{
    public enum SelectedGameMode
    {
        Continue,
        NewWithGrabBagSpawner,
        NewWithSpecificFiguresSpawner,
    }
    
    public class PlayerSelectedGameModeSignal: Signal<SelectedGameMode>
    {
        
    }
}