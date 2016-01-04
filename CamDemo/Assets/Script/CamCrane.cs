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

	//CalcDiffDegree turnDiffDeg;
	//CalcDiffDegree trimDiffDeg;

	uGUI_Accel accelGui;

	Text Txt_GyroDeg;
	Text Txt_ValueAccel;
	Text Txt_Value_magneticHeading_raw;
	Text Txt_Value_trueHeading_raw;
	Text Txt_Value_compassTrueDeg_raw;
	Text Txt_Value_base_magneticHeading;
	Text Txt_Value_magneticHeading;
	Text Txt_Value_noiseReduction_compass;

	Text Txt_Status;
	//string Message_Status = "";


	/*!	Limit enable/disable
	 * 	Limit enable/disable
   	 * 	@note		 	None
   	 * 	@attention		None 
   	 */	
	public void SetLimitation(bool sw){
		turn.limitation = sw;		//Limit range sw
		trim.limitation = sw;		//Limit sw
		zoom.limitation = sw;		//Limit range sw
	}
	/*!	Set resolution frame rate
	 * 	Set resolution frame rate
	 * 	@param[in]		n. (rate is 60/n)
   	 * 	@note		 	None
   	 * 	@attention		None 
   	 */	
	public void SetFpsResolution(float n){
		NoiseReductionTake2Frame = n;
	}
	/*!	Set resolution frame rate
	 * 	Set resolution frame rate
	 * 	@param[in]		n. (rate is 60/n)
   	 * 	@note		 	None
   	 * 	@attention		None 
   	 */	
	public void SetHome_magneticHeading  (){
		base_magneticHeading = Input.compass.magneticHeading;//Always 0.
	}

	//
	/*!	Initial procedure.
	 * 	Initial class for degree. 
   	 * 	@note		 	None
   	 * 	@attention		None 
   	 */	
	void Init(){
		GameObject go = GameObject.Find("Canvas/Text_ValueAccel");

		Txt_GyroDeg = GameObject.Find("Canvas/Text_ValueGyro").GetComponent<Text>();
		Txt_ValueAccel = GameObject.Find("Canvas/Text_ValueAccel").GetComponent<Text>();
		Txt_Value_magneticHeading_raw = GameObject.Find("Canvas/Text_Value_magneticHeading_raw").GetComponent<Text>();
		Txt_Value_trueHeading_raw = GameObject.Find("Canvas/Text_Value_trueHeading_raw").GetComponent<Text>();
		Txt_Value_compassTrueDeg_raw = GameObject.Find("Canvas/Text_Value_compassTrueDeg_raw").GetComponent<Text>();
		Txt_Value_base_magneticHeading = GameObject.Find("Canvas/Text_Value_base_magneticHeading").GetComponent<Text>();
		Txt_Value_magneticHeading = GameObject.Find("Canvas/Text_Value_magneticHeading").GetComponent<Text>();
		Txt_Value_noiseReduction_compass = GameObject.Find("Canvas/Text_Value_noiseReduction_compass").GetComponent<Text>();
				
		Txt_Status = GameObject.Find("Canvas/Text_Status").GetComponent<Text>();


		accelGui = GetComponent<uGUI_Accel>();

		go_CamHorizontal = GameObject.Find("CamHorizontal");
		go_CamVertical = GameObject.Find("CamVertical");
		go_CamZoom = GameObject.Find("CamZoom");
		
		turn = gameObject.AddComponent<ValueRange>();
		trim = gameObject.AddComponent<ValueRange>();
		zoom = gameObject.AddComponent<ValueRange>();

		//trimDiffDeg = go_CamVertical.AddComponent<CalcDiffDegree>();
		//turnDiffDeg = go_CamHorizontal.AddComponent<CalcDiffDegree>();

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
	//float Detected_acceleration_DegX = 0f;
	void update_Turn(float turnValue){
		//turn.AbsAdjustValue = Detected_acceleration_DegX;
		HorizontalOffsetRot = Quaternion.Euler(turnValue,0f,0f);
		go_CamHorizontal.transform.localRotation = HorizontalBaseRot * HorizontalOffsetRot;;
	}
	//--------------------
	void update_Trim(float trimValue){
		//trim.AbsAdjustValue = Detected_acceleration_DegY;
		//Debug.Log ("trim.ResultValue:"+trim.ResultValue);
		VerticalOffsetRot = Quaternion.Euler(0,trimValue,0);
		go_CamVertical.transform.localRotation = VerticalBaseRot * VerticalOffsetRot;
	}
	//--------------------
	float AccelZoom = 0;

	[System.NonSerialized]
	public float Detected_acceleration_DegY = 0f;
	//public float AccelX = 0f;
	//public float AccelY = 0f;

	bool EnableSensor = true;//!< Enable accel sensor

	[System.NonSerialized]
	public float NoiseReductionTake2Frame = 1.0f;//!< n/60(or30) frame for lerp.
	Vector3 noiseReduction_acceleration = new Vector3(0,0,0); 	//!< Last value acceleration sensor for noise reduction.
	float noiseReduction_compass = 0f;							//!< Last value compass sensor for noise reduction.
	float magneticHeading = 0f;									//!< 
	/*!	Accelerate operation.
	 *	Accelerate operation.
   	 * 	@note		 	LCD面が、
   	 * 					水平上向き 	y = -1,							z = 0
   	 * 					手前45度	y = -0.5 ※後方上45度と同じ		z = -0.5
   	 * 					上垂直		y = -1.0						z = 0
   	 * 					後方上45度	y = -0.5 ※前方上45度と同じ		z = 0.5
   	 * 					水平下向き	y = 1.0							z = 0
   	 * 
   	 * 	@attention		None
   	 */	
	Vector3 SensorOperation(){
		float qx, qy;
		//transform.rotation = Quaternion.Euler(0, -Input.compass.trueHeading, 0);
		Vector3 acceleration = Input.acceleration;	//Accel sensor
		magneticHeading = Input.compass.magneticHeading;//compass sensor (0~360)
		Txt_Value_magneticHeading.text = magneticHeading.ToString("F5");

		if(!EnableSensor){
			return( new Vector3(0,0,0) );
		}
		//Begin: raw level noise reduction for accel sensor
		noiseReduction_acceleration = Vector3.Lerp(noiseReduction_acceleration, acceleration, (NoiseReductionTake2Frame/60f) );//need arithmetic degree(-180~0~180)
		acceleration = noiseReduction_acceleration;
		//End: raw level noise reduction for accel sensor
		//Begin: Accel sensor
		float q = acceleration.z;
		if(acceleration.y>0){
			if(acceleration.z>0){
				q = 1.0f + acceleration.y;//0~2.0, (90deg/1.0)
			}
			else{
				q = -1.0f - acceleration.y;
			}
		}
		const float Mag_AccelY = 90f;
		qy = q * -Mag_AccelY; //Direction Trim
		//End: Accel sensor


		//Begin: raw level noise reduction for compass sensor
		//magneticHeading = turn.adj180(magneticHeading);
		//noiseReduction_compass = Mathf.Lerp(noiseReduction_compass, magneticHeading, (NoiseReductionTake2Frame/60f));//
		//Lerpで -180 から 180 への飛び値用。 
		noiseReduction_compass = Mathf.LerpAngle(noiseReduction_compass, magneticHeading, (NoiseReductionTake2Frame/60f));//
		magneticHeading = noiseReduction_compass;
		Txt_Value_noiseReduction_compass.text = noiseReduction_compass.ToString("F5");
		//End: raw level noise reduction for compass sensor

		//Begin: Compass sensor
		qx = turn.adj180(magneticHeading - base_magneticHeading);; //Direction Turn ココも後で元に戻して確認（回り現象） 
		//qx = turn.adj180(magneticHeading); //Direction Turn (DEBUG)
		//Debug.Log ("qx: "+qx);
		//End: Compass sensor

		return( new Vector3(qx,qy,0) );
	}

	void test(){
		Txt_ValueAccel.text = Input.acceleration.ToString("F5");
		//The heading in degrees relative to the magnetic North Pole. (Read Only) ※北からの相対角度（平面上） 
		Txt_Value_magneticHeading_raw.text = Input.compass.magneticHeading.ToString("F5");
		//The heading in degrees relative to the geographic North Pole. (Read Only)  ※北からの相対角度（平面上） 
		//Must have location services enabled (using GPS?)
		Txt_Value_trueHeading_raw.text = Input.compass.trueHeading.ToString("F5");
		Quaternion compasRot = Quaternion.Euler(0, -Input.compass.trueHeading, 0);
		Vector3 compassDeg = compasRot.eulerAngles;
		Txt_Value_compassTrueDeg_raw.text = compassDeg.ToString("F5");



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
	float base_magneticHeading = 0f;// Front relative +degree from north pole.



	void Init_Sensor(){
		noiseReduction_acceleration = Input.acceleration;

		//StartCoroutine("Location_Start");// Initialize for Location with GPS/Wi-Fi network 

		Input.location.Start();
		Input.compass.enabled = true; //Enable compass sensor
		Input.location.Start();
		SetHome_magneticHeading();
		//base_magneticHeading = Input.compass.magneticHeading;//Always 0.
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
		#if (!UNITY_EDITOR)
		if(initial_frame){
			base_magneticHeading = Input.compass.magneticHeading;
			if(base_magneticHeading==0f){
				return;
			}
			else{
				initial_frame = false;
				//base_magneticHeading = turn.adj180(base_magneticHeading);
				noiseReduction_compass = base_magneticHeading;
			}
		}
		#endif
		Txt_Value_base_magneticHeading.text = base_magneticHeading.ToString("F5");

		KeyOperation();

		Vector3 sensor = SensorOperation();
		turn.AbsAdjustValue = sensor.x;
		trim.AbsAdjustValue = sensor.y;

		update_Turn(turn.ResultValue);
		update_Trim(trim.ResultValue);
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





