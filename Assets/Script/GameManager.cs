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
        public static GameState CurrentState { get; set; } = GameState.Playing;

        public event Action OnGameWin;
        public event Action OnGameLose;

        [Header("References")]
        [SerializeField] private HealthSystem enemyHouseHealth;
        [SerializeField] private HealthSystem playerHealth;

        [SerializeField] private CameraController cameraController;

        [Header("Level System")]
        [SerializeField] private GameObject gameObjectA;
        [SerializeField] private ObjectPooling objectPool;
        [SerializeField] private GameObject poolRoot;
        [SerializeField] private LevelController levelController;
        public GameObject GameObjectA => gameObjectA;
        private void Start()
        {
            this.levelController.LoadLevel(0);

            if (this.enemyHouseHealth != null)
            {
                this.enemyHouseHealth.OnDeath += this.HandleWin;
            }

            if (this.playerHealth != null)
            {
                this.playerHealth.OnDeath += this.HandleLose;
            }
        }

        private void HandleWin()
        {
            if (CurrentState != GameState.Playing) return;

            this.cameraController.AssignTarget(0.75f, 1f);
            CurrentState = GameState.Win;

            OnGameWin?.Invoke();

            Invoke(nameof(GoToNextLevel), 0.5f);
        }

        private void HandleLose()
        {
            if (CurrentState != GameState.Playing) return;

            CurrentState = GameState.Lose;
            Time.timeScale = 0f;

            this.OnGameLose?.Invoke();
        }

        private void GoToNextLevel()
        {
            Time.timeScale = 1f;

            DespawnAllPooledObjects();

            if (LevelController.currentLevelIndex < this.levelController.Levels.Length)
            {
                this.levelController.Levels[LevelController.currentLevelIndex].levelPrefab.SetActive(false);
            }

            LevelController.currentLevelIndex++;

            if (LevelController.currentLevelIndex >= this.levelController.Levels.Length)
            {
                Debug.Log("ALL LEVELS COMPLETED!");
                return;
            }

            this.levelController.LoadLevel(LevelController.currentLevelIndex);
        }

        private void DespawnAllPooledObjects()
        {
            for (int i = this.poolRoot.transform.childCount - 1; i >= 0; i--)
            {
                var child = poolRoot.transform.GetChild(i).gameObject;
                if (child.activeSelf)
                    objectPool.Despawn(child);
            }
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