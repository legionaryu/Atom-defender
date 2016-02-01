using UnityEngine;
using System.Collections.Generic;

public class Turret2 : Turret {

    void Start () {
	    
	}
	
	void FixedUpdate () {
        timer += Time.fixedDeltaTime;
        if(timer > cooldown)
        {
            var listToRemove = new List<Creeper>();
            foreach (var c in creepersOnArea)
            {
                if (c)
                {
                    c.life -= damage;
                }
                else
                {
                    listToRemove.Add(c);
                }
            }
            foreach(var c in listToRemove)
            {
                creepersOnArea.Remove(c);
            }
            timer -= cooldown;
            //timer = 0;
        }
	}
}
