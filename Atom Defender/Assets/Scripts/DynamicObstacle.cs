using UnityEngine;
using Pathfinding;

public class DynamicObstacle : MonoBehaviour {
	void Start () {
        var obstacle = GetComponent<GraphUpdateScene>();
        obstacle.Apply();
        //AstarPath.active.UpdateGraphs(obstacle as GraphUpdateObject);
    }
}
