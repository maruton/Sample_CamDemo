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
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public float ParamV = 1.0f;
	public float ParamH = 1.0f;

	public void Button_v2(){
		ParamV = 0.2f;
		Debug.Log ("Button_v2");
	}
	public void Button_v4(){
		ParamV = 0.4f;
	}
	public void Button_v6(){
		ParamV = 0.6f;
	}
	public void Button_v8(){
		ParamV = 0.8f;
	}
	public void Button_v10(){
		ParamV = 1.0f;
	}
	public void Button_h2(){
		ParamH = 0.2f;
	}
	public void Button_h4(){
		ParamH = 0.4f;
	}
	public void Button_h6(){
		ParamH = 0.6f;
	}
	public void Button_h8(){
		ParamH = 0.8f;
	}
	public void Button_h10(){
		ParamH = 1.0f;
	}
}
