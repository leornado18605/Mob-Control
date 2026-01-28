namespace Script
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "Game/LevelDataSO", fileName = "LevelDataSO")]
    public class LevelDataSO : ScriptableObject
    {
        public GameObject[] towerPrefab;
        public GameObject[] obstaclePrefab;
        public GameObject[] awardPrefab;
    }
}