using UnityEngine;
using System.Collections;

public class coinController : MonoBehaviour {

	private Rigidbody rb;

	void Start ()
	{
		rb = GetComponent<Rigidbody> ();
	}

	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime);
		//GetComponent<Rigidbody>().rotation = Quaternion.identity;
	}
}
