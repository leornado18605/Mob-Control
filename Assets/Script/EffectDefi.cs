using UnityEngine;

namespace Script
{
    [CreateAssetMenu(fileName = "New Effect", menuName = "ScriptableObjects/EffectDefinition")]
    public class EffectDefinition : ScriptableObject
    {
        [SerializeField] private string poolKey;

        public void Play(Vector3 position, Quaternion rotation)
        {
            if (string.IsNullOrEmpty(poolKey)) return;

            ParticleManager.Instance.SpawnFromSO(poolKey, position, rotation);
        }

        public void Play(Vector3 position)
        {
            Play(position, Quaternion.identity);
        }
    }
}