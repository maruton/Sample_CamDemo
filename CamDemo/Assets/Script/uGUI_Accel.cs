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

	Text Button_Limit;
	Text Button_fps;
	CamCrane cCamCrane;
	void Start () {
		cCamCrane = gameObject.GetComponent<CamCrane>();
		Button_Limit = (GameObject.Find("Canvas/Button_Limit").GetComponent<Button>()).GetComponent<Text>();
		Button_fps = (GameObject.Find("Canvas/Button_Limit").GetComponent<Button>()).GetComponent<Text>();;

		Button_Limit = GameObject.Find("Canvas/Button_Limit/Text").GetComponent<Text>();
		Button_fps = GameObject.Find("Canvas/Button_fps/Text").GetComponent<Text>();


		Update_Params();
	}
	
	// Update is called once per frame
	void Update () {
	}

	void Update_Params(){
		Button_fps.text = fpsButtonName[fps];
		cCamCrane.SetFpsResolution(fpsParam[fps]);

		if(Limit){
			Button_Limit.text = "limitOn";
		}
		else{
			Button_Limit.text = "limitOff";
		}
		cCamCrane.SetLimitation(Limit);
	}

	int fps = 0;	//!< 0:60fps  1:30fps  2:15fps
	string[] fpsButtonName = new string[] { "fps60","fps30","fps15" };
	float[] fpsParam = new float[] { 1f,2f,4 };
	public void cb_Button_fps(){
		fps = (fps+1)%3;
		Update_Params();
	}
	public bool Limit = false;	//!< true:60  false:30
	public void cb_Button_Limit(){
		Limit = !Limit;
		Update_Params();
	}
	public void cb_Home(){
		cCamCrane.SetHome_magneticHeading();
	}
}
