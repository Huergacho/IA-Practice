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
    public EnemyStats Stats => _enemyStats;

    private Rigidbody _rb;
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
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
}

