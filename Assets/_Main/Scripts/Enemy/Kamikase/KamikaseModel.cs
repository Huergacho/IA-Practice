using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class KamikaseModel : SecurityEnemyModel
{
    [SerializeField] private ParticleSystem particleSystem;
    private KamikaseController _controller;
    protected override void Start()
    {
        base.Start();
        particleSystem = GetComponent<ParticleSystem>();
    }
    private void Explode()
    {
        _controller.onExplode -= Explode;
        print("Explote");
        particleSystem.Play();
    }

    public void SuscribeEvents(KamikaseController controller)
    {
        _controller = controller;
        controller.onExplode += Explode;
        controller.onMove += Move;
        controller.onRotate += LookDir;
    }
}
