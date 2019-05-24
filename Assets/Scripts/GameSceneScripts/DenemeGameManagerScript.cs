using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static System.Math;
using UnityEngine.UI;

public class DenemeGameManagerScript : MonoBehaviour {

	public GameObject redBlock;
	public GameObject blueBlock;
	public GameObject dummyBlock;

	public Transform gameHelper;
	public Transform gameInitHelper;
	public Transform undoActionHelper;

	public Material[] materials;

	private int dimension;
	private int augmentedDimension;

	private int cubeHelper = 15;

	int stateCounter = 0;
	int clickCounter = 0;

	public int gameHelperHeightCounterNew = 0;
	public int gameHelperHeightCounterOld = 0;

	public List<GameObject> pendingCubes;
	public List<GameObject> cubes;
	public List<GameObject> undoCubes;

	public int[] initialCubes;
	public int[,] answerCubes;

	public int[,] pendingCubesMatrix;
	public int[,] cubesMatrix;
	public int[,] undoMatrix;

	private int xAxisLenght;
	private int yAxisLenght;

	public int currentScore = 0;
	public int bestScore = 0;

	public Text currentScoreText;
	public Text bestScoreText;

	public WebClientSide ws;

	private List<int> answers;

	// Use this for initialization
	void Start () {
		cubes = new List<GameObject>();
		pendingCubes = new List<GameObject> ();
		undoCubes = new List<GameObject> ();

		answers = new List<int>();

		stateCounter++;

		dimension = StaticValueScript.dimensionSize;
		augmentedDimension = Convert.ToInt32(Math.Pow(2,dimension));

		organizeInitialArrays();

		ws = GetComponent<WebClientSide>();
		ws.Connect();
	}

	void organizeInitialArrays(){
		initialCubes = new int[augmentedDimension];
		answerCubes = new int[augmentedDimension,augmentedDimension];

		if(dimension == 4){
			pendingCubesMatrix = new int[4,4];
			cubesMatrix = new int[4,4];
			undoMatrix = new int[4,4];

			xAxisLenght = 4;
			yAxisLenght = 4;
		}
		else if(dimension == 5){
			pendingCubesMatrix = new int[4,8];
			cubesMatrix = new int[4,8];
			undoMatrix = new int[4,8];

			xAxisLenght = 4;
			yAxisLenght = 8;

		}
		else if(dimension == 6){
			pendingCubesMatrix = new int[8,8];
			cubesMatrix = new int[8,8];
			undoMatrix = new int[8,8];

			xAxisLenght = 8;
			yAxisLenght = 8;
		}
	}

