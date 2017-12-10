using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class LevelGenerator : MonoBehaviour {

	public Texture[] Paintings;
	public Texture MissingPainting;

	public Dictionary<Floorplan, int> FloorplanPrefabs = new Dictionary<Floorplan, int>();
	public List<Floorplan> LevelFloorplans = new List<Floorplan>();

	public static LevelGenerator Instance;

	// Use this for initialization
	void Start () {

		if (!Application.isPlaying)
			return;

		Instance = this;

		ImageRegistry.UpdateImagesRemoveIncompatibles();

		LoadPrefabs();

		List<Floorplan> FloorplanList = GenerateLevelLayout(Paintings.Length);

		LevelFloorplans = GenerateLevel(FloorplanList);

		int imgIndex = 0;
		foreach (Floorplan fp in LevelFloorplans) {

			foreach (CanvasHandler canvas in fp.PaintingHolder.GetComponentsInChildren<CanvasHandler>()) {

				if (imgIndex < Paintings.Length)
					canvas.Img = Paintings[imgIndex];
				else
					canvas.Img = MissingPainting;

				imgIndex++;

			}

		}

	}

	public void LoadPrefabs() {

		Object[] FPPrefabs = new Object[0];
		FPPrefabs = Resources.LoadAll("Floorplans/", typeof(GameObject));
		foreach (Object obj in FPPrefabs) {
			GameObject gObj = null;
			try {
				gObj = (GameObject)obj;
			} catch {
				continue;
			}
			if (gObj.name != "FP_Wall" && gObj.GetComponent<Floorplan>() && !FloorplanPrefabs.ContainsKey(gObj.GetComponent<Floorplan>()))
				FloorplanPrefabs.Add(gObj.GetComponent<Floorplan>(), gObj.GetComponent<Floorplan>().getImageCount());

		}

	}

	public List<Floorplan> GenerateLevelLayout(int Paintings) {

		int paintings = 0;
		int startFPID = 0;

		List<Floorplan> FloorplanList = FloorplanPrefabs.Select(pair => pair.Key).ToList();
		List<Floorplan> layout = new List<Floorplan>();

		foreach (Floorplan fp in FloorplanList)
			if (fp.name == "FP_End") {
				startFPID = FloorplanList.IndexOf(fp);
				break;
			}

		while (paintings < Paintings) {

			Floorplan next = FloorplanList[startFPID];

			if (layout.Count > 0){

				int rand = startFPID;

				while (rand == startFPID)
					rand = Random.Range(0, FloorplanList.Count);

				next = FloorplanList[rand];
			
			}

			layout.Add(next);

			Debug.Log("Added room " + next.name + ", which contains " + next.getImageCount() + " image slots.");

			paintings += next.getImageCount();

		}

		//Add a wall at the end of the level layout
		layout.Add(((GameObject)Resources.Load("Floorplans/FP_Wall", typeof(GameObject))).GetComponent<Floorplan>());

		return layout;

	}

	public List <Floorplan> GenerateLevel (List <Floorplan> Layout) {

		List<Floorplan> Instances = new List<Floorplan>();

		for (int i = 0; i < Layout.Count; i++) {

			Instances.Add (Instantiate (Layout[i]));

			if (i > 0) {
				Instances[i].transform.position = AlignFloorplan(Instances[i], Instances[i - 1]);
				Instances[i - 1].next = Instances[i];
			} else
				Floorplan.FirstFloorplan = Instances[i];
		}

		return Instances;

	}

	public Vector3 AlignFloorplan (Floorplan b, Floorplan a) {

		a.setStartEndPos();
		b.setStartEndPos();

		Vector3 NewPos = Vector3.zero;

		NewPos = a.EndPos.position + (b.transform.position - b.StartPos.position);

		return NewPos;

	}

	// Update is called once per frame
	void Update () {
		Instance = this;
	}

}
