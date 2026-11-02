using System;
using UnityEngine;

namespace Modules.SharedModule.Runtime.Scripts.Observers
{
    public class DestroyObserver : MonoBehaviour
    {
        public event Action Destroyed;

        private void OnDestroy()
        {
            Destroyed?.Invoke();
        }
    }
}
