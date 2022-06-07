using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class FlockingManager : MonoBehaviour
{
    private List<IFlockingBehaviour> _behaviour = new List<IFlockingBehaviour>();
    [SerializeField] private float ratio;

    [SerializeField] private LayerMask _detectLayers;
    private Entity _entity;
    private void Awake()
    {
        _entity = GetComponent<Entity>();
        InitializedBehabiours();
    }
    private List<Transform> GetNeightBours()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, ratio, _detectLayers);
        var boids = new List<Transform>();
        for (int i = 0; i < colls.Length; i++)
        {
            if (colls[i].transform == this.transform) continue;
            boids.Add(colls[i].transform);
        }
        return boids;
    }
    void InitializedBehabiours()
    {
        var behaviours = GetComponents<IFlockingBehaviour>();
        _behaviour = new List<IFlockingBehaviour>(behaviours);
    }
    private void Update()
    {
        Vector3 dir = Vector3.zero;
        var neighbours = GetNeightBours();
        for (int i = 0; i < _behaviour.Count; i++)
        {
            var curr = _behaviour[i];
            dir += curr.GetDir(neighbours, transform);
        }
        _entity.Move(transform.forward);
        _entity.LookDir(dir);
    }
    

}
