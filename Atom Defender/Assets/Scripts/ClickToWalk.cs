using UnityEngine;
using Pathfinding;
using System.Linq;

public class ClickToWalk : MonoBehaviour
{
    private Seeker agent;
	void Start()
    {
        agent = GetComponent<Seeker>();
    }

	void Update ()
    {
	    if(Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if(Physics.Raycast(ray, out hitInfo))
            {
                //Debug.Log("CalculatePath:" + agent.CalculatePath(hitInfo.point, path));
                //agent.GetNewPath(agent.transform.position, hitInfo.point).
                Debug.Log("SetDestination:" + agent.StartPath(agent.transform.position, hitInfo.point, OnPathComplete) + " | hitInfo.point:" + hitInfo.point);
                //Debug.Log("SetDestination:" + agent.GetNewPath(agent.transform.position, hitInfo.point) + " | hitInfo.point:" + hitInfo.point);
            }
        }
	}

    public void OnPathComplete(Path p)
    {
        Debug.Log("Len:" + p.path.Count + " | " + string.Join(",", p.path.Select(x => x.position.ToString()).ToArray()));
    }

}
