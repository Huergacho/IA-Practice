using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class Entity : MonoBehaviour
{
    [SerializeField] protected float rotationSpeed;
    // [SerializeField] protected EnemyStats _enemyStats;
    [SerializeField] private float speed;
   // public EnemyStats Stats => _enemyStats;

    protected Rigidbody _rb;
    protected virtual void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    public virtual void Move(Vector3 dir)
    {
        _rb.velocity = new Vector3(dir.x * speed, _rb.velocity.y, dir.z * speed);

    }

    public virtual void SmoothRotation(Vector3 dest)
    {
        var direction = (dest - transform.position);
        if (direction != Vector3.zero)
        {
            var rotDestiny = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotDestiny, rotationSpeed * Time.deltaTime);
        }
    }
    public virtual void LookDir(Vector3 dir)
    {

        dir.y = 0;
        transform.forward = Vector3.Lerp(transform.forward, dir, 0.02f);
    }

}
