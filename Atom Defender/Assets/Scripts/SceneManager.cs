using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour {
    public GameObject turret1Base;
    public GameObject turret2Base;
    public GameObject creeper1Base;
    public GameObject creeper2Base;
    public Vector3 creeperStartPosition;
    public Vector3 creeperTargetPosition;

    private Seeker pathTester;

	void Start () {
        pathTester = GetComponent<Seeker>();
	}
	
	void Update () {
	
	}
}
