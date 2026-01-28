using UnityEngine;
using Common;
using Combat;

namespace Script
{
    using System.Collections;
    using DG.Tweening;
    using UnityEngine.AI;

    public class ShooterManager : MonoBehaviour
    {
        [SerializeField] private ObjectPooling pooling;
        [SerializeField] private Transform spawnPoint;

        [Header("Unit")]
        [SerializeField] private EnumSpawnType spawnType;
        [SerializeField] private Transform enemyGoal;
        [SerializeField] private Transform cannonGoal;
        [SerializeField] private float navDelay = 0.15f;

        [SerializeField] private BulletDefinition currentBullet;
        [SerializeField] private Vector3          shootDirection = Vector3.zero;
        [SerializeField] private BulletDefinition superBullet;

        [SerializeField] private Transform fireOrigin;

        public void SetBullet(BulletDefinition definition)
        {
            this.currentBullet = definition;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void SpawnBurst(int amount)
        {
            if (!this.CanSpawn(amount)) return;

            Vector3 origin = spawnPoint.position;
            Vector3 dir = GetShootDirection();

            for (int i = 0; i < amount; i++) this.SpawnOne(origin, dir);
        }

        private bool CanSpawn(int amount)
        {
            if (amount <= 0) return false;
            if (this.pooling == null) return false;
            if (this.spawnPoint == null) return false;
            if (this.currentBullet == null) return false;
            return true;
        }

        private Vector3 GetShootDirection()
        {
            return this.shootDirection == Vector3.zero ? Vector3.forward : this.shootDirection.normalized;
        }
        #region Spawn
        private void SpawnOne(Vector3 origin, Vector3 dir)
        {
            var obj = pooling.Spawn(
                this.currentBullet.poolKey,
                fireOrigin.position
            );

            if (!obj.TryGetComponent<UnitBehaviour>(out var unit)) return;

            unit.PrepareUnit();
            unit.type = this.spawnType;
            unit.SetRotationByType();

            var goal = (spawnType == EnumSpawnType.Enemy)
                ? this.enemyGoal
                : this.cannonGoal;

            unit.SetGoal(goal);

            this.StartCoroutine(this.FlyToSpawnThenGo(obj, unit));
        }

        private IEnumerator FlyToSpawnThenGo(GameObject obj, UnitBehaviour unit)
        {
            var agent = unit.Agent;

            if (agent != null)
                agent.enabled = false;

            var end   = this.spawnPoint.position;    // spawnPoint

            yield return obj.transform
                .DOMove(end, 0.25f)
                .SetEase(Ease.OutQuad)
                .WaitForCompletion();

            if (agent != null)
            {
                agent.enabled        = true;
                agent.updateRotation = false;
            }

            unit.Go();
        }

        private void SetupUnit(GameObject obj)
        {
            if (!obj.TryGetComponent<UnitBehaviour>(out var unit)) return;

            unit.type = this.spawnType;
            unit.SetRotationByType();
            var goal = (spawnType == EnumSpawnType.Enemy) ? this.enemyGoal : this.cannonGoal;
            unit.SetGoal(goal);

            this.StartCoroutine(this.GoLate(obj, unit));
        }

        private IEnumerator GoLate(GameObject obj, UnitBehaviour unit)
        {
            yield return new WaitForSeconds(navDelay);

            if (obj == null || unit == null) yield break;

            unit.Go();
        }
#endregion
        #region Super Shot (Full Energy)
        public void SpawnSuper()
        {
            if (!CanSpawnSuper()) return;

            var origin = spawnPoint.position;
            var dir    = GetShootDirection();

            SpawnSuperOne(origin, dir);
        }

        private bool CanSpawnSuper()
        {
            if (pooling == null) return false;
            if (spawnPoint == null) return false;
            if (superBullet == null) return false;
            if (string.IsNullOrWhiteSpace(superBullet.poolKey)) return false;
            return true;
        }

        private void SpawnSuperOne(Vector3 origin, Vector3 dir)
        {
            var obj = pooling.Spawn(superBullet.poolKey, origin);
            if (obj == null) return;

            this.SetupUnit(obj);
        }

        #endregion

    }
}