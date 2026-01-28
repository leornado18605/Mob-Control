namespace Script
{
    using DG.Tweening;
    using UnityEngine;

    public class CameraController : MonoBehaviour
    {
        [Header("Main Camera")]
        [SerializeField] private Camera cameraMain;
        [SerializeField] private LevelController levelController;

        public void AssignTarget(float timeDelay, float timeDuration)
        {
            if (cameraMain == null) cameraMain = Camera.main;

            var arr = levelController.Levels;
            if (arr.Length == 0)
            {
                return;
            }

            int nextIndex = LevelController.currentLevelIndex + 1;

            float cameraPosZ = arr[nextIndex].spawnPoint.position.z;

            DOVirtual.DelayedCall(timeDelay, () =>
            {
                cameraMain.transform.DOMoveZ(cameraPosZ, timeDuration);
            });
        }
    }
}