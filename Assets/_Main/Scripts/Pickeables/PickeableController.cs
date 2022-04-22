using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class PickeableController : MonoBehaviour
{
    [System.Serializable]
    public class PickeableObject
    {
        public Pickeable item;
        public float chance;
    }
    [System.Serializable]
    public class Rooms
    {
        public Transform spawnPoint;
        public float chance;
    }
    private Dictionary<Pickeable, float> items = new Dictionary<Pickeable, float>();
    [SerializeField] private List<PickeableObject> pickeables;
    [SerializeField] private List<Rooms> spawnPoints;
    private Dictionary<Transform, float> posibleSpawns = new Dictionary<Transform, float>();
    private void Start()
    {
        foreach (PickeableObject pickeable in pickeables)
        {
            items.Add(pickeable.item, pickeable.chance);
        }
        foreach (Rooms room in spawnPoints)
        {
            posibleSpawns.Add(room.spawnPoint, room.chance);
        }
        InstancePickables();
    }
    private void InstancePickables()
    {
        foreach (var item in items)
        {
            var spawnItem = MyEngine.MyRandom.GetRandomWeight(items);
            var spawnPoints = MyEngine.MyRandom.GetRandomWeight(posibleSpawns);
            Instantiate(spawnItem, spawnPoints.position, spawnPoints.rotation);
            posibleSpawns.Remove(spawnPoints);

        }
    }
}
