using UnityEngine;
using System.Collections;

public class Creeper : MonoBehaviour {
    public float speed = 1;

    private Seeker pathFinder;

	void Start () {
        pathFinder = GetComponent<Seeker>();
	}
	
	void Update ()
    {
        var path = pathFinder.GetCurrentPath();
        //Debug.Log(pathFinder.GetCurrentPath() == null);
        if (path != null)
        {
            //path.ge
        }
	}
}
