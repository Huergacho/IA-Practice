using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class ObstacleAvoidance : ISteering
{
    public enum Behaviours
    {
        Flee,
        Seek,
        Persuit,
        Evade
    }
    private Transform _self;
    private IVel _target;
    private ObstacleAvoidanceSO _properties;
    private Dictionary<Behaviours, ISteering> _behaviourDict = new Dictionary<Behaviours, ISteering>();
    private ISteering _actualBehaviour;
    public ISteering ActualBehaviour => _actualBehaviour;
    public ObstacleAvoidance(Transform self, IVel target, ObstacleAvoidanceSO properties)
    {
        _properties = properties;
        _self = self;
        _target = target;
        SetBehaviours(target.GetTarget, self, _target, _properties.PredictionTime);
    }

    void SetBehaviours(Transform target, Transform self, IVel targetVel, float predictionTime)
    {
        var flee = new Flee(target, self);
        _behaviourDict.Add(Behaviours.Flee, flee);
        var seek = new Seek(target, self);
        _behaviourDict.Add(Behaviours.Seek, seek);
        var persuit = new Persuit(target, self, targetVel, predictionTime);
        _behaviourDict.Add(Behaviours.Persuit, persuit);
        var evade = new Evade(target, self, targetVel, predictionTime);
        _behaviourDict.Add(Behaviours.Evade, evade);
        SetActualBehaviour(Behaviours.Seek);
    }
    public void SetActualBehaviour(Behaviours desiredBehaviour)
    {
        _actualBehaviour = _behaviourDict[desiredBehaviour];
    }
    public ISteering GetBehaviour(Behaviours behaviour)
    {
        if (!_behaviourDict.ContainsKey(behaviour))
        {
            Debug.Log("No existe ese Behaviour");
            return null;

        }
        else
        {
            return _behaviourDict[behaviour];
        }
    }
    public Vector3 GetDir()
    {
        
        Collider[] obj = Physics.OverlapSphere(_self.position, _properties.Radius, _properties.ObstacleLayers);
        Collider closestObj = null;
        float nearDistance = 0;
        for (int i = 0; i < obj.Length; i++)
        {
            Collider currObs = obj[i];
            Vector3 dir = currObs.transform.position - _self.position;
            float currentAngle = Vector3.Angle(_self.forward, dir);
            if (currentAngle < _properties.Angle / 2)
            {
                float currentDistance = Vector3.Distance(_self.position, currObs.transform.position);
                if (closestObj == null || currentDistance < nearDistance)
                {
                    closestObj = currObs;
                    nearDistance = currentDistance;
                }
            }
        }
        if (closestObj != null)
        {
            if (nearDistance == _properties.Radius)
            {
                nearDistance = _properties.Radius - 0.00001f;
            }
            var point = closestObj.ClosestPoint(_self.position);
            Vector3 dir = ((_self.position + _self.right * 0.0000000001f) - point);
            return dir.normalized;
        }

        return Vector3.zero;
    }
    public Vector3 GetFixedDir()
    {
        var direction = (GetDir() * _properties.AvoidanceMult + _actualBehaviour.GetDir() * _properties.BehaviourMult).normalized;
        return direction;
    }

    public void SetTarget(IVel target)
    {
        _target = target;
        _actualBehaviour.SetTarget(target.GetTarget);
    }
    public void SetTarget(Transform target)
    {
        _actualBehaviour.SetTarget(target);
    } 
}
