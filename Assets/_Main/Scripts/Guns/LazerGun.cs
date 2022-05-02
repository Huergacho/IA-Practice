using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class LazerGun : MonoBehaviour
{
    [SerializeField] private BulletStats _bulletStats;
    [SerializeField] private LayerMask contactLayer;
    [SerializeField] private float shootCooldown;
    [SerializeField] private TrailRenderer bulletTrail;
    [SerializeField] private ParticleSystem bulletExplodeParticles;
    private float currentTime;

    private void Awake()
    {
        
    }
    private void Start()
    {
        currentTime = shootCooldown;
    }
    private void Update()
    {
        if(currentTime < shootCooldown)
        {
            currentTime += Time.deltaTime;
        }
    }
    public void Shoot(Vector3 startPos, Vector3 dir)
    {

        if (currentTime < shootCooldown)
        {
            return;
        }
        if (Physics.Raycast(startPos,dir, out RaycastHit hit, float.MaxValue,contactLayer))
        {
            if ((_bulletStats.DamageLayer & 1 << hit.collider.gameObject.layer) == 1 << hit.collider.gameObject.layer)
            {
                var enemyLife = hit.collider.gameObject.GetComponent<LifeController>();
                enemyLife.TakeDamage(_bulletStats.Damage);
                enemyLife.CheckCurrentLife();
            }
                TrailRenderer trail = Instantiate(bulletTrail, startPos, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hit));

        }
        currentTime = 0;
    }
    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit ray)
    {
        float time = 0;
        Vector3 startPos = trail.transform.position;
        while(time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPos, ray.point, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        trail.transform.position = ray.point;
        Destroy(trail.gameObject, trail.time);
    }
}
