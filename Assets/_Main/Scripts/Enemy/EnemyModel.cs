using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class EnemyModel : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private EnemyStats _enemyStats;
    [SerializeField] private LazerBullet _lazerBullets;
    [SerializeField] private Transform _firePoint;
    public EnemyStats Stats => _enemyStats;
    private float currCooldown;


    private Rigidbody _rb;
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        currCooldown = 0;
    }
    private void Move(Vector3 dir, float desiredSpeed)
    {
        _rb.velocity = new Vector3(dir.x * desiredSpeed, _rb.velocity.y, dir.z * desiredSpeed);

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
    public void Shoot()
    {
        currCooldown -= Time.deltaTime;
        if(currCooldown < 0)
        {
            var obj = _lazerBullets;
            Instantiate(obj, _firePoint.position, _firePoint.rotation);
            currCooldown = _enemyStats.ShootCoooldown;
        }
    }
    public void LookDir(Vector3 dir)
    {
        dir.y = 0;
        transform.forward = Vector3.Lerp(transform.forward, dir, 0.02f);
    }

    public void SuscribeEvents(EnemyController controller)
    {
        controller.onRotate += LookDir;
        controller.onMove += Move;
    }
    private void DestroyActions()
    {
        Destroy(gameObject);
    }
}

