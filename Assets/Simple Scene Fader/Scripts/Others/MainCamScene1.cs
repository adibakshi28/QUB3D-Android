using UnityEngine;
using System.Collections;

public class MainCamScene1 : MonoBehaviour {

	// STEPS

	// 1) Drag prefab named "SSF" from the prefab folder in your FIRST scene.
	// 2) Open tag manager & add "SceneFader" as a new tag
	// 3) Use the code given below to load your scene.
	// 4) Make sure that scene you are loading is added in your build settings.

	public void OpenScene2Button () {
		SimpleSceneFader.ChangeSceneWithFade("Scene 2");
	}

}
