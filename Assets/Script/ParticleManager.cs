using UnityEngine;
using Common;

namespace Script
{
    public class ParticleManager : MonoBehaviour
    {
        public static ParticleManager Instance { get; private set; }

        [SerializeField] private ObjectPooling pooling;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SpawnFromSO(string key, Vector3 pos, Quaternion rot)
        {
            if (pooling == null) return;
            pooling.Spawn(key, pos, rot);
        }
    }
}