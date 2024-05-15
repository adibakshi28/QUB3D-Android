using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;

public class GameController : MonoBehaviour {

	public int currentVersion;                       // increment this field by 1 in every newer versions of the game release
	public string instiantialID;

	[HideInInspector]
	public int colours , cameraType;
	[HideInInspector]
	public float fallTimeDelay;

	InterstitialAd interstitial;

	void Start () {
		if (PlayerPrefs.GetInt ("hasPlayed") < 50) {                  // (50 is used to ensure hasPlayed has a value less than 100 which indicates that the game has been played previously...... expect that 100 and 50 as numbers ha no other significance)
			PlayerPrefs.SetInt("hasPlayed",100);
			Debug.Log ("Application Running for the first time");

			//Game Default Settings
			PlayerPrefs.SetInt("Colors",4);
			PlayerPrefs.SetFloat("FallTimeDelay",0.4f);
			PlayerPrefs.SetInt ("CameraType", 3);      // 3: 3D ;;;; 2: 2D
			PlayerPrefs.SetInt ("SoundEnabled", 1);    // 1:Enabled ;;;;0:Disabled   
		} 
		else {
			PlayerPrefs.SetInt ("firstTime", 0);
		}
		PlayerPrefs.SetInt("timesLaunched",(PlayerPrefs.GetInt("timesLaunched")+1));     //  incriments by 1 every time the application is launched 

		if (!(PlayerPrefs.GetInt ("version") == currentVersion)) {
			PlayerPrefs.SetInt ("version", currentVersion);
			//  put all the new player pref statements or changes to previously existant in future versions here eg. new players , coin gifts etc
			PlayerPrefs.SetInt ("VibrationEnabled", 1);    // 1:Enabled ;;;;0:Disabled  
			PlayerPrefs.SetInt("BkMusicEnabled",1);      // 1:Enabled ;;;;0:Disabled  
		}

		DontDestroyOnLoad (this.gameObject);

		RequestInterstitialAds();

		Screen.autorotateToLandscapeLeft = false;
		Screen.autorotateToLandscapeRight = false;
		Screen.autorotateToPortrait = true;
		Screen.autorotateToPortraitUpsideDown = true;

		colours = PlayerPrefs.GetInt ("Colors");
		fallTimeDelay = PlayerPrefs.GetFloat ("FallTimeDelay");
		cameraType = PlayerPrefs.GetInt ("CameraType");

		SceneManager.LoadScene ("Main_Menu");

	}

	private void RequestInterstitialAds()
	{
		// Initialize an InterstitialAd.
		interstitial = new InterstitialAd(instiantialID);

		//***Test***
	//		AdRequest request = new AdRequest.Builder()
	//		.AddTestDevice(AdRequest.TestDeviceSimulator)       // Simulator.
	//		.AddTestDevice("2077ef9a63d2b398840261c8221a0c9b")  // My test device.
	//		.Build();  

		//***Production***
		AdRequest request = new AdRequest.Builder().Build();

		// Load the interstitial with the request.
		interstitial.LoadAd(request);
	}


	public void showInterstitialAd()
	{
		//Show Ad
		if (interstitial.IsLoaded ()) {
			interstitial.Show ();
		} 
		else {
			RequestInterstitialAds ();
		}
	}
}
