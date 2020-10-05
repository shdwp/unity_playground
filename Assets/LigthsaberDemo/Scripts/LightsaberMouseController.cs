using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LigthsaberDemo.Scripts
{
    public class LightsaberMouseController: MonoBehaviour
    {
        public CuttableObjectsController cuttableObjectsController;
        public Transform target;
        
        public float rotationSpeed;
        public float minDistance = 6f;
        public Vector2 rotationXClamp = new Vector2(30f, -100f);
        public Vector2 rotationZClamp = new Vector2(-70f, 70f);
        
        private Camera _camera;
        private List<Vector2> _slashPoints = new List<Vector2>();

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            var mousePos = Input.mousePosition;
            var worldPos = _camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
            worldPos.y -= 4f;

            target.position = worldPos;

            if (Input.GetMouseButton(0))
            {
                var targetRotationX = Mathf.Lerp(rotationXClamp.x, rotationXClamp.y, Mathf.InverseLerp(Screen.height, 0f, mousePos.y));
                var targetRotationZ = Mathf.Lerp(rotationZClamp.x, rotationZClamp.y, Mathf.InverseLerp(0f, Screen.width, mousePos.x));

                var rot = Quaternion.identity;
                rot *= Quaternion.Euler(targetRotationX, 0f, 0f);
                rot *= Quaternion.Euler(0f, 0f, targetRotationZ);

                target.localRotation = Quaternion.RotateTowards(target.localRotation, rot, rotationSpeed * Time.deltaTime);
                _slashPoints.Add(new Vector2(worldPos.x, worldPos.y));
                Debug.DrawLine(worldPos, worldPos*2f, Color.cyan, 3f);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                target.localEulerAngles = Vector3.zero;

                if (_slashPoints.Count > 0 && Vector3.Distance(_slashPoints.First(), _slashPoints.Last()) > minDistance)
                {
                    Debug.Log($"dist {Vector3.Distance(_slashPoints.First(), _slashPoints.Last())}");
                    float slope = CalculateLinearRegressionSlope(_slashPoints);
                    var plane = new Plane(new Vector3(-1f, -slope, -1f), new Vector3(0f, 0f, 2f), new Vector3(1f, slope, 1f));
                    Debug.DrawRay(Vector3.zero, plane.normal, Color.red, 2f, false);

                    cuttableObjectsController.Cut(plane);
                }

                _slashPoints.Clear();
            }
        }
        
        private float CalculateLinearRegressionSlope(IEnumerable<Vector2> vectors)
        {
            float sumOfX = 0;
            float sumOfY = 0;
            float sumOfXSq = 0;
            float sumCodeviates = 0;
        
            foreach (var v in vectors) {
                sumCodeviates += v.x * v.y;
                sumOfX += v.x;
                sumOfY += v.y;
                sumOfXSq += v.x * v.x;
            }

            var count = vectors.Count();
            var ssX = sumOfXSq - ((sumOfX * sumOfX) / count);
            var sCo = sumCodeviates - ((sumOfX * sumOfY) / count);
        
            return sCo / ssX;
        }
    }
}