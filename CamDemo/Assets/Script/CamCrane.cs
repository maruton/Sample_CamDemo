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
using UnityEngine.UI; //need for uGUI
using UnityEngine.EventSystems; //need for EventTrigger 

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
		zoom.Sub();
	}

	/*!	Zoom down camera .
	 *	Simply zoom down camera. 
   	 * 	@note		 	None
   	 * 	@attention		None 
   	 */	
	void Entry_ZoomDown(){
		zoom.Add();
	}


	
	ValueRange turn;// = new ValueRange(); //!< Control to range for turn.
	ValueRange trim;// = new ValueRange();	//!< Control to range for trim.
	ValueRange zoom;// = new ValueRange(); //!< Control to range for zoom.

	CalcDiffDegree turnDiffDeg;
	CalcDiffDegree trimDiffDeg;

	uGUI_Accel accelGui;

	Text Txt_AccelValue;
	Text Txt_CompassMagValue;
	Text Txt_CompassTrueValue;
	Text Txt_CompassAccuracy;
	Text Txt_CompassRot;

	/*!	Initial procedure.
	 * 	Initial class for degree. 
   	 * 	@note		 	None
   	 * 	@attention		None 
   	 */	
	void Init(){

		GameObject go = GameObject.Find("Canvas/Text_AccelValue");
		Txt_AccelValue = go.GetComponent<Text>();
		go = GameObject.Find("Canvas/Text_CompassMagHeadValue");
		Txt_CompassMagValue = go.GetComponent<Text>();
		go = GameObject.Find("Canvas/Text_CompassTrueHeadValue");
		Txt_CompassTrueValue = go.GetComponent<Text>();
		go = GameObject.Find("Canvas/Text_CompassAccuracyValue");
		Txt_CompassAccuracy = go.GetComponent<Text>();

		go = GameObject.Find("Canvas/Text_CompassRot");
		Txt_CompassRot = go.GetComponent<Text>();

		accelGui = GetComponent<uGUI_Accel>();

		go_CamHorizontal = GameObject.Find("CamHorizontal");
		go_CamVertical = GameObject.Find("CamVertical");
		go_CamZoom = GameObject.Find("CamZoom");
		
		turn = gameObject.AddComponent<ValueRange>();
		trim = gameObject.AddComponent<ValueRange>();
		zoom = gameObject.AddComponent<ValueRange>();

		trimDiffDeg = go_CamVertical.AddComponent<CalcDiffDegree>();
		turnDiffDeg = go_CamHorizontal.AddComponent<CalcDiffDegree>();

		turn.RangeValue = 0f;
		turn.SetResolution(2f);		//Moving step
		turn.SetRange(-80f, 80f);	//Degree range Left,Right
		turn.limitation = true;		//Limit range sw
		turn.AbsAdjustOffsetValue = 0f;//Absolute offset for Accel sensor

		trim.RangeValue = 0f;
		trim.SetResolution(2f);		//Moving step
		trim.SetRange(-10f, 35f);	//Degree range Down,Up
		trim.limitation = true;		//Limit sw
		trim.AbsAdjustOffsetValue = 20f;//Absolute offset for Accel sensor

		zoom.RangeValue = 3.3f;
		zoom.SetResolution(0.05f);	//Moving step
		zoom.SetRange(1.6f, 5f);	//Zoom up, Zoom down
		zoom.limitation = true;		//Limit range sw

		HorizontalLeapSourceRot = HorizontalBaseRot;
		HorizontalLeapTargetRot = HorizontalBaseRot;
		HorizontalLeapResultRot = HorizontalBaseRot;

		VerticalLeapSourceRot = VerticalBaseRot;
		VerticalLeapTargetRot = VerticalBaseRot;
		VerticalLeapResultRot = VerticalBaseRot;

	}
	//--------------------
	Quaternion HorizontalBaseRot = Quaternion.Euler(0,0,90);//!< Model offset for model.
	Quaternion HorizontalOffsetRot = Quaternion.identity;	//!< Turn offset for moving.
	Quaternion VerticalBaseRot = Quaternion.Euler(0,0,0);	//!< Trim offset for model.
	Quaternion VerticalOffsetRot = Quaternion.identity;		//!< Trim offset for moving.
	//--------------------
	void update_Zoom(){
		//Debug.Log (zoom.RangeValue);
		go_CamZoom.transform.localPosition = new Vector3(0,0,zoom.RangeValue + AccelZoom);
		
	}
	//--------------------
	public bool EnableLeap_Turn = true;
	void update_Turn(){
		turn.AbsAdjustValue = Detected_acceleration_DegX;
		HorizontalOffsetRot = Quaternion.Euler(turn.ResultValue,0,0);
		if(EnableLeap_Turn){
			Update_TurnLeap();
		}
		else{
			HorizontalLeapResultRot = HorizontalBaseRot * HorizontalOffsetRot;
		}
		go_CamHorizontal.transform.localRotation = HorizontalLeapResultRot;
	}
	Quaternion HorizontalLeapSourceRot;
	Quaternion HorizontalLeapTargetRot;
	Quaternion HorizontalLeapResultRot;
	float Turn_LapTime = 0f;
	float Turn_Time = 0f;
	int Mode_TurnLeap = 0;
	public float Mag_TurnLeapDelta = 1.0f;// (0.01~1.0)  0.2=smooth & slow.0.4,0.6,1.0
	void Update_TurnLeap(){
		Mag_TurnLeapDelta = accelGui.ParamH;
		if(Detected_acceleration_X){
			Detected_acceleration_X = false;
			Quaternion currentRot = go_CamHorizontal.transform.localRotation;//Check Local?Global?
			//Debug.Log ("currentRot = "+currentRot.ToString("F5"));
			HorizontalLeapTargetRot = HorizontalBaseRot * HorizontalOffsetRot;
			HorizontalLeapSourceRot = currentRot;
			Turn_Time = (Quaternion.Angle(HorizontalLeapTargetRot , HorizontalLeapSourceRot))/180.0f;
			Turn_LapTime = 0f;
			Mode_TurnLeap = 1;
			/*Debug.Log ("0 : HorizontalLeapTargetRot="+HorizontalLeapTargetRot.ToString("F5")+
			           "HorizontalLeapSourceRot="+HorizontalLeapSourceRot.ToString("F5")+
			           "  ("+Turn_LapTime.ToString("F5")+"/"+Turn_Time.ToString("F5")+")  "
			           );*/
		}
		if(Mode_TurnLeap==1){
			float t = (Time.deltaTime * Mag_TurnLeapDelta);
			if(t==0.0f) t = 0.005f;//Safe code.
			Turn_LapTime += t;
			if(Turn_LapTime<Turn_Time){//Leap moving.
				HorizontalLeapResultRot = Quaternion.Lerp(HorizontalLeapSourceRot, HorizontalLeapTargetRot, Turn_LapTime/Turn_Time);
				/*Debug.Log ("1a: HorizontalLeapTargetRot="+HorizontalLeapTargetRot.ToString("F5")+
				           "HorizontalLeapSourceRot="+HorizontalLeapSourceRot.ToString("F5")+
				           "  ("+Turn_LapTime.ToString("F5")+"/"+Turn_Time.ToString("F5")+")  "
				           );*/
			}
			else{//Leap move end.
				HorizontalLeapResultRot = HorizontalLeapTargetRot;
				Turn_LapTime = 0f;
				/*Debug.Log ("0e: HorizontalLeapTargetRot="+HorizontalLeapTargetRot.ToString("F5")+
				           "HorizontalLeapSourceRot="+HorizontalLeapSourceRot.ToString("F5")+
				           "  ("+Turn_LapTime.ToString("F5")+"/"+Turn_Time.ToString("F5")+")  "
				           );*/
				Mode_TurnLeap = 0;
			}
		}
	}
	//--------------------
	public bool EnableLeap_Trim = true;
	void update_Trim(){
		trim.AbsAdjustValue = Detected_acceleration_DegY;
		VerticalOffsetRot = Quaternion.Euler(0,trim.ResultValue,0);
		if(EnableLeap_Trim){
			Update_TrimLeap();
		}
		else{
			VerticalLeapResultRot = VerticalBaseRot * VerticalOffsetRot;
		}
		go_CamVertical.transform.localRotation = VerticalLeapResultRot;
	}
	Quaternion VerticalLeapSourceRot;
	Quaternion VerticalLeapTargetRot;
	Quaternion VerticalLeapResultRot;
	float Trim_LapTime = 0f;
	float Trim_Time = 0f;
	int Mode_TrimLeap = 0;
	public float Mag_TrimLeapDelta = 1.0f;// (0.01~1.0)  10.0f=smooth & slow.
	void Update_TrimLeap(){
		Mag_TrimLeapDelta = accelGui.ParamV;
		switch(Mode_TrimLeap){
		case 0://Leap start
			if(Detected_acceleration_Y){
				Detected_acceleration_Y = false;
				Quaternion currentRot = go_CamVertical.transform.localRotation;
				//Debug.Log ("currentRot = "+currentRot.ToString("F5"));
				VerticalLeapTargetRot = VerticalBaseRot * VerticalOffsetRot;
				VerticalLeapSourceRot = currentRot;
				Trim_Time = (Quaternion.Angle(VerticalLeapTargetRot , VerticalLeapSourceRot))/180.0f;
				Trim_LapTime = 0f;
				Mode_TrimLeap = 1;
				/*Debug.Log ("0 : VerticalLeapTargetRot="+VerticalLeapTargetRot.ToString("F5")+
				           "VerticalLeapSourceRot="+VerticalLeapSourceRot.ToString("F5")+
				           "  ("+Trim_LapTime.ToString("F5")+"/"+Trim_Time.ToString("F5")+")  "
				           );*/
			}
			break;
		case 1://<Leap moving
			//float t = Time.deltaTime;
			float t = (Time.deltaTime * Mag_TrimLeapDelta);
			if(t==0.0f) t = 0.005f;//Safe code.
			Trim_LapTime += t;
			if(Trim_LapTime<Trim_Time){//Leap moving.
				VerticalLeapResultRot = Quaternion.Lerp(VerticalLeapSourceRot, VerticalLeapTargetRot, Trim_LapTime/Trim_Time);
				/*Debug.Log ("1a: VerticalLeapTargetRot="+VerticalLeapTargetRot.ToString("F5")+
				           "VerticalLeapSourceRot="+VerticalLeapSourceRot.ToString("F5")+
				           "  ("+Trim_LapTime.ToString("F5")+"/"+Trim_Time.ToString("F5")+")  "
				           );*/
			}
			else{//Leap move end.
				VerticalLeapResultRot = VerticalLeapTargetRot;
				Trim_LapTime = 0f;
				/*Debug.Log ("0e: VerticalLeapTargetRot="+VerticalLeapTargetRot.ToString("F5")+
				           "VerticalLeapSourceRot="+VerticalLeapSourceRot.ToString("F5")+
				           "  ("+Trim_LapTime.ToString("F5")+"/"+Trim_Time.ToString("F5")+")  "
				           );*/
				Mode_TrimLeap = 0;
			}
			break;

		default:
			break;
		}
	}
	//--------------------
	const float Mag_AccelX = 80f;	const float Mag_AccelY = 80f;
	float AccelZoom = 0;
	const float Mag_AccelZoom = 1f;

	Vector3 Last_acceleration = new Vector3(0,0,0);
	bool Detected_acceleration_X = false;
	bool Detected_acceleration_Y = false;
	public float Detected_acceleration_DegX = 0f;
	public float Detected_acceleration_DegY = 0f;
	//public float AccelX = 0f;
	//public float AccelY = 0f;

	public bool EnableAccelSensor = true;//!< Enable accel sensor
	public bool EnableAccelSensor_DetectDiffrencial = true;//!< Enable accel sensor motion detect.
	/*!	Accelerate operation.
	 *	Accelerate operation.
   	 * 	@note		 	None
   	 * 	@attention		None 
   	 */	
	void AccelerateOperation(){

		Vector3 acceleration = Input.acceleration;
		float compassMag = Input.compass.magneticHeading;
		float compassTrue = Input.compass.trueHeading;
		float compassAccuracy = Input.compass.headingAccuracy;

		Txt_AccelValue.text = acceleration.ToString("F5");
		Txt_CompassMagValue.text = compassMag.ToString("F5");
		Txt_CompassTrueValue.text = compassTrue.ToString("F5");
		Txt_CompassAccuracy.text = compassAccuracy.ToString("F5");

		Quaternion compasRot = Quaternion.Euler(0, -Input.compass.trueHeading, 0);
		Vector3 compassDeg = compasRot.eulerAngles;
		//Txt_CompassRot.text = compasRot.ToString("F5")+"/"+compassDeg.ToString("F5");
		Txt_CompassRot.text = compasRot.ToString("F5")+"/"+Input.compass.rawVector.ToString("F5");



		//transform.rotation = Quaternion.Euler(0, -Input.compass.trueHeading, 0);


		if(!EnableAccelSensor){
			acceleration = Vector3.zero;
			Detected_acceleration_DegX = Detected_acceleration_DegY = 0f;
			return;
		}
		//Debug.Log (acceleration);
		float diff;
		//Debug.Log (acceleration);

		bool accelEnableX = false;
		bool accelEnableY = false;

		if(EnableAccelSensor_DetectDiffrencial){
			//Moved abit far distance that must be move now.
			if(!accelEnableX){
				diff = Last_acceleration.x - acceleration.x;
				//Debug.Log ("diff="+diff.ToString("F5")+"acceleration.x="+acceleration.x.ToString("F5")+"  before_acceleration.x="+before_acceleration.x.ToString("F5")  );
				if(Mathf.Abs(diff)>0.015f) accelEnableX = true;
			}
			//Moved abit far distance that must be move now.
			if(!accelEnableY){
				diff = Last_acceleration.y - acceleration.y;
				//Debug.Log ("diff="+diff.ToString("F5")+"acceleration.y="+acceleration.y.ToString("F5") );
				if(Mathf.Abs(diff)>0.015f) accelEnableY = true;
			}
		}
		if(accelEnableX){
			Detected_acceleration_DegX = acceleration.x * Mag_AccelX; //Exclision Chatter
			Last_acceleration.x = acceleration.x;
			Detected_acceleration_X = true;
		}
		if(accelEnableY){
			Detected_acceleration_DegY = acceleration.y * -Mag_AccelY; //Exclision Chatter
			Last_acceleration.y = acceleration.y;
			Detected_acceleration_Y = true;
		}
	}

	void Update () {
		KeyOperation();
		AccelerateOperation();

		update_Turn();
		update_Trim();
		update_Zoom();

		//「戻るキー」でアプリ終了 (Android)。ESC でアプリ終了(Win) 
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android && Input.GetKey(KeyCode.Escape)){//ESCキーで終了。 
			Application.Quit();
		}
		#endif
		#if UNITY_STANDALONE_WIN
		if(Input.GetKey(KeyCode.Escape)){//ESCキーで終了。 
			Application.Quit();
		}
		#endif
	}
	//-----
	GameObject go_CamHorizontal;  //!< Object for turn Horizontal.
	GameObject go_CamVertical; //!< Object for turn vertical.
	GameObject go_CamZoom;	   //!< Object for zooming.
	
	//-----
	void Awake(){
		Init();
	}
	void Start () {

		Quaternion a = new Quaternion(0f, 0.20773f, 0f, 0.97819f);
		Quaternion b = new Quaternion(0f, 0.00170f, 0f, 1.0f);
		float t = Quaternion.Angle(a , b);
		//Debug.Log("t="+t);
		Input.compass.enabled = true; //Enable compass
		Input.location.Start();
	}
	
	/*!	Turn right camera.
	 *	Simply turn right camera. 
   	 * 	@note		 	None
   	 * 	@attention		None 
   	 */	
	void Entry_Turn_Right(){
		turn.Sub();
	}
	
	/*!	Turn left camera.
	 *	Simply turn left camera. 
   	 * 	@note		 	None
   	 * 	@attention		None 
   	 */	
	void Entry_Turn_Left(){
		turn.Add();
	}
	
	/*!	Lookup camera.
	 *	Simply lookup camera. 
   	 * 	@note		 	None
   	 * 	@attention		None 
   	 */	
	void Entry_LookUp(){
		trim.Sub();
	}
	
	/*!	Lookdown camera.
	 *	Simply lookdown camera. 
   	 * 	@note		 	None
   	 * 	@attention		None 
   	 */	
	void Entry_LookDown(){
		trim.Add();
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
}

/*
//Quaternion→Vector3
Vector3 vec3         = qua.eulerAngles;

//Vector3→Quaternion
Quaternion.Euler qua = Quaternion.Euler(vec3);
*/

