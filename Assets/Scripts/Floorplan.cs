using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Floorplan : MonoBehaviour {

	public bool First;

	public Transform[] Waypoints;

	public Floorplan next;
	public Floorplan prev;

	private Transform StartPos;
	private Transform EndPos;

	public static Floorplan FirstFloorplan;

	// Use this for initialization
	void Start () {

		if (!Application.isPlaying)
			return;
		
		//Get the start and end points of this floorplan
		foreach (Transform pos in transform.GetChild(0).GetComponentInChildren <Transform>()) {
			if (pos.name == "Start")
				StartPos = pos;
			else if (pos.name == "End")
				EndPos = pos;
		}
		
	}
	
	// Update is called once per frame
	void Update () {

		//Render lines and rays for in-editor flow visualization
		for (int i = 0; i < Waypoints.Length; i++){
			Debug.DrawRay(Waypoints[i].position, Waypoints[i].right * 0.5f, Waypoints[i].name.Contains ("Skip") ? Color.grey : Color.blue);
			Debug.DrawRay(Waypoints[i].position, -Waypoints[i].up * 2, Waypoints[i].name.Contains ("Skip") ? Color.grey : Color.blue);
			if (i < Waypoints.Length - 1)
				Debug.DrawLine(Waypoints[i].position, Waypoints[i + 1].position, Color.green);
		}
		//Lines 
		if (prev)
			Debug.DrawLine(Waypoints[0].position, prev.Waypoints[prev.Waypoints.Length - 1].position, Color.yellow);

		//Set FirstFloorplan
		if (First) {
			FirstFloorplan = this;
			First = false;
		}

		//Sync prev/next floorplans
		if (next)
			next.prev = this;

		//Check during editmode if there is no StarPos or EndPos
		if (!StartPos || !EndPos) {
			//Get the start and end points of this floorplan
			foreach (Transform pos in transform.GetChild(0).GetComponentInChildren<Transform>()) {
				if (pos.name == "Start")
					StartPos = pos;
				else if (pos.name == "End")
					EndPos = pos;
			}
			//Check if both endpoints were found during the last pass, warn the user otherwise
			if (!StartPos || !EndPos)
				if (!StartPos && EndPos)
					Debug.LogError("Cound not find StartPos");
				else if (StartPos && !EndPos)
					Debug.LogError("Could not find EndPos");
				else
					Debug.LogError("Could not find StarPos nor EndPos");
			
		}

	}

}
