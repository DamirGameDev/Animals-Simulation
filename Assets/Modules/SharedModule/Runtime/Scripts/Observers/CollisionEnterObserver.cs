using System;
using UnityEngine;

namespace Modules.SharedModule.Runtime.Scripts.Observers
{
    public class CollisionEnterObserver : MonoBehaviour
    {
        public event Action<Collision> Entered;

        private void OnCollisionEnter(Collision collision)
        {
            Entered?.Invoke(collision);
        }
    }
}
