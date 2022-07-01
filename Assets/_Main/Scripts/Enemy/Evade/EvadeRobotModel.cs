using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class EvadeRobotModel : BaseEnemyModel
{
    public void SuscribeEvents(EvadeRobotController controller)
    {
        controller.onMove += Move;
        controller.onRotate += LookDir;
    }
}
