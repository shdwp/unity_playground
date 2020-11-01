using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace MatchTwoGame.Scripts.Backend
{
    /// <summary>
    /// Backend remote access class, manages https requests to the remote API.
    /// </summary>
    public class BackendClient
    {
        /// <summary>
        /// Scheme JSON has been downloaded
        /// </summary>
        /// <param name="urls">list of texture urls</param>
        public delegate void SchemeDownloaded(List<string> urls);
        
        /// <summary>
        /// Texture has been downloaded
        /// </summary>
        /// <param name="tex">texture</param>
        /// <param name="progress">number of textures left to download</param>
        public delegate void TextureDownloaded(Texture2D tex, int progress);
        
        /// <summary>
        /// One of the requests has failed, halting the operation
        /// </summary>
        /// <param name="result"></param>
        public delegate void RemoteRequestFailed(UnityWebRequest.Result result);

        /// <summary>
        /// Scheme JSON has been downloaded
        /// </summary>
        public event SchemeDownloaded schemeDownloaded;
        
        /// <summary>
        /// Texture has been download
        /// </summary>
        public event TextureDownloaded textureDownloaded;
        
        /// <summary>
        /// One of the requests has failed, operation halted
        /// </summary>
        public event RemoteRequestFailed downloadFailed;

        // requests throttling (to show off animations)
        public float schemeLoadThrottleSeconds = 3f;
        public float textureLoadThrottleSeconds = 1f;
        
        /// <summary>
        /// URL of the scheme
        /// </summary>
        private string _schemeUrl = "https://github.com/shdwp/unity_playground/raw/master/MobileBuiltInRenderer/RemoteAssets/MatchTwoGame/scheme.json";
        
        /// <summary>
        /// Coroutine to start downloading resources from external source.
        /// Will download scheme JSON first, then downloads all of the enclosed textures.
        /// </summary>
        /// <returns></returns>
        public IEnumerator DataDownloadCoroutine()
        {
            var schemeRequest = UnityWebRequest.Get(_schemeUrl);
            yield return schemeRequest.SendWebRequest();

            if (!CheckIfSuccessfulAndNotifyIfNeeded(schemeRequest))
            {
                yield break;
            }

            var scheme = JsonConvert.DeserializeObject<List<string>>(schemeRequest.downloadHandler.text);
            schemeDownloaded?.Invoke(scheme);

            yield return new WaitForSeconds(schemeLoadThrottleSeconds);

            var operations = new List<UnityWebRequestAsyncOperation>();
            foreach (var url in scheme)
            {
                operations.Add(UnityWebRequestTexture.GetTexture(url).SendWebRequest());
            }

            for (int i = 0; i < operations.Count; i++)
            {
                var op = operations[i];
                yield return op;

                if (CheckIfSuccessfulAndNotifyIfNeeded(op.webRequest))
                {
                    var tex = DownloadHandlerTexture.GetContent(op.webRequest);
                    textureDownloaded?.Invoke(tex, operations.Count - i - 1);
                    yield return new WaitForSeconds(textureLoadThrottleSeconds);
                }
                else
                {
                    yield break;
                }
            }
        }

        /// <summary>
        /// Check if UnityWebRequest was successful and fire failure event if it was not
        /// </summary>
        /// <param name="req"></param>
        /// <returns>true if successful, false otherwise</returns>
        private bool CheckIfSuccessfulAndNotifyIfNeeded(UnityWebRequest req)
        {
            if (req.result != UnityWebRequest.Result.Success)
            {
                downloadFailed?.Invoke(req.result);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}