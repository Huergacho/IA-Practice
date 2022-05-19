using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SecurityEnemyView : MonoBehaviour
{
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    void DetectAnimation(bool detectBool)
    {
        _animator.SetBool("Detect",detectBool);
    }
    public void SuscribeEvents(SecurityEnemyController controller)
    {
        controller.onDetect += DetectAnimation;
    }
}
