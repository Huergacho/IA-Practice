using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
[RequireComponent(typeof(Animator))]
public class CameraView : MonoBehaviour
{
    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    public void SuscribeEvents(CameraController controller)
    {
        controller.onIdle += IdleAnimation;
        controller.onSeek += DetectAnimation;
    }
    private void IdleAnimation()
    {
        _animator.SetBool("Detect", false);

    }
    private void DetectAnimation(Transform aux)
    {
        _animator.SetBool("Detect", true);
    }
}
