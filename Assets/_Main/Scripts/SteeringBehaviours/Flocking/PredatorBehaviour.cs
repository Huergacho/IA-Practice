using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorBehaviour : MonoBehaviour, IFlockingBehaviour
{
    [SerializeField] private float _multiplier = 1;
    public float range;
    [SerializeField] private int _capacity;
    [SerializeField] private LayerMask _detectLayers;
    public float Multiplier { get => _multiplier; set => _multiplier = value; }

    Collider[] _colls;
    private void Awake()
    {
        _colls =  new Collider[_capacity];
    }
    public Vector3 GetDir(List<Transform> boids, Transform self)
    {
        Vector3 dir = Vector3.zero;
        int countColls = Physics.OverlapSphereNonAlloc(self.position, range, _colls, _detectLayers);
        for (int i = 0; i < countColls; i++)
        {
            Vector3 dirSeparation = self.position - _colls[i].transform.position;
            dir += dirSeparation;
        }
            
        if (countColls != 0)
        {
            dir = dir / countColls;
        }
        
        return dir * Multiplier;

    }
}
