namespace Script
{
    using System;
    using System.Collections;
    using UnityEngine;

    public class TowerBase : MonoBehaviour
    {
        [SerializeField] private ShooterManager      shooter;
        [SerializeField] private TowerData    towerData;
        [SerializeField] private HealthSystem healthSystem;
        private                  Coroutine    _loop;

        private void OnEnable()
        {
            if (shooter == null || towerData == null) return;
        }

        private void Start()
        {
            healthSystem.Init(towerData.healthTower);
            shooter.SetBullet(towerData.bulletDefinition);

            healthSystem.Init(towerData.healthTower);

            _loop = StartCoroutine(SpawnLoop());
        }

        private void OnDisable()
        {
            if (_loop != null) StopCoroutine(_loop);
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                float loop  = Mathf.Max(0.05f, towerData.spawnTimeLoop);
                int   count = Mathf.Max(1, towerData.countSpawn);

                this.shooter.SpawnBurst(count);

                yield return new WaitForSeconds(loop);
            }
        }
    }
}