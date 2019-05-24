using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour {

	private Transform centerPoint;
	public float xSpread;
	public float zSpread;
	public float yOffset;
	public GameObject gameManager;
	float timer = 0;
	private DenemeGameManagerScript gameManagerScript;

	public GameObject easyPlane;
	public GameObject normalPlane;
	public GameObject hardPlane;

	void Start(){
		gameManagerScript = gameManager.GetComponent<DenemeGameManagerScript> ();
		activateTerrain();
	}

	void activateTerrain(){
		if(StaticValueScript.dimensionSize == 4){
			Instantiate (easyPlane,new Vector3(0, 0, 0), Quaternion.identity);
			centerPoint = easyPlane.transform;
		}
		else if(StaticValueScript.dimensionSize == 5){
			Instantiate (normalPlane,new Vector3(-30, 0, 0), Quaternion.identity);
			centerPoint = normalPlane.transform;
		}
		else if(StaticValueScript.dimensionSize == 6){
			Instantiate (hardPlane,new Vector3(-30, 0, -30), Quaternion.identity);
			centerPoint = hardPlane.transform;
		}
	}

	// Update is called once per frame
	void Update () {
		if (gameManagerScript.gameHelperHeightCounterOld >= 6) {
			yOffset = gameManagerScript.gameHelperHeightCounterOld * 35;
		}

		Rotate();
		transform.LookAt (centerPoint);


	}

	void Rotate() {
		float rotSpeed = 1;

		if (Input.GetKey ("a")) {
			timer += rotSpeed / 20;
			float x = Mathf.Cos (timer) * xSpread;
			float z = Mathf.Sin (timer) * zSpread;
			Vector3 pos = new Vector3 (x, yOffset, z);
			transform.position = pos + centerPoint.position;

		} else if (Input.GetKey ("d")) {
			timer -= rotSpeed / 20;
			float x = Mathf.Cos (timer) * xSpread;
			float z = Mathf.Sin (timer) * zSpread;
			Vector3 pos = new Vector3 (x, yOffset, z);
			transform.position = pos + centerPoint.position;

		} else if (Input.GetKey ("w")) {
			if(zSpread>10){
				zSpread -= 5;
				xSpread = zSpread;
				//yOffset = (zSpread*2);
				float x = Mathf.Cos (timer) * xSpread;
				float z = Mathf.Sin (timer) * zSpread;
				Vector3 pos = new Vector3 (x, yOffset, z);
				transform.position = pos + centerPoint.position;
			}

		} else if (Input.GetKey ("s")) {
			if (zSpread < 500) {
				zSpread += 5;
				xSpread = zSpread;
				//yOffset = (zSpread*2);
				float x = Mathf.Cos (timer) * xSpread;
				float z = Mathf.Sin (timer) * zSpread;
				Vector3 pos = new Vector3 (x, yOffset, z);
				transform.position = pos + centerPoint.position;
			}
		}
 	}
}
