namespace Script
{
    using System;
    using Unity.Cinemachine;
    using UnityEngine;

    public class BillBoardCamera : MonoBehaviour
    {
        [SerializeField]   private Camera cameraMain;

        private void LateUpdate()
        {
            this.SetRotationObject();
        }

        private void SetRotationObject()
        {
            transform.rotation = this.cameraMain.transform.rotation;

        }
    }
}