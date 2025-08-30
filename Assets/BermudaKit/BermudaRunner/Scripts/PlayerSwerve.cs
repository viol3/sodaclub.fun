using Lean.Touch;
using UnityEngine;

namespace Bermuda.Runner
{
    public class PlayerSwerve : MonoBehaviour
    {
        [SerializeField] private float _speedMultiplier = 1f;

        public event System.Action OnSwerveStart;
        public event System.Action<Vector2> OnSwerve;
        public event System.Action OnSwerveEnd;
        void Start()
        {
            LeanTouch.OnFingerDown += LeanTouch_OnFingerDown;
            LeanTouch.OnFingerUpdate += LeanTouch_OnFingerUpdate;
            LeanTouch.OnFingerUp += LeanTouch_OnFingerUp;
        }

        private void LeanTouch_OnFingerDown(LeanFinger obj)
        {
            OnSwerveStart?.Invoke();
        }

        private void LeanTouch_OnFingerUp(LeanFinger obj)
        {
            OnSwerveEnd?.Invoke();
        }

        private void LeanTouch_OnFingerUpdate(LeanFinger finger)
        {
            if (finger.Set)
            {
                OnSwerve?.Invoke(finger.ScaledDelta * _speedMultiplier);
            }
        }

        private void OnDestroy()
        {
            LeanTouch.OnFingerDown -= LeanTouch_OnFingerDown;
            LeanTouch.OnFingerUpdate -= LeanTouch_OnFingerUpdate;
            LeanTouch.OnFingerUp -= LeanTouch_OnFingerUp;
        }

    }
}