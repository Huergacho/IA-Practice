using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothnessMovement;

    private void Update()
    {
        Move();
    }
    void Move()
    {
        Vector3 desiredPos = target.position + offset;
        Vector3 finalPos = Vector3.Lerp(transform.position, desiredPos, smoothnessMovement * Time.deltaTime);
        transform.position = finalPos;
    }
}
