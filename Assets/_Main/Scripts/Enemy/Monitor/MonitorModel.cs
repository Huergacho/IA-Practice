using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonitorModel : BaseEnemyModel
{
    public void SuscribeEvents(MonitorController controller)
    {
        controller.onRotate += LookDir;
        controller.onShoot += Shoot;
    }
}
