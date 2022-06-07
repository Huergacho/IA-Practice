using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Alignment : MonoBehaviour, IFlockingBehaviour
{
   [SerializeField] private float _multiplier = 1;

    public float Multiplier { get => _multiplier; set => _multiplier = value; }

    public Vector3 GetDir(List<Transform> boids, Transform self)
    {
        Vector3 dir = Vector3.zero;
        if(boids.Count > 0)
        {
            for (int i = 0; i < boids.Count; i++)
            {
                dir += boids[i].forward;
            }
            dir = dir / boids.Count;
        }
        return dir * Multiplier;

    }
}
