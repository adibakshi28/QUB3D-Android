using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class GPGSController : MonoBehaviour {
	
	[HideInInspector]
	public bool connectedToGooglePlaySevice=false,superAddictAchievementCalled=false;

	void Start () {
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
		PlayGamesPlatform.InitializeInstance(config);
		// Activate the Google Play Games platform
		PlayGamesPlatform.Activate();
		Signup ();   // Attempt signup at start of the game 
	}

	public void Signup(){
		if (!connectedToGooglePlaySevice) {
			Debug.Log ("Tring to Sign user up");
			// authenticate user:
			Social.localUser.Authenticate((bool success) => {
				connectedToGooglePlaySevice=success;

				if(!superAddictAchievementCalled){
					superAddictAchievementCalled=true;
					SuperAddict(); // Call the number of times launched achievement function
				}

			});
		}
	}

	public void ShowAchievementsUI(){
		// show achievements UI
		Debug.Log("Showing Achievement Login");
		Social.ShowAchievementsUI();
	}

	public void ShowLeaderBoard(){
		// show leaderboard UI
		Debug.Log("Showing Leader Board");
		Social.ShowLeaderboardUI();
	}


	public void PostScoreInLeaderBoard(int score){
		Debug.Log ("Posting Score in leader board");
		Social.ReportScore(score, GPGSIds.leaderboard_hall_of_qub, (bool success) => {
			// handle success or failure
		});
	}

	// Indivudial Achievements Functions 

	public void StarterAchievement(){    // Done
		Debug.Log("Starter achievement earned");
		Social.ReportProgress(GPGSIds.achievement_starter, 100.0f, (bool success) => {
			// handle success or failure
		});
	}

	public void ShamelessAchievement(){    // Done
		Social.ReportProgress(GPGSIds.achievement_shameless, 100.0f, (bool success) => {
			// handle success or failure
		});
	}

	public void FiveInARowAchievement(){   // Done
		Social.ReportProgress(GPGSIds.achievement_5_at_a_time, 100.0f, (bool success) => {
			// handle success or failure
		});
	}

	public void SixInARowAchievement(){    // Done
		Social.ReportProgress(GPGSIds.achievement_6_at_a_time, 100.0f, (bool success) => {
			// handle success or failure
		});
	}

	public void SevenInARowAchievement(){    // Done
		Social.ReportProgress(GPGSIds.achievement_7_at_a_time, 100.0f, (bool success) => {
			// handle success or failure
		});
	}

	public void PointsHunterAchievement(){   // Done
		Social.ReportProgress(GPGSIds.achievement_points_hunter, 100.0f, (bool success) => {
			// handle success or failure
		});
	}

	public void CenturyOfCenturiesAchievement(){    // Done
		Social.ReportProgress(GPGSIds.achievement_century_of_centuries, 100.0f, (bool success) => {
			// handle success or failure
		});
	}

	public void FastAsLightAchievement(){   // Done
		Social.ReportProgress(GPGSIds.achievement_fast_as_light, 100.0f, (bool success) => {
			// handle success or failure
		});
	}

	public void RainbowManiaAchievement(){     // Done
		Social.ReportProgress(GPGSIds.achievement_rainbow_mania, 100.0f, (bool success) => {
			// handle success or failure
		});
	}

	public void NotBadAchievement(){   // Done
		Social.ReportProgress(GPGSIds.achievement_not_bad, 100.0f, (bool success) => {
			// handle success or failure
		});
	}

	public void CubeHunterAchievement(){   // Done
		Social.ReportProgress(GPGSIds.achievement_cube_hunter, 100.0f, (bool success) => {
			// handle success or failure
		});
	}

	public void CubenemyAchievement(){    // Done
		Social.ReportProgress(GPGSIds.achievement_cubenemy, 100.0f, (bool success) => {
			// handle success or failure
		});
	}

	public void UltimateDestroyerAchievement(){    // Done
		Social.ReportProgress(GPGSIds.achievement_ultimate_destroyer, 100.0f, (bool success) => {
			// handle success or failure
		});
	}

	public void SuperAddict(){     // Done
		PlayGamesPlatform.Instance.IncrementAchievement(
			GPGSIds.achievement_super_addict,1 , (bool success) => {
				// handle success or failure
		});
	}

	public void QUBSlayer(int cubesDestroyed){   // Done
		PlayGamesPlatform.Instance.IncrementAchievement(
			GPGSIds.achievement_qub_slayer, cubesDestroyed, (bool success) => {
				// handle success or failure
		});
	}

	// Achievements Helper Functions 

	public void InARowAchievementDetermine(int cubesDestroyed){
		if (cubesDestroyed >= 5) {
			FiveInARowAchievement ();
			if (cubesDestroyed >= 6) {
				SixInARowAchievement ();
				if (cubesDestroyed >= 7) {
					SevenInARowAchievement ();
				}
			}
		}
	}

	public void ScoreInAGameAchievementDetermine(int score ,int colours,int speed){
		if(score>2500){
			if (colours == 7) {
				RainbowManiaAchievement ();
			}
			if (speed == 8) {
				FastAsLightAchievement ();
			}
			if (score > 5000) {
				PointsHunterAchievement ();
				if (score > 10000) {
					CenturyOfCenturiesAchievement ();
				}
			}
		}
	}

	public void CubesDestroyedInAGameAchievementDetermine(int cubesDestroyed){
		if (cubesDestroyed>50) {
			NotBadAchievement ();
			if (cubesDestroyed > 100) {
				CubeHunterAchievement ();
				if (cubesDestroyed > 500) {
					CubenemyAchievement ();
					if (cubesDestroyed > 1000) {
						UltimateDestroyerAchievement ();
					}
				}
			}
		}
	}


}
