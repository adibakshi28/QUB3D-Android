using UnityEngine;
using System.Collections;

public class MainCamScene2 : MonoBehaviour {

	public void OpenScene1Button () {
		SimpleSceneFader.ChangeSceneWithFade("Scene 1");
	}

}
