using UnityEngine;
using Combat;
using System.Collections;
using Script;

public class CollisionManager : MonoBehaviour
{
    public int canonDamageToEnemy = 20;
    public int enemyDamageToCanon = 10;
    public float attackInterval = 1f;


    private IDamageable iDamageAble;
    private Coroutine _attackRoutine;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsEnemy(other.gameObject)) return;


        if (CompareTag("Canon") && EnemyHouse.Instance != null)
        {
            iDamageAble = EnemyHouse.Instance.Health;
        }
        else
        {
            iDamageAble = other.GetComponentInParent<IDamageable>();
        }

        if (_attackRoutine == null)
            _attackRoutine = StartCoroutine(AttackLoop());
    }


    private void OnTriggerExit(Collider other)
    {
        if (!IsEnemy(other.gameObject)) return;
        StopAttack();
    }

    private IEnumerator AttackLoop()
    {
        while (this.iDamageAble != null && this.iDamageAble.CurrentHealth > 0)
        {
            int dmg = CompareTag("Canon")
                ? canonDamageToEnemy
                : enemyDamageToCanon;

            this.iDamageAble.TakeDamage(dmg);
            yield return new WaitForSeconds(attackInterval);
        }

        _attackRoutine = null;
    }

    private void StopAttack()
    {
        if (_attackRoutine != null)
        {
            StopCoroutine(_attackRoutine);
            _attackRoutine = null;
        }

        this.iDamageAble = null;
    }

    private bool IsEnemy(GameObject other)
    {
        if (CompareTag("Canon"))
            return other.CompareTag("Enemy") || other.CompareTag("EnemyHouse");

        if (CompareTag("Enemy"))
            return other.CompareTag("Canon");

        return false;
    }
}