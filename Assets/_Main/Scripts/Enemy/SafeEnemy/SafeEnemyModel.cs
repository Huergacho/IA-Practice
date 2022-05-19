using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeEnemyModel : SecurityEnemyModel
{
    public void SuscribeEvents(SafeEnemyController controller)
    {
        controller._onRotate += LookDir;
        controller._onMove += Move;
        controller.onShoot += Shoot;
    }
}
