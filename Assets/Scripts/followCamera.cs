using UnityEngine;
using System.Collections;

public class followCamera : MonoBehaviour {
	public GameObject target;
	
	void LateUpdate() {
		transform.LookAt(target.transform);
	}
}
