using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class Pickeable : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    private ParticleSystem particles;
    private void Start()
    {
        particles = GetComponentInChildren<ParticleSystem>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if((playerLayer & (1 << other.gameObject.layer)) > 0)
        {
            GameManager.Instance.AddItem();
            particles.Play();
        }
    }
}
