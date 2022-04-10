using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonitorModel : MonoBehaviour
{
    [SerializeField] private GameObject aux;
    [SerializeField] private float rotationSpeed;
    private void Seek(Transform target)
    {

        if(target != null)
        {
            SmoothRotation(target.position);
        }
    }
    private void Idle()
    {

    }
    public void SuscribeEvents(MonitorController controller)
    {
        controller.onIdle += Idle;
        controller.onSeek += Seek;
    }
    private void SmoothRotation(Vector3 dest)
    {
        var direction = (dest - transform.position);
        if (direction != Vector3.zero)
        {
            var rotDestiny = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotDestiny, rotationSpeed * Time.deltaTime);
        }
    }


}
