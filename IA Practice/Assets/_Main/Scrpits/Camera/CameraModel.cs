using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraModel : MonoBehaviour
{
    [SerializeField] private float _detectionDistance;
    [SerializeField] private float _aperture;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private LayerMask _targetLayer;
    private void Update()
    {
        
    }
    public bool LineOfSight(Transform target)
    {
        Vector3 diff = target.position - transform.position;
        float distance = diff.magnitude;

        if (distance > _detectionDistance) return false;

        var angleToTarget = Vector3.Angle(diff, transform.forward);
        if (angleToTarget > _aperture / 2) return false;

        if (Physics.Raycast(transform.forward, diff.normalized, distance, _obstacleLayer)) { print("AaAaA"); return false; }
        print(target.gameObject.name);
        return true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _detectionDistance);

        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, _aperture / 2, 0) * transform.forward * _detectionDistance);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -_aperture / 2, 0) *  transform.forward * _detectionDistance);
    }
    public Transform[] CheckTargets()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, _detectionDistance,_targetLayer);
        Transform[] targets = new Transform[colls.Length];
        for (int i = 0; i < colls.Length; i++)
        {
            if (LineOfSight(colls[i].transform))
            {
                targets[i] = colls[i].transform;

            }
        }
        return targets;
    }
}
