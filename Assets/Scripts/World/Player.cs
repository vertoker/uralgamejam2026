using UnityEngine;

namespace World
{
    public class Player : MonoBehaviour
    {
        [field: SerializeField] public Transform PlayerTransform { get; private set; }
        [field: SerializeField] public CharacterController CharacterController { get; private set; }
        [field: SerializeField] public Transform CameraTransform { get; private set; }
        [field: SerializeField] public Camera Camera { get; private set; }
    }
}