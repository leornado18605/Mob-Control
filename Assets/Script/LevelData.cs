using UnityEngine;

[System.Serializable]
public class LevelData
{
    public GameObject levelPrefab;
    public Transform  spawnPoint;

    public GameObject enemyGoal;
    public GameObject[] canonGoal;
}