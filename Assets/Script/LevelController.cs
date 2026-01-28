namespace Script
{
    using DG.Tweening;
    using UnityEngine;

    public class LevelController : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private LevelData[] levels;
        public static int currentLevelIndex = 0;

        public LevelData[] Levels => this.levels;

        public void LoadLevel(int index)
        {
            Debug.Log("Loading level " + index);
            GameManager.CurrentState = GameState.Playing;

            this.levels[index].levelPrefab.SetActive(true);

            Transform target = levels[index].spawnPoint;

            this.gameManager.GameObjectA.transform
                .DOMove(target.position, 1f)
                .SetEase(Ease.InOutSine);
        }
    }
}