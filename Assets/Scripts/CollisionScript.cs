using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionScript : MonoBehaviour {

	void OnCollisionEnter(Collision collisionInfo){
		if (this.tag == "BlueBlock") {
			if (collisionInfo.collider.tag == "RedBlock") {
				Destroy (this);
				Destroy (collisionInfo.gameObject);
			}
		}
		if(this.tag == "RedBlock"){
			if (collisionInfo.collider.tag == "BlueBlock") {
				Destroy (this);
				Destroy (collisionInfo.gameObject);
			}
		}

	}
}
