using strange.extensions.signal.impl;

namespace BlockGame.Scripts.Views.Signals
{
    /// <summary>
    /// Fired when user commands detached grid move
    /// </summary>
    public class PlayerMoveDetachedGridSignal: Signal<PlayerMoveDetachedGridSignal.Direction>
    {
        public enum Direction
        {
            Left,
            Right,
            Down
        }
    }
}