using UnityEngine;

namespace Script
{
    using UnityEngine.UI;

    public class EnergyBar : MonoBehaviour
    {
        [Header("Rule")]
        [SerializeField] private int shotsPerEnergy = 3;
        [SerializeField] private int maxEnergy = 21;
        [SerializeField] private Image imageEnergy;
        [SerializeField] private ShooterManager shooterManager;
        private int  shotCount;
        private int  energy;
        private bool isFull;

        public int   Energy => energy;
        public float energyPercent = 0.025f;

        public void AddShot(int amount = 1)
        {
            if (!CanAdd(amount)) return;

            AddToShot(amount);
            UpdateEnergyFromShots();
        }

        #region Feature: Validate
        private bool CanAdd(int amount)
        {
            if (amount <= 0 || this.isFull) return false;
            return true;
        }
        #endregion

        #region Feature: Shot Count
        private void AddToShot(int amount)
        {
            shotCount += amount;
            this.GetEnergyPercent();

            if (this.IsFullEnergy())
            {
                isFull                 = true;
                Debug.Log("IsFull");
                this.ChangeColorEnergy();
            }
        }

        private void ChangeColorEnergy()
        {
            this.imageEnergy.color = Color.yellow;
        }
        private bool IsFullEnergy()
        {
            var numberCheckFull = imageEnergy.fillAmount > 0.999f;
            return numberCheckFull;
        }
        #endregion

        #region Feature: Energy Calcultor
        private void UpdateEnergyFromShots()
        {
            int newEnergy = CalcEnergy(shotCount);

            if (newEnergy >= maxEnergy)
            {
                SetFull();
                return;
            }

            SetEnergy(newEnergy);
        }

        private int CalcEnergy(int shots)
        {
            int step = Mathf.Max(1, shotsPerEnergy);
            return shots / step;
        }

        private void SetFull()
        {
            energy = maxEnergy;
            isFull = true;
        }

        private void SetEnergy(int value)
        {
            energy = value;
        }
#endregion

        private float GetEnergyPercent()
        {
            var energyFillAmount = imageEnergy.fillAmount += energyPercent;
            return energyFillAmount;
        }
        public bool OnRelease()
        {
            if (!isFull) return false;

            Debug.Log("Release -> Fire Super");
            return true;
        }

        public void ResetBar()
        {
            imageEnergy.fillAmount = 0;
            imageEnergy.color      = Color.red;
            shotCount              = 0;
            energy                 = 0;
            isFull                 = false;
        }
    }
}