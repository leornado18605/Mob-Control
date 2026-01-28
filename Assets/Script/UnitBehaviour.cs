using UnityEngine;
using UnityEngine.AI;

namespace Script
{
    using System;
    using Combat;

    public class UnitBehaviour : MonoBehaviour
    {
        [Header("Type")]
        public EnumSpawnType type;

        [Header("Move")]
        [SerializeField] private NavMeshAgent agent;

        [Header("Keep On NavMesh")]
        [SerializeField] private float keepRange = 2f;
        [SerializeField] private float fixDistance = 0.3f;
        [SerializeField] private float checkTime   = 0.2f;

        [Header("Scan Target")]
        [SerializeField] private float scanRadius = 6f;
        [SerializeField] private float     chaseStopDistance = 1.2f;
        [SerializeField] private LayerMask scanMask          = ~0;

        [SerializeField] private BulletDefinition unitData;
        [SerializeField] private HealthSystem     healthSystem;

        [SerializeField] private bool             isEnemyHouse;

        [SerializeField] private float      stopDistance   = 1.2f;
        [SerializeField] private float      resumeDistance = 1.5f;

        private                  bool       _isAttacking;
        private readonly         Collider[] _hits = new Collider[30];
        private                  Transform  _chaseTarget;
        private                  Transform  _goal;
        private                  float      _nextCheck;

        public NavMeshAgent Agent { get; private set; }

        private void Awake()
        {
            this.Agent = this.agent;
            if (this.agent != null) this.agent.updateRotation = false;

        }

        private void Update()
        {
            if (Time.time < this._nextCheck) return;
            this._nextCheck = Time.time + checkTime;

            this.Find();
        }

        private void Start()
        {
            healthSystem.Init(unitData.maxHealth);
            healthSystem.OnDeath += OnUnitDeath;
        }
        public void SetGoal(Transform goalTf)
        {
            this._goal = goalTf;
        }

        #region Movement
        public void Go()
        {
            if (agent == null)
            {
                return;
            }
            if (_goal == null)
            {
                return;
            }
            this.agent.isStopped = false;
            this.agent.SetDestination(this._goal.position);
        }

        public void Stop()
        {
            if (agent == null) return;
            agent.isStopped = true;
            agent.ResetPath();
        }

        #endregion

        #region FindTarget (Tag)
        private void Find()
        {
            if (!IsAgentReady()) return;

            this._chaseTarget = GetNearestTarget(this.GetEnemyTag());
            if (this._chaseTarget == null)
            {
                this._isAttacking = false;
                this.Go(); return;
            }

            float dist = Vector3.Distance(transform.position, _chaseTarget.position);

            if (_isAttacking)
            {
                if (dist > this.resumeDistance)
                {
                    this._isAttacking = false;
                    ChaseTo(_chaseTarget);
                }
                else
                {
                    this.Stop();
                    this.agent.velocity = Vector3.zero;
                }
                return;
            }

            if (dist <= stopDistance)
            {
                this._isAttacking = true;
                this.Stop();
                this.agent.velocity = Vector3.zero;
                return;
            }

            ChaseTo(_chaseTarget);
        }

        private bool IsAgentReady()
        {
            if (agent == null) return false;
            if (!agent.enabled) return false;
            if (!agent.isOnNavMesh) return false;
            return true;
        }

        private void ChaseTo(Transform t)
        {
            if (t == null) return;
            agent.isStopped = false;
            agent.SetDestination(t.position);
        }

        private string GetEnemyTag()
        {
            return this.type == EnumSpawnType.Cannon ? "Enemy" : "Canon";
        }

        private Transform GetNearestTarget(string tagName)
        {
            int count = ScanAround(out Collider[] list);
            return PickNearestByTag(list, count, tagName);
        }


        private int ScanAround(out Collider[] list)
        {
            list = this._hits;
            return Physics.OverlapSphereNonAlloc(this.transform.position, this.scanRadius, this._hits, this.scanMask);
        }

        private Transform PickNearestByTag(Collider[] list, int count, string tagName)
        {
            Transform best     = null;
            var     bestDist = 999999f;

            for (var i = 0; i < count; i++)
            {
                var t = GetValidTarget(list[i], tagName);
                if (t == null) continue;

                float d = (t.position - transform.position).sqrMagnitude;
                if (d < bestDist)
                {
                    bestDist = d;
                    best     = t;
                }
            }

            return best;
        }

        private Transform GetValidTarget(Collider col, string tagName)
        {
            if (col == null) return null;

            Transform t = col.transform;
            if (t == transform) return null;
            if (!t.CompareTag(tagName)) return null;

            return t;
        }
        #endregion

        #region Damage

        private void OnUnitDeath()
        {
            this.Stop();
        }

        public void SetRotationByType()
        {
            if (type == EnumSpawnType.Cannon)
            {
                this.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
            else if (type == EnumSpawnType.Enemy)
            {
                this.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }

        #endregion

    }
}