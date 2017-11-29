using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewerControl : MonoBehaviour {

	public float LerpSpeed;

	public Floorplan CurrentFloorplan;
	public int CurrentImage;
	public int lastDir = 1;

	public static ViewerControl Instance;

	// Use this for initialization
	void Start () {

		CurrentFloorplan = Floorplan.FirstFloorplan;

		Instance = this;
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetButtonDown ("Prev"))
            PrevImage();
		else if (Input.GetButtonDown ("Next"))
            NextImage();

		if (!CurrentFloorplan)
			CurrentFloorplan = Floorplan.FirstFloorplan;

		/* 
		 * Update the viewer location.
         * This is called every frame 
         * but is its own method to 
         * keep the main Update() 
         * method clean. 
		*/
		UpdateLocation();
		
	}

	private void UpdateLocation () {

		Transform destination = CurrentFloorplan.Waypoints[CurrentImage];

		//What do we do if we finish moving tp the destination
		if (isSame(transform.position, destination.position, 0.5f))
			//If the destination waypoint is simply a skippable transition, proceed to the next waypoint
			if (destination.name.Contains("Skip"))
				if (lastDir > 0)
					NextImage();
			else if (lastDir < 0)
					PrevImage();

		if (isSame(transform.position, destination.position, 0.00001f))
			return;

		transform.position = Vector3.Lerp(transform.position, destination.position, Time.deltaTime * LerpSpeed * 10);
		transform.rotation = Quaternion.Lerp(transform.rotation, CurrentFloorplan.Waypoints[CurrentImage].rotation, Time.deltaTime * LerpSpeed * 10);

	}

	public void PrevImage() {
		lastDir = -1;
		//If we're not on the first image, simply decrease the image index
		if (CurrentImage > 0)
			CurrentImage--;
		//If we're on the first image, take us to the previous floorplan and make the image index the last image
		else {
			if (CurrentFloorplan.prev) {
				CurrentFloorplan = CurrentFloorplan.prev;
				CurrentImage = CurrentFloorplan.Waypoints.Length - 1;
			} else {
				//TODO Add proper UI for this scenario
				Debug.Log("First image, cannot reverse");
				lastDir = 0;
			}
		}
		AudioManager.playWoosh ();
	}

	public void NextImage() {
		lastDir = 1;
		//If we're not on the last image, simply increase the image index
		if (CurrentImage < CurrentFloorplan.Waypoints.Length - 1)
			CurrentImage++;
		
		//If we're on the last image, take us to the next floorplan and reset the image index
		else {
			if (CurrentFloorplan.next) {
				CurrentFloorplan = CurrentFloorplan.next;
				CurrentImage = 0;
			} else {
				//TODO Add proper UI for this scenario
				Debug.Log("Last image, cannot proceed");
				lastDir = 0;
			}
		}
		AudioManager.playWoosh ();
	}

	public bool isSame(Quaternion val1, Quaternion val2, float f) {
		return isSame(val1.eulerAngles, val2.eulerAngles, f);
	}

	public bool isSame (Vector3 val1, Vector3 val2, float f) {
		return isSame(val1.x, val2.x, f) && isSame(val1.y, val2.y, f) && isSame(val1.z, val2.z, f);
	}

	public bool isSame (float val1, float val2, float f) {
		return Mathf.Abs(val1 - val2) < f;
	}

}
