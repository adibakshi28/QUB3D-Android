using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class Game : MonoBehaviour {

	public Transform tile;
	public Vector2 mapSize;
	[Range(0,1)]
	public float outlinePercent;
	public Text scoreText;
	public GameObject nextTile1, nextTile2, nextTile3, popUpTxtCanvas,scoreTxtGameObj;
	public GameObject[] explosions;
	public AudioClip cubeDestroyAudio, gameOverAudio,stompAudio;

	public Color colorStart = Color.red;
	public Color colorEnd = Color.green;
	public float duration = 1.0F;

	[HideInInspector]
	public int score=0,baseScore=20,scoreMultiplier=2;   // for speed of 1 and 4 colours
	[HideInInspector]
	public List<int> cellDataDisplay;           // Just for dispaly purpose of realtime value of cells and direct communication with cell game object
	// as both are stored in 1D Lists
	[HideInInspector]
	public int[,] cellData=new int[20,20];   //arbitury size to encompass all elements in a 20*20 grid at max ; All modifications to cell data are made here first
	[HideInInspector]
	public List<GameObject> cell;
	[HideInInspector]
	public List<Vector2> adjSameDataCells;    // List of 2D adj indices having same data as found in checked indice  (Modified by Check() )
	[HideInInspector]
	public List<Renderer> rend;

	int colours,adjCellThreshold = 4;
	float fallTimeDelay ;

	private bool gameOver = false,controlEnabled=false;
	private Vector2 activeCell1,activeCell2,activeCell3;       // Stores indices of currently active (user controlled) cells
	private int configration=1,waveCubes,nextCellData1,nextCellData2,nextCellData3,nextWaveCubes;    // configration Stores Rotation configration of a wave 
	private float copy_FallTimeOffset;       // A variable storing fallTimeDelay data .it is never modified and is used to keep fallTimeDelay secure
	private Animator scoreTxtAnimator;

	AudioSource aud;
	GameController gameControllerScript;                  // To assign game level variables
	LevelController levelControllerScript;                // To call game over() 

	void Awake(){
		levelControllerScript = GameObject.FindGameObjectWithTag ("LevelDataController").GetComponent<LevelController> ();
		gameControllerScript = GameObject.FindGameObjectWithTag ("GameDataController").GetComponent<GameController> ();
		levelControllerScript.gameScript = this.gameObject.GetComponent<Game> ();
		colours = gameControllerScript.colours;
		fallTimeDelay = gameControllerScript.fallTimeDelay;
	}

	void Start () {
		aud = GetComponent<AudioSource> ();
		scoreTxtAnimator = scoreTxtGameObj.GetComponent<Animator> ();
		copy_FallTimeOffset = fallTimeDelay;
		baseScore = ((9 - (int)(fallTimeDelay * 10)) * baseScore) + ((4 - colours) * baseScore / 2);
		scoreMultiplier = colours - 1;
		scoreText.text = "0";
		GenerateMap ();
	}

	void LateUpdate(){            // Update cell GameObject realtime 
		if (!gameOver) {
			int cl = 0;           // control variable to keep track of cells      
			float lerp = Mathf.PingPong(Time.time, duration) / duration;
			for (int y = 0; y < mapSize.y; y++) {
				for (int x = 0; x < mapSize.x; x++) {
					cellDataDisplay [cl] = cellData [y, x];   // Assign orignal/modified cell data to display cell daya for display in inspector
					CellDataDisplayer(cell[cl],cellDataDisplay[cl]);
					rend[cl].material.color = Color.Lerp(colorStart, colorEnd, lerp);
					cl++;
				}
			}
		}
	}

	public void CellDataDisplayer(GameObject cell,int data){
		switch (data) {        
		case 0:
			cell.transform.GetChild(0).gameObject.SetActive(false);
			cell.transform.GetChild(1).gameObject.SetActive(false);
			cell.transform.GetChild(2).gameObject.SetActive(false);
			cell.transform.GetChild(3).gameObject.SetActive(false);
			cell.transform.GetChild(4).gameObject.SetActive(false);
			cell.transform.GetChild(5).gameObject.SetActive(false);
			cell.transform.GetChild(6).gameObject.SetActive(false);
			break;
		case 1:
			cell.transform.GetChild(0).gameObject.SetActive(true);
			cell.transform.GetChild(1).gameObject.SetActive(false);
			cell.transform.GetChild(2).gameObject.SetActive(false);
			cell.transform.GetChild(3).gameObject.SetActive(false);
			cell.transform.GetChild(4).gameObject.SetActive(false);
			cell.transform.GetChild(5).gameObject.SetActive(false);
			cell.transform.GetChild(6).gameObject.SetActive(false);
			break;
		case 2:
			cell.transform.GetChild(0).gameObject.SetActive(false);
			cell.transform.GetChild(1).gameObject.SetActive(true);
			cell.transform.GetChild(2).gameObject.SetActive(false);
			cell.transform.GetChild(3).gameObject.SetActive(false);
			cell.transform.GetChild(4).gameObject.SetActive(false);
			cell.transform.GetChild(5).gameObject.SetActive(false);
			cell.transform.GetChild(6).gameObject.SetActive(false);
			break;
		case 3:
			cell.transform.GetChild(0).gameObject.SetActive(false);
			cell.transform.GetChild(1).gameObject.SetActive(false);
			cell.transform.GetChild(2).gameObject.SetActive(true);
			cell.transform.GetChild(3).gameObject.SetActive(false);
			cell.transform.GetChild(4).gameObject.SetActive(false);
			cell.transform.GetChild(5).gameObject.SetActive(false);
			cell.transform.GetChild(6).gameObject.SetActive(false);
			break;
		case 4:
			cell.transform.GetChild(0).gameObject.SetActive(false);
			cell.transform.GetChild(1).gameObject.SetActive(false);
			cell.transform.GetChild(2).gameObject.SetActive(false);
			cell.transform.GetChild(3).gameObject.SetActive(true);
			cell.transform.GetChild(4).gameObject.SetActive(false);
			cell.transform.GetChild(5).gameObject.SetActive(false);
			cell.transform.GetChild(6).gameObject.SetActive(false);
			break;
		case 5:
			cell.transform.GetChild(0).gameObject.SetActive(false);
			cell.transform.GetChild(1).gameObject.SetActive(false);
			cell.transform.GetChild(2).gameObject.SetActive(false);
			cell.transform.GetChild(3).gameObject.SetActive(false);
			cell.transform.GetChild(4).gameObject.SetActive(true);
			cell.transform.GetChild(5).gameObject.SetActive(false);
			cell.transform.GetChild(6).gameObject.SetActive(false);
			break;
		case 6:
			cell.transform.GetChild(0).gameObject.SetActive(false);
			cell.transform.GetChild(1).gameObject.SetActive(false);
			cell.transform.GetChild(2).gameObject.SetActive(false);
			cell.transform.GetChild(3).gameObject.SetActive(false);
			cell.transform.GetChild(4).gameObject.SetActive(false);
			cell.transform.GetChild(5).gameObject.SetActive(true);
			cell.transform.GetChild(6).gameObject.SetActive(false);
			break;
		case 7:
			cell.transform.GetChild(0).gameObject.SetActive(false);
			cell.transform.GetChild(1).gameObject.SetActive(false);
			cell.transform.GetChild(2).gameObject.SetActive(false);
			cell.transform.GetChild(3).gameObject.SetActive(false);
			cell.transform.GetChild(4).gameObject.SetActive(false);
			cell.transform.GetChild(5).gameObject.SetActive(false);
			cell.transform.GetChild(6).gameObject.SetActive(true);
			break;
		default:
			cell.transform.GetChild(0).gameObject.SetActive(false);
			cell.transform.GetChild(1).gameObject.SetActive(false);
			cell.transform.GetChild(2).gameObject.SetActive(false);
			cell.transform.GetChild(3).gameObject.SetActive(false);
			cell.transform.GetChild(4).gameObject.SetActive(false);
			cell.transform.GetChild(5).gameObject.SetActive(false);
			cell.transform.GetChild(6).gameObject.SetActive(false);
			break;
		}
	}

	void StartWave(bool firstWave=false){
		controlEnabled = false;
		if (firstWave) {
			waveCubes = Random.Range (2, 4);   // waveCubes assigned from 2 or 3 randomly
		} 
		else {
			waveCubes = nextWaveCubes;
		}

		switch(waveCubes){
		case 2:                      // Two cubes in new wave 
			activeCell1 = new Vector2 (mapSize.y - 1, 3);
			activeCell2 = new Vector2 (mapSize.y - 1, activeCell1.y + 1);
			int i1 = (int)activeCell1.x, j1 = (int)activeCell1.y, i2 = (int)activeCell2.x, j2 = (int)activeCell2.y;
			if (firstWave) {
				cellData [i1, j1] = Random.Range (1, colours + 1);   // Generate Random from 1 to colours      (1)
				do {
					cellData [i2, j2] = Random.Range (1, colours + 1);   // Generate Random from 1 to colours   (diffetent from above generation)    (2)
				} while(cellData [i2, j2] == cellData [i1, j1]);
				SetNewWave ();
			} else {
				cellData [i1, j1] = nextCellData1;   
				cellData [i2, j2] = nextCellData2;
				SetNewWave ();
			}  
			break;

		case 3:         //Three Cubes in new wave
			activeCell1 = new Vector2 (mapSize.y - 1, 3);
			activeCell2 = new Vector2 (mapSize.y - 1, activeCell1.y + 1);
			activeCell3 = new Vector2 (mapSize.y - 1, activeCell1.y - 1);
			i1 = (int)activeCell1.x;
			j1 = (int)activeCell1.y;
			i2 = (int)activeCell2.x; 
			j2 = (int)activeCell2.y;
			int i3=(int)activeCell3.x,j3=(int)activeCell3.y ;
			if (firstWave) {
				cellData[i1,j1]=Random.Range (1, colours+1);   // Generate Random from 1 to colours      (1)
				do{
					cellData[i2,j2]=Random.Range (1, colours+1);   // Generate Random from 1 to colours   (diffetent from above generation)    (2)
				} while(cellData[i2,j2]==cellData[i1,j1]);
				do{
					cellData[i3,j3]=Random.Range (1, colours+1);   // Generate Random from 1 to colours   (diffetent from above generation)    (3)
				} while(cellData[i3,j3]==cellData[i1,j1]);
				SetNewWave();
			} 
			else {
				cellData [i1, j1] = nextCellData1;   
				cellData [i2, j2] = nextCellData2;
				cellData [i3, j3] = nextCellData3;
				SetNewWave();
			}  
			break;
		}
		configration=1;
		Invoke("FallDown",fallTimeDelay);
	}

	void SetNewWave(){
		int currentCell1Data = cellData [(int)activeCell1.x, (int)activeCell1.y];
		nextTile3.gameObject.SetActive (true);
		nextWaveCubes=Random.Range(2,4);
		switch (nextWaveCubes) {
		case 2:
			nextCellData1 = Random.Range (1, colours + 1);
			CellDataDisplayer (nextTile1, nextCellData1);
			do {
				nextCellData2 = Random.Range (1, colours + 1);
			} while((nextCellData2 == nextCellData1)&&(nextCellData2==currentCell1Data));
			CellDataDisplayer (nextTile2, nextCellData2);
			nextTile3.gameObject.SetActive (false);
			break;
		case 3:
			nextCellData1 = Random.Range (1, colours + 1);
			CellDataDisplayer (nextTile1, nextCellData1);
			do {
				nextCellData2 = Random.Range (1, colours + 1);
			} while((nextCellData2 == nextCellData1)&&(nextCellData2==currentCell1Data));
			CellDataDisplayer (nextTile2, nextCellData2);
			do {
				nextCellData3 = Random.Range (1, colours + 1);
			} while(nextCellData3 == nextCellData1);
			CellDataDisplayer (nextTile3, nextCellData3);
			break;
		}
	}


	void FallDown(){
		switch (waveCubes) {
		case 2:         // For 2 Cubes in a wave
			int i1 = (int)activeCell1.x, j1 = (int)activeCell1.y, i2 = (int)activeCell2.x, j2 = (int)activeCell2.y;
			switch (configration) {
			case 1:
				if ((i1 > 0) && (cellData [i1 - 1, j1] == 0) && (cellData [i2 - 1, j2] == 0)) {     
					cellData [i1 - 1, j1] = cellData [i1, j1];
					cellData [i2 - 1, j2] = cellData [i2, j2];
					cellData [i1, j1] = 0;
					cellData [i2, j2] = 0;
					activeCell1.x = i1 - 1;
					activeCell2.x = i2 - 1;
					controlEnabled = true;
					Invoke ("FallDown", fallTimeDelay);
				} else {
					controlEnabled = false;
					if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
						aud.clip = stompAudio;
						aud.pitch = 0.5f;
						aud.Play ();
						aud.pitch = 1;
					}
					Check (activeCell1, true);
				}
				break;
			case 2:
				if ((i2 > 0) && (cellData [i2 - 1, j2] == 0)) {
					cellData [i2 - 1, j2] = cellData [i2, j2];
					cellData [i1 - 1, j1] = cellData [i1, j1];
					cellData [i1, j1] = 0;
					activeCell1.x = i1 - 1;
					activeCell2.x = i2 - 1;
					controlEnabled = true;
					Invoke ("FallDown", fallTimeDelay);
				} else {
					controlEnabled = false;
					if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
						aud.clip = stompAudio;
						aud.pitch = 0.5f;
						aud.Play ();
						aud.pitch = 1;
					}
					Check (activeCell1, true);
				}
				break;
			case 3:
				if ((i1 > 0) && (cellData [i1 - 1, j1] == 0) && (cellData [i2 - 1, j2] == 0)) {     
					cellData [i1 - 1, j1] = cellData [i1, j1];
					cellData [i2 - 1, j2] = cellData [i2, j2];
					cellData [i1, j1] = 0;
					cellData [i2, j2] = 0;
					activeCell1.x = i1 - 1;
					activeCell2.x = i2 - 1;
					controlEnabled = true;
					Invoke ("FallDown", fallTimeDelay);
				} else {
					controlEnabled = false;
					if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
						aud.clip = stompAudio;
						aud.pitch = 0.5f;
						aud.Play ();
						aud.pitch = 1;
					}
					Check (activeCell1, true);
				}
				break;
			case 4:
				if ((i1 > 0) && (cellData [i1 - 1, j1] == 0)) {     
					cellData [i1 - 1, j1] = cellData [i1, j1];
					cellData [i2 - 1, j2] = cellData [i2, j2];
					cellData [i2, j2] = 0;
					activeCell1.x = i1 - 1;
					activeCell2.x = i2 - 1;
					controlEnabled = true;
					Invoke ("FallDown", fallTimeDelay);
				} else {
					controlEnabled = false;
					if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
						aud.clip = stompAudio;
						aud.pitch = 0.5f;
						aud.Play ();
						aud.pitch = 1;
					}
					Check (activeCell1, true);
				}
				break;
			default:
				break;
			}
			break;

		case 3:               // For 3 Cubes in a wave
			i1 = (int)activeCell1.x;
			j1 = (int)activeCell1.y; 
			i2 = (int)activeCell2.x; 
			j2 = (int)activeCell2.y;
			int i3=(int)activeCell3.x,j3=(int)activeCell3.y ;
			switch (configration) {
			case 1:
				if ((i1 > 0) && (cellData [i1 - 1, j1] == 0) && (cellData [i2 - 1, j2] == 0) && (cellData [i3 - 1, j3] == 0)) {     
					cellData [i1 - 1, j1] = cellData [i1, j1];
					cellData [i2 - 1, j2] = cellData [i2, j2];
					cellData [i3 - 1, j3] = cellData [i3, j3];
					cellData [i1, j1] = 0;
					cellData [i2, j2] = 0;
					cellData [i3, j3] = 0;
					activeCell1.x = i1 - 1;
					activeCell2.x = i2 - 1;
					activeCell3.x = i3 - 1;
					controlEnabled = true;
					Invoke ("FallDown", fallTimeDelay);
				} else {
					controlEnabled = false;
					if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
						aud.clip = stompAudio;
						aud.pitch = 0.5f;
						aud.Play ();
						aud.pitch = 1;
					}
					Check (activeCell1, true);
				}
				break;
			case 2:
				if ((i2 > 0) && (cellData [i2 - 1, j2] == 0)) {
					cellData [i2 - 1, j2] = cellData [i2, j2];
					cellData [i1 - 1, j1] = cellData [i1, j1];
					cellData [i3 - 1, j3] = cellData [i3, j3];
					cellData [i3, j3] = 0;
					activeCell1.x = i1 - 1;
					activeCell2.x = i2 - 1;
					activeCell3.x = i3 - 1;
					controlEnabled = true;
					Invoke ("FallDown", fallTimeDelay);
				} else {
					controlEnabled = false;
					if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
						aud.clip = stompAudio;
						aud.pitch = 0.5f;
						aud.Play ();
						aud.pitch = 1;
					}
					Check (activeCell1, true);
				}
				break;
			case 3:
				if ((i1 > 0) && (cellData [i1 - 1, j1] == 0) && (cellData [i2 - 1, j2] == 0) && (cellData [i3 - 1, j3] == 0)) {     
					cellData [i1 - 1, j1] = cellData [i1, j1];
					cellData [i2 - 1, j2] = cellData [i2, j2];
					cellData [i3 - 1, j3] = cellData [i3, j3];
					cellData [i1, j1] = 0;
					cellData [i2, j2] = 0;
					cellData [i3, j3] = 0;
					activeCell1.x = i1 - 1;
					activeCell2.x = i2 - 1;
					activeCell3.x = i3 - 1;
					controlEnabled=true;
					Invoke ("FallDown", fallTimeDelay);
				} else {
					controlEnabled = false;
					if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
						aud.clip = stompAudio;
						aud.pitch = 0.5f;
						aud.Play ();
						aud.pitch = 1;
					}
					Check (activeCell1, true);
				}
				break;
			case 4:
				if ((i3 > 0) && (cellData [i3 - 1, j3] == 0)) {     
					cellData [i3 - 1, j3] = cellData [i3, j3];
					cellData [i1 - 1, j1] = cellData [i1, j1];
					cellData [i2 - 1, j2] = cellData [i2, j2];
					cellData [i2, j2] = 0;
					activeCell1.x = i1 - 1;
					activeCell2.x = i2 - 1;
					activeCell3.x = i3 - 1;
					controlEnabled=true;
					Invoke ("FallDown", fallTimeDelay);
				} else {
					controlEnabled = false;
					if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
						aud.clip = stompAudio;
						aud.pitch = 0.5f;
						aud.Play ();
						aud.pitch = 1;
					}
					Check (activeCell1, true);
				}
				break;
			default:
				break;
			}
			break;
		}
	}


	void Check(Vector2 checkingCell,bool primaryCheck=false){
		if ((primaryCheck) && (checkingCell.x == mapSize.y - 1) && (!gameOver)) {    // if game over
			gameOver = true;
			if (PlayerPrefs.GetInt ("VibrationEnabled") == 1) {
				Handheld.Vibrate ();
			}
			if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
				aud.pitch = 1.7f;
				aud.clip = gameOverAudio;
				aud.Play ();
			}
			nextTile1.SetActive(false);
			nextTile2.SetActive (false);
			levelControllerScript.GameOver (score);
		} 
		else {            // if not game over
			adjSameDataCells.Clear ();
			adjSameDataCells.Add (checkingCell);
			int[,] tempCellData = new int[20, 20];      // A copy of cell data array
			int checkData;                           // Data contained in checking cell
			System.Array.Copy (cellData, tempCellData, cellData.Length);
			checkData = tempCellData [(int)(checkingCell.x), (int)(checkingCell.y)];
			for (int ckcl = 0; ckcl < adjSameDataCells.Count; ckcl++) {   // Loop from 0 to last indice of adjecent same data indices list 

				checkingCell = adjSameDataCells [ckcl];    // Assign Checking cell 

				if (tempCellData [(int)(checkingCell.x), (int)(checkingCell.y)] == checkData) {
					// Checking
					if (((checkingCell.x + 1) < mapSize.y) && (checkData == tempCellData [(int)(checkingCell.x + 1), (int)(checkingCell.y)])) {   //Check Up
						Vector2 addIndice = new Vector2 ((checkingCell.x + 1), checkingCell.y);
						adjSameDataCells.Add (addIndice);
					}
					if (((checkingCell.x) > 0) && (checkData == tempCellData [(int)(checkingCell.x - 1), (int)(checkingCell.y)])) {   //Check Down
						Vector2 addIndice = new Vector2 ((checkingCell.x - 1), checkingCell.y);
						adjSameDataCells.Add (addIndice);
					}
					if (((checkingCell.y + 1) < mapSize.x) && (checkData == tempCellData [(int)(checkingCell.x), (int)(checkingCell.y + 1)])) {   //Check Right
						Vector2 addIndice = new Vector2 (checkingCell.x, (checkingCell.y + 1));
						adjSameDataCells.Add (addIndice);
					}
					if (((checkingCell.y) > 0) && (checkData == tempCellData [(int)(checkingCell.x), (int)(checkingCell.y - 1)])) {   //Check Left
						Vector2 addIndice = new Vector2 (checkingCell.x, (checkingCell.y - 1));
						adjSameDataCells.Add (addIndice);
					}
					// Checking over
					tempCellData [(int)(checkingCell.x), (int)(checkingCell.y)] = -1;    //Assign data to checked indice =-1 (Prevent indice being added twice)
				}
			}
			adjSameDataCells = adjSameDataCells.Distinct ().ToList ();   // make elements in the list distinct 

			if (adjSameDataCells.Count >= adjCellThreshold) {           // Destroy only if no of adj faces <= adjCellThreshold ( default to 4)
				if (primaryCheck) {
					destroy = true;
					StartCoroutine (DestroyerUpdate ());   
				} 
				else {
					destroy = true;
				}
			} else {
				if (primaryCheck) {
					destroy = false;
					StartCoroutine (DestroyerUpdate ());
				}
			}
		}
	} 


	bool destroy ;
	[HideInInspector]
	public List<GameObject> createdExplosions;
	IEnumerator DestroyerUpdate(){   
		function_start:
		if (destroy) {
			if (PlayerPrefs.GetInt ("VibrationEnabled") == 1) {
				Handheld.Vibrate ();
			}
			for (int ind = 0; ind < adjSameDataCells.Count; ind++) {
				GameObject newExplosion = Instantiate (explosions [cellData [(int)adjSameDataCells [ind].x, (int)adjSameDataCells [ind].y]-1], cell [((int)adjSameDataCells [ind].x) * 7 + (int)adjSameDataCells [ind].y].transform.position, Quaternion.identity) as GameObject;
				createdExplosions.Add (newExplosion);
				cellData [(int)adjSameDataCells [ind].x, (int)adjSameDataCells [ind].y] = 0;
				yield return new WaitForSeconds(0.065f);
			}
			if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
				aud.clip = cubeDestroyAudio;
				aud.Play ();
			}
			for (int ex = 0; ex < createdExplosions.Count; ex++) {
				Destroy (createdExplosions [ex]);
			}
			createdExplosions.Clear ();
			int addScore = baseScore + ((adjSameDataCells.Count () - 4) * scoreMultiplier * baseScore);  // Bonus granted = ((count-4)*multiplier) *
			score = score + addScore;
			scoreTxtAnimator.SetTrigger ("Inc");
			GameObject popUpCanv = Instantiate (popUpTxtCanvas, transform.position, Quaternion.identity) as GameObject;
			PopUpTxtScript popUpScript = popUpCanv.GetComponent<PopUpTxtScript> ();
			popUpScript.addScore = addScore;
			scoreText.text = score.ToString ();
		}
		destroy = false;

		for (int m = 0; m < mapSize.y; m++) {                              
			for (int n = 0; n < mapSize.x; n++) {
				if (cellData [m,n] != 0) {
					int i = m, j = n; 
					while ((i > 0) && (cellData [i-1,j] == 0)) {
						cellData [i-1,j] = cellData [i,j];
						cellData [i, j] = 0;
						i = i - 1; 
						yield return new WaitForSeconds(0.05f);
					}
					Vector2 movedIndex = new Vector2 (i, j);
					Check (movedIndex,false);
					if (destroy) {
						goto function_start;
					}
				}
			}
		}
		StartWave ();  
	}


	public void GenerateMap(){
		int cellCount = 0;
		cellDataDisplay.Clear ();
		GameObject newTile;
		string holderName = "Generated Map";
		Transform mapHolder = new GameObject (holderName).transform;
		mapHolder.parent = transform;
		for (int y = 0; y < mapSize.y; y++) {
			for (int x = 0; x < mapSize.x; x++) {
				Vector3 tilePosition = new Vector3 (-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y/2 + 0.5f + y);
				newTile = (Instantiate (tile, tilePosition, Quaternion.Euler (Vector3.right * 90))as Transform).gameObject;
				newTile.transform.localScale = Vector3.one * (1 - outlinePercent);
				newTile.transform.parent = mapHolder; 
				cell.Add (newTile);
				Renderer tempRend = cell[cellCount].gameObject.GetComponent<Renderer>();
				rend.Add (tempRend);
				cellCount++;
				cellDataDisplay.Add (0);
				cellData [y, x] = 0;
			}
		}
		StartWave (true);
	}

	public void HorizontalMoveBtn(int dir){            // left:-1 ;; right:+1
		if (controlEnabled) {
			switch (waveCubes) {
			case 2:       // For 2 Cubes in a wave
				int i1 = (int)activeCell1.x, j1 = (int)activeCell1.y, i2 = (int)activeCell2.x, j2 = (int)activeCell2.y;
				switch (configration) {
				case 1:   
					if (dir == 1) {           // request right movement
						if ((activeCell2.y + 1 <= mapSize.x - 1) && (cellData [i2, j2 + 1] == 0)) {
							cellData [i2, j2 + 1] = cellData [i2, j2];
							cellData [i1, j1 + 1] = cellData [i1, j1];
							cellData [i1, j1] = 0;
							activeCell1.y = activeCell1.y + 1;
							activeCell2.y = activeCell2.y + 1;
						}
					} else {                                 // request left movement
						if ((activeCell1.y - 1 >= 0) && (cellData [i1, j1 - 1] == 0)) {
							cellData [i1, j1 - 1] = cellData [i1, j1];
							cellData [i2, j2 - 1] = cellData [i2, j2];
							cellData [i2, j2] = 0;
							activeCell1.y = activeCell1.y - 1;
							activeCell2.y = activeCell2.y - 1;
						}
					}
					break;
				case 2:
					if (dir == 1) {           // request right movement
						if ((activeCell2.y + 1 <= mapSize.x - 1) && (cellData [i2, j2 + 1] == 0) && (cellData [i1, j1 + 1] == 0)) {
							cellData [i2, j2 + 1] = cellData [i2, j2];
							cellData [i1, j1 + 1] = cellData [i1, j1];
							cellData [i1, j1] = 0;
							cellData [i2, j2] = 0;
							activeCell1.y = activeCell1.y + 1;
							activeCell2.y = activeCell2.y + 1;
						}
					} else {                                 // request left movement
						if ((activeCell1.y - 1 >= 0) && (cellData [i1, j1 - 1] == 0) && (cellData [i2, j2 - 1] == 0)) {
							cellData [i1, j1 - 1] = cellData [i1, j1];
							cellData [i2, j2 - 1] = cellData [i2, j2];
							cellData [i2, j2] = 0;
							cellData [i1, j1] = 0;
							activeCell1.y = activeCell1.y - 1;
							activeCell2.y = activeCell2.y - 1;
						}
					}
					break;
				case 3:
					if (dir == 1) {           // request right movement
						if ((activeCell1.y + 1 <= mapSize.x - 1) && (cellData [i1, j1 + 1] == 0)) {
							cellData [i1, j1 + 1] = cellData [i1, j1];
							cellData [i2, j2 + 1] = cellData [i2, j2];
							cellData [i2, j2] = 0;
							activeCell1.y = activeCell1.y + 1;
							activeCell2.y = activeCell2.y + 1;
						}
					} else {                                 // request left movement
						if ((activeCell2.y - 1 >= 0) && (cellData [i2, j2 - 1] == 0)) {
							cellData [i2, j2 - 1] = cellData [i2, j2];
							cellData [i1, j1 - 1] = cellData [i1, j1];
							cellData [i1, j1] = 0;
							activeCell1.y = activeCell1.y - 1;
							activeCell2.y = activeCell2.y - 1;
						}
					}
					break;
				case 4:
					if (dir == 1) {           // request right movement
						if ((activeCell2.y + 1 <= mapSize.x - 1) && (cellData [i2, j2 + 1] == 0) && (cellData [i1, j1 + 1] == 0)) {
							cellData [i2, j2 + 1] = cellData [i2, j2];
							cellData [i1, j1 + 1] = cellData [i1, j1];
							cellData [i1, j1] = 0;
							cellData [i2, j2] = 0;
							activeCell1.y = activeCell1.y + 1;
							activeCell2.y = activeCell2.y + 1;
						}
					} else {                                 // request left movement
						if ((activeCell1.y - 1 >= 0) && (cellData [i1, j1 - 1] == 0) && (cellData [i2, j2 - 1] == 0)) {
							cellData [i1, j1 - 1] = cellData [i1, j1];
							cellData [i2, j2 - 1] = cellData [i2, j2];
							cellData [i2, j2] = 0;
							cellData [i1, j1] = 0;
							activeCell1.y = activeCell1.y - 1;
							activeCell2.y = activeCell2.y - 1;
						}
					}
					break;
				}
				break;

			case 3:       // For 3 Cubes in a wave
				i1 = (int)activeCell1.x; j1 = (int)activeCell1.y; i2 = (int)activeCell2.x; j2 = (int)activeCell2.y;
				int i3=(int)activeCell3.x,j3=(int)activeCell3.y ;
				switch (configration) {
				case 1:
					if (dir == 1) {           // request right movement
						if ((activeCell2.y + 1 <= mapSize.x - 1) && (cellData [i2, j2 + 1] == 0)) {
							cellData [i2, j2 + 1] = cellData [i2, j2];
							cellData [i1, j1 + 1] = cellData [i1, j1];
							cellData [i3, j3 + 1] = cellData [i3, j3];
							cellData [i3, j3] = 0;
							activeCell1.y = activeCell1.y + 1;
							activeCell2.y = activeCell2.y + 1;
							activeCell3.y = activeCell3.y + 1;
						}
					} else {                                 // request left movement
						if ((activeCell3.y - 1 >= 0) && (cellData [i3, j3 - 1] == 0)) {
							cellData [i3, j3 - 1] = cellData [i3, j3];
							cellData [i1, j1 - 1] = cellData [i1, j1];
							cellData [i2, j2 - 1] = cellData [i2, j2];
							cellData [i2, j2] = 0;
							activeCell1.y = activeCell1.y - 1;
							activeCell2.y = activeCell2.y - 1;
							activeCell3.y = activeCell3.y - 1;
						}
					}
					break;
				case 2:
					if (dir == 1) {           // request right movement
						if ((activeCell2.y + 1 <= mapSize.x - 1) && (cellData [i2, j2 + 1] == 0) && (cellData [i1, j1 + 1] == 0) && (cellData [i3, j3 + 1] == 0)) {
							cellData [i2, j2 + 1] = cellData [i2, j2];
							cellData [i1, j1 + 1] = cellData [i1, j1];
							cellData [i3, j3 + 1] = cellData [i3, j3];
							cellData [i1, j1] = 0;
							cellData [i2, j2] = 0;
							cellData [i3, j3] = 0;
							activeCell1.y = activeCell1.y + 1;
							activeCell2.y = activeCell2.y + 1;
							activeCell3.y = activeCell3.y + 1;
						}
					} else {                                 // request left movement
						if ((activeCell1.y - 1 >= 0) && (cellData [i1, j1 - 1] == 0) && (cellData [i2, j2 - 1] == 0) && (cellData [i3, j3 - 1] == 0)) {
							cellData [i1, j1 - 1] = cellData [i1, j1];
							cellData [i2, j2 - 1] = cellData [i2, j2];
							cellData [i3, j3 - 1] = cellData [i3, j3];
							cellData [i2, j2] = 0;
							cellData [i1, j1] = 0;
							cellData [i3, j3] = 0;
							activeCell1.y = activeCell1.y - 1;
							activeCell2.y = activeCell2.y - 1;
							activeCell3.y = activeCell3.y - 1;
						}
					}
					break;
				case 3:
					if (dir == 1) {           // request right movement
						if ((activeCell3.y + 1 <= mapSize.x - 1) && (cellData [i3, j3 + 1] == 0)) {
							cellData [i3, j3 + 1] = cellData [i3, j3];
							cellData [i1, j1 + 1] = cellData [i1, j1];
							cellData [i2, j2 + 1] = cellData [i2, j2];
							cellData [i2, j2] = 0;
							activeCell1.y = activeCell1.y + 1;
							activeCell2.y = activeCell2.y + 1;
							activeCell3.y = activeCell3.y + 1;
						}
					} else {                                 // request left movement
						if ((activeCell2.y - 1 >= 0) && (cellData [i2, j2 - 1] == 0)) {
							cellData [i2, j2 - 1] = cellData [i2, j2];
							cellData [i1, j1 - 1] = cellData [i1, j1];
							cellData [i3, j3 - 1] = cellData [i3, j3];
							cellData [i3, j3] = 0;
							activeCell1.y = activeCell1.y - 1;
							activeCell2.y = activeCell2.y - 1;
							activeCell3.y = activeCell3.y - 1;
						}
					}
					break;
				case 4:
					if (dir == 1) {           // request right movement
						if ((activeCell2.y + 1 <= mapSize.x - 1) && (cellData [i2, j2 + 1] == 0) && (cellData [i1, j1 + 1] == 0) && (cellData [i3, j3 + 1] == 0)) {
							cellData [i2, j2 + 1] = cellData [i2, j2];
							cellData [i1, j1 + 1] = cellData [i1, j1];
							cellData [i3, j3 + 1] = cellData [i3, j3];
							cellData [i1, j1] = 0;
							cellData [i2, j2] = 0;
							cellData [i3, j3] = 0;
							activeCell1.y = activeCell1.y + 1;
							activeCell2.y = activeCell2.y + 1;
							activeCell3.y = activeCell3.y + 1;
						}
					} else {                                 // request left movement
						if ((activeCell1.y - 1 >= 0) && (cellData [i1, j1 - 1] == 0) && (cellData [i2, j2 - 1] == 0) && (cellData [i3, j3 - 1] == 0)) {
							cellData [i1, j1 - 1] = cellData [i1, j1];
							cellData [i2, j2 - 1] = cellData [i2, j2];
							cellData [i3, j3 - 1] = cellData [i3, j3];
							cellData [i2, j2] = 0;
							cellData [i1, j1] = 0;
							cellData [i3, j3] = 0;
							activeCell1.y = activeCell1.y - 1;
							activeCell2.y = activeCell2.y - 1;
							activeCell3.y = activeCell3.y - 1;
						}
					}
					break;
				}
				break;
			}
		}
	}

	public void DownMoveFaster(){
		if (controlEnabled) {
			fallTimeDelay = copy_FallTimeOffset / 4;
		}
	}
	public void DownMovementNormal(){
		fallTimeDelay = copy_FallTimeOffset;
	} 

	public void Rotator(){      // Clockwise rotation with pivot set as active cell 1
		if (controlEnabled) {
			switch (waveCubes) {
			case 2:        // For 2 Cubes in a wave
				int i2 = (int)activeCell2.x, j2 = (int)activeCell2.y;
				switch (configration) {
				case 1:
					if ((i2 - 1 >= 0) && (cellData [i2 - 1, j2 - 1] == 0)) {
						if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
							levelControllerScript.aud.Play ();
						}
						cellData [i2 - 1, j2 - 1] = cellData [i2, j2];
						cellData [i2, j2] = 0;
						activeCell2.x = activeCell2.x - 1;
						activeCell2.y = activeCell2.y - 1;
						configration = 2;
					}
					break;
				case 2:
					if ((j2 - 1 >= 0) && (cellData [i2 + 1, j2 - 1] == 0)) {
						if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
							levelControllerScript.aud.Play ();
						}
						cellData [i2 + 1, j2 - 1] = cellData [i2, j2];
						cellData [i2, j2] = 0;
						activeCell2.x = activeCell2.x + 1;
						activeCell2.y = activeCell2.y - 1;
						configration = 3;
					}
					break;
				case 3:
					if (cellData [i2 + 1, j2 + 1] == 0) {
						if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
							levelControllerScript.aud.Play ();
						}
						cellData [i2 + 1, j2 + 1] = cellData [i2, j2];
						cellData [i2, j2] = 0;
						activeCell2.x = activeCell2.x + 1;
						activeCell2.y = activeCell2.y + 1;
						configration = 4;
					}
					break;
				case 4:
					if ((j2 + 1 <= mapSize.x - 1) && (cellData [i2 - 1, j2 + 1] == 0)) {
						if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
							levelControllerScript.aud.Play ();
						}
						cellData [i2 - 1, j2 + 1] = cellData [i2, j2];
						cellData [i2, j2] = 0;
						activeCell2.x = activeCell2.x - 1;
						activeCell2.y = activeCell2.y + 1;
						configration = 1;
					}
					break;
				default:
					break;	
				}
				break;

			case 3:       // For 3 Cubes in a wave
				i2 = (int)activeCell2.x; j2 = (int)activeCell2.y;
				int i3=(int)activeCell3.x,j3=(int)activeCell3.y ;
				switch (configration) {
				case 1:
					if ((i2 - 1 >= 0) && (cellData [i2 - 1, j2 - 1] == 0)&&(i3+1<=mapSize.y-1)) {
						if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
							levelControllerScript.aud.Play ();
						}
						cellData [i2 - 1, j2 - 1] = cellData [i2, j2];
						cellData [i2, j2] = 0;
						cellData [i3 + 1, j3 + 1] = cellData [i3, j3];
						cellData [i3, j3] = 0;
						activeCell2.x = activeCell2.x - 1;
						activeCell2.y = activeCell2.y - 1;
						activeCell3.x = activeCell3.x + 1;
						activeCell3.y = activeCell3.y + 1;
						configration = 2;
					}
					break;
				case 2:
					if ((j2 - 1 >= 0) && (cellData [i2 + 1, j2 - 1] == 0) && (j3+1<=mapSize.x-1) && (cellData[i3-1,j3+1]==0) ) {
						if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
							levelControllerScript.aud.Play ();
						}
						cellData [i2 + 1, j2 - 1] = cellData [i2, j2];
						cellData [i2, j2] = 0;
						cellData [i3 - 1, j3 + 1] = cellData [i3, j3];
						cellData [i3, j3] = 0;
						activeCell2.x = activeCell2.x + 1;
						activeCell2.y = activeCell2.y - 1;
						activeCell3.x = activeCell3.x - 1;
						activeCell3.y = activeCell3.y + 1;
						configration = 3;
					}
					break;
				case 3:
					if ((i3 - 1 >= 0) && (cellData [i3 - 1, j3 - 1] == 0)&&(i2+1<=mapSize.y-1)) {
						if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
							levelControllerScript.aud.Play ();
						}
						cellData [i2 + 1, j2 + 1] = cellData [i2, j2];
						cellData [i2, j2] = 0;
						cellData [i3 - 1, j3 - 1] = cellData [i3, j3];
						cellData [i3, j3] = 0;
						activeCell2.x = activeCell2.x + 1;
						activeCell2.y = activeCell2.y + 1;
						activeCell3.x = activeCell3.x - 1;
						activeCell3.y = activeCell3.y - 1;
						configration = 4;
					}
					break;
				case 4:
					if ((j2 + 1 <= mapSize.x - 1) && (cellData [i2 - 1, j2 + 1] == 0) && (j3 - 1 >= 0) && (cellData [i3 + 1, j3 - 1] == 0)) {
						if (PlayerPrefs.GetInt ("SoundEnabled") == 1) {
							levelControllerScript.aud.Play ();
						}
						cellData [i2 - 1, j2 + 1] = cellData [i2, j2];
						cellData [i2, j2] = 0;
						cellData [i3 + 1, j3 - 1] = cellData [i3, j3];
						cellData [i3, j3] = 0;
						activeCell2.x = activeCell2.x - 1;
						activeCell2.y = activeCell2.y + 1;
						activeCell3.x = activeCell3.x + 1;
						activeCell3.y = activeCell3.y - 1;
						configration = 1;
					}
					break;
				default:
					break;	
				}
				break;
			}
		}
	}
}


