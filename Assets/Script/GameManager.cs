namespace Script
{
    using UnityEngine;
    using System;
    using Common;
    using DG.Tweening;

    public enum GameState
    {
        Playing,
        Win,
        Lose,
        Pause
    }

    public class GameManager : Singleton<GameManager>
    {
        public GameState CurrentState { get; private set; } = GameState.Playing;

        public event Action OnGameWin;
        public event Action OnGameLose;

        [Header("References")]
        [SerializeField] private HealthSystem enemyHouseHealth;
        [SerializeField] private HealthSystem playerHealth;

        [Header("Main Camera")]
        [SerializeField] private Camera cameraMain;

        [Header("Level System")]
        [SerializeField] private LevelData[] levels;
        [SerializeField] private GameObject gameObjectA;
        private int currentLevelIndex = 0;
        private ObjectPooling objectPool;
        [SerializeField] private GameObject poolRoot;

        private void Start()
        {
            if (cameraMain == null) cameraMain = Camera.main;

            LoadLevel(0);

            if (enemyHouseHealth != null)
                enemyHouseHealth.OnDeath += HandleWin;

            if (playerHealth != null)
                playerHealth.OnDeath += HandleLose;
        }

        private void HandleWin()
        {
            this.AssignTarget(0.75f, 1f);
            if (CurrentState != GameState.Playing) return;

            CurrentState = GameState.Win;

            OnGameWin?.Invoke();

            Destroy(this.poolRoot.gameObject);

            Invoke(nameof(GoToNextLevel), 0.5f);
        }

        private void AssignTarget(float timeDelay, float timeDuration)
        {
            if (cameraMain == null) cameraMain = Camera.main;
            if (cameraMain == null) return;

            var cameraPosZ = this.levels[this.currentLevelIndex + 1].spawnPoint.transform.position.z;

            DOVirtual.DelayedCall(timeDelay, () =>
            {
                cameraMain.transform.DOMoveZ(cameraPosZ, timeDuration);
            });
        }

        private void HandleLose()
        {
            if (CurrentState != GameState.Playing) return;

            CurrentState = GameState.Lose;
            Time.timeScale = 0f;

            Debug.Log("GAME MANAGER: LOSE");
            OnGameLose?.Invoke();
        }

        private void GoToNextLevel()
        {
            Time.timeScale = 1f;

            if (this.currentLevelIndex < this.levels.Length)
            {
                DOVirtual.DelayedCall(3f, () =>
                {
                    levels[currentLevelIndex - 1].levelPrefab.SetActive(false);
                });
            }

            this.currentLevelIndex++;

            if (this.currentLevelIndex >= levels.Length)
            {
                Debug.Log("ALL LEVELS COMPLETED!");
                return;
            }

            this.LoadLevel(this.currentLevelIndex);
        }

        private void LoadLevel(int index)
        {
            CurrentState = GameState.Playing;

            levels[index].levelPrefab.SetActive(true);

            Transform target = levels[index].spawnPoint;

            this.gameObjectA.transform
                .DOMove(target.position, 1f)
                .SetEase(Ease.InOutSine);
        }

        public void PauseGame()
        {
            if (CurrentState != GameState.Playing) return;

            CurrentState = GameState.Pause;
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            if (CurrentState != GameState.Pause) return;

            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
        }
    }
}