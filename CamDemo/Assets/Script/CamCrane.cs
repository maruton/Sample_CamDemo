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
 * 	@brief		Value range class <br>
 * 	@attention	None
 * 	@note		None
 * 	@author		Maruton.
 */
public class ValueRange {// range -180 ~ 0 ~ +180

	float resolution = 2f;
	float limitPlus = 40f;
	float limitMinus = -40f;
	float absAdjustValue = 0;

	float resultValue = 0;
	public float ResultValue{
		get{
			resultValue = evaluateLimit(rangeValue + absAdjustValue);
			return(resultValue);
		}
	}
	public float AbsAdjustOffsetValue = 0;
	public float AbsAdjustValue{
		set{absAdjustValue = value - AbsAdjustOffsetValue;}
		get{return(absAdjustValue);}
	}
	float rangeValue = 0;
	public float RangeValue{
		set{rangeValue = value;}
		get{return(rangeValue);}
	}
	public bool limitation = false;
	public void SetResolution(float a){
		resolution = a;
	}
	public void SetRange(float minus, float plus){
		limitMinus = minus; limitPlus = plus;
	}
	public void Add(){
		rangeValue += resolution;
		if(rangeValue>180f) rangeValue = -180f + (rangeValue - 180f);
		rangeValue = evaluateLimit(rangeValue);
	}
	public void Sub(){
		rangeValue -= resolution;
		if(rangeValue<-180f) rangeValue = 180f + (rangeValue + 180f);
		rangeValue = evaluateLimit(rangeValue);
	}
	float evaluateLimit(float a){
		if(limitation){
			if(a<=limitMinus) a = limitMinus;
			if(a>=limitPlus) a = limitPlus;
		}
		return(a);
	}
	float adj180(float a){
		if(a>180f){
			while(a>180f) a-= 180f;
			a = -180f + a;
		}
		else if(a<-180f){
			while(a<-180f) a+= 180f;
			a = 180f - a;
		}
		return(a);
	}
}

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


	/*!	Initial procedure.
	 * 	Initial class for degree. 
   	 * 	@note		 	None
   	 * 	@attention		None 
   	 */	
	void Init(){
		turn.RangeValue = 0;
		turn.SetResolution(2f);		//Moving step
		turn.SetRange(-80f, 80f);	//Left,Right
		turn.limitation = true;		//Limit sw

		trim.RangeValue = 0;
		trim.SetResolution(2f);		//Moving step
		trim.SetRange(-10f, 35f);	//Down,Up
		trim.limitation = true;		//Limit sw
		trim.AbsAdjustOffsetValue = 20;

		zoom.RangeValue = 3.3f;
		zoom.SetResolution(0.05f);	//Moving step
		zoom.SetRange(1.6f, 5f);	//Zoom up, Zoom down
		zoom.limitation = true;		//Limit sw
	}
	ValueRange zoom = new ValueRange(); //!< Control to range for zoom.
	ValueRange turn = new ValueRange(); //!< Control to range for turn.
	ValueRange trim = new ValueRange();	//!< Control to range for trim.
	Quaternion HorizontalBaseRot = Quaternion.Euler(0,0,90);//!< Model offset for model.
	Quaternion HorizontalOffsetRot = Quaternion.identity;	//!< Turn offset for moving.
	void update_Turn(){
		//Debug.Log (turn.RangeValue);
		turn.AbsAdjustValue = AccelX;
		HorizontalOffsetRot = Quaternion.Euler(turn.ResultValue,0,0);
		go_CamHorizontal.transform.rotation = HorizontalBaseRot * HorizontalOffsetRot;
		//go_CamHorizontal.transform.rotation = HorizontalBaseRot * HorizontalOffsetRot * HorizontalAccelRot;
		//go_CamHorizontal.transform.rotation = HorizontalBaseRot;// * HorizontalOffsetRot;
	}
	Quaternion VerticalBaseRot = Quaternion.Euler(0,0,0);	//!< Trim offset for model.
	Quaternion VerticalOffsetRot = Quaternion.identity;		//!< Trim offset for moving.
	void update_Trim(){
		//Debug.Log (trim.RangeValue);
		//Debug.Log (trim.ResultValue);
		trim.AbsAdjustValue = AccelY;
		VerticalOffsetRot = Quaternion.Euler(0,trim.ResultValue,0);
		go_CamVertical.transform.localRotation = VerticalBaseRot * VerticalOffsetRot;
		//go_CamVertical.transform.localRotation = VerticalBaseRot * VerticalOffsetRot * VerticalAccelRot;
		//go_CamVertical.transform.localRotation = VerticalBaseRot * VerticalOffsetRot;
	}
	void update_Zoom(){
		//Debug.Log (zoom.RangeValue);
		go_CamZoom.transform.localPosition = new Vector3(0,0,zoom.RangeValue + AccelZoom);
		
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

	const float VerticalStep = 2f; //!< Resolution for camera lookup and lookdown.
	Quaternion rotVertical = Quaternion.identity;
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
	//-----
	GameObject go_CamHorizontal;  //!< Object for turn Horizontal.
	GameObject go_CamVertical; //!< Object for turn vertical.
	GameObject go_CamZoom;	   //!< Object for zooming.

	//-----
	void Awake(){
		go_CamHorizontal = GameObject.Find("CamHorizontal");
		go_CamVertical = GameObject.Find("CamVertical");
		go_CamZoom = GameObject.Find("CamZoom");
		Init();

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
	//Quaternion HorizontalAccelRot = Quaternion.identity;
	//Quaternion VerticalAccelRot = Quaternion.identity;

	const float LimitVertical = 0.5f;

	const float LimitZoom = 0.5f;
	float AccelZoom = 0;
	const float Mag_AccelZoom = 1f;

	Vector3 before_acceleration = new Vector3(0,0,0);
	int chatterX = 0;	int before_direcX = 0;	int direcX = 0;
	int chatterY = 0;	int before_direcY = 0;	int direcY = 0;
	/*!	Accelerate operation.
	 *	Accelerate operation.
   	 * 	@note		 	None
   	 * 	@attention		None 
   	 */	
	void AccelerateOperation(){
		float diff;
		Vector3 acceleration = Input.acceleration;
		//Debug.Log (acceleration);

		bool enableX = false;
		bool enableY = false;

		//Exclusion hand jitter X
		if(acceleration.x>before_acceleration.x) direcX = 1;
		else if(acceleration.x>before_acceleration.x) direcX = -1;
		else direcX = 0;
		if(before_direcX==direcX) chatterX++;
		if(chatterX>8) enableX = true;// Adjustable feel for exclusion hand jitter.(1/30frame*n)

		//Exclusion hand jitter Y
		if(acceleration.y>before_acceleration.y) direcY = 1;
		else if(acceleration.y>before_acceleration.y) direcY = -1;
		else direcY = 0;
		if(before_direcX==direcY) chatterY++;
		if(chatterY>12) enableY = true;// Adjustable feel for exclusion hand jitter.(1/30frame*n)


		if(enableX){
			Raw_AccelX = acceleration.x;
			diff = Raw_Before_AccelX - Raw_AccelX;
			//if(acceleration.x>SensorXLimit) 		Entry_Turn_Left();
			//else if(acceleration.x<-SensorXLimit) 	Entry_Turn_Right();
			if(Mathf.Abs(diff)>0.01f) {
				AccelX = acceleration.x * Mag_AccelX; //Exclision Chatter
				//HorizontalAccelRot = Quaternion.Euler(new Vector3(AccelX, 0, 0));
			}
			Raw_Before_AccelX = Raw_AccelX;
		}
		if(enableY){
			Raw_AccelY = acceleration.y;
			diff = Raw_Before_AccelY - Raw_AccelY;
			
			//if(acceleration.y>SensorYLimit) 		Entry_LookUp();
			//else if(acceleration.y<-SensorYLimit) 	Entry_LookDown();
			
			if(Mathf.Abs(diff)>0.01f) {
				AccelY = acceleration.y * -Mag_AccelY; //Exclision Chatter
				//VerticalAccelRot = Quaternion.Euler(new Vector3(0, AccelY, 0));
			}
			Raw_Before_AccelY = Raw_AccelY;
		}
		before_acceleration = acceleration;
	}

	void Update () {
		KeyOperation();
		AccelerateOperation();

		update_Turn();
		update_Trim();
		update_Zoom();

	}
}

