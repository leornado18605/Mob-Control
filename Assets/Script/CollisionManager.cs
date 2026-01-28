using UnityEngine;
using Combat;
using System.Collections;
using Script;

public class CollisionManager : MonoBehaviour
{
    [SerializeField] private float attackInterval = 1f;

    private int         _damage;
    private IDamageable _currentTarget;
    private Coroutine   _attackRoutine;

    public void Initialize(int damageValue)
    {
        this._damage = damageValue;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsEnemy(other.gameObject)) return;


        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            this._currentTarget = damageable;

            if (this._attackRoutine == null) this._attackRoutine = this.StartCoroutine(this.AttackLoop());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!this.IsEnemy(other.gameObject)) return;
        this.StopAttack();
    }

    private IEnumerator AttackLoop()
    {
        while (this._currentTarget != null && this._currentTarget.CurrentHealth > 0)
        {
            this._currentTarget.TakeDamage(_damage);
            yield return new WaitForSeconds(attackInterval);
        }
        this._attackRoutine = null;
    }

    private void StopAttack()
    {
        if (this._attackRoutine != null)
        {
            this.StopCoroutine(_attackRoutine);
            this._attackRoutine = null;
        }
        _currentTarget = null;
    }

    private bool IsEnemy(GameObject other)
    {
        if (this.CompareTag("Canon"))
            return other.CompareTag("Enemy") || other.CompareTag("EnemyHouse");

        return other.CompareTag("Canon");
    }
}