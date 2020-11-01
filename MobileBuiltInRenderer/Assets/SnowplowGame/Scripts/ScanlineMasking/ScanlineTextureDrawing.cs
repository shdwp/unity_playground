using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SnowplowGame.Scripts
{
    /// <summary>
    /// Helper class to draw in scaline texture.
    /// Will provide API to fill pixels in accordance with scanline position information so that
    /// synchronized with shader output coordinates will appear static.
    /// </summary>
    public class ScanlineTextureDrawing
    {
        /// <summary>
        /// Output texture
        /// </summary>
        public Texture2D tex;
        
        /// <summary>
        /// Scanline position in UV format
        /// </summary>
        public float scanline;
        
        /// <summary>
        /// Scanline position in texture height format
        /// </summary>
        public int scanlineTexY => (int)(scanline * tex.height);

        // cache for pixel arrays
        private Dictionary<int, Color[]> _pixelsCache = new Dictionary<int, Color[]>();

        /// <summary>
        /// Instantiate with empty texture of size
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public ScanlineTextureDrawing(int w, int h)
        {
            tex = new Texture2D(w, h);
            tex.SetPixels(new Color[w * h]);
            tex.Apply();
        }

        /// <summary>
        /// Instantiate with existing texture
        /// </summary>
        /// <param name="texture">texture</param>
        /// <param name="copy">copy texture (immutable) or write into it</param>
        public ScanlineTextureDrawing(Texture2D texture, bool copy)
        {
            if (copy)
            {
                tex = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
                Graphics.CopyTexture(texture, tex);
            }
            else
            {
                tex = texture;
            }
            
            tex.Apply();
        }

        /// <summary>
        /// Update scanline position from UV distance
        /// </summary>
        /// <param name="distance"></param>
        public void UpdateScanlineFromUVDistance(float distance)
        {
            scanline = distance - Mathf.Floor(distance);
        }

        /// <summary>
        /// Clear rows at and before scanline (backwards)
        /// </summary>
        /// <param name="amount">amount of rows to clear</param>
        public void ClearRows(int amount)
        {
            var data = new Color[tex.width];
            for (int i = 0; i < amount; i++)
            {
                tex.SetPixels(0, CalculateTextureY(-i), tex.width, 1, data);
            }
        }

        /// <summary>
        /// Fill area (x, y - center)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public void FillArea(int x, int y, int w, int h)
        {
            var topleftX = x - w / 2;
            var topleftY = y - h / 2;

            var texW = w;
            var texH = h;
            
            if (topleftX < 0)
            {
                texW -= Math.Abs(topleftX);
            } else if (topleftX + texW >= tex.width)
            {
                texW -= topleftX + texW - tex.width;
            }
            
            var texX = Math.Max(0, topleftX);
            GetPixelArray(texW, out var array);
            
            for (int row = 0; row < texH; row++)
            {
                var texY = CalculateTextureY(topleftY + (row - texH/2));
                tex.SetPixels(texX, texY, texW, 1, array);
            }
        }

        /// <summary>
        /// Apply changes to output texture
        /// </summary>
        public void Apply()
        {
            tex.Apply();
        }

        /// <summary>
        /// Calculate distance in pixels between current scanline position and one provided in argument
        /// </summary>
        /// <param name="otherScanlineTexY"></param>
        /// <returns></returns>
        public int CalculateTexturePixelsSinceScanline(int otherScanlineTexY)
        {
            return CalculateTextureYDelta(otherScanlineTexY, scanlineTexY);
        }

        /// <summary>
        /// Calculate distance between two texture pixel coordinates
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public int CalculateTextureYDelta(int a, int b)
        {
            var d = Math.Abs(a - b);
            if (d > tex.width / 2)
            {
                d = tex.width - d;
            }

            return d;
        }

        private int CalculateTextureY(int y)
        {
            var texY = y + scanlineTexY;

            if (texY >= tex.height)
            {
                texY = texY - tex.height;
            }

            if (texY < 0)
            {
                texY = tex.height + texY;
            }

            return texY;
        }

        /// <summary>
        /// Get array of white pixels, either allocating those or getting it from cache
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="array"></param>
        private void GetPixelArray(int amount, out Color[] array)
        {
            if (!_pixelsCache.TryGetValue(amount, out array))
            {
                array = Enumerable.Repeat(Color.white, amount).ToArray();
                _pixelsCache[amount] = array;
            }
        }
    }
}