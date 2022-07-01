using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(LifeController))]
public class Lifebar : MonoBehaviour
{
    [SerializeField] private Image lifeBarImage; 
    private LifeController _lifeController;
    private void Awake()
    {
        _lifeController = GetComponent<LifeController>();

    }
    private void Start()
    {
        _lifeController.updateLifeBar += UpdateLifeBarStatus;
    }

    void UpdateLifeBarStatus(float currPorcentaje)
    {
        lifeBarImage.fillAmount = currPorcentaje / 100f;
    }

}

