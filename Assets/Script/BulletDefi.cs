using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(menuName = "Combat/Bullet Definition", fileName = "BulletDefinition")]
    public class BulletDefinition : ScriptableObject
    {
        [Header("Pooling")]
        public string poolKey = "Bullet";

        public float fireCooldown = 0.15f;

        [Header("Health")]
        public int maxHealth = 100;

        public int damage = 20;
    }
}