// Trong LevelController.cs

using DG.Tweening;
using Script;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private LevelData[] levels;
    [SerializeField] private Shooter     shooter;

    public static int currentLevelIndex = 0;

    public static Transform CurrentEnemyGoal;
    public static Transform CurrentCanonGoal;

    public LevelData[] Levels => this.levels;
    public void LoadLevel(int index)
    {
        GameManager.CurrentState = GameState.Playing;
        var data = this.levels[index];
        data.levelPrefab.SetActive(true);

        CurrentEnemyGoal = data.enemyGoal.transform;
        CurrentCanonGoal = data.canonGoal.Length > 0 ? data.canonGoal[0].transform : null;

        if (shooter != null)
        {
            shooter.SetGoals(CurrentEnemyGoal, CurrentCanonGoal);
        }

        this.gameManager.GameObjectA.transform
            .DOMove(data.spawnPoint.position, 1f)
            .SetEase(Ease.InOutSine);
    }
}