using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Diagnostics;

public class playerController : MonoBehaviour {
	
	public float speed;
	public Text scoreText;
	public Text scoreUpdateText;

	private AudioSource soundCoin;
	private AudioSource soundWall;
	private AudioSource soundBlocker;
	private float gameTimer;
	private float startTime;
	private int count;
	private bool boolScoreUpdateTextMoving;
	private Rigidbody rb;
	private bool boolStart;

	private Matrix4x4 calibrationMatrix;


	void Start ()
	{
		calibrateAccelerometer ();

		rb = GetComponent<Rigidbody>();
		count = 0;
		UpdateScore(0); 

		gameTimer = Time.unscaledTime;
		startTime = gameTimer;
		boolStart = true;
		//boolScoreUpdateTextMoving = false;

		//SET AUDIO SOURCES FOR GAME SOUNDFX
		AudioSource[] allMyAudioSources = GetComponents<AudioSource>();
		soundWall = allMyAudioSources[0];
		soundCoin = allMyAudioSources[1];
		soundBlocker = allMyAudioSources[2];
	}


	void StartupRoutine(){
		//CHECK FOR GAME START
		if (gameTimer.Equals (startTime)) {
			//IF JUST STARTED, THEN READY AND RESET TIMER
			scoreUpdateText.text = "READY!!";								
			gameTimer = Time.unscaledTime;
		}else if((Time.unscaledTime - gameTimer)  > 1){
			//IF TEXT IS CURRENTLY SET, THEN GO
			if(scoreUpdateText.text.Equals("READY!!"))
			{
				scoreUpdateText.text = "SET!!";
				gameTimer = Time.unscaledTime;
			}else if(scoreUpdateText.text.Equals("SET!!"))
			{
				scoreUpdateText.text = "GO!!";
				gameTimer = Time.unscaledTime;
			}else if(scoreUpdateText.text.Equals("GO!!"))
			{
				scoreUpdateText.text = "";
				boolStart = false;
			}
		}
	}

		
	void FixedUpdate(){
		if (boolStart.Equals (true)) {
			StartupRoutine ();
		} else {
			if(string.Compare(scoreUpdateText.text,"") != 0){

				if((Time.unscaledTime - gameTimer)  > 1){
					scoreUpdateText.text = "";
				}
			}
		}




		//IF GAME JUST STARTED, PREPARE FOR READY,SET,GO
		//if (boolStart.Equals (true)) {
		//	StartupRoutine ();
		//}

		//reposition scoreUpdateText
//		if (boolScoreUpdateTextMoving.Equals (true)) {
//			//if the scoretext is moving, then continue
//
//		}else{
//				//if not moving, then is it active?
//			if(scoreUpdateText.IsActive()){
//				//if active, then check timer
//				if (Time.unscaledTime - gameTimer > 3) {
//					//if greater than three seconds since activation, then start moving  x=30.17,y=-15.3,x=0
//					scoreUpdateText.rectTransform.anchoredPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
//				}
//			}else{
//			}
//		}
	

		if (SystemInfo.deviceType == DeviceType.Desktop) {
			float moveHorizontal = Input.GetAxis ("Horizontal");
			float moveVertical = Input.GetAxis ("Vertical");
			Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
			rb.AddForce (movement * speed * Time.deltaTime);
		} else {


			//float moveHorizontal = getAccelero
			
			Vector3 acceleration = Input.acceleration;
			Vector3 fixedAcceleration = getAccelerometer(acceleration);
			Vector3 movement = new Vector3 (fixedAcceleration.x, 0.0f, fixedAcceleration.y);
			rb.AddForce (movement * speed * Time.deltaTime);


			//float moveHorizontal = Input.acceleration.x;
			//float moveVertical = Input.acceleration.y;
			//Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
			//rb.AddForce (movement * speed * Time.deltaTime);
		}
	}

	//Whenever you need an accelerator value from the user
	//call this function to get the 'calibrated' value
	Vector3 getAccelerometer(Vector3 accelerator){
		Vector3 accel = this.calibrationMatrix.MultiplyVector(accelerator);
		return accel;
	}


	
	void OnCollisionEnter(Collision collision) 
	{

		if (collision.gameObject.CompareTag ("Wall")) {
			PlaySound (soundWall);
			UpdateScore(-5);
		} else if (collision.gameObject.CompareTag ("Blocker")) {
			PlaySound (soundBlocker);

			UpdateScore(-10);
		}
	}


	void OnTriggerEnter(Collider other) 
	{
		if (other.gameObject.CompareTag ("Coin")) {
			other.gameObject.SetActive (false);
			PlaySound(soundCoin);

			UpdateScore(+50);
		}
	}

	void PlaySound(AudioSource audioSource)
	{
		if (SystemInfo.deviceType == DeviceType.Desktop) {
			audioSource.Play();
		}else
		{
			audioSource.Play();
			//Handheld.Vibrate();
		}
	}

	void UpdateScore(int intValue)
	{
		scoreText.text = "";
		count = count + intValue;
		if (count > 0) {
			scoreText.text = "$ " + count.ToString ();
		} else {
			scoreText.text = "$ 0";
		}
		if (intValue > 0) {
			scoreUpdateText.text = "+" + intValue.ToString ();
		} else {
			scoreUpdateText.text = intValue.ToString ();
		}
		gameTimer = Time.unscaledTime;

		if (count > 0) {
			boolStart = false;
		}



		int intCoinsLeft = GameObject.FindGameObjectsWithTag ("Coin").Length;

		if(intCoinsLeft == 0)
		{
			scoreUpdateText.text = "LEVEL COMPLETE!!";
		}

	}

	//Used to calibrate the initial iPhoneIput.acceleration input
	void calibrateAccelerometer(){
		Vector3 wantedDeadZone = Input.acceleration;
		Quaternion rotateQuaternion = Quaternion.FromToRotation(new Vector3(0f, 0f, -1f), wantedDeadZone);
		
		//create identity matrix ... rotate our matrix to match up with down vec
		Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, rotateQuaternion, new Vector3(1f, 1f, 1f));
		//get the inverse of the matrix
		this.calibrationMatrix = matrix.inverse;
	}
	



}


