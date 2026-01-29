using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class EnergyBar : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int maxEnergy = 21;
        [SerializeField] private int   shotsPerEnergy = 3;
        [SerializeField] private Image imageEnergy;
        [SerializeField] private Color normalColor = Color.red;
        [SerializeField] private Color fullColor   = Color.yellow;

        private int  shotCount;
        private bool isFull;

        private int TotalShotsNeeded => maxEnergy * shotsPerEnergy;

        public void AddShot(int amount = 1)
        {
            if (isFull) return;

            shotCount += amount;

            float fill = (float)shotCount / TotalShotsNeeded;
            imageEnergy.fillAmount = fill;

            if (shotCount >= TotalShotsNeeded)
            {
                isFull                 = true;
                imageEnergy.fillAmount = 1f;
                imageEnergy.color      = fullColor;
                Debug.Log("ENERGY FULL - READY TO SUPER!");
            }
        }

        public bool CanFireSuper()
        {
            return this.isFull;
        }

        public void ResetBar()
        {
            shotCount              = 0;
            isFull                 = false;
            imageEnergy.fillAmount = 0f;
            imageEnergy.color      = normalColor;
            Debug.Log("Energy Bar Reset");
        }
    }
}