/*!
 * 	@file		CamCrane.cs 
 * 	@brief		'CamCrane' Class source file. 
 * 	@note		None 
 *	@attention 
 *  			[CamCrane.cs]										<br>
 *				Copyright (c) [2015] [Maruton] 						<br>
 *				This software is released under the MIT License. 	<br>
 *				http://opensource.org/licenses/mit-license.php 		<br>
 */
using UnityEngine;
using System.Collections;

/*!
 * 	@brief		CamCrane class <br>
 * 	@attention	None
 * 	@note		None
 * 	@author		Maruton.
 */
public class CamCrane : MonoBehaviour {
	//---
	const float ZoomDedault = 3.3f;
	const float ZoomNearLimit = 1.4f;
	const float ZoomFarLimit = 30f;
	float offsetZoom = ZoomDedault; //!< Offset of camera zoom(z).

	/*	Accessor for offsetCamZoom
	 */
	float OffsetZoom {
		get{ return(offsetZoom); }
		set{ offsetZoom = value;
			if(offsetZoom>ZoomFarLimit) offsetZoom = ZoomFarLimit;
			else if(offsetZoom<ZoomNearLimit) offsetZoom = ZoomNearLimit;
		}
	}

	const float ResolutionZoom = 0.05f; //!< Resolution for camera zoom.
	/*!	Zoom up camera .
	 *	Simply zoom up camera. 
   	 * 	@note		 	None
   	 * 	@attention		None 
   	 */	
	void Entry_ZoomUp(){
		OffsetZoom -= ResolutionZoom;
	}

	/*!	Zoom down camera .
	 *	Simply zoom down camera. 
   	 * 	@note		 	None
   	 * 	@attention		None 
   	 */	
	void Entry_ZoomDown(){
		OffsetZoom += ResolutionZoom;
	}

	const float HorizontalStep = 2f; //!< Resolution for camera turn.
	Quaternion rotHorizontal = Quaternion.identity;

	/*!	Turn right camera.
	 *	Simply turn right camera. 
   	 * 	@note		 	None
   	 * 	@attention		None 
   	 */	
	void Entry_Turn_Right(){
		rotHorizontal *= Quaternion.Euler(0, -HorizontalStep, 0);
	}

	/*!	Turn left camera.
	 *	Simply turn left camera. 
   	 * 	@note		 	None
   	 * 	@attention		None 
   	 */	
	void Entry_Turn_Left(){
		rotHorizontal *= Quaternion.Euler(0,  HorizontalStep, 0);
	}

	const float VerticalStep = 2f; //!< Resolution for camera lookup and lookdown.
	Quaternion rotVertical = Quaternion.identity;
	/*!	Lookup camera.
	 *	Simply lookup camera. 
   	 * 	@note		 	None
   	 * 	@attention		None 
   	 */	
	void Entry_LookUp(){
		rotVertical *= Quaternion.Euler(VerticalStep, 0, 0);
	}

	/*!	Lookdown camera.
	 *	Simply lookdown camera. 
   	 * 	@note		 	None
   	 * 	@attention		None 
   	 */	
	void Entry_LookDown(){
		rotVertical *= Quaternion.Euler(-VerticalStep, 0, 0);
	}
	//-----
	GameObject go_CamHorizontal;  //!< Object for turn Horizontal.
	GameObject go_CamVertical; //!< Object for turn vertical.
	GameObject go_CamZoom;	   //!< Object for zooming.

	//-----
	void Awake(){
		go_CamHorizontal = GameObject.Find("CamHorizontal");
		go_CamVertical = GameObject.Find("CamVertical");
		go_CamZoom = GameObject.Find("CamZoom");

	}
	void Start () {
	
	}

	/*!	Keyboard operation.
	 *	Input for PC's keyboard.
   	 * 	@note		 	None
   	 * 	@attention		None 
   	 */	
	void KeyOperation(){
		if(Input.GetKey(KeyCode.D)){
			Entry_Turn_Right();
		}
		else if(Input.GetKey(KeyCode.A)){
			Entry_Turn_Left();
		}

		if(Input.GetKey(KeyCode.W)){
			Entry_LookDown();
		}
		else if(Input.GetKey(KeyCode.S)){
			Entry_LookUp();
		}

		if(Input.GetKey(KeyCode.PageUp)){
			Entry_ZoomUp();
		}
		else if(Input.GetKey(KeyCode.PageDown)){
			Entry_ZoomDown();
		}
	}
	//---
	const float SensorXLimit = 0.5f;
	const float SensorYLimit = 0.5f;
	float AccelX = 0;				float AccelY = 0;
	float Raw_AccelX = 0;			float Raw_AccelY = 0;
	float Raw_Before_AccelX = 0;	float Raw_Before_AccelY = 0;
	const float Mag_AccelX = 80f;	const float Mag_AccelY = 80f;
	Quaternion AccelHorizontalRot = Quaternion.identity;
	Quaternion AccelVerticalRot = Quaternion.identity;

	const float LimitVertical = 0.5f;

	const float LimitZoom = 0.5f;
	float AccelZoom = 0;
	float Raw_AccelZoom = 0;
	float Raw_Before_AccelZoom = 0;
	const float Mag_AccelZoom = 1f;

	//---
	void AccelerateOperation(){
		float diff;
		Vector3 acceleration = Input.acceleration;
		//Debug.Log (acceleration);

		Raw_AccelX = acceleration.x;

		diff = Raw_Before_AccelX - Raw_AccelX;
		if(acceleration.x>SensorXLimit) 		Entry_Turn_Left();
		else if(acceleration.x<-SensorXLimit) 	Entry_Turn_Right();
		if(Mathf.Abs(diff)>0.01f) {
			AccelX = acceleration.x * Mag_AccelX; //Exclision Chatter
			AccelHorizontalRot = Quaternion.Euler(new Vector3(0, AccelX, 0));
		}
		Raw_Before_AccelX = Raw_AccelX;

		Raw_AccelY = acceleration.y;
		diff = Raw_Before_AccelY - Raw_AccelY;

		if(acceleration.y>SensorYLimit) 		Entry_LookUp();
		else if(acceleration.y<-SensorYLimit) 	Entry_LookDown();

		if(Mathf.Abs(diff)>0.01f) {
			AccelY = acceleration.y * Mag_AccelY; //Exclision Chatter
			AccelVerticalRot = Quaternion.Euler(new Vector3(AccelY, 0, 0));
		}
		Raw_Before_AccelY = Raw_AccelY;

	}

	void Update () {
		KeyOperation();
		AccelerateOperation();

		go_CamHorizontal.transform.rotation    = rotHorizontal * AccelHorizontalRot;
		go_CamVertical.transform.localRotation = rotVertical * AccelVerticalRot;

		Vector3 zoom = go_CamZoom.transform.localPosition;
		zoom.z = OffsetZoom + AccelZoom;
		go_CamZoom.transform.localPosition = zoom;
	}
}
