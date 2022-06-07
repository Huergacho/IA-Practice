using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiderBehaviour : MonoBehaviour, IFlockingBehaviour
{
    [SerializeField] private float _multiplier = 1;
    public Transform target;
    public float Multiplier { get => _multiplier; set => _multiplier = value; }
    public Vector3 GetDir(List<Transform> boids, Transform self)
    {
        return (target.position - self.position).normalized * Multiplier;
    }
}
