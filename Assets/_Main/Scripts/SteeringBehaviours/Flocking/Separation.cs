using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
class Separation : MonoBehaviour,IFlockingBehaviour
{
    [SerializeField] private float _multiplier = 1;
    public float range;
    public float Multiplier { get => _multiplier; set => _multiplier = value; }

    public Vector3 GetDir(List<Transform> boids,Transform self)
    {
        Vector3 dir = Vector3.zero;
        int countedBoids = 0;
        if (boids.Count > 0)
        {
            for (int i = 0; i < boids.Count; i++)
            {

                Vector3 dirSeparation = self.position - boids[i].position;
                if(dirSeparation.magnitude > range)
                {
                    continue;
                }
                dir += dirSeparation;
                countedBoids++;
            }
            if(countedBoids != 0)
            {
                dir = dir / boids.Count;
            }
        }
        return dir * Multiplier;

    }
}
