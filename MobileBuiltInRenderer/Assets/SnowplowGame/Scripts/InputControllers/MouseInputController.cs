using System;
using SnowplowGame.Scripts.Cars;
using UnityEngine;

namespace SnowplowGame.Scripts.InputControllers
{
    /// <summary>
    /// Input controller for mouse. Controls PlowTruckController, setting up new steering target when user
    /// presses mouse
    /// </summary>
    public class MouseInputController : MonoBehaviour
    {
        // DI
        public PlowTruckController truckController;

        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButton(0)) 
            {
                // calculate mouse position on track plane
                var screenSpaceMousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1f);
                var ray = _camera.ScreenPointToRay(screenSpaceMousePosition);
                var plane = new Plane(Vector3.up, Vector3.zero);

                plane.Raycast(ray, out float distance);
                var p = ray.origin + ray.direction * distance;
                
                // set new steering target
                truckController.SteerTowards(p.z);
            }
        }
    }
}