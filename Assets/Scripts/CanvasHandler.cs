using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasHandler : MonoBehaviour {

	public Texture Img;

	private Material mat;
	new private Transform renderer;

	// Use this for initialization
	void Start () {

		mat = GetComponentInChildren<Renderer>().material;

		mat.mainTexture = Img;

		renderer = transform.GetChild(0);

		float width = (float)Img.width / (float)Img.height;

		renderer.localScale = new Vector3(width, 1, 1);
		mat.SetTextureScale("_DetailNormalMap", new Vector2(width, 1));
		
	}

}
