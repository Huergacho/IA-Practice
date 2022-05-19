using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
[RequireComponent(typeof(Animator))]
public class MonitorView : MonoBehaviour
{
    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    public void SuscribeEvents(MonitorController controller)
    {
        controller.onDetect += DetectAnimation;
    }
    private void DetectAnimation(bool state)
    {
        _animator.SetBool("Detect", state);
    }
}
