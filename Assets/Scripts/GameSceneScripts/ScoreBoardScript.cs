using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScoreBoardScript : MonoBehaviour
{
	public int currentScore = 0;
	public int bestScore = 0;
	DenemeGameManagerScript gameManager;

    // Start is called before the first frame update
    void Start()
    {
		gameManager = GetComponent<DenemeGameManagerScript>();
		currentScore = gameManager.currentScore;
		bestScore = gameManager.currentScore;
    }

    // Update is called once per frame
    void Update()
    {
         
    }

}
