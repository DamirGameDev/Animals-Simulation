using Modules.SharedModule.Runtime.Scripts.Observers;
using UnityEngine;

namespace Modules.AnimalModule.Runtime.Scripts.Animal
{
    public class AnimalSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
        [field: SerializeField] public Collider Collider { get; private set; }
        [field: SerializeField] public CollisionEnterObserver CollisionObserver { get; private set; }
    }
}
