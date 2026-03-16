using UnityEngine;

[CreateAssetMenu(menuName = nameof(GameSettings), 
    fileName = nameof(GameSettings))]
public class GameSettings : ScriptableObject
{
    [field: SerializeField] public bool StartWithMagicMode { get; private set; } = false;
    
    [field: Header("Player")]
    [field: SerializeField] public float PlayerSpeed { get; private set; } = 2f;
    [field: SerializeField] public float MouseSensitivityX { get; private set; } = 5f;
    [field: SerializeField] public float MouseSensitivityY { get; private set; } = 5f;
    [field: SerializeField] public float MouseYMin { get; private set; } = -80f;
    [field: SerializeField] public float MouseYMax { get; private set; } = 80f;
    [field: SerializeField, Range(0f, 1f)] public float CameraLerp { get; private set; } = 0.7f;
}