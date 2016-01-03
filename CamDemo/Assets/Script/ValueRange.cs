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

	float resolution = 2f;		//!< Additional/Subtraction value for degree.
	float limitPlus = 40f;		//!< Limit degree value for positive degree.
	float limitMinus = -40f;	//!< Limit degree value for negative degree.

	float resultValue = 0f;		//!< Result value. 
	public float ResultValue{
		get{
			resultValue = adj180(evaluateLimit(rangeValue + absAdjustValue)+offsetValue);
			return(resultValue);
		}
	}
	float offsetValue = 0f; //!< offset degree.
	/*!
 *  @brief		Accessor for offsetValue
 * 	@attention	None
 * 	@note		Offset value.
 * 	@author		Maruton.
 */
	public float OffsetValue{
		set{offsetValue = value;}
		get{return(offsetValue);}
	}

	float absAdjustOffsetValue = 0f; //!< Abusolute offset degree.(that effective by limit range too)
 /*!
 *  @brief		Accessor for absAdjustOffsetValue
 * 	@attention	None
 * 	@note		Usable any sensor that dection absolute value, setup default heading vector.
 * 	@author		Maruton.
 */
	public float AbsAdjustOffsetValue{
		set{absAdjustOffsetValue = value;}
		get{return(absAdjustOffsetValue);}
	}
	float absAdjustValue = 0f;	//!< Abusolute offset degree.(that effective by limit range too)
	/*!
 *  @brief		Accessor for absAdjustValue
 * 	@attention	None
 * 	@note		Usable any sensor that dection absolute value, moving control to heading vector.
 * 	@author		Maruton.
 */
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

 /*!
 * 	@brief		Adjust range to value of degree -180 to 180 <br>
 * 	@attention	None
 * 	@note		None
 * 	@author		Maruton.
 */
	public float adj180(float a){
		while(a>360f) a-= 360f;
		while(a<-360f) a+= 360f;
		if(a>180f){
			a = (a-180f) - 180f;
		}
		else if(a<-180f){
			a = (a+180f) + 180f;
		}
		return(a);
	}
	/*
	public float adj180(float a){
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
	*/
}
