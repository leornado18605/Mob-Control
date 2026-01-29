using UnityEngine;
using Common;
using Combat;
using System.Collections;
using DG.Tweening;

namespace Script
{
    public class Shooter : MonoBehaviour
    {
        [SerializeField] private ObjectPooling pooling;
        [SerializeField] private Transform     spawnPoint;

        [Header("Unit")]
        [SerializeField] private EnumSpawnType spawnType;
        [SerializeField] private Transform     enemyGoal;
        [SerializeField] private Transform     cannonGoal;
        [SerializeField] private float         navDelay = 0.15f;

        [SerializeField] private BulletDefinition currentBullet;
        [SerializeField] private Vector3          shootDirection = Vector3.zero;
        [SerializeField] private BulletDefinition superBullet;
        [SerializeField] private Transform        fireOrigin;

        [Header("Speed Boost Settings")]
        [SerializeField] private float boostSpeed = 12f;
        [SerializeField] private float boostAcceleration = 30f;
        [SerializeField] private float boostDuration     = 1.0f;

        public float acceleration;
        public void  SetBullet(BulletDefinition definition) => this.currentBullet = definition;

        public void SetGoals(Transform newEnemyGoal, Transform newCannonGoal)
        {
            this.enemyGoal  = newEnemyGoal;
            this.cannonGoal = newCannonGoal;
        }

        private UnitBehaviour InitializeUnit(GameObject obj)
        {
            if (obj == null || !obj.TryGetComponent<UnitBehaviour>(out var unit)) return null;

            unit.PrepareUnit();
            unit.type = this.spawnType;
            unit.SetRotationByType();

            //Select Goal
            Transform targetGoal = (spawnType == EnumSpawnType.Enemy) ? enemyGoal : cannonGoal;
            unit.SetGoal(targetGoal);

            return unit;
        }

        private Vector3 GetShootDirection() =>
            this.shootDirection == Vector3.zero ? Vector3.forward : this.shootDirection.normalized;

        #region Spawn Normal
        public void SpawnBurst(int amount)
        {
            if (!this.CanSpawn(amount)) return;

            Vector3 origin = spawnPoint.position;
            Vector3 dir    = GetShootDirection();

            for (int i = 0; i < amount; i++)
            {
                var obj = pooling.Spawn(this.currentBullet.poolKey, fireOrigin.position);
                var unit = InitializeUnit(obj);

                if (unit != null)
                    StartCoroutine(FlyToSpawnThenGo(obj, unit));
            }
        }

        private void SetSpeedWhenSpawn(UnitBehaviour unit)
        {
            var agent = unit.Agent;

            float originalSpeed = agent.speed;

            agent.speed        = boostSpeed;
            agent.acceleration = boostAcceleration;

            DOTween.Sequence()
                .AppendInterval(boostDuration)
                .OnComplete(() =>
                {
                    if (unit != null && agent != null && agent.enabled)
                    {
                        agent.speed        = originalSpeed;
                        agent.acceleration = this.acceleration;
                    }
                });
        }
        private bool CanSpawn(int amount) =>
            amount > 0 && pooling != null && spawnPoint != null && currentBullet != null;

        private IEnumerator FlyToSpawnThenGo(GameObject obj, UnitBehaviour unit)
        {
            var agent = unit.Agent;
            if (agent != null) agent.enabled = false;

            yield return obj.transform
                .DOMove(this.spawnPoint.position, 0.25f)
                .SetEase(Ease.OutQuad)
                .WaitForCompletion();

            if (agent != null)
            {
                agent.enabled = true;
                agent.updateRotation = false;
            }
            unit.Go();
            this.SetSpeedWhenSpawn(unit);
        }
        #endregion

        #region Super Shot
        public void SpawnSuper()
        {
            if (!CanSpawnSuper()) return;

            var obj = pooling.Spawn(superBullet.poolKey, spawnPoint.position);
            var unit = InitializeUnit(obj);

            if (unit != null)
                StartCoroutine(GoLate(obj, unit));
        }

        private bool CanSpawnSuper() => this.pooling != null && spawnPoint != null && superBullet != null && !string.IsNullOrEmpty(superBullet.poolKey);

        private IEnumerator GoLate(GameObject obj, UnitBehaviour unit)
        {
            yield return new WaitForSeconds(navDelay);
            if (obj != null && unit != null)
            {
                this.SetSpeedWhenSpawn(unit);
                unit.Go();
            }
        }
        #endregion
    }
}