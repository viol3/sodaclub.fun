using System;
using Bermuda.Animation;
using PathCreation;
using UnityEngine;
using Ali.Helper;
using DG.Tweening;

namespace Bermuda.Runner
{
    public class BermudaRunnerCharacter : LocalSingleton<BermudaRunnerCharacter>
    {
        [SerializeField] private Transform _localMover;
        [SerializeField] private Transform _localMoverTarget;
        
        [SerializeField] private SimpleAnimancer _animancer;
        [SerializeField] private PlayerSwerve _playerSwerve;
        [Space]
        [SerializeField] private string _idleAnimName = "Idle";
        [SerializeField] private float _idleAnimSpeed = 1f;
        [SerializeField] private string _runAnimName = "Walking";
        [SerializeField] private float _runAnimSpeed = 2f;
        [Space]
        [SerializeField] private float _tempSpeed;
        
        private PathCreator _pathCreator;
        private GameConfig _gameConfig;

        private Vector3 _oldPosition;
        private float _distance = 0;
        private bool _running = false;
        private bool _dodgingBack = false;
        private Tweener _forwardSpeedTweeen;
        private float _initialSpeed;
        private bool _isEndGameStarted = false;

        protected override void Awake()
        {
            base.Awake();
            _playerSwerve.OnSwerve += PlayerSwerve_OnSwerve;
            _gameConfig = GameManager.Instance.GameConfig();
            _distance = _gameConfig.StartDistance;
            _oldPosition = _localMoverTarget.localPosition;
        }
        
        private void OnDestroy()
        {
            GameManager.OnGameplayEnded -= StopMoving;
        }

        public void Init()
        {
            GameManager.OnGameplayEnded += StopMoving;

            _pathCreator = GameManager.Instance.GetCurrentLevel().GetPathCreator();
            _distance = _gameConfig.StartDistance;
            _oldPosition = _localMoverTarget.localPosition;
            _initialSpeed = _gameConfig.ForwardSpeed;
            _tempSpeed = _gameConfig.ForwardSpeed;
            _localMover.transform.localPosition = new Vector3(0f, _localMover.transform.localPosition.y, _localMover.transform.localPosition.z);
            UpdatePath();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("EndGame") && !_isEndGameStarted)
            {
                _isEndGameStarted = true;
                CameraController.Instance.SetPov(CameraPov.EndGame);
            }
            if (other.CompareTag("FinishLine"))
            {
                GameManager.Instance.FinishGamePlay(true);
            }
        }

        void UpdateRotation()
        {
            if (!_gameConfig.MoveEnabled)
            {
                return;
            }

            if (!_gameConfig.RotateEnabled || !_animancer)
            {
                return;
            }

            Vector3 direction = _localMoverTarget.localPosition - _oldPosition;
            direction.z += 0.6f;
            _animancer.GetAnimatorTransform().forward = Vector3.Lerp(_animancer.GetAnimatorTransform().forward, direction.normalized, _gameConfig.RotateSpeed * Time.deltaTime);
        }

        public void SetSwerve(bool value)
        {
            _gameConfig.CanSwerve = value;
        }

        public void SetRotateEnabled(bool value)
        {
            _gameConfig.RotateEnabled = value;
        }

        public void SetEnabled(bool value)
        {
            _gameConfig.MoveEnabled = value;
        }

        public void StopMoving()
        {
            _running = false;
            _gameConfig.CanSwerve = false;
            _gameConfig.MoveEnabled = false;
            _gameConfig.RotateEnabled = false;
            IdleAnimation();
        }
        public void StartMoving(bool willRotateEnabled)
        {
            _running = true;
            _gameConfig.CanSwerve = true;
            _gameConfig.MoveEnabled = true;
            _gameConfig.RotateEnabled = willRotateEnabled;
            RunAnimation();
        }

