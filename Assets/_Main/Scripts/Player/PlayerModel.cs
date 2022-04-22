using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class PlayerModel : MonoBehaviour,IVel
{
    [SerializeField] private PlayerStats _stats;
    public PlayerStats Stats => _stats;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float currentTime;
    [SerializeField] private LazerBullet bulletPrefab;
    [SerializeField] private Transform firePoint;
    private Camera _camera;
    private Rigidbody _rb;
    public float GetVel => _rb.velocity.magnitude;

    public Vector3 GetFoward => _rb.velocity.normalized;

    public Transform GetTarget => transform;
    #region UnityMethods
    private void Awake()
    {
        _camera = Camera.main;
        _rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        
    }
    #endregion

    #region Movement
    public void Move(Vector2 dir)
    {
        _rb.velocity = new Vector3(currentSpeed * dir.normalized.x, _rb.velocity.y, currentSpeed * dir.normalized.y);
        SmoothRotation(GetMousePosition());
    }

    public void Idle()
    {
        _rb.velocity = Vector3.zero;
        SmoothRotation(GetMousePosition());
    }

    private void Walk(Vector2 dir)
    {
        ModifySpeed(_stats.WalkSpeed);
        Move(dir);
    }

    private void Run(Vector2 dir)
    {
        ModifySpeed(_stats.RunSpeed);
        Move(dir);
    }
    
    private void ModifySpeed(float desiredSpeed)
    {
        currentSpeed = Mathf.SmoothStep(currentSpeed, desiredSpeed, Time.deltaTime * 10);
    }
    #endregion

    #region MouseCalculation
    private void SmoothRotation(Vector3 dest)
    {
        var direction = (dest - transform.position);
        if (direction != Vector3.zero)
        {
            var rotDestiny = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotDestiny, _stats.RotSpeed * Time.deltaTime);
        }
    }
    private Vector3 GetMousePosition()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            var target = hitInfo.point;
            target.y = transform.position.y;
            var distance = Vector3.Distance(transform.position, hitInfo.point);
            if (distance >= 1f)
            {
                return target;
            }
            else
            {
                return target;
            }
        }
        else
        {
            return Vector3.zero;
        }
    }
    public void Shoot()
    {
        var bulletClone = bulletPrefab;
        Instantiate(bulletClone, firePoint.position, firePoint.rotation);
    }
    #endregion
    public void SuscribeEvents(PlayerController controller)
    {
        controller._onWalk += Walk;
        controller._onIdle += Idle;
        controller._onRun += Run;
        controller._onDie += DestroyActions;
        //controller._onShoot += Shoot;
    }
    public void DestroyActions()
    {
        GameManager.Instance.PlayerIsDead();
    }
}
