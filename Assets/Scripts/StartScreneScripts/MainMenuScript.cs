using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.IO;

public class MainMenuScript : MonoBehaviour {

	public Text ProblemNumber;
	public InputField probInp;

	public void PlayEasyGame(){
		StaticValueScript staticValues = GetComponent<StaticValueScript>();
		staticValues.setDimensionSize(4);
	}

	public void PlayNormalGame(){
		StaticValueScript staticValues = GetComponent<StaticValueScript>();
		staticValues.setDimensionSize(5);
	}

	public void PlayHardGame(){
		StaticValueScript staticValues = GetComponent<StaticValueScript>();
		staticValues.setDimensionSize(6);
	}

	public void QuitGame(){
		Application.Quit ();
		Debug.Log ("QUIT!");
	}


	public void StartTheGame(){

		string tempProbText = ProblemNumber.text;

		try{
			if(tempProbText.Equals("")){
				StaticValueScript.problemNumber = 0;
			}
			else{
				StaticValueScript.problemNumber = Int32.Parse(tempProbText);
			}
			SceneManager.LoadScene("GameScene");

		}
		catch(Exception e){
			ProblemNumber.text = "";
			probInp.text = "";

			//EditorUtility.DisplayDialog("Unacceptable Number", "Please enter a valid number in order to proceed","Ok");
		}


	}
}
