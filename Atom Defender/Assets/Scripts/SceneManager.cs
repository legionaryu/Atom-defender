using UnityEngine;
using UnityEngine.UI;
using Pathfinding;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour {
    [System.Serializable]
    public class Wave
    {
        public int creeper1Count;
        public int creeper2Count;
    }

    public GameObject turret1Prefab;
    public GameObject turret2Prefab;
    public GameObject creeper1Prefab;
    public GameObject creeper2Prefab;
    public GameObject positionSelector;
    public Vector3 creeperStartPosition;
    public Vector3 creeperTargetPosition;
    public Button btnStartGame;
    public Text txtMoney;
    public Text txtWaveNo;
    public Text txtCountDown;
    public float money = 200;
    public float spawnCooldown = 0.1f;
    [SerializeField]
    public Wave[] waves;

    private bool gameStarted = false;
    private Material positionSelectorMaterial;
    private GraphUpdateScene positionSelectorGraph;
    private Color positionSelectorInitialColor;
    private Int3 lastValidNodePosition;
    private bool lastValidState = false;
    private GraphNode creeperStartNode;
    private GraphNode creeperTargetNode;
    private HashSet<Creeper> allCreepers = new HashSet<Creeper>();

    public static SceneManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start ()
    {
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
        if (gameStarted)
        {
            if (positionSelectorMaterial)
            {
                var c = positionSelectorMaterial.color;
                c.a = (Mathf.Sin(Time.time * 2) + 1.3f) * 0.1f;
                positionSelectorMaterial.color = c;
            }
        }
    }
    
	void FixedUpdate ()
    {
        if (gameStarted)
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
                    if (connectionCount != 8)
                    {
                        var c = Color.red;
                        c.a = positionSelectorMaterial.color.a;
                        positionSelectorMaterial.color = c;
                        lastValidState = false;
                    }
                    else
                    {
                        if (lastValidNodePosition != node.position)
                        {
                            var guo = new GraphUpdateObject(positionSelectorGraph.GetBounds());
                            guo.modifyWalkability = positionSelectorGraph.modifyWalkability;
                            guo.setWalkability = positionSelectorGraph.setWalkability;
                            if (GraphUpdateUtilities.UpdateGraphsNoBlock(guo, creeperStartNode, creeperTargetNode, true))
                            {
                                lastValidState = true;
                            }
                            else
                            {
                                lastValidState = false;
                            }
                            lastValidNodePosition = node.position;
                        }

                        if (lastValidState)
                        {
                            var c = positionSelectorInitialColor;
                            c.a = positionSelectorMaterial.color.a;
                            positionSelectorMaterial.color = c;

                            if (Input.GetMouseButtonDown(0))
                            {
                                var turret2 = GameObject.Instantiate(turret2Prefab) as GameObject;
                                var p = (Vector3)node.position;
                                p.y = turret2.transform.position.y;
                                turret2.transform.position = p;
                                lastValidState = false;
                                foreach (var creeper in allCreepers)
                                {
                                    creeper.RecalculatePath();
                                }
                            }
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

    public void StartGame()
    {
        gameStarted = true;
        if(btnStartGame)
        {
            btnStartGame.enabled = false;
        }
    }

    void EndGame()
    {
        gameStarted = false;
        if (btnStartGame)
        {
            btnStartGame.enabled = true;
        }
    }

    public void CreeperDied(Creeper creeper, float reward)
    {
        if (allCreepers.Contains(creeper))
        {
            money += reward;
            allCreepers.Remove(creeper);
        }
    }

    public void CreeperArrived(Creeper creeper)
    {
        CreeperDied(creeper, 0);
        var creeperObj = GameObject.Instantiate(creeper1Prefab) as GameObject;
        var p = creeperStartPosition;
        p.y = creeperObj.transform.position.y;
        creeperObj.transform.position = p;
        allCreepers.Add(creeperObj.GetComponent<Creeper>());
    }
}
