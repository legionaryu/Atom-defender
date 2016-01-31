using UnityEngine;
using System.Collections.Generic;

public class Turret1 : Turret {
    
    void Start () {
	    
	}
	
	void FixedUpdate () {
        timer += Time.fixedDeltaTime;
        if(timer > cooldown)
        {
            if(creepersOnArea.Count > 0)
            {
                var c = creepersOnArea[0];
                c.life -= damage;
            }
            timer -= cooldown;
            //timer = 0;
        }
	}
}
