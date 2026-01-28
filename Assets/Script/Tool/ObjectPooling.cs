using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    /// <summary>
    /// Simple Object Pooling focused on spawning.
    /// - Prewarm pools (instantiate upfront)
    /// - Spawn (get from pool) by prefab key
    /// - Despawn (return to pool)
    /// - Auto expand when pool is empty (optional)
    /// </summary>
    public class ObjectPooling : MonoBehaviour
    {
        [Serializable]
        public class PoolConfig
        {
            public string key;                 // Optional: custom key. If empty, prefab name will be used.
            public GameObject prefab;
            [Min(0)] public int prewarmCount = 10;
            public bool autoExpand = true;
            public Transform poolRoot;          // Optional: where inactive instances are stored
        }

        [Header("Pools")]
        [SerializeField] private List<PoolConfig> pools = new List<PoolConfig>();

        // Runtime structures
        private readonly Dictionary<string, PoolRuntime> _runtime = new Dictionary<string, PoolRuntime>(64);

        private class PoolRuntime
        {
            public PoolConfig config;
            public readonly Queue<GameObject> inactive = new Queue<GameObject>(64);
        }

        private void Awake()
        {
            Debug.Log("Awake");
            BuildPools();
        }

        private void BuildPools()
        {
            Debug.Log("BuildPools");
            _runtime.Clear();

            foreach (var p in pools)
            {
                if (p == null || p.prefab == null) continue;

                var key = MakeKey(p);
                if (_runtime.ContainsKey(key))
                {
                    Debug.LogWarning($"[ObjectPooling] Duplicate pool key '{key}'. Skipping one.", this);
                    continue;
                }

                var rt = new PoolRuntime { config = p };
                _runtime.Add(key, rt);

                // Prewarm
                for (int i = 0; i < p.prewarmCount; i++)
                {
                    var go = CreateInstance(rt);
                    Despawn(go); // will enqueue back
                }
            }
        }

        private static string MakeKey(PoolConfig p)
        {
            return string.IsNullOrWhiteSpace(p.key) ? p.prefab.name : p.key.Trim();
        }

        private GameObject CreateInstance(PoolRuntime pool)
        {
            var parent = pool.config.poolRoot != null ? pool.config.poolRoot : transform;
            var go = Instantiate(pool.config.prefab, parent);
            go.name = pool.config.prefab.name; // keep clean names for hierarchy
            go.SetActive(false);

            // Attach helper component so instance knows how to return.
            var tag = go.GetComponent<PooledObject>();
            if (tag == null) tag = go.AddComponent<PooledObject>();
            tag.Bind(this, MakeKey(pool.config));

            return go;
        }

        /// <summary>
        /// Spawn using prefab name as key (or config.key if you set it).
        /// </summary>
        public GameObject Spawn(string key, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                Debug.LogError("[ObjectPooling] Spawn key is null/empty.", this);
                return null;
            }

            if (!_runtime.TryGetValue(key, out var pool))
            {
                Debug.LogError($"[ObjectPooling] Pool '{key}' not found. Add it to pools list.", this);
                return null;
            }

            GameObject go = null;

            // Get inactive instance
            while (pool.inactive.Count > 0)
            {
                go = pool.inactive.Dequeue();
                if (go != null) break; // skip destroyed refs
            }

            // Create if empty
            if (go == null)
            {
                if (!pool.config.autoExpand)
                {
                    Debug.LogWarning($"[ObjectPooling] Pool '{key}' empty and autoExpand=false.", this);
                    return null;
                }
                go = CreateInstance(pool);
            }

            // Activate and place
            if (parent != null) go.transform.SetParent(parent, false);
            go.transform.SetPositionAndRotation(position, rotation);
            go.SetActive(true);

            return go;
        }

        public GameObject Spawn(string key, Vector3 position)
            => Spawn(key, position, Quaternion.identity, null);

        public GameObject Spawn(string key, Transform spawnPoint)
            => Spawn(key, spawnPoint.position, spawnPoint.rotation, null);

        /// <summary>
        /// Return object to pool (safe to call multiple times).
        /// </summary>
        public void Despawn(GameObject go)
        {
            if (go == null) return;

            var tag = go.GetComponent<PooledObject>();
            if (tag == null || string.IsNullOrWhiteSpace(tag.PoolKey))
            {
                // Not from pool -> just disable
                go.SetActive(false);
                return;
            }

            if (!_runtime.TryGetValue(tag.PoolKey, out var pool))
            {
                // Pool missing (e.g., scene changed) -> just disable
                go.SetActive(false);
                return;
            }

            // Move back to pool root
            var root = pool.config.poolRoot != null ? pool.config.poolRoot : transform;
            go.transform.SetParent(root, false);

            go.SetActive(false);
            pool.inactive.Enqueue(go);
        }

        /// <summary>
        /// Convenience: despawn after delay without coroutine allocation in caller.
        /// </summary>
        public void Despawn(GameObject go, float delaySeconds)
        {
            if (go == null) return;
            if (delaySeconds <= 0f) { Despawn(go); return; }
            StartCoroutine(DespawnAfter(go, delaySeconds));
        }

        private System.Collections.IEnumerator DespawnAfter(GameObject go, float delay)
        {
            yield return new WaitForSeconds(delay);
            Despawn(go);
        }

        /// <summary>
        /// Optional: rebuild pools in runtime (e.g., after editing list).
        /// WARNING: Existing pooled instances will not be tracked if you call this mid-game.
        /// </summary>
        public void Rebuild()
        {
            BuildPools();
        }
    }

    /// <summary>
    /// Small tag so instances can return themselves to pool.
    /// </summary>
    public sealed class PooledObject : MonoBehaviour
    {
        public string PoolKey { get; private set; }
        private ObjectPooling _pool;

        public void Bind(ObjectPooling pool, string key)
        {
            _pool = pool;
            PoolKey = key;
        }

        public void Despawn()
        {
            if (_pool != null) _pool.Despawn(gameObject);
            else gameObject.SetActive(false);
        }
    }
}