using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField]private int itemsPicked;

    private void Start()
    {
        Instance = this;
    }
    public void AddItem()
    {
        itemsPicked++;
    }
}
