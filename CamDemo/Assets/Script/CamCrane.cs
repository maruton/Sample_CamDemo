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
	Text Txt_GyroDeg;


	Text Txt01;
	Text Txt02;
	Text Txt03;
	Text Txt04;
	Text Txt05;
	Text Txt06;
	Text Txt07;
	Text Txt08;

	Text Txt_Status;
	string Message_Status = "";

	/*!	Initial procedure.
	 * 	Initial class for degree. 
   	 * 	@note		 	None
   	 * 	@attention		None 
   	 */	
	void Init(){

		GameObject go = GameObject.Find("Canvas/Text_AccelValue");
		Txt_AccelValue = go.GetComponent<Text>();

		go = GameObject.Find("Canvas/Text_GyroValue");
		Txt_GyroDeg = go.GetComponent<Text>();



		go = GameObject.Find("Canvas/Text_Status");
		Txt_Status = go.GetComponent<Text>();

		go = GameObject.Find("Canvas/Text01");
		Txt01 = go.GetComponent<Text>();
		go = GameObject.Find("Canvas/Text02");
		Txt02 = go.GetComponent<Text>();
		go = GameObject.Find("Canvas/Text03");
		Txt03 = go.GetComponent<Text>();
		go = GameObject.Find("Canvas/Text04");
		Txt04 = go.GetComponent<Text>();
		go = GameObject.Find("Canvas/Text05");
		Txt05 = go.GetComponent<Text>();
		go = GameObject.Find("Canvas/Text06");
		Txt06 = go.GetComponent<Text>();
		go = GameObject.Find("Canvas/Text07");
		Txt07 = go.GetComponent<Text>();
		go = GameObject.Find("Canvas/Text08");
		Txt08 = go.GetComponent<Text>();


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
		//Debug.Log ("trim.ResultValue:"+trim.ResultValue);
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
	const float Mag_AccelX = 90f;	const float Mag_AccelY = 90f;
	float AccelZoom = 0;
	const float Mag_AccelZoom = 1f;

	Vector3 Last_acceleration = new Vector3(0,0,0);
	float Last_magneticHeading = 0f;
	bool Detected_acceleration_X = false;
	bool Detected_acceleration_Y = false;
	public float Detected_acceleration_DegX = 0f;
	public float Detected_acceleration_DegY = 0f;
	//public float AccelX = 0f;
	//public float AccelY = 0f;

	bool EnableAccelSensor = true;//!< Enable accel sensor
	bool EnableAccelSensor_DetectDiffrencial = true;//!< Enable accel sensor motion detect.
	Vector3 noiseReduction_acceleration = new Vector3(0,0,0);
	float noiseReduction_compass = 0f;

	public bool JittterReductionA = false;//true;	//Noise reduction filter A for acceleration sensor
	public bool JittterReductionB = true;//false;	//Noise reduction filter B for acceleration sensor
	public float SmoothFrame = 1.0f;//!< n/60 frame for lerp.
	float compass_diff = 0f;
	float magneticHeading = 0f;
	/*!	Accelerate operation.
	 *	Accelerate operation.
   	 * 	@note		 	LCD面が、
   	 * 					水平上向き 	y = -1,							z = 0
   	 * 					手前45度	y = -0.5 ※後方上45度と同じ		z = -0.5
   	 * 					上垂直		y = -1.0						z = 0
   	 * 					後方上45度	y = -0.5 ※前方上45度と同じ		z = 0.5
   	 * 					水平下向き	y = 1.0							z = 0
   	 * 
   	 * 	@attention		Slow&Smooth:  JittterReductionA=false, JittterReductionB=true
   	 * 					Fast&Responce:JittterReductionA=true, JittterReductionB=false
   	 */	
	void AccelerateOperation(){
		//transform.rotation = Quaternion.Euler(0, -Input.compass.trueHeading, 0);
		Vector3 acceleration = Input.acceleration;
		magneticHeading = turn.adj180(Input.compass.magneticHeading);

		if(!EnableAccelSensor){
			acceleration = Vector3.zero;
			Detected_acceleration_DegX = Detected_acceleration_DegY = 0f;
			return;
		}
		//Begin: raw level noise reduction
		if(JittterReductionB){
			float noiseReduction_acceleration_Response = SmoothFrame/60f;
			noiseReduction_acceleration = Vector3.Lerp(noiseReduction_acceleration, acceleration, noiseReduction_acceleration_Response);
			acceleration = noiseReduction_acceleration;

			float noiseReduction_compass_Response = SmoothFrame/60f;

			//Round lerp
			Vector3 a = new Vector3(magneticHeading, 0f, 0f);
			Vector3 b = new Vector3(noiseReduction_compass, 0f, 0f);
			Vector3 c = Vector3.Lerp(b, a, noiseReduction_compass_Response);
			noiseReduction_compass = c.x;

			//noiseReduction_compass = Mathf.Lerp(noiseReduction_compass, magneticHeading, noiseReduction_compass_Response);
			magneticHeading = noiseReduction_compass;

		}
		//End: raw level noise reduction


		float diff = 0f;
		bool accelEnableX = false;
		bool accelEnableY = false;
		bool accelEnableZ = false;
		bool compassEnable = false;
		if(JittterReductionA){
			//const float jitterNoise = 0.015f;
			const float jitterNoise_Accel = 0.015f;	//!< Accel jitter(normalized 0~1.0)
			const float jitterNoise_Compass = 1f;	//!< Compass jitter (degree)
			if(EnableAccelSensor_DetectDiffrencial){
				if(!accelEnableX){
					diff = Last_acceleration.x - acceleration.x;
					if(Mathf.Abs(diff)>jitterNoise_Accel){
						accelEnableX = true;
						Last_acceleration.x = acceleration.x;
					}
				}
				if(!accelEnableY){
					diff = Last_acceleration.y - acceleration.y;
					if(Mathf.Abs(diff)>jitterNoise_Accel){
						accelEnableY = true;
						Last_acceleration.y = acceleration.y;
					}
				}
				if(!accelEnableZ){
					diff = Last_acceleration.z - acceleration.z;
					if(Mathf.Abs(diff)>jitterNoise_Accel){
						accelEnableZ = true;
						Last_acceleration.z = acceleration.z;
					}
				}
				if(!compassEnable){
					diff = Last_magneticHeading - magneticHeading;
					if(Mathf.Abs(diff)>jitterNoise_Compass){
						compassEnable = true;
						Last_magneticHeading = magneticHeading;
					}
				}
			}
		}
		else{
			accelEnableX = true;
			accelEnableY = true;
			accelEnableZ = true;
			compassEnable = true;
		}
		turn.limitation = false; //Limit range sw (DEBUG)
		trim.limitation = false; //Limit sw(DEBUG)
		//Make new degree by acceleration

		/*
		//Begin: Use acceleration sensor
		if(accelEnableX){
			Detected_acceleration_DegX = acceleration.x * Mag_AccelX; //Direction Turn
			Last_acceleration.x = acceleration.x;
			Detected_acceleration_X = true;
			Txt07.text = "Detected_acceleration_DegX"+Detected_acceleration_DegX.ToString("F5");
		}
		//End: Use acceleration sensor
		*/
		//Begin: Use Compass sensor
		if(compassEnable){
			compass_diff = turn.adj180(magneticHeading - base_magneticHeading);
			//Detected_acceleration_DegX = compass_diff * Mag_AccelX / 180f; //Direction Turn
			Detected_acceleration_DegX = compass_diff; //Direction Turn
			Detected_acceleration_X = true;
		}
		//End: Use Compass sensor

		if(accelEnableZ && accelEnableY){
			float q = acceleration.z;
			if(acceleration.y>0){
				if(acceleration.z>0){
					q = 1.0f + acceleration.y;//0~2.0, (90deg/1.0)
				}
				else{
					q = -1.0f - acceleration.y;
				}
			}
			Detected_acceleration_DegY = q * -Mag_AccelY; //Direction Trim
			Detected_acceleration_Y = true;
			//Txt08.text = "Detected_acceleration_DegY"+Detected_acceleration_DegY.ToString("F5");
		}
		/*
		if(accelEnableY){
			float y = acceleration.y;
			if(acceleration.z>0){
				if(acceleration.y<0) y -= acceleration.z;
				else y += acceleration.z;
			}
			Detected_acceleration_DegY = y * -Mag_AccelY; //Direction Trim
			Last_acceleration.y = acceleration.y;
			Detected_acceleration_Y = true;
			Txt08.text = "Detected_acceleration_DegY"+Detected_acceleration_DegY.ToString("F5");
		}
		*/
	}

	void test(){
		Txt_AccelValue.text = Input.acceleration.ToString("F5");

		//The heading in degrees relative to the magnetic North Pole. (Read Only) ※北からの相対角度（平面上） 
		Txt01.text = "compass.magneticHeading: "+Input.compass.magneticHeading.ToString("F5");

		//The heading in degrees relative to the geographic North Pole. (Read Only)  ※北からの相対角度（平面上） 
		//Must have location services enabled (using GPS?)
		Txt02.text = "compass.trueHeading: "+Input.compass.trueHeading.ToString("F5");


		Quaternion compasRot = Quaternion.Euler(0, -Input.compass.trueHeading, 0);
		Vector3 compassDeg = compasRot.eulerAngles;
		Txt03.text = "compassTrueDeg: "+compassDeg.ToString("F5");




		/*
		//The raw geomagnetic data measured in microteslas. (Read Only) 
		Txt04.text = "compass.rawVector: "+Input.compass.rawVector.ToString("F5");

		Txt05.text = "compass.rawVector normalize: "+ Vector3.Normalize( Input.compass.rawVector ).ToString("F5");

		Vector3 normalized = Vector3.Normalize( Input.compass.rawVector );
		normalized *= 180.0f;
		Txt06.text = normalized.ToString("F0");
		//		avatar.transform.
		*/

		//読み取り精度値の取得。未サポートの場合は0を返す 
		//Txt06.text = "compass.headingAccuracy: "+Input.compass.headingAccuracy.ToString("F5");

			


		Input.gyro.enabled = true;
		if(Input.gyro.enabled){
			Quaternion gyroRot = Input.gyro.attitude;
			Quaternion gyro = Quaternion.Euler(90, 0, 0) * (new Quaternion(-gyroRot.x,-gyroRot.y, gyroRot.z, gyroRot.w));
			Txt_GyroDeg.text = gyroRot.ToString("F5");
		}
	}

	bool initial_frame = true;
	float base_magneticHeading = 0f;// Front relative degree from north pole.



	void Init_Sensor(){
		noiseReduction_acceleration = Input.acceleration;

		//StartCoroutine("Location_Start");// Initialize for Location with GPS/Wi-Fi network 

		Input.location.Start();
		Input.compass.enabled = true; //Enable compass sensor
		Input.location.Start();
		base_magneticHeading = Input.compass.magneticHeading;//Always 0.
		if(SystemInfo.supportsGyroscope){//Check&Enable Gyro sensor
			Input.gyro.enabled = true;
		}
	}
	void Start () {
		Init_SysInfo();
		Init_Sensor();
		/*
		Quaternion a = new Quaternion(0f, 0.20773f, 0f, 0.97819f);
		Quaternion b = new Quaternion(0f, 0.00170f, 0f, 1.0f);
		float t = Quaternion.Angle(a , b);
		*/
		//Debug.Log("t="+t);

	}

	void Update () {
		Txt05.text = "base_magneticHeading: "+base_magneticHeading.ToString("F5");
		if(initial_frame){
			base_magneticHeading = Input.compass.magneticHeading;
			if(base_magneticHeading==0f){
				return;
			}
			else{
				initial_frame = false;
				base_magneticHeading = turn.adj180(base_magneticHeading);
				noiseReduction_compass = base_magneticHeading;
			}
		}
		Txt04.text = "base_magneticHeading: "+base_magneticHeading.ToString("F5");
		Txt05.text = "magneticHeading: "+magneticHeading.ToString("F5");
		Txt06.text = "compass_diff: "+compass_diff.ToString("F5");


		KeyOperation();
		AccelerateOperation();



		update_Turn();
		update_Trim();
		update_Zoom();


		//Indicate status
		string statusMessage = "";
		statusMessage = systemInfo+"\n";

		if(Input.gyro.enabled) statusMessage += "Gyro[On]";
		if(!Input.gyro.enabled) statusMessage += "Gyro[Off]";
		Txt_Status.text = statusMessage;


		test();



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
	string 	systemInfo = "";
	void Init_SysInfo(){
		systemInfo += "\nDeviceModel: "+SystemInfo.deviceModel;
		systemInfo += "\nDeviceName: "+SystemInfo.deviceName;
		systemInfo += "\nDeviceType: "+SystemInfo.deviceType;
		systemInfo += "\ngraphicsDeviceID: "+SystemInfo.graphicsDeviceID;
		systemInfo += "\ngraphicsDeviceName: "+SystemInfo.graphicsDeviceName;
		systemInfo += "\ngraphicsDeviceType: "+SystemInfo.graphicsDeviceType;
		systemInfo += "\ngraphicsDeviceVendor: "+SystemInfo.graphicsDeviceVendor;
		systemInfo += "\ngraphicsDeviceVendorID: "+SystemInfo.graphicsDeviceVendorID;
		systemInfo += "\ngraphicsDeviceVersion: "+SystemInfo.graphicsDeviceVersion;
		systemInfo += "\ngraphicsMemorySize: "+SystemInfo.graphicsMemorySize;
		systemInfo += "\ngraphicsMultiThreaded: "+SystemInfo.graphicsMultiThreaded;
		systemInfo += "\ngraphicsShaderLevel: "+SystemInfo.graphicsShaderLevel;
		systemInfo += "\nmaxTextureSize: "+SystemInfo.maxTextureSize;
		systemInfo += "\nprocessorCount: "+SystemInfo.processorCount;
		systemInfo += "\nprocessorType: "+SystemInfo.processorType;
		systemInfo += "\nsupportedRenderTargetCount: "+SystemInfo.supportedRenderTargetCount;
		systemInfo += "\nsupports3DTextures: "+SystemInfo.supports3DTextures;
		systemInfo += "\nsupportsComputeShaders: "+SystemInfo.supportsComputeShaders;
		systemInfo += "\nsupportsImageEffects: "+SystemInfo.supportsImageEffects;
		//systemInfo += "\nSupportsRenderTextureFormat: "+(SystemInfo.SupportsRenderTextureFormat==true)?"true":"false";
		systemInfo += "\nsupportsRenderTextures: "+SystemInfo.supportsRenderTextures;
		systemInfo += "\nsupportsShadows: "+SystemInfo.supportsShadows;
		systemInfo += "\nsupportsSparseTextures: "+SystemInfo.supportsSparseTextures;
		systemInfo += "\nsupportsStencil: "+SystemInfo.supportsStencil;
		//systemInfo += "\nSupportsTextureFormat: "+SystemInfo.SupportsTextureFormat;
		systemInfo += "\nsystemMemorySize: "+SystemInfo.systemMemorySize;
		systemInfo += "\nsupportsAccelerometer: "+SystemInfo.supportsAccelerometer;
		systemInfo += "\nsupportsGyroscope: "+SystemInfo.supportsGyroscope;
		systemInfo += "\nsupportsVibration: "+SystemInfo.supportsVibration;
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


	/*!	Initialize Location devide
	 *	Initialize Location devide that GPS, Wi-Fi network locator.
   	 * 	@note		 	None
   	 * 	@attention		None
	 */	
	IEnumerator Location_Start(){
		// First, check if user has location service enabled
		if (!Input.location.isEnabledByUser){
			yield break;//GPSが有効になっていない場合 
		}
		// Start service before querying location
		Input.location.Start();
		
		// Wait until service initializes
		int maxWait = 20;
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0){
			yield return new WaitForSeconds(1);
			maxWait--;
		}
		
		// Service didn't initialize in 20 seconds
		if (maxWait < 1){
			print("Timed out");
			yield break;//「Wi-Fi/モバイル接続時の位置情報」が有効になっていない場合  
		}
		
		// Connection has failed
		if (Input.location.status == LocationServiceStatus.Failed){
			print("Unable to determine device location");
			yield break;
		}
		else{
			// Access granted and location value could be retrieved
			print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
		}
		
		// Stop service if there is no need to query location updates continuously
		//Input.location.Stop();
	}
}

/*
//Quaternion→Vector3
Vector3 vec3         = qua.eulerAngles;

//Vector3→Quaternion
Quaternion.Euler qua = Quaternion.Euler(vec3);
*/





