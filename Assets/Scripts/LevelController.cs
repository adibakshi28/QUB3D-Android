using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class LevelController : MonoBehaviour {

	public GameObject camera2D, camera3D, nextTileDisplay, gameOverPanel, pausePanel, highscoreText, glitter, muteBtn, unMuteBtn,musicOnBtn,musicOffBtn, vibEnablerBtn, vibDisablBtn, backGroundMusicController;
	public AudioClip clickAudio, playBkMusic, overBkMusic;
	[HideInInspector]
	public Game gameScript;
	[HideInInspector]
	public AudioSource aud, bkAud;

	private bool gameOver=false;


	GameController gameControllerScript; 

	void Awake(){
		gameControllerScript = GameObject.FindGameObjectWithTag ("GameDataController").GetComponent<GameController> ();
		switch (gameControllerScript.cameraType) {
		case 2:
			camera2D.SetActive (true);
			camera3D.SetActive (false);
			Vector3 temp = nextTileDisplay.transform.position;
			temp.z = -1.1f;
			temp.x = -1;
			nextTileDisplay.transform.position = temp;
			break;
		case 3:
			camera2D.SetActive (false);
			camera3D.SetActive (true);
			Vector3 tmp = nextTileDisplay.transform.position;
			tmp.z = 0;
			tmp.x = -1.5f;
			nextTileDisplay.transform.position = tmp;
			break;
		default:
			camera2D.SetActive (false);
			camera3D.SetActive (true);
			break;
		}
	}

	void Start(){
		aud = GetComponent<AudioSource> ();
		bkAud = backGroundMusicController.GetComponent<AudioSource> ();
		aud.clip = clickAudio;
		if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
			muteBtn.SetActive (true);
			unMuteBtn.SetActive (false);
		} 
		else {
			muteBtn.SetActive (false);
			unMuteBtn.SetActive (true);		
		}
		if (PlayerPrefs.GetInt ("BkMusicEnabled") == 1) {
			musicOffBtn.SetActive (true);
			musicOnBtn.SetActive (false);
			bkAud.clip = playBkMusic;
			bkAud.PlayDelayed (0.5f);
		} 
		else {
			musicOffBtn.SetActive (false);
			musicOnBtn.SetActive (true);	
			bkAud.Stop ();
		}
		if (PlayerPrefs.GetInt ("VibrationEnabled") == 1) {
			vibDisablBtn.SetActive (false);
			vibEnablerBtn.SetActive (true);
		} 
		else {
			vibDisablBtn.SetActive (true);
			vibEnablerBtn.SetActive (false);		
		}
	}

	void Update(){
		if((Input.GetKeyDown(KeyCode.Escape))&&(!gameOver)){
			Pause ();
		}
	}

	public void GameOver(int score){
		gameScript.enabled = false;
		gameOver = true;
		Debug.Log ("Game Over");
		if (PlayerPrefs.GetInt ("BkMusicEnabled") == 1) {
			bkAud.Stop ();
		} 
		StartCoroutine (GameEnd (score));
	}
	IEnumerator GameEnd(int score){
		yield return new WaitForSeconds(0.5f);
		gameOverPanel.SetActive (true);
		SetHighscore (score);
		if (PlayerPrefs.GetInt ("BkMusicEnabled") == 1) {
			bkAud.clip = overBkMusic;
			bkAud.volume = 0.15f;
			bkAud.Play ();
		} 
		yield return new WaitForSeconds(0.5f);
		gameControllerScript.showInterstitialAd ();            //  show interinstial ads
	}


	public void Pause(){
		if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
			aud.clip = clickAudio;
			aud.Play ();
		}
		pausePanel.SetActive (true);
		Time.timeScale = 0;
	}

	public void UnPause(){
		if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
			aud.clip = clickAudio;
			aud.Play ();
		}
		Time.timeScale = 1;
		pausePanel.SetActive (false);
	}

	public void Mute(){
		PlayerPrefs.SetInt ("SoundEnabled", 0);
		muteBtn.SetActive (false);
		unMuteBtn.SetActive (true);
	}
	public void UnMute(){
		PlayerPrefs.SetInt ("SoundEnabled", 1);
		aud.Play ();
		muteBtn.SetActive (true);
		unMuteBtn.SetActive (false);
	}

	public void MusicOff(){
		PlayerPrefs.SetInt ("BkMusicEnabled", 0);
		musicOffBtn.SetActive (false);
		musicOnBtn.SetActive (true);
		bkAud.Stop ();
	}
	public void MusicOn(){
		PlayerPrefs.SetInt ("BkMusicEnabled", 1);
		musicOffBtn.SetActive (true);
		musicOnBtn.SetActive (false);
		bkAud.clip = playBkMusic;
		bkAud.Play ();
	}


	public void VibEnable(){
		if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
			aud.clip = clickAudio;
			aud.Play ();
		}
		PlayerPrefs.SetInt ("VibrationEnabled", 0);
		vibEnablerBtn.SetActive (false);
		vibDisablBtn.SetActive (true);
	}
	public void VibDisable(){
		if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
			aud.clip = clickAudio;
			aud.Play ();
		}
		PlayerPrefs.SetInt ("VibrationEnabled", 1);
		vibEnablerBtn.SetActive (true);
		vibDisablBtn.SetActive (false);
	}

	public void Restart(){
		if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
			aud.clip = clickAudio;
			aud.Play ();
		}
		Time.timeScale = 1;
		SceneManager.LoadScene("Game_Scene");
	}

	public void MainMenu(){
		if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
			aud.clip = clickAudio;
			aud.Play ();
		}
		Time.timeScale = 1;
		SimpleSceneFader.ChangeSceneWithFade("Main_Menu");
	}

	void SetHighscore(int currentGameScore){    // "Highscore1 is the highest score which is stored in '0' indice of this array"
		int[] tempHighScore = new int[8];
		for (int i = 0; i <= 6; i++) {
			tempHighScore [i] = PlayerPrefs.GetInt ("Highscore" + (i + 1).ToString ());   // '0' indice stores previous highest score 
		}
		tempHighScore [7] = currentGameScore;   // last indice of array stores current score;
		Array.Sort(tempHighScore);
		for (int i = 0; i <= 6; i++) {
			PlayerPrefs.SetInt ("Highscore" + (i + 1).ToString (),tempHighScore [7-i]);  
		}
		if (currentGameScore == PlayerPrefs.GetInt ("Highscore1")) {
			highscoreText.gameObject.SetActive (true);
			Instantiate (glitter, transform.position, Quaternion.identity);
		}
	}

}