	bool checkIfArrayIsCreated(){
		int zeroCount1 = 0;
		int zeroCount2 = 0;

		for(int i = 0; i<augmentedDimension;i++){
			if(initialCubes[i] == 0){
				zeroCount1++;
			}
		}

		for(int i = 0; i<augmentedDimension;i++){
			for(int j = 0; j<augmentedDimension;j++){
				if(answerCubes[i,j] == 0){
					zeroCount2++;
				}
			}
		}

		if(zeroCount1 == augmentedDimension){
			return false;
		}else{
			if(zeroCount2 == augmentedDimension*augmentedDimension){
				return false;
			}
			else{
				return true;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(stateCounter==1 && checkIfArrayIsCreated()){
			Debug.Log("Başla");
			initializeCubesMatrix();
			setGameHelperIntialPosition ();
			stateCounter++;
			checkBestScore();
		}
		if (!gameIsFinished ()) {
			if (Input.GetKeyDown ("space")) {
				transformListToMatrix ();
				if (checkIfThereIsAnyWhiteCube ()) {
					changeGravity ();
					changeUndoMatrix();
					AddPendingBlockMatrixToMainMatrix ();
					setGameHelperIntialPosition ();
					checkBestScore();
					addToTheAnswer();
				} else {
					Debug.Log ("THERE IS WHITE CUBE");
				}
			}
			if(Input.GetMouseButtonDown(0)){
				changePendingCubes ();
			}
			if(Input.GetMouseButtonDown(1)){
				undo();
				checkBestScore();
			}
		}
	}

	void changePendingCubes(){
		if (clickCounter > augmentedDimension-1) {
			clickCounter = 0;
		}
		for (int i = 0; i < augmentedDimension; i++) {
			if (answerCubes [clickCounter, i] == 1) {
				Renderer rend = pendingCubes [i].GetComponent<Renderer> ();
				rend.sharedMaterial = materials [0];
				pendingCubes [i].tag = "BlueBlock";
				pendingCubes [i].name = "blue block";
			} else if (answerCubes [clickCounter, i] == -1) {
				Renderer rend = pendingCubes [i].GetComponent<Renderer> ();
				rend.sharedMaterial = materials [1];
				pendingCubes [i].tag = "RedBlock";
				pendingCubes [i].name = "red block";
			} else {
				Debug.Log ("Input Hatalı!");
			}
		}
		clickCounter++;
	}
		
	public void initializeCubesMatrix(){		
		int index = 0;
		for (int i = 0; i < xAxisLenght; i++) {
			for (int j = 0; j < yAxisLenght; j++,index++) {
				cubesMatrix [i,j] = initialCubes [index];
			}
		}
		createInitialCubes ();
	}

	public void createInitialCubes(){
		for (int i = 0; i < xAxisLenght; i++) {
			for (int j = 0;  j < yAxisLenght; j++) {
				if (cubesMatrix [i, j] > 0) {
					addBlock (blueBlock,cubesMatrix[i,j]);
				} else if (cubesMatrix [i, j] < 0) {
					addBlock (redBlock,Mathf.Abs(cubesMatrix [i, j]));
				}
				Vector3 vector1 = new Vector3 (-cubeHelper,0,0);
				gameInitHelper.position += vector1;
			}
			Vector3 vector2 = new Vector3 (cubeHelper*yAxisLenght,0,-cubeHelper);
			gameInitHelper.position += vector2;
		}
	}

	public void addBlock(GameObject blockPrefab,int number){
		for (int i = 0; i < number; i++) {
			Vector3 vector = new Vector3 (0,(cubeHelper),0);
			gameInitHelper.position += vector;
			GameObject block = Instantiate (blockPrefab,gameInitHelper.position,gameInitHelper.rotation);
		}
		Vector3 vector1 = new Vector3 (0,-((cubeHelper)*(number)),0);
		gameInitHelper.position += vector1;
	}

	public void setGameHelperIntialPosition(){
		if (gameHelperHeightCounterOld == 0) {
			for(int i = 0; i < xAxisLenght; i++){
				for (int j = 0; j < yAxisLenght; j++) {
					int matrixNumber = Mathf.Abs(cubesMatrix [i, j]);
					if(matrixNumber > gameHelperHeightCounterOld){
						gameHelperHeightCounterOld = matrixNumber;
					}
				}
			}
				
			Vector3 vector = new Vector3 (0,cubeHelper*(gameHelperHeightCounterOld+1),0);
			gameHelper.position = gameHelper.position + vector;
			createCubes ();

		}
		else{
			for (int i = 0; i < xAxisLenght; i++) {
				for (int j = 0; j < yAxisLenght; j++) {
					int matrixNumber = Mathf.Abs(cubesMatrix [i, j]);
					if (matrixNumber > gameHelperHeightCounterNew) {
						gameHelperHeightCounterNew = matrixNumber;
					}
				}
			}

			Vector3 vector = new Vector3 (0,cubeHelper*(gameHelperHeightCounterNew-gameHelperHeightCounterOld+1),0);
			gameHelper.position = gameHelper.position + vector;
			gameHelperHeightCounterOld = gameHelperHeightCounterNew;
			gameHelperHeightCounterNew = 0;
			StartCoroutine (Wait());
		}
	}



	public void createCubes(){
		for (int i = 0; i < xAxisLenght; i++) {
			for (int j = 0; j < yAxisLenght; j++) {
				GameObject block =  Instantiate (dummyBlock,gameHelper.position,gameHelper.rotation);
				pendingCubes.Add (block);
				Vector3 vector1 = new Vector3 (-cubeHelper,0,0);
				gameHelper.position = gameHelper.position + vector1;
			}
			Vector3 vector2 = new Vector3(cubeHelper*yAxisLenght,0,-cubeHelper);
			gameHelper.position = gameHelper.position + vector2;
		}
		Vector3 vector3 = new Vector3 (0,0,xAxisLenght*cubeHelper);
		gameHelper.position = gameHelper.position + vector3;
	}

	public List<GameObject> getCubeList(){
		return cubes;
	}


	IEnumerator Wait(){
		yield return new WaitForSeconds(2.5f);
		createCubes ();
		Vector3 vector1 = new Vector3 (0,cubeHelper*(-1),0);
		gameHelper.position = gameHelper.position + vector1;
	}

	IEnumerator WaitForCreation(){
		yield return new WaitForSeconds(2.5f);
	}



	public void transformListToMatrix(){
		int x = 0;
		for (int i = 0; i < xAxisLenght; i++) {
			for (int j = 0; j < yAxisLenght; j++) {
				if (pendingCubes [x].tag.Equals ("BlueBlock")) {
					pendingCubesMatrix [i,j] = 1;
				} else if (pendingCubes [x].tag.Equals ("RedBlock")) {
					pendingCubesMatrix [i,j] = -1;
				} else if (pendingCubes [x].tag.Equals ("DummyBlock")) {
					pendingCubesMatrix [i,j] = 0;
				}
				x++;
			}
		}
	}

	bool checkIfThereIsAnyWhiteCube(){
		for(int i = 0; i < xAxisLenght; i++){
			for (int j = 0; j < yAxisLenght; j++) {
				if (pendingCubesMatrix [i, j] == 0) {
					return false;
				}
			}
		}
		return true;
	}

	public void changeGravity(){
		for (int i = 0; i < augmentedDimension; i++) {
			pendingCubes[i].GetComponent<Rigidbody>().useGravity = true;
		}
		pendingCubes.Clear();
	}

	public void AddPendingBlockMatrixToMainMatrix(){
		string matrix = "";
		for (int i = 0; i < xAxisLenght; i++) {
			for (int j = 0; j < yAxisLenght; j++) {
				cubesMatrix [i,j] = cubesMatrix [i,j] + pendingCubesMatrix [i,j];
				matrix = matrix +(cubesMatrix[i,j]) + ",";
				pendingCubesMatrix [i,j] = 0;
			}
			matrix = matrix + "\n";
		}
		//print(matrix);
	}

	bool gameIsFinished(){
		for (int i = 0; i < xAxisLenght; i++) {
			for (int j = 0; j < yAxisLenght; j++) {
				if (cubesMatrix [i,j] != 0) {
					return false;
				}
			}
		}
		return true;
	}

	void checkBestScore(){
		currentScore = 0;
		for (int i = 0; i < xAxisLenght; i++) {
			for (int j = 0; j < yAxisLenght; j++) {
				if( cubesMatrix [i,j] == 0 ){
					currentScore++;
				}
			}
			if(currentScore > bestScore){
				bestScore = currentScore;
			}
		}
		currentScoreText.text = "Current Score: " + currentScore;
		bestScoreText.text = "Best Score: " + bestScore;
	}

	void changeUndoMatrix(){
		string matrix = "";
		for (int i = 0; i < xAxisLenght; i++) {
			for (int j = 0; j < yAxisLenght; j++) {
				undoMatrix[i,j] = -(pendingCubesMatrix [i,j]);
				matrix = matrix + (undoMatrix[i,j]) + ",";
			}
			matrix = matrix + "\n";
		}
		//print(matrix);
	}

	public void undo(){
		Vector3 vector = new Vector3 (0,-(cubeHelper/2),0);
		undoActionHelper.position = gameHelper.position + vector;

		if(undoMatrix[0,0] != 0){
			for (int i = 0; i < xAxisLenght; i++) {
				for (int j = 0; j < yAxisLenght; j++) {
					if(undoMatrix[i,j] == 1){
						GameObject block =  Instantiate (blueBlock,undoActionHelper.position,undoActionHelper.rotation);
						undoCubes.Add (block);
						cubesMatrix[i,j] = cubesMatrix[i,j] + undoMatrix[i,j];
					}
					else{
						GameObject block =  Instantiate (redBlock,undoActionHelper.position,undoActionHelper.rotation);
						undoCubes.Add (block);
						cubesMatrix[i,j] = cubesMatrix[i,j] + undoMatrix[i,j];
					}
					undoMatrix[i,j] = 0;
					Vector3 vector1 = new Vector3 (-cubeHelper,0,0);
					undoActionHelper.position = undoActionHelper.position + vector1;
				}
				Vector3 vector2 = new Vector3(cubeHelper*yAxisLenght,0,-cubeHelper);
				undoActionHelper.position = undoActionHelper.position + vector2;
			}

			for (int i = 0; i < augmentedDimension; i++) {
				undoCubes[i].GetComponent<Rigidbody>().useGravity = true;
			}
			undoCubes.Clear();
			removeTheLastAnswer();
		}
		else{
			print("We dont do there");
		}
	}

	private void addToTheAnswer(){
		answers.Add(clickCounter);
		for(int i=0; i<answers.Count; i++){
			print(i+": "+answers[i]);
		}
	}

	private void removeTheLastAnswer(){
		answers.RemoveAt(answers.Count-1);
		for(int i=0; i<answers.Count; i++){
			print(i+": "+answers[i]);
		}
	}

}
