using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    [SerializeField] private float speed;
    private Camera _camera;
    private Rigidbody _rb;
    private void Awake()
    {
        _camera = Camera.main;
        _rb = GetComponent<Rigidbody>();
    }
    public void Move(Vector2 dir)
    {

        _rb.velocity = new Vector3(speed * dir.normalized.x, _rb.velocity.y, speed * dir.normalized.y);
        transform.LookAt(CorrectRotation());
    }
    public void Idle()
    {
        _rb.velocity = Vector3.zero;
        transform.LookAt(CorrectRotation());

    }
    private Vector3 CorrectRotation()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            var target = hitInfo.point;
            target.y = transform.position.y;
            var distance = Vector3.Distance(transform.position, hitInfo.point);
            if(distance >= 1f)
            {
                return target;
            }
            else
            {
                return Vector3.zero;
            }
        }
        else
        {
            return Vector3.zero;
        }
    }
    public void SuscribeEvents(PlayerController controller)
    {
        controller._onWalk += Move;
        controller._onIdle += Idle;
    }
}
