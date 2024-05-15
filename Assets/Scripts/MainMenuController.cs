using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

	public GameObject mainMenuBtns, settingsPanel, highscorePanel,colorIncBtn,colorDecBtn,speedIncBtn,speedDecBtn,backGroundMusicController;
	public Text currentColorsValue, currentSpeedValue, cameraView, soundEnabled, vibrationEnabled, bkMusicEnabled;
	public Text[] highScoreTxt=new Text[7];

	private int colorMax=7,colorMin=4;
	private float speedMax=8,speedMin=1;
	private bool inSettings=false,inHighscore=false;

	AudioSource aud,bkAud;
	GameController gameControllerScript; 

	void Start(){
		gameControllerScript = GameObject.FindGameObjectWithTag ("GameDataController").GetComponent<GameController> ();
		aud = GetComponent<AudioSource> ();
		bkAud = backGroundMusicController.GetComponent<AudioSource> ();
		currentColorsValue.text = PlayerPrefs.GetInt ("Colors").ToString ();
		currentSpeedValue.text = (9-(int)(PlayerPrefs.GetFloat ("FallTimeDelay")*10)).ToString ();
		cameraView.text=PlayerPrefs.GetInt ("CameraType").ToString()+"D";
		if(PlayerPrefs.GetInt ("SoundEnabled")==1){
			soundEnabled.text="ON";
		}
		else{
			soundEnabled.text="OFF";
		}
		if(PlayerPrefs.GetInt ("BkMusicEnabled")==1){
			bkMusicEnabled.text="ON";
			bkAud.PlayDelayed (0.5f);
		}
		else{
			bkMusicEnabled.text="OFF";
			bkAud.Stop ();
		}
		if(PlayerPrefs.GetInt ("VibrationEnabled")==1){
			vibrationEnabled.text="ON";
		}
		else{
			vibrationEnabled.text="OFF";
		}
		if (PlayerPrefs.GetInt ("Colors") == colorMax) {
			colorIncBtn.SetActive (false);
		}
		if (PlayerPrefs.GetInt ("Colors") == colorMin) {
			colorDecBtn.SetActive (false);
		}
		if ((9-(int)(PlayerPrefs.GetFloat ("FallTimeDelay")*10)) == speedMax) {
			speedIncBtn.SetActive (false);
		}
		if ((9-(int)(PlayerPrefs.GetFloat ("FallTimeDelay")*10)) == speedMin) {
			speedDecBtn.SetActive (false);
		}
		HighScoreDisplay ();
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (inSettings || inHighscore) {
				MainMenuBtn ();
				inSettings = false;
				inHighscore = false;
			} else {
				Application.Quit ();
			}
		}
	}

	public void NewGame(){
		if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
			aud.Play ();
		}
		settingsPanel.SetActive (false);
		highscorePanel.SetActive (false);
		mainMenuBtns.SetActive (false);
		SimpleSceneFader.ChangeSceneWithFade("Game_Scene");
	}

	public void HighScore(){
		if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
			aud.Play ();
		}
		mainMenuBtns.SetActive (false);
		highscorePanel.SetActive (true);
		inHighscore = true;
	}

	public void Settings(){
	    // change game player pref and then call a function to assign re-game variables according to new value player pref
		if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
			aud.Play ();
		}
		mainMenuBtns.SetActive(false);
		settingsPanel.SetActive (true);
		inSettings = true;
	}

	public void Exit(){
		if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
			aud.Play ();
		}
		Application.Quit ();
	}

	public void MainMenuBtn(){
		if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
			aud.Play ();
		}
		settingsPanel.SetActive (false);
		inSettings = false;
		highscorePanel.SetActive (false);
		inHighscore = false;
		mainMenuBtns.SetActive (true);
	}

	public void ColourValueChangeBtn(int dir){           // -1: Decrease ;;;  +1:Increase
		if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
			aud.Play ();
		}
		if (dir > 0) {            // Increase
			PlayerPrefs.SetInt ("Colors",(PlayerPrefs.GetInt ("Colors")+1));
		} 
		else {
			PlayerPrefs.SetInt ("Colors",(PlayerPrefs.GetInt ("Colors")-1));
		}
		currentColorsValue.text = PlayerPrefs.GetInt ("Colors").ToString ();
		gameControllerScript.colours = PlayerPrefs.GetInt ("Colors");
		if (PlayerPrefs.GetInt ("Colors") == colorMax) {
			colorIncBtn.SetActive (false);
		} 
		else {
			colorIncBtn.SetActive (true);
		}
		if (PlayerPrefs.GetInt ("Colors") == colorMin) {
			colorDecBtn.SetActive (false);
		}
		else {
			colorDecBtn.SetActive (true);
		}
	}

	public void SpeedValueChangeBtn(int dir){           // -1: Decrease ;;;  +1:Increase
		if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
			aud.Play ();
		}
		if (dir > 0) {     // Increase
			PlayerPrefs.SetFloat ("FallTimeDelay",PlayerPrefs.GetFloat ("FallTimeDelay")+0.1f);
		} 
		else {
			PlayerPrefs.SetFloat ("FallTimeDelay",PlayerPrefs.GetFloat ("FallTimeDelay")-0.1f);
		}
		currentSpeedValue.text = (9-(int)(PlayerPrefs.GetFloat ("FallTimeDelay")*10)).ToString ();
		gameControllerScript.fallTimeDelay = PlayerPrefs.GetFloat ("FallTimeDelay");
		if ((9-(int)(PlayerPrefs.GetFloat ("FallTimeDelay")*10)) >= speedMax) {
			speedIncBtn.SetActive (false);
		} 
		else {
			speedIncBtn.SetActive (true);
		}
		if ((9-(int)(PlayerPrefs.GetFloat ("FallTimeDelay")*10)) <= speedMin) {
			speedDecBtn.SetActive (false);
		}
		else {
			speedDecBtn.SetActive (true);
		}
	}

	public void CameraViewChangerBtn(){
		if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
			aud.Play ();
		}
		if (PlayerPrefs.GetInt ("CameraType") == 3) {
			PlayerPrefs.SetInt ("CameraType", 2);
		} 
		else {
			PlayerPrefs.SetInt ("CameraType", 3);
		}
		cameraView.text=PlayerPrefs.GetInt ("CameraType").ToString()+"D";
		gameControllerScript.cameraType = PlayerPrefs.GetInt ("CameraType");
	}

	public void BkMusicEnablerBtn(){
		if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
			aud.Play ();
		}
		if (PlayerPrefs.GetInt ("BkMusicEnabled") == 1) {
			PlayerPrefs.SetInt ("BkMusicEnabled", 0);
			bkAud.Stop ();
		} 
		else {
			PlayerPrefs.SetInt ("BkMusicEnabled", 1);
			bkAud.Play ();
		}
		if(PlayerPrefs.GetInt ("BkMusicEnabled")==1){
			bkMusicEnabled.text="ON";
		}
		else{
			bkMusicEnabled.text="OFF";
		}
	}


	public void SoundEnablerBtn(){
		if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
			aud.Play ();
		}
		if ((PlayerPrefs.GetInt ("SoundEnabled") == 1) ){
			PlayerPrefs.SetInt ("SoundEnabled", 0);
		} 
		else {
			PlayerPrefs.SetInt ("SoundEnabled", 1);
		}
		
		if(PlayerPrefs.GetInt ("SoundEnabled")==1){
			soundEnabled.text="ON";
		}
		else{
			soundEnabled.text="OFF";
		}
	}

	public void VibrationEnablerBtn(){
		if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
			aud.Play ();
		}
		if ((PlayerPrefs.GetInt ("VibrationEnabled") == 1) ){
			PlayerPrefs.SetInt ("VibrationEnabled", 0);
		} 
		else {
			PlayerPrefs.SetInt ("VibrationEnabled", 1);
		}

		if(PlayerPrefs.GetInt ("VibrationEnabled")==1){
			vibrationEnabled.text="ON";
		}
		else{
			vibrationEnabled.text="OFF";
		}
	}

	void HighScoreDisplay(){
		for (int i = 1; i <= 7; i++) {
			highScoreTxt[i-1].text=i.ToString()+" : "+PlayerPrefs.GetInt ("Highscore" + i.ToString ());
		}
	}

}
