using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class LazerBullet : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private LayerMask colLayers;
    [SerializeField] private float lifeTime;
    private float currTime;
    private Rigidbody _rb;
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        currTime = lifeTime;
    }
    private void Update()
    {
        Move();
        currTime -= Time.deltaTime;
        if(currTime <= 0)
        {
            Destroy(gameObject);
        }
    }
    void Move()
    {
        //_rb.AddForce(transform.forward, ForceMode.Acceleration);
        speed = speed + Time.time;
        transform.position += (transform.forward * speed * Time.deltaTime);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if((collision.gameObject.layer & (1 << colLayers)) != 0)
        {
                Destroy(gameObject);
        }
    }
}
