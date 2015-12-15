/*!
 * 	@file		ValueRange.cs 
 * 	@brief		'ValueRange' Class source file. 
 * 	@note		None 
 *	@attention 
 *  			[ValueRange.cs]										<br>
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
public class ValueRange : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	float resolution = 2f;
	float limitPlus = 40f;
	float limitMinus = -40f;
	float absAdjustValue = 0f;
	
	float resultValue = 0f;
	public float ResultValue{
		get{
			resultValue = evaluateLimit(rangeValue + absAdjustValue);
			return(resultValue);
		}
	}
	float absAdjustOffsetValue = 0f;
	public float AbsAdjustOffsetValue{
		set{absAdjustOffsetValue = value;}
		get{return(absAdjustOffsetValue);}
	}
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
