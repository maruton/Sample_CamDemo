/*!
 * 	@file		CalcDifferenceDegree.cs 
 * 	@brief		'CalcDifferenceDegree' Class source file. 
 * 	@note		None 
 *	@attention 
 *  			[CalcDifferenceDegree.cs]							<br>
 *				Copyright (c) [2015] [Maruton] 						<br>
 *				This software is released under the MIT License. 	<br>
 *				http://opensource.org/licenses/mit-license.php 		<br>
 */
using UnityEngine;
using System.Collections;

public class CalcDiffDegree : MonoBehaviour {
	public GameObject go_SourceUnit;
	public GameObject go_TargetUnity;
	
	void Start () {
	}
	
	public float DeltaDegree(GameObject go_src, GameObject go_dst){
		Vector3 srcPos = go_src.transform.position;
		Vector3 dstPos = go_dst.transform.position;
		
		Vector3 relativeDegree = (dstPos - srcPos).normalized;
		//Debug.Log ("srcPos="+srcPos+" dstPos="+dstPos+" relativeDegree="+relativeDegree);
		Vector3 srcforward = transform.forward;//Direction forward src.
		float angle = Vector3.Angle(relativeDegree , srcforward);
		//Debug.Log ("angle="+angle+" srcforward="+srcforward);
		return(angle);
	}
	public float DeltaDegree(Vector3 srcPos, Vector3 dstPos){
		Vector3 relativeDegree = (dstPos - srcPos).normalized;
		//Debug.Log ("srcPos="+srcPos+" dstPos="+dstPos+" relativeDegree="+relativeDegree);
		Vector3 srcforward = transform.forward;//Direction forward src.
		float angle = Vector3.Angle(relativeDegree , srcforward);
		//Debug.Log ("angle="+angle+" srcforward="+srcforward);
		return(angle);
	}
	public float DeltaDegree(Quaternion srcQua, Quaternion dstQua){
		return(DeltaDegree(srcQua.eulerAngles, dstQua.eulerAngles));
	}

	//void Update () {
	//	float deltaAngle = DeltaDegree(go_SourceUnit, go_TargetUnity);
	//}
}
