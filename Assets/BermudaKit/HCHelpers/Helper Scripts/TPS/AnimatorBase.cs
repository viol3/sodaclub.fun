using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ali.Helper.TPS
{
    public abstract class AnimatorBase : MonoBehaviour
    {
        public abstract void PlayAnimation(string animName);
        public abstract void SetSpeed(float speed);
    }
}