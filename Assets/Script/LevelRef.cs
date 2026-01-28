using UnityEngine;

namespace Script
{
    public class LevelRefs : MonoBehaviour
    {
        [Header("Spawn Points")]
        public Transform playerSpawnPoint;

        [Header("Navigation Goals")]
        public Transform enemyGoal;  // Đích của quân Địch
        public Transform cannonGoal; // Đích của quân Ta

        [Header("Combat References")]
        public HealthSystem enemyHouseHealth;

        // Senior Tip: Cache sẵn các thành phần bên trong để Controller không phải dùng GetComponents
        [Header("Internal References")]
        public AwardObstacle[] obstacles;
        public TowerBase[] towers;

        [ContextMenu("Auto Fill Refs")]
        public void AutoFill()
        {
            obstacles        = GetComponentsInChildren<AwardObstacle>(true);
            towers           = GetComponentsInChildren<TowerBase>(true);
            enemyHouseHealth = GetComponentInChildren<HealthSystem>(true);
        }
    }
}