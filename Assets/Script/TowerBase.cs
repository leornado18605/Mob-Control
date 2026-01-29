namespace Script
{
    using System;
    using System.Collections;
    using UnityEngine;

    public class TowerBase : MonoBehaviour
    {
        [SerializeField] private Shooter      spawn;
        [SerializeField] private TowerData    towerData;
        [SerializeField] private HealthSystem healthSystem;
        private                  Coroutine    _loop;

        private void OnEnable()
        {
            if (this.spawn == null || towerData == null) return;
        }

        private void Start()
        {
            this.healthSystem.Init(towerData.healthTower);
            this.spawn.SetBullet(towerData.bulletDefinition);

            this._loop = this.StartCoroutine(this.SpawnLoop());
        }

        private void OnDisable()
        {
            if (_loop != null) StopCoroutine(_loop);
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                float loop  = Mathf.Max(0.05f, this.towerData.spawnTimeLoop);
                int   count = Mathf.Max(1, this.towerData.countSpawn);

                this.spawn.SpawnBurst(count);

                yield return new WaitForSeconds(loop);
            }
        }
    }
}