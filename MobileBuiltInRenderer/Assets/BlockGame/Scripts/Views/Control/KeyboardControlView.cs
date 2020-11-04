using System;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace BlockGame.Scripts.Views.Control
{
    public class KeyboardControlView : View
    {
        public enum Direction
        {
            Left,
            Right,
            Down
        }
        
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