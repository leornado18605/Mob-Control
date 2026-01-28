using UnityEngine;

namespace Script
{
    public class EnemyHouse : Singleton<EnemyHouse>
    {
        [SerializeField] private TowerData    towerData;
        [SerializeField] private HealthSystem healthSystem;
        public                   HealthSystem Health { get; private set; }

        private void Awake()
        {
            Health = healthSystem;
        }
        private void Start()
        {
            healthSystem.Init(towerData.healthTower);
        }
    }
}