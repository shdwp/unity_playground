using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lib
{
    /// <summary>
    /// ObjectPool implementation for GameObject.
    /// Will activate/deactivate instances when they are dequeued/freed.
    /// </summary>
    public class GameObjectPool
    {
        // prefab to use during instantiation
        private GameObject _prefab;
        
        // instances that are deemed active (occupied)
        private List<GameObject> _activeInstances = new List<GameObject>();
        
        // instances that are currently free
        private Queue<GameObject> _freeInstances = new Queue<GameObject>();

        /// <summary>
        /// Create object pool
        /// </summary>
        /// <param name="prefab"></param>
        public GameObjectPool(GameObject prefab)
        {
            _prefab = prefab;
        }
        
        /// <summary>
        /// Dequeue instance. Will create one from prefab if no free instances available.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public GameObject Dequeue(Transform parent = null)
        {
            GameObject instance;

            if (_freeInstances.Count == 0)
            {
                instance = GameObject.Instantiate(_prefab, parent);
            }
            else
            {
                instance = _freeInstances.Dequeue();
                instance.transform.SetParent(parent);
            }
            
            _activeInstances.Add(instance);
            instance.SetActive(true);
            instance.transform.position = Vector3.zero;
            return instance;
        }

        /// <summary>
        /// Free previously dequeued instance.
        /// </summary>
        /// <param name="instance"></param>
        public void Free(GameObject instance)
        {
            if (instance.activeSelf)
            {
                instance.SetActive(false);
                _freeInstances.Enqueue(instance);
                _activeInstances.Remove(instance);
            }
        }

        /// <summary>
        /// Free active instances based on predicate.
        /// </summary>
        /// <param name="pred"></param>
        public void Free(Predicate<GameObject> pred)
        {
            for (int i = 0; i < _activeInstances.Count; i++)
            {
                var activeInstance = _activeInstances[i];
                if (pred(activeInstance))
                {
                    Free(activeInstance);
                    i--;
                }
                
            }
        }
    }
}