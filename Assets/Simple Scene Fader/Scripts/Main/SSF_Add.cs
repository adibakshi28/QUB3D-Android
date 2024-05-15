using UnityEngine;
using System.Collections;

public class SSF_Add : MonoBehaviour {

	public GameObject SSFPrefab;

	// Use this for initialization
	void Awake () {
		if( GameObject.FindGameObjectWithTag("SceneFader") == null ){
			GameObject.Instantiate( SSFPrefab, new Vector3(0f,0f,0f), Quaternion.identity );
		}
	}
}
