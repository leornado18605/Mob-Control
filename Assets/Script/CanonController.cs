using Combat;
using UnityEngine;
using DG.Tweening;

namespace Script
{
    using Unity.Cinemachine;

    public class CanonController : MonoBehaviour
    {
        [Header("Move")]
        [SerializeField] private float deadZonePixels = 1f;
        [SerializeField] private float sensitivity = 0.01f;
        [SerializeField] private float minX = -3f;
        [SerializeField] private float maxX = 3f;
        [SerializeField] private float smoothTime = 0.08f;
        [SerializeField] private CharacterController characterController;

        private float _targetX;
        private float _smoothVelocityX;

        [Header("Shoot (Player Input)")]
        [SerializeField] private Shooter spawn;
        [SerializeField] private BulletDefinition currentBullet;

        [SerializeField] private EnergyBar energyBar;
        private float _nextFireTime;

        [Header("Recoil (DOTween)")]
        [SerializeField] private Transform barrel;
        [SerializeField] private float recoilDistance = 0.15f;
        [SerializeField] private float kickDuration   = 0.05f;
        [SerializeField] private float returnDuration = 0.08f;

        [Header("Unit Recoil (DOTween)")]
        [SerializeField] private float unitRecoilDistance = 0.2f;
        [SerializeField] private float unitKickDuration   = 0.06f;
        [SerializeField] private float unitReturnDuration = 0.1f;
        private Vector3 _unitOriginPos;
        private Tween   _unitRecoilTween;

        private Vector3 _barrelOriginPos;
        private Tween   _recoilTween;

#if UNITY_EDITOR
        private bool _mouseDown;
        private Vector2 _lastScreenPos;
#endif

        private void Awake()
        {
            _targetX = transform.position.x;

            if (currentBullet != null && this.spawn != null) this.spawn.SetBullet(currentBullet);

            //Dotween
            if (barrel != null)
                _barrelOriginPos = barrel.localPosition;

            _unitOriginPos = transform.position;

        }

        private void Update()
        {
            HandleMouseMoveInput();
            if (_mouseDown)
            {
                HandleShootInput();
            }
            ApplySmoothedMovement();
        }
        private void HandleShootInput()
        {
            if (!Input.GetMouseButton(0))
            {
                return;
            }
            if (!CanFire()) return;

            FireOnce();
        }

        private bool CanFire()
        {
            if (Time.time < _nextFireTime) return false;
            if (this.spawn == null) return false;
            if (currentBullet == null) return false;
            return true;
        }

        private void FireOnce()
        {

            this._nextFireTime = Time.time + Mathf.Max(0f, currentBullet.fireCooldown);
            this.spawn.SpawnBurst(1);

            if (energyBar != null)
            {
                this.energyBar.AddShot(1);
            }

            this.PlayRecoil();
        }

        private void HandleMouseMoveInput()
        {
            if (TryGetMouseDeltaPixels(out float dxPixels)) this.AccumulateTargetX(dxPixels);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private bool TryGetMouseDeltaPixels(out float dxPixels)
        {
            dxPixels = 0f;

            if (Input.GetMouseButtonDown(0))
            {
                this._mouseDown  = true;
                this._lastScreenPos = Input.mousePosition;
                HandleShootInput();
                return false;
            }

            if (Input.GetMouseButtonUp(0))
            {
                this._mouseDown = false;

                if (energyBar != null)
                {
                    if (this.energyBar.CanFireSuper())
                    {
                        this.spawn.SpawnSuper();
                        Debug.Log("FIRE SUPER!");

                        this.energyBar.ResetBar();
                    }
                }
                return false;
            }

            if (!this._mouseDown) return false;

            Vector2 current = Input.mousePosition;
            Vector2 delta = current - this._lastScreenPos;
            this._lastScreenPos = current;

            if (Mathf.Abs(delta.x) < this.deadZonePixels) return false;

            dxPixels = -delta.x;
            return true;
        }

        private void AccumulateTargetX(float dxPixels)
        {
            float dxWorld = dxPixels * this.sensitivity;
            this._targetX = Mathf.Clamp(this._targetX + dxWorld, minX, maxX);
        }

        private void ApplySmoothedMovement()
        {
            float currentX = transform.position.x;

            float newX = Mathf.SmoothDamp(
                currentX, this._targetX,
                ref this._smoothVelocityX,
                Mathf.Max(0.0001f, smoothTime)
            );

            newX = Mathf.Clamp(newX, minX, maxX);

            Vector3 current = transform.position;
            Vector3 target = new Vector3(newX, current.y, current.z);

            if (this.characterController != null)
                this.characterController.Move(target - current);
            else
                this.transform.position = target;
        }

        private void PlayRecoil()
        {
            if (barrel == null) return;

            this._recoilTween?.Kill();

            this.barrel.localPosition = this._barrelOriginPos;

            this._recoilTween = DOTween.Sequence()
                .Append(
                    barrel.DOLocalMoveY(
                        _barrelOriginPos.y - recoilDistance,
                        kickDuration
                    ).SetEase(Ease.OutQuad)
                )
                .Append(
                    barrel.DOLocalMoveY(
                        _barrelOriginPos.y,
                        returnDuration
                    ).SetEase(Ease.OutBack)
                );
        }
    }


}