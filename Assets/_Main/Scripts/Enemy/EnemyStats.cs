using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
[CreateAssetMenu(fileName = "EnemyStats",menuName = "Stats/EnemyStats",order = 0)]
public class EnemyStats : ScriptableObject
{
    [SerializeField] private float patrolSpeed;
    public float PatrolSpeed => patrolSpeed;
    [SerializeField] private float seeksSpeed;
    public float SeekSpeed => seeksSpeed;

    [SerializeField] private float chaseSpeed;
    public float ChaseSpeed => chaseSpeed;

    [SerializeField] private float maxLife;
    public float MaxLife => maxLife;

    [SerializeField] private float shootCooldown;
    public float ShootCoooldown => shootCooldown;
}
