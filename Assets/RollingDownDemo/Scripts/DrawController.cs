using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RollingDownDemo.Scripts
{
    public class DrawController : MonoBehaviour
    {
        public float drawDistanceTreshold = 0.001f;
        public float segmentSize = 0.01f;
        public DrawnMeshBuilderPreviewController previewController;
        
        private DrawnMeshBuilder _builder;
        private bool _shouldStartSegment = true;
        private Vector2? _lastDAreaInputPos;
        
        private Rect? _drawingAreaRect;
        private int _lastScreenWidth, _lastScreenHeight;

        private void Awake()
        {
            _builder = new DrawnMeshBuilder();
            previewController.builder = _builder;
        }

        private void Update()
        {
            if (!_drawingAreaRect.HasValue || Screen.width != _lastScreenWidth || Screen.height != _lastScreenHeight)
            {
                var min = (float)Math.Min(Screen.width, Screen.height);
                
                _drawingAreaRect = new Rect(
                    (1f - min / Screen.width) / 2f, 
                    (1f - min / Screen.height) / 2f, 
                    min / Screen.width,
                    min / Screen.height
                );
                
                _lastScreenWidth = Screen.width;
                _lastScreenHeight = Screen.height;
            }
            
            if (Input.GetMouseButton(0))
            {
                var drawingAreaPos = ScreenToDrawingAreaPos(Input.mousePosition);
            
                if (_lastDAreaInputPos.HasValue)
                {
                    if ((_lastDAreaInputPos.Value - drawingAreaPos).magnitude > drawDistanceTreshold)
                    {
                        PlotLine(_lastDAreaInputPos.Value, drawingAreaPos);
                        _lastDAreaInputPos = drawingAreaPos;
                    }
                }
                else
                {
                    _lastDAreaInputPos = drawingAreaPos;
                }
            }
            else
            {
                _shouldStartSegment = true;
                _lastDAreaInputPos = null;
            }
        }

        private void PlotLine(Vector2 startPos, Vector2 endPos)
        {
            if (!_shouldStartSegment)
            {
                _builder.AddContinuationSegment(DrawingAreaToMeshPos(endPos), segmentSize);
            }
            else
            {
                _builder.AddSegment(DrawingAreaToMeshPos(startPos), DrawingAreaToMeshPos(endPos), segmentSize);
                _shouldStartSegment = false;
            }
        }

        private Vector2 ScreenToDrawingAreaPos(Vector2 pos)
        {
            if (!_drawingAreaRect.HasValue)
            {
                Debug.Assert(false, "_drawingAreaRect not set!");
                return Vector2.zero;
            }
            
            var screenPos = pos / new Vector2(Screen.width, Screen.height);
            var drawingAreaPos = new Vector2(
                Mathf.InverseLerp(_drawingAreaRect.Value.xMin, _drawingAreaRect.Value.xMax, screenPos.x),
                Mathf.InverseLerp(_drawingAreaRect.Value.yMin, _drawingAreaRect.Value.yMax, screenPos.y)
            );

            return drawingAreaPos;
        }

        private Vector2 DrawingAreaToMeshPos(Vector2 pos)
        {
            return pos - new Vector2(0.5f, 0.5f);
        }
    }
}