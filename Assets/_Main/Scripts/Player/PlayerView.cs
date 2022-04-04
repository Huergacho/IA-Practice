using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof(Animator))]
public class PlayerView : MonoBehaviour
{
    private Animator _animator;
    private float speedValue;
    [SerializeField] private float transitionTime;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void Start()
    {
    }
    public void SuscribeEvents(PlayerController controller)
    {
        controller._onIdle += Idle;
        controller._onWalk += Walk;
        controller._onRun += Run;
    }
    void Walk(Vector2 aux)
    {
        // Si quisiera tirarle un movimiento omnidireccional de caminata puedo sacar la direccion hacia la que esta caminando por aca :D

            _animator.SetFloat("Speed",Mathf.SmoothStep(_animator.GetFloat("Speed"),0.5f,Time.deltaTime * transitionTime));

    }
    void Run(Vector2 aux)
    {

        //_animator.SetFloat("Speed", 1);

            _animator.SetFloat("Speed", Mathf.SmoothStep(_animator.GetFloat("Speed"), 1f, Time.deltaTime * transitionTime));
    }
    void Idle()
    {
        //_animator.SetFloat("Speed", 0);
        if(_animator.GetFloat("Speed") < 0.1f)
        {
            _animator.SetFloat("Speed", 0);

        }
        _animator.SetFloat("Speed", Mathf.SmoothStep(_animator.GetFloat("Speed"), 0f, Time.deltaTime * transitionTime));

    }
}
