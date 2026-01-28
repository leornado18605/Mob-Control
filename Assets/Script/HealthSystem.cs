using UnityEngine;
using System;

namespace Script
{
    using Common;
    using TMPro;
    using UnityEngine.UI;

    public class HealthSystem : MonoBehaviour, IDamageable
    {
        public                   int   CurrentHealth { get; private set; }
        [SerializeField] private Image healthBar;
        public event Action            OnDeath;
        private int                    maxHealth;

        [SerializeField] private bool     isEnemyHouse;
        [SerializeField] private TMP_Text healthText;

        public void Init(int maxHealth)
        {
            this.maxHealth       = maxHealth;
            CurrentHealth        = maxHealth;

            this.UpdateUI();
        }

        public void TakeDamage(int damage)
        {
            if (damage <= 0) return;

            this.CurrentHealth -= damage;
            this.CurrentHealth    =  Mathf.Clamp(CurrentHealth, 0, maxHealth);

            UpdateUI();

            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            this.OnDeath?.Invoke();

            if (TryGetComponent<Common.PooledObject>(out var pooled))
                pooled.Despawn();
            else
                gameObject.SetActive(false);
        }

        private void UpdateUI()
        {
            if (this.isEnemyHouse)
            {
                this.healthText.text = this.CurrentHealth.ToString();
            }

            else
            {
                this.healthBar.fillAmount = (float)CurrentHealth / maxHealth;
            }
        }
    }
}