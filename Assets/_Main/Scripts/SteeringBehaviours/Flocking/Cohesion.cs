using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class Cohesion : MonoBehaviour, IFlockingBehaviour
{
    [SerializeField] private float _multiplier = 1;
    public float Multiplier { get => _multiplier; set => _multiplier = value; }

    public Vector3 GetDir(List<Transform> boids, Transform self)
    {
        Vector3 dir = Vector3.zero;
        if (boids.Count > 0)
        {
        Vector3 center = Vector3.zero;
            for (int i = 0; i < boids.Count; i++)
            {
                center += boids[i].position;
            }
            center = center / boids.Count;
            dir = (center - self.position).normalized;
        }
        return dir * Multiplier;

    }
}

