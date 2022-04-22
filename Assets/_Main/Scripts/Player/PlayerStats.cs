using System;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayerStats", menuName = "Stats", order = 0)]
public class PlayerStats : ScriptableObject
{
    [SerializeField] private float walkSpeed;
    public float WalkSpeed => walkSpeed;

    [SerializeField] private float runSpeed;
    public float RunSpeed => runSpeed;

    [SerializeField] private float rotSpeed;
    public float RotSpeed => rotSpeed;

    [SerializeField] private float shootCooldown;
    public float ShootCooldown => shootCooldown;

    [SerializeField] private float maxLife;
    public float MaxLife => maxLife;
}

