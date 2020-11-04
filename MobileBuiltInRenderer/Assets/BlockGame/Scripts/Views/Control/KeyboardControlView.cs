using System;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace BlockGame.Scripts.Views.Control
{
    /// <summary>
    /// Control class for keyboard (AD - moves tetrominoes, S makes them fall faster).
    ///
    /// Yields events continuously until player released the button.
    /// </summary>
    public class KeyboardControlView : View
    {
        /// <summary>
        /// Direction enum for returned events
        /// </summary>
        public enum Direction
        {
            Left,
            Right,
            Down
        }
        
        /// <summary>
        /// Signal for outgoing move events
        /// </summary>
        public Signal<Direction> directionKeyPressed = new Signal<Direction>();
        
        private void Update()
        {
            if (Input.GetKey(KeyCode.A))
            {
                directionKeyPressed.Dispatch(Direction.Left);
            }

            if (Input.GetKey(KeyCode.D))
            {
                directionKeyPressed.Dispatch(Direction.Right);
            }

            if (Input.GetKey(KeyCode.S))
            {
                directionKeyPressed.Dispatch(Direction.Down);
            }
        }
    }
}