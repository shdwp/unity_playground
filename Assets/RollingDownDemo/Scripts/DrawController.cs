using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RollingDownDemo.Scripts
{
    [RequireComponent(typeof(Canvas))]
    public class DrawController : MonoBehaviour
    {
        public Image canvasImage;
        public float drawDistanceTreshold = 45f;
        private DrawnMeshBuilder _builder;
        private bool _shouldStartSegment = false;
        public DrawnMeshBuilderPreviewController previewController;
        
        private Canvas _canvas;
        private RectTransform _canvasRect;
        
        private Texture2D _drawTex;

        private Vector2? _lastDAreaInputPos;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvasRect = _canvas.GetComponent<RectTransform>();
            _builder = new DrawnMeshBuilder();
            previewController.builder = _builder;
        }

        private void Start()
        {
            _drawTex = new Texture2D(512, 512);

            _canvas = GetComponent<Canvas>();
            canvasImage.sprite = Sprite.Create(_drawTex, new Rect(0, 0, 500, 400), new Vector2(0.5f, 0.5f));
        }

        private void Update()
        {
            var drawingAreaPos = ScreenToDrawingAreaPos(Input.mousePosition);
            
            if (Input.GetMouseButton(0))
            {
                if (_lastDAreaInputPos.HasValue)
                {
                    var drawnDistance = (_lastDAreaInputPos.Value - drawingAreaPos).magnitude;

                    if (drawnDistance > drawDistanceTreshold)
                    {
                        PlotLine(_lastDAreaInputPos.Value, drawingAreaPos);
                        
                        var iterations = Mathf.Ceil(drawnDistance / drawDistanceTreshold);
                        for (int i = 0; i < iterations; i++)
                        {
                            var factor = 0f;
                            if (i > 0)
                            {
                                factor = i / iterations;
                            }

                            var pos = Vector2.Lerp(_lastDAreaInputPos.Value, drawingAreaPos, factor);
                            
                            for (int x = -2; x < 4; x++)
                            {
                                for (int y = -2; y < 4; y++)
                                {
                                    _drawTex.SetPixel((int) pos.x + x, (int) pos.y + y, Color.blue);
                                }
                            }
                        }
                        
                        _drawTex.Apply();
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
                _builder.AddContinuationSegment(DrawingAreaToMeshPos(endPos), 0.1f);
            }
            else
            {
                _builder.AddSegment(DrawingAreaToMeshPos(startPos), DrawingAreaToMeshPos(endPos), 0.1f);
                _shouldStartSegment = false;
            }
        }

        private Vector2 ScreenToDrawingAreaPos(Vector2 pos)
        {
            /*
            var canvasPos = new Vector2(
                pos.x / Screen.width * _canvasRect.rect.width, 
                pos.y / Screen.height * _canvasRect.rect.height
            ); 
            
            return new Vector2(
                canvasPos.x - (_canvasRect.rect.width - canvasImage.rectTransform.rect.width) / 2,
                canvasPos.y - (_canvasRect.rect.height - canvasImage.rectTransform.rect.height) / 2f
            );
            */

            return Input.mousePosition / new Vector2(Screen.width, Screen.height);
        }

        private Vector2 DrawingAreaToMeshPos(Vector2 pos)
        {
            return pos;
            //return pos / canvasImage.rectTransform.rect.size;
        }
    }
}