using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour {
    public GameObject turret1Prefab;
    public GameObject turret2Prefab;
    public GameObject creeper1Prefab;
    public GameObject creeper2Prefab;
    public GameObject positionSelector;
    public Vector3 creeperStartPosition;
    public Vector3 creeperTargetPosition;

    private Seeker pathTester;
    private Material positionSelectorMaterial;
    private GraphUpdateScene positionSelectorGraph;
    private ABPath defaultPath;
    private Color positionSelectorInitialColor;
    private Int3 lastValidNodePosition;
    private List<GraphNode> lastNodesChanged = new List<GraphNode>();
    private GraphNode creeperStartNode;
    private GraphNode creeperTargetNode;

    void Start ()
    {
        pathTester = GetComponent<Seeker>();
        defaultPath = (ABPath)pathTester.StartPath(creeperStartPosition, creeperTargetPosition);//pathTester.GetNewPath(creeperStartPosition, creeperTargetPosition);
        if (positionSelector)
        {
            var r = positionSelector.GetComponent<MeshRenderer>();
            positionSelectorMaterial = r.material;
            positionSelectorInitialColor = r.sharedMaterial.color;
            positionSelectorGraph =  positionSelector.GetComponent<GraphUpdateScene>();
        }
        creeperStartNode = AstarPath.active.GetNearest(creeperStartPosition).node;
        creeperTargetNode = AstarPath.active.GetNearest(creeperTargetPosition).node;
    }
	
    void Update()
    {
        if(positionSelectorMaterial)
        {
            var c = positionSelectorMaterial.color;
            c.a = (Mathf.Sin(Time.time * 2) + 1.3f) * 0.1f;
            positionSelectorMaterial.color = c;
        }
    }
    
	void FixedUpdate ()
    {
        if (positionSelector)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                var node = AstarPath.active.GetNearest(hitInfo.point).node;
                positionSelector.transform.position = (Vector3)node.position;
                var connectionCount = 0;
                node.GetConnections(delegate (GraphNode other) { connectionCount++; });
                if(connectionCount != 8)
                {
                    var c = Color.red;
                    c.a = positionSelectorMaterial.color.a;
                    positionSelectorMaterial.color = c;
                    //if (lastNodesChanged.Count > 0)
                    //{
                    //    foreach (var n in lastNodesChanged)
                    //    {
                    //        n.Tag = 1;
                    //    }
                    //    lastNodesChanged.Clear();
                    //}
                }
                else
                {
                    bool validPath = false;
                    if (lastValidNodePosition != node.position)
                    {
                        //if(lastNodesChanged.Count > 0)
                        //{
                        //    foreach(var n in lastNodesChanged)
                        //    {
                        //        n.Tag = 1;
                        //    }
                        //    lastNodesChanged.Clear();
                        //}
                        //positionSelector.GetComponent<GraphUpdateScene>().Apply();
                        //lastNodesChanged.AddRange((AstarPath.active.graphs[0] as GridGraph).GetNodesInArea(positionSelectorGraph.GetBounds()));
                        //foreach (var n in lastNodesChanged)
                        //{
                        //    n.Tag = 2;
                        //}
                        //defaultPath = (ABPath)pathTester.StartPath(creeperStartPosition, creeperTargetPosition);
                        //AstarPath.WaitForPath(defaultPath);
                        lastValidNodePosition = node.position;
                    }
                    //var distance = Vector3.Distance(defaultPath.endPoint, creeperTargetPosition);
                    //if (distance < 0.5f)
                    var guo = new GraphUpdateObject(positionSelectorGraph.GetBounds());
                    guo.modifyWalkability = positionSelectorGraph.modifyWalkability;
                    guo.setWalkability = positionSelectorGraph.setWalkability;
                    Debug.Log(positionSelectorGraph.GetBounds());
                    if (GraphUpdateUtilities.UpdateGraphsNoBlock(guo, creeperStartNode, creeperTargetNode, true))
                    {
                        validPath = true;
                    }
                    if (validPath)
                    {
                        var c = positionSelectorInitialColor;
                        c.a = positionSelectorMaterial.color.a;
                        positionSelectorMaterial.color = c;
                    }
                    else
                    {
                        var c = Color.red;
                        c.a = positionSelectorMaterial.color.a;
                        positionSelectorMaterial.color = c;
                    }
                }
            }
        }
    }
}
