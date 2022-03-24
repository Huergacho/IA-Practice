using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public static class Timer
{
    public static bool isCounting;
    public static bool StartTimer(float time, float minTime)
    {
        time -= Time.deltaTime;
        if(time > 0) return false;

        return true;
    }

}
