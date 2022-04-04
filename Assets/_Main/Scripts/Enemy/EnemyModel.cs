using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class EnemyModel : MonoBehaviour
{

    private void Chase()
    {
        print("Chase");
    }
    private void Patrol()
    {
        print("Patrol");
    }
    private void Idle()
    {
        print("Idle");
    }
    public void SuscribeEvents(EnemyController controller)
    {
        controller.onChase += Chase;
        controller.onIdle += Idle;
        controller.onPatrol += Patrol;
    }
}

