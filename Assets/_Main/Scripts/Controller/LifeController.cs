﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class LifeController : MonoBehaviour
{
    private float _currentLife;
    [SerializeField] private float _maxLife;
    public Action actionToDo;
    [SerializeField] private bool isInmortal;
    private float lifeCache;
    public event Action<float> updateLifeBar;
    private void Start()
    {
        _currentLife = _maxLife;
        lifeCache = _maxLife;
    }
    public void TakeDamage(float damage)
    {
        _currentLife -= damage;
        updateLifeBar?.Invoke(_currentLife);
    }
    public void CheckCurrentLife()
    {
        if(_currentLife <= 0)
        {
            if (!isInmortal)
            {
                actionToDo?.Invoke();
            }
        }
    }
    public bool HasTakenDamage()
    {
        if(lifeCache == _currentLife)
        {
            return false;
        }
        lifeCache = _currentLife;
        return true;
    }

}
