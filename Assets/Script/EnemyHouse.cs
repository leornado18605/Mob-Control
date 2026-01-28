using UnityEngine;

namespace Script
{
    public class EnemyHouse : MonoBehaviour
    {
        [SerializeField] private TowerData    towerData;
        [SerializeField] private HealthSystem healthSystem;
        public                   HealthSystem Health { get; private set; }

        private void Awake()
        {
            this.Health = this.healthSystem;
        }
        private void Start()
        {
            this.healthSystem.Init(towerData.healthTower);
        }
    }
}