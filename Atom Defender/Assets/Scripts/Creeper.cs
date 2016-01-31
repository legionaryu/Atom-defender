using UnityEngine;
using System.Collections;
using Pathfinding;

public class Creeper : MonoBehaviour {
    public float life = 1000;
    public float reward = 50;

    private Seeker pathFinder;

	void Start () {
        pathFinder = GetComponent<Seeker>();
        pathFinder.StartPath(transform.position, SceneManager.Instance.creeperTargetPosition);
    }
	
	void FixedUpdate ()
    {
        if (life <= 0)
        {
            SceneManager.Instance.CreeperDied(this, reward);
            GameObject.Destroy(gameObject);
        }
        else
        {
            var path = (ABPath)pathFinder.GetCurrentPath();
            //Debug.Log(pathFinder.GetCurrentPath() == null);
            if (path != null && Vector3.Distance(transform.position, path.endPoint) < 0.5)
            {
                SceneManager.Instance.CreeperArrived(this);
                GameObject.Destroy(gameObject);
            }
        }
	}

    public void RecalculatePath()
    {
        pathFinder.StartPath(transform.position, SceneManager.Instance.creeperTargetPosition);
    }
}
