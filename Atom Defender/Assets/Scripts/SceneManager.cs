using UnityEngine;
using Pathfinding;

public class SceneManager : MonoBehaviour {
    public GameObject turret1Base;
    public GameObject turret2Base;
    public GameObject creeper1Base;
    public GameObject creeper2Base;
    public GameObject positionSelector;
    public Vector3 creeperStartPosition;
    public Vector3 creeperTargetPosition;

    private Seeker pathTester;
    private Material positionSelectorMaterial;

	void Start ()
    {
        pathTester = GetComponent<Seeker>();
        if(positionSelector)
        {
            var r = positionSelector.GetComponent<MeshRenderer>();
            positionSelectorMaterial = r.material;
        }
	}
	
    void Update()
    {
        if(positionSelectorMaterial)
        {
            var c = positionSelectorMaterial.color;
            c.a = (Mathf.Sin(Time.time * 2) + 1.1f) * 0.15f;
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
            }
        }
    }
}
