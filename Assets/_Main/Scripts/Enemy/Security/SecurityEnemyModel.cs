using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class SecurityEnemyModel : BaseEnemyModel
{
    public void SuscribeEvents(SecurityEnemyController myController)
    {
        myController.onRotate += LookDir;
        myController.onMove += Move;
        myController.onShoot += Shoot;
    }
}

