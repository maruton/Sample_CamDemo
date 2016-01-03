using UnityEngine;
using System.Collections;
using System.Collections.Generic; //need gor List
using UnityEngine.UI; //need for uGUI
using UnityEngine.EventSystems; //need for EventTrigger 

class uGUI_Accel: MonoBehaviour {

	// Use this for initialization

	void Awake(){
		/*
		GameObject go;
		Button btn;
		go = GameObject.Find("Canvas/ButtonV2");
		btn = go.GetComponentInChildren<Button>();
		// EventTrigger for button.
		EventTrigger trigger_button;
		if( (trigger_button = go.GetComponent<EventTrigger>())!=null) Destroy(trigger_button);
		trigger_button = go.AddComponent<EventTrigger>();
		trigger_button.triggers = new List<EventTrigger.Entry>();
		
		EventTrigger.Entry entry;
		
		entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerDown;//OnEdge
		entry.callback.AddListener( (x) => { Button_v2(); } );
		trigger_button.triggers.Add (entry);
		*/
		/*
		entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerUp;//OffEdge
		entry.callback.AddListener( (x) => { Button_v2(); } );
		trigger_button.triggers.Add (entry);
		*/
	}

	CamCrane cCamCrane;
	void Start () {
		cCamCrane = gameObject.GetComponent<CamCrane>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void SetMode_Quick(){
		cCamCrane.JittterReductionA = true;
		cCamCrane.JittterReductionB = false;
	}
	void SetMode_Smooth(){
		cCamCrane.JittterReductionA = false;
		cCamCrane.JittterReductionB = true;
	}
	public void Button_Smooth60(){
		SetMode_Smooth();
		cCamCrane.SmoothFrame = 1f;
	}
	public void Button_Smooth30(){
		SetMode_Smooth();
		cCamCrane.SmoothFrame = 2f;
	}

	public float ParamV = 1.0f;
	public float ParamH = 1.0f;

	public void Button_v2(){
		SetMode_Quick();
		ParamV = 0.2f;
	}
	public void Button_v4(){
		SetMode_Quick();
		ParamV = 0.4f;
	}
	public void Button_v6(){
		SetMode_Quick();
		ParamV = 0.6f;
	}
	public void Button_v8(){
		SetMode_Quick();
		ParamV = 0.8f;
	}
	public void Button_v10(){
		SetMode_Quick();
		ParamV = 1.0f;
	}
	public void Button_h2(){
		SetMode_Quick();
		ParamH = 0.2f;
	}
	public void Button_h4(){
		SetMode_Quick();
		ParamH = 0.4f;
	}
	public void Button_h6(){
		SetMode_Quick();
		ParamH = 0.6f;
	}
	public void Button_h8(){
		SetMode_Quick();
		ParamH = 0.8f;
	}
	public void Button_h10(){
		SetMode_Quick();
		ParamH = 1.0f;
	}
}
