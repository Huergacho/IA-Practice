using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
[CreateAssetMenu(fileName = "BulletStat", menuName = "Bullets", order = 0)]
public class BulletStats : ScriptableObject
{
    [SerializeField] private float _damage;
    public float Damage => _damage;

    [SerializeField] private LayerMask _damageLayer;
    public LayerMask DamageLayer => _damageLayer;
}
