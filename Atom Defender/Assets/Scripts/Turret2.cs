using UnityEngine;
using System.Collections.Generic;

public class Turret2 : Turret {

    void Start () {
	    
	}
	
	void FixedUpdate () {
        timer += Time.fixedDeltaTime;
        if(timer > cooldown)
        {
            foreach (var c in creepersOnArea)
            {
                c.life -= damage;
            }
            timer -= cooldown;
            //timer = 0;
        }
	}
}
