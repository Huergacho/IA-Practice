using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class BaseEnemyController : MonoBehaviour
{
    [SerializeField] protected ObstacleAvoidanceSO obstacleAvoidanceSO;
    [SerializeField] protected PlayerModel _actualTarget = null;
    protected Dictionary<Steerings, ISteering> behaviours = new Dictionary<Steerings, ISteering>();
    protected ObstacleAvoidance _obstacleAvoidance;
    protected LineOfSight _lineOfSight;
    protected INode _root;
    protected LifeController lifeController;
    protected virtual void Awake()
    {
        _lineOfSight = GetComponent<LineOfSight>();
        lifeController = GetComponent<LifeController>();
        lifeController.actionToDo += DieActions;
        InitBehaviours();
        _obstacleAvoidance = new ObstacleAvoidance(transform, _actualTarget, obstacleAvoidanceSO, behaviours);
    }
    protected virtual void Start()
    {
        InitDesitionTree();
        InitFSM();
    }
    protected virtual void InitBehaviours()
    {
        throw new NotImplementedException();
    }

    protected virtual void InitDesitionTree()
    {
        throw new NotImplementedException();
    }

    protected virtual void InitFSM()
    {
        throw new NotImplementedException();
    }
    protected bool CheckForPlayer()
    {
        return GameManager.Instance.IsPlayerAlive;
    }
    protected virtual void DieActions()
    {
    }

}
