using strange.extensions.signal.impl;

namespace BlockGame.Scripts.Signals
{
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