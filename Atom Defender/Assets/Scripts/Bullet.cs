using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
    public Transform Target { get; set; }
    public float Speed { get; set; }
    public float Damage { get; set; }

    void FixedUpdate () {
        if (Target)
        {
            var direction = (Target.position - transform.position).normalized;
            transform.Translate(direction * Speed * Time.fixedDeltaTime);
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
	}

    void OnTriggerEnter(Collider other)
    {
        var creeper = other.gameObject.GetComponent<Creeper>(); 
        if(creeper)
        {
            creeper.life -= Damage;
        }
        GameObject.Destroy(gameObject);
    }
}
