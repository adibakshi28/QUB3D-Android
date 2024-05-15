using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopUpTxtScript : MonoBehaviour {

	public Text popUpText;
	[HideInInspector]
	public int addScore;
//	public Color clr;

	void Start () {
		Destroy (this.gameObject, 1.5f);
		popUpText.text = addScore.ToString ();
//		popUpText.color = clr;
	}

}