        private void PlayerSwerve_OnSwerve(Vector2 direction)
        {
#if UNITY_EDITOR
            if(Input.GetMouseButton(0) == false)
            {
                return;
            }
#endif
            if (_running && _gameConfig.CanSwerve)
            {
                _localMoverTarget.localPosition = _localMoverTarget.localPosition + Vector3.right * direction.x * _gameConfig.StrafeSpeed * Time.deltaTime;
                ClampLocalPosition();
            }
        }

        void ClampLocalPosition()
        {
            Vector3 pos = _localMoverTarget.localPosition;
            pos.x = Mathf.Clamp(pos.x, -_gameConfig.ClampLocalX, _gameConfig.ClampLocalX);
            _localMoverTarget.localPosition = pos;

        }

        void Update()
        {
            MoveForward();
            FollowLocalMoverTarget();
            UpdateRotation();
            UpdatePath();
            _oldPosition = _localMover.localPosition;
        }

        public void StartToRun()
        {
            SetEnabled(true);
            SetSwerve(true);
            if (_gameConfig.MoveEnabled)
            {
                _running = true;
                RunAnimation();
            }
        }

        public void PlayAnimation(string animName, float animSpeed)
        {
            if (!_animancer) return;
            
            _animancer.PlayAnimation(animName);
            _animancer.SetStateSpeed(animSpeed);
        }
        public float GetForwardSpeed()
        {
            return _gameConfig.ForwardSpeed;
        }
        public void SetForwardSpeed(float value)
        {
            if (_forwardSpeedTweeen != null)
            {
                _forwardSpeedTweeen.Kill();
            }
            _tempSpeed = value;
        }
        public void SetForwardSpeed(float value, float duration)
        {
            if (_forwardSpeedTweeen != null)
            {
                _forwardSpeedTweeen.Kill();
            }
            _forwardSpeedTweeen = DOTween.To(() => _tempSpeed, x => _tempSpeed = x, value, duration);
        }
        public void SetLocalRotation(Vector3 eulerAngles)
        {
            _animancer.transform.localEulerAngles = eulerAngles;
        }
        public void IdleAnimation()
        {
            PlayAnimation(_idleAnimName, _idleAnimSpeed);
        }
        public void RunAnimation()
        {
            PlayAnimation(_runAnimName, _runAnimSpeed);
        }
        public float GetHorizontalRatio()
        {
            return GameUtility.GetRatioFromValue(_localMover.localPosition.x, -_gameConfig.ClampLocalX, _gameConfig.ClampLocalX);
        }
        public Transform GetLocalMover()
        {
            return _localMover;
        }
        public Transform GetLocalMoverTarget()
        {
            return _localMoverTarget;
        }
        void MoveForward()
        {
            if (_gameConfig.MoveEnabled && _running && !_dodgingBack)
            {
                _distance += _tempSpeed * Time.deltaTime;
            }
        }
        void FollowLocalMoverTarget()
        {
            if (!_gameConfig.CanSwerve)
            {
                return;
            }
            Vector3 nextPos = new Vector3(_localMoverTarget.localPosition.x, _localMover.localPosition.y, _localMover.localPosition.z); ;
            _localMover.localPosition = Vector3.Lerp(_localMover.localPosition, nextPos, _gameConfig.StrafeLerpSpeed * Time.deltaTime);
        }
        void UpdatePath()
        {
            if (_gameConfig.MoveEnabled)
            {
                transform.position = _pathCreator.path.GetPointAtDistance(_distance);
                transform.eulerAngles = _pathCreator.path.GetRotationAtDistance(_distance).eulerAngles + new Vector3(0f, 0f, 90f);
            }
        }
        public Transform GetSimpleAnimancer() { return _animancer.transform; }
        public float GetInitialSpeed() => _initialSpeed;
        public PathCreator GetPath() => _pathCreator;
        public float GetPathDistance() => _distance;
        public float GetTempSpeed() => _tempSpeed;
    }
}