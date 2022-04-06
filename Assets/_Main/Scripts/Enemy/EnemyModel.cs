using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class EnemyModel : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    private Rigidbody _rb;
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    public void Chase(Vector3 dir)
    {
        _rb.velocity = new Vector3(dir.x * speed, _rb.velocity.y, dir.z * speed);
    }
    private void Patrol()
    {

    }
    private void Idle()
    {
        _rb.velocity = Vector3.zero;

    }
    public void SmoothRotation(Vector3 dest)
    {
        var direction = (dest - transform.position);
        if (direction != Vector3.zero)
        {
            var rotDestiny = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotDestiny, rotationSpeed * Time.deltaTime);
        }
    }
    public void LookDir(Vector3 dir)
    {
        dir.y = 0;
        transform.forward = Vector3.Lerp(transform.forward, dir, 0.02f);
    }
    public void SuscribeEvents(EnemyController controller)
    {
        controller.onSeek += Chase;
        controller.onIdle += Idle;
        controller.onPatrol += Patrol;
        controller.onRotate += SmoothRotation;
    }
}

