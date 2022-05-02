using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class LazerBullet : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject objectMesh;
    private ParticleSystem impactEffect;
    [SerializeField] private LayerMask colLayers;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float lifeTime;
    [SerializeField] private int dmg;
    private float currTime;
    private Rigidbody _rb;
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        impactEffect = GetComponent<ParticleSystem>();
        currTime = lifeTime;
    }
}
//    private void OnCollisionEnter(Collision collision)
//    {
//        if((enemyMask & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
//        {
//            var life = collision.gameObject.GetComponent<LifeController>();
//            life.TakeDamage(dmg);
//            life.CheckCurrentLife();
//            DestroyActions();
            
//        }
//        else if((colLayers & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
//        {
//            DestroyActions();
//        }
//    }
//    private void DestroyActions()
//    {
//        _rb.velocity = Vector3.zero;
//        objectMesh.SetActive(false);
//        impactEffect.Play();
//    }
//}
