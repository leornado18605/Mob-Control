namespace Script
{
    using Combat;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Game/Tower Data", fileName = "TowerData")]
    public class TowerData : ScriptableObject
    {
        public float            spawnTimeLoop = 2f;
        public int              countSpawn    = 4;
        public BulletDefinition bulletDefinition;
        public int              healthTower;
    }
}