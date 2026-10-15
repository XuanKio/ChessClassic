using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Game/Characters/Character Data")]
public sealed class CharacterData : ScriptableObject
{
    [Header("Movement")]
    [SerializeField, Min(0f)] private float moveSpeed = 5f;
    [SerializeField, Min(0f)] private float acceleration = 25f;
    [SerializeField, Min(0f)] private float deceleration = 35f;

    public float MoveSpeed => moveSpeed;
    public float Acceleration => acceleration;
    public float Deceleration => deceleration;
}