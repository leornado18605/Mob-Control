using UnityEngine;
using Common;

namespace Script
{
    public class EffectAutoDespawn : MonoBehaviour
    {
        [SerializeField] private float        duration = 1.5f;
        private                  PooledObject _pooledObject;

        private void Awake()
        {
            _pooledObject = GetComponent<PooledObject>();
        }

        private void OnEnable()
        {
            CancelInvoke(nameof(Despawn));
            Invoke(nameof(Despawn), duration);
        }

        private void Despawn()
        {
            if (_pooledObject != null) _pooledObject.Despawn();
            else gameObject.SetActive(false);
        }
    }
}