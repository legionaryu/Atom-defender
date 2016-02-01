using UnityEngine;
using System.Collections.Generic;

public class Turret1 : Turret {
    public GameObject bulletPrefab;
    public float bulletSpeed = 7;

	void FixedUpdate () {
        timer += Time.fixedDeltaTime;
        if(timer > cooldown)
        {
            while(creepersOnArea.Count > 0)
            {
                var c = creepersOnArea[0];
                if(c)
                {
                    Shoot(c.transform);
                    timer = 0;
                    break;
                }
                else
                {
                    creepersOnArea.RemoveAt(0);
                }
            }
        }
	}

    void Shoot(Transform target)
    {
        var bulletObj = GameObject.Instantiate(bulletPrefab) as GameObject;
        var p = transform.position;
        p.y = bulletObj.transform.position.y;
        bulletObj.transform.position = p;
        var bullet = bulletObj.GetComponent<Bullet>();
        bullet.Damage = damage;
        bullet.Speed = bulletSpeed;
        bullet.Target = target;
    }
}
