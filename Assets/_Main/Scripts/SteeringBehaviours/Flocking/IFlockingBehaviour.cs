using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public interface IFlockingBehaviour
{
    public float Multiplier { get; set; }
    public Vector3 GetDir(List<Transform> boids, Transform self);
}
