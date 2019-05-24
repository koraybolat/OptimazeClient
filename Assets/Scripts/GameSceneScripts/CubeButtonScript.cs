using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CubeButtonScript : MonoBehaviour {

	public GameObject blueCube;
	public GameObject redCube;
	public Material[] materials;

	private Renderer rend;

	void Start(){
		rend = this.GetComponent<Renderer>();
		rend.enabled = true;
	}

	// Update is called once per frame
	void Update () {

	}

	/*void OnMouseDown(){	
			if (this.tag.Equals ("DummyBlock")) {
				//this = (GameObject)PrefabUtility.ReplacePrefab (blueCube);
				//GameObject block = Instantiate (blueCube,this.transform.position,this.transform.rotation);
				//readjustCube (block);
				rend.sharedMaterial = materials[index];
				this.tag = "BlueBlock";
				this.name = "blue block";
			}
			else if (this.tag.Equals ("BlueBlock")) {
				//GameObject block = Instantiate (redCube,this.transform.position,this.transform.rotation);
				//readjustCube (block);
				index++;
				rend.sharedMaterial = materials[index];
				this.tag = "RedBlock";
				this.name = "red block";
			}
			else if (this.tag.Equals ("RedBlock")) {
				//GameObject block = Instantiate (blueCube,this.transform.position,this.transform.rotation);
				//readjustCube (block);
				index--;
				rend.sharedMaterial = materials[index];
				this.tag = "BlueBlock";
				this.name = "blue block";
			}
	}*/

	void readjustCube(GameObject block){
		block.GetComponent<Rigidbody>().useGravity = false;
		Destroy (this);
		Destroy (this.gameObject);
	}
}
