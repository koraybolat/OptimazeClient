using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticValueScript : MonoBehaviour
{
	static public int dimensionSize = 4;
	static public int problemNumber = 101;
    

	// Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void setDimensionSize(int variable){
		dimensionSize = variable;
	}

	public void setProblemNumber(int problem){
		problemNumber = problem;
	}
}
