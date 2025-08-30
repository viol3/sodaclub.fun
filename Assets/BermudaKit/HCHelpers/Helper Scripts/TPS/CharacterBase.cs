using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ali.Helper.TPS
{
    public abstract class CharacterBase : MonoBehaviour
    {
        public abstract void ProcessVelocity(Vector3 velocity);
    }
}