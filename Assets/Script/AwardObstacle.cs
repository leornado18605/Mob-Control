using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.AI;

namespace Script
{
    using System;
    using Random = UnityEngine.Random;

    public class AwardObstacle : MonoBehaviour
    {
        public enum AwardAdd
        {
            multi,
            devide
        }

        [Header("Award")]
        [SerializeField] private AwardAdd awardAdd = AwardAdd.multi;
        [SerializeField] private int           countBullet = 2;
        [SerializeField] private EnumSpawnType type1;
        [Header("Pooling")]
        [SerializeField] private ObjectPooling pooling;

        [SerializeField] private string poolKeyOverride = "";

        [Header("Collision")]
        [SerializeField] private Collider obstacleCollider;

        [Header("Goals")]
        [SerializeField] private Transform enemyGoal;
        [SerializeField] private Transform canonGoal;

        [Header("NavMesh Safe")]
        [SerializeField] private float navRange = 2f;

        [SerializeField] private float spawnOffsetRadius = 0.3f;

        [Header("Optional Despawn")]
        [SerializeField] private float despawnAfter = 0f;

        private readonly HashSet<int> _seen = new HashSet<int>();

        private Transform goal;

        public void SetGoals(Transform newEnemyGoal, Transform newCanonGoal)
        {
            this.enemyGoal = newEnemyGoal;
            this.canonGoal = newCanonGoal;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == null) return;
            if (!CompareTag(other)) return;

            int id = other.gameObject.GetInstanceID();
            if (!this._seen.Add(id)) return;

            var unitType = GetUnitType(other);

            Transform targetGoal = (unitType == EnumSpawnType.Enemy)
                ? LevelController.CurrentEnemyGoal
                : LevelController.CurrentCanonGoal;

            var spawnCount = GetSpawnCount();
            var key        = GetPoolKey(other);

            if (!string.IsNullOrWhiteSpace(key))
            {
                SpawnClones(key, spawnCount, unitType, targetGoal);
            }
        }

        private bool CompareTag(Collider other)
        {
            return other.CompareTag("Enemy") || other.CompareTag("Canon");
        }
        private int GetSpawnCount()
        {
            if (countBullet <= 0) return 0;

            if (awardAdd == AwardAdd.multi)
            {
                return countBullet * 2 - 1;
            }
            int half = countBullet / 2;

            return half <= 0 ? 0 : half;

        }
        private EnumSpawnType GetUnitType(Collider other)
        {
            if (other.TryGetComponent<UnitBehaviour>(out var unit))
                return unit.type;

            return other.CompareTag("Enemy") ? EnumSpawnType.Enemy : EnumSpawnType.Cannon;
        }

        private string GetPoolKey(Collider other)
        {
            if (!string.IsNullOrWhiteSpace(poolKeyOverride))
                return poolKeyOverride.Trim();

            if (other.TryGetComponent<PooledObject>(out var tag) && !string.IsNullOrWhiteSpace(tag.PoolKey))
                return tag.PoolKey;

            return other.gameObject.name;
        }

        private void SpawnClones(string key, int count, EnumSpawnType type, Transform goal)
        {
            if (pooling == null) return;

            for (var i = 0; i < count; i++)
            {
                var pos = GetSpawnPos(i);
                if (!TryGetNavPos(pos, navRange, out Vector3 navPos))
                    navPos = pos;

                var clone = this.pooling.Spawn(key, navPos);
                if (clone == null) continue;

                if (type == EnumSpawnType.Cannon)
                    clone.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                else
                    clone.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

                this._seen.Add(clone.GetInstanceID());

                this.IgnoreObstacleCollision(clone);

                this.SetupUnitAndGo(clone, type, goal);
            }
        }

        private Vector3 GetSpawnPos(int index)
        {
            Vector2 rnd = Random.insideUnitCircle * Mathf.Max(0f, spawnOffsetRadius);
            Vector3 off = new Vector3(rnd.x, 0f, rnd.y);

            float t = (index % 6) * 0.12f;
            off += new Vector3(t, 0f, -t);

            return this.transform.position + off;
        }

        private static bool TryGetNavPos(Vector3 pos, float range, out Vector3 navPos)
        {
            navPos = pos;
            var r = Mathf.Max(0.1f, range);

            if (!NavMesh.SamplePosition(pos, out NavMeshHit hit, r, NavMesh.AllAreas)) return false;
            navPos = hit.position;
            return true;
        }

        private void IgnoreObstacleCollision(GameObject clone)
        {
            if (obstacleCollider == null) return;

            if (clone.TryGetComponent<Collider>(out var col))
                Physics.IgnoreCollision(col, this.obstacleCollider, true);
        }

        private void SetupUnitAndGo(GameObject clone, EnumSpawnType type, Transform goal)
        {
            if (!clone.TryGetComponent<UnitBehaviour>(out var unit)) return;

            unit.type = type;

            if (goal != null)
                unit.SetGoal(goal);

        }
    }
}