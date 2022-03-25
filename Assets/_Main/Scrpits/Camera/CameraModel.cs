using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraModel : MonoBehaviour
{
    [SerializeField] private GameObject aux;

    private void Seek(Transform target)
    {
        aux.SetActive(true);
        if(target != null)
        {
            transform.LookAt(target);
        }
    }
    private void Idle()
    {
        aux.SetActive(false);

    }
    public void SuscribeEvents(CameraController controller)
    {
        controller.onIdle += Idle;
        controller.onSeek += Seek;
    }


}
