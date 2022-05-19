using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class BaseEnemyModel : MonoBehaviour
{
    [SerializeField] protected float rotationSpeed;
    [SerializeField] protected EnemyStats _enemyStats;
    [SerializeField] protected Transform _firePoint;
    [SerializeField] protected LazerGun _gun;
    public EnemyStats Stats => _enemyStats;

    protected Rigidbody _rb;
    protected virtual void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    protected virtual void Move(Vector3 dir, float desiredSpeed)
    {
        _rb.velocity = new Vector3(dir.x * desiredSpeed, _rb.velocity.y, dir.z * desiredSpeed);

    }

    protected virtual void SmoothRotation(Vector3 dest)
    {
        var direction = (dest - transform.position);
        if (direction != Vector3.zero)
        {
            var rotDestiny = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotDestiny, rotationSpeed * Time.deltaTime);
        }
    }
    protected virtual void Shoot()
    {
        _gun.Shoot(_firePoint.position, _firePoint.forward);
    }
    protected virtual void LookDir(Vector3 dir)
    {

        dir.y = 0;
        transform.forward = Vector3.Lerp(transform.forward, dir, 0.02f);
    }
}
