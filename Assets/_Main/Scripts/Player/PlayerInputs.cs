using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class PlayerInputs : MonoBehaviour
{
    private float _getV;
    public float GetV => _getV;
    private float _getH;
    public float GetH => _getH;
    public bool IsMoving()
    {
        if (_getH != 0 || _getV != 0) { return true; }
        else return false;
    }
    public bool IsRunning()
    {
        return Input.GetKey(KeyCode.LeftShift);
    }
    public void UpdateInputs()
    {
        _getH = Input.GetAxis("Horizontal");
        _getV = Input.GetAxis("Vertical");
    }
    public bool isShooting()
    {
        if (!Input.GetMouseButton(1))
        {
            return false;
        }
        return true;
    }
}
