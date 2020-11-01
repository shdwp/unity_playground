using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace LigthsaberDemo.Scripts
{
    /// <summary>
    /// User input controller for lightsaber object.
    /// </summary>
    public class LightsaberMouseInputController: MonoBehaviour
    {
        /// <summary>
        /// TargetObjectsController that will receive user-made cut planes
        /// </summary>
        public TargetObjectsController targetObjectsController;
        
        /// <summary>
        /// Transform of the lightsaber object (to be moved with user inputs)
        /// </summary>
        public Transform saberTransform;
        
        /// <summary>
        /// Maximum rotation speed (during slashing)
        /// </summary>
        public float rotationSpeed = 950f;

        /// <summary>
        /// Minimum distance of the swipe to be considered a cut
        /// </summary>
        public float cutMinimumDistance = 6f;
        
        /// <summary>
        /// Maximum distance of the swipe to be considered a cut. When this distance is archived cut is performed
        /// regardless of whether user has released mouse button.
        /// </summary>
        public float cutMaximumDistance = 21f;
        
        /// <summary>
        /// Saber rotation limits (up-down)
        /// </summary>
        public Vector2 rotationXClamp = new Vector2(30f, -100f);
        
        /// <summary>
        /// Saber rotation limits (left-right)
        /// </summary>
        public Vector2 rotationZClamp = new Vector2(-70f, 70f);
        
        private Camera _camera;
        
        // all mouse positions of ongoing swipe (world-space)
        private List<Vector2> _swipePoints = new List<Vector2>();

        // whether swipe was completed prematurely and no new gestures are accepted until user releases mouse button
        private bool _swipeGestureCompleted = false;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            // update lightsaber object to match mouse position 
            var mousePos = Input.mousePosition;
            var saberWorldPos = _camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
            // correlate for hilt
            saberWorldPos.y -= 4f;
            saberTransform.position = saberWorldPos;

            // distance of the current swipe
            var swipeDistance = 0f;
            if (_swipePoints.Count > 0)
            {
                swipeDistance = Vector3.Distance(_swipePoints.First(), _swipePoints.Last());
            }

            if (Input.GetMouseButton(0))
            {
                // user is in the process of a cut, we need to calculate rotation based on the position of the mouse on screen
                var targetRotationX = Mathf.Lerp(rotationXClamp.x, rotationXClamp.y, Mathf.InverseLerp(Screen.height, 0f, mousePos.y));
                var targetRotationZ = Mathf.Lerp(rotationZClamp.x, rotationZClamp.y, Mathf.InverseLerp(0f, Screen.width, mousePos.x));

                // calculate local rotation quaternion and apply it to the saber object
                var rot = Quaternion.identity;
                rot *= Quaternion.Euler(targetRotationX, 0f, 0f);
                rot *= Quaternion.Euler(0f, 0f, targetRotationZ);
                saberTransform.localRotation = Quaternion.RotateTowards(saberTransform.localRotation, rot, rotationSpeed * Time.deltaTime);

                if (!_swipeGestureCompleted && swipeDistance > cutMaximumDistance)
                {
                    PerformCutBasedOnSwipeData();
                    
                    // no new gesture can be performed now until user releases mouse button
                    _swipeGestureCompleted = true;
                }
                else
                {
                    // add world-space position of mouse at z of 0 (where target mesh is)
                    // @TODO: include target z into calculations?
                    _swipePoints.Add(_camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, _camera.transform.position.z)));
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                // user released mouse button, move sword to its default rotation
                saberTransform.localEulerAngles = Vector3.zero;
                
                if (!_swipeGestureCompleted && swipeDistance > cutMinimumDistance)
                {
                    // overall distance is greater than threshold, therefore _swipePoints are considered a cut
                    PerformCutBasedOnSwipeData();
                }

                // clear points array since gesture is now complete
                _swipePoints.Clear();

                // reset bool so new swipe gesture can be started
                _swipeGestureCompleted = false;
            }
        }

        private void PerformCutBasedOnSwipeData()
        {
            // calculate slope based on linear regression based on all _swipePoints to act as a normal of the plane
            float slope = LightsaberDemoLib.CalculateLinearRegressionSlope(_swipePoints);
                    
            // calculate centroid of _swipePoints to act as a center of the plane
            var centroid2D = _swipePoints.Aggregate(Vector2.zero, (acc, vec) => acc + vec) / _swipePoints.Count;
            var centroid = new Vector3(centroid2D.x, centroid2D.y, 0f);
                    
            var a = new Vector3(-1f, -slope, -1f);
            var b = new Vector3(1f, slope, 1f);
            var c = new Vector3(0f, 0f, 2f);
                    
            // pass plane to the target controller which will perform the mesh split
            targetObjectsController.Cut(new Plane(a, b, c).normal, centroid);
        }
    }
}