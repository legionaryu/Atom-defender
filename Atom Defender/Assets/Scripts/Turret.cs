using UnityEngine;
using System.Collections.Generic;

public class Turret : MonoBehaviour {

    public int cost = 200;
    public float damage = 400;
    public float cooldown = 0.5f;

    protected float timer = 0;
    protected List<Creeper> creepersOnArea = new List<Creeper>();

    void OnTriggerEnter(Collider other)
    {
        var creeper = other.gameObject.GetComponent<Creeper>();
        if (creeper)
        {
            creepersOnArea.Add(creeper);
        }
    }
    void OnTriggerExit(Collider other)
    {
        var creeper = other.gameObject.GetComponent<Creeper>();
        if (creeper)
        {
            creepersOnArea.Remove(creeper);
        }
    }
}
