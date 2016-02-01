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
    public Text txtLives;
    public float spawnCooldown = 0.1f;
    public float waveCooldown = 10f;
    [SerializeField]
    public Wave[] waves;

    private Wave currentWave = new Wave();
    private int lives = 10;
    private int waveCount = 0;
    private int selectedTurretIndex = 1;
    private float money = 200;
    private float waveCooldownTimer = 0;
    private float spawnCooldownTimer = 0;
    private bool gameStarted = false;
    private Material positionSelectorMaterial;
    private GraphUpdateScene positionSelectorGraph;
    private Color positionSelectorInitialColor;
    private Int3 lastValidNodePosition;
    private bool lastValidState = false;
    private GraphNode creeperStartNode;
    private GraphNode creeperTargetNode;
    private HashSet<Creeper> allCreepers = new HashSet<Creeper>();
    private HashSet<GameObject> allTurrets = new HashSet<GameObject>();

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
        if (txtMoney) txtMoney.text = "Money: $" + money.ToString("F0");
        if (txtWaveNo) txtWaveNo.text = "Wave no. " + (waveCount + 1).ToString("D");
        if (txtCountDown) txtCountDown.text = "Next wave in " + (waveCooldownTimer).ToString("F") + " secs";
        if (txtLives) txtLives.text = "Lives: " + lives.ToString("D");

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
            waveCooldownTimer -= Time.fixedDeltaTime;
            if (waveCooldownTimer <= 0)
            {
                waveCooldownTimer = 0;
                spawnCooldownTimer -= Time.fixedDeltaTime;
                if (spawnCooldownTimer <= 0)
                {
                    if (currentWave.creeper1Count == 0)
                        currentWave.creeper1Count = waves[waveCount].creeper1Count;
                    if (currentWave.creeper2Count == 0)
                        currentWave.creeper2Count = waves[waveCount].creeper2Count;

                    if (currentWave.creeper1Count > 0)
                    {
                        var creeperObj = GameObject.Instantiate(creeper1Prefab) as GameObject;
                        var p = creeperStartPosition;
                        p.y = creeperObj.transform.position.y;
                        creeperObj.transform.position = p;
                        allCreepers.Add(creeperObj.GetComponent<Creeper>());
                        currentWave.creeper1Count--;
                    }
                    if (currentWave.creeper2Count > 0)
                    {
                        var creeperObj = GameObject.Instantiate(creeper2Prefab) as GameObject;
                        var p = creeperStartPosition;
                        p.y = creeperObj.transform.position.y;
                        creeperObj.transform.position = p;
                        allCreepers.Add(creeperObj.GetComponent<Creeper>());
                        currentWave.creeper2Count--;
                    }
                    if(currentWave.creeper1Count <= 0 && currentWave.creeper2Count <= 0)
                    {
                        if(waveCount == 4)
                        {
                            EndGame();
                            return;
                        }
                        waveCount++;
                        waveCooldownTimer = waveCooldown;
                    }
                    spawnCooldownTimer += spawnCooldown;
                }
            }
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
                            var prefabObj = (selectedTurretIndex == 1 ? turret1Prefab : turret2Prefab);
                            Turret selectedTurretPrefab;
                            if(selectedTurretIndex == 1)
                            {
                                selectedTurretPrefab = prefabObj.GetComponent<Turret1>();
                            }
                            else
                            {
                                selectedTurretPrefab = prefabObj.GetComponent<Turret2>();
                            }
                            if (Input.GetMouseButtonDown(0) && money >= selectedTurretPrefab.cost)
                            {
                                money -= selectedTurretPrefab.cost;
                                var turret = GameObject.Instantiate(prefabObj) as GameObject;
                                allTurrets.Add(turret);
                                var p = (Vector3)node.position;
                                p.y = turret.transform.position.y;
                                turret.transform.position = p;
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
        lives = 10;
        waveCount = 0;
        money = 200;
        gameStarted = true;
        waveCooldownTimer = 5;
        spawnCooldownTimer = spawnCooldown;
        currentWave.creeper1Count = 0;
        currentWave.creeper2Count = 0;
        if (btnStartGame)
        {
            btnStartGame.gameObject.SetActive(false);
        }
    }

    void EndGame()
    {
        gameStarted = false;
        if (btnStartGame)
        {
            btnStartGame.gameObject.SetActive(true);
        }
        foreach(var c in allCreepers)
        {
            GameObject.Destroy(c.gameObject);
        }
        foreach(var t in allTurrets)
        {
            GameObject.Destroy(t);
        }
    }

    public void SelectTurret1()
    {
        selectedTurretIndex = 1;
    }

    public void SelectTurret2()
    {
        selectedTurretIndex = 2;
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
        lives--;
        if(lives <= 0)
        {
            EndGame();
        }
        //var creeperObj = GameObject.Instantiate(creeper1Prefab) as GameObject;
        //var p = creeperStartPosition;
        //p.y = creeperObj.transform.position.y;
        //creeperObj.transform.position = p;
        //allCreepers.Add(creeperObj.GetComponent<Creeper>());
    }
}
