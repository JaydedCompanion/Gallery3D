using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	public AudioClip Woosh;

	public static AudioClip sWoosh;

	public static List<KeyValuePair<Transform, AudioSource>> Stickies = new List<KeyValuePair<Transform, AudioSource>> ();

	public void Update () {

		foreach (KeyValuePair <Transform, AudioSource> pair in Stickies)
			pair.Value.transform.position = Vector3.Lerp (pair.Value.transform.position, pair.Key.position, Time.deltaTime * (ViewerControl.Instance ? ViewerControl.Instance.LerpSpeed : 0.5f) * 30);

	}

	// Use this for initialization
	void Start () {

		sWoosh = Woosh;
		
	}

	public static AudioSource playWoosh () {
		return PlayClipAtPoint (sWoosh, Camera.main.transform.position);
	}

	public static AudioSource playWoosh (Transform pos, bool sticky) {
		AudioSource instance = PlayClipAtPoint (sWoosh, pos.position);
		Stickies.Add (new KeyValuePair<Transform, AudioSource> (pos, instance));
		return instance;
	}

	public static AudioSource PlayClipAtPoint (AudioClip sound, Vector3 pos) {

		AudioSource instance = new GameObject ("AudioSource (" + sound + ")").AddComponent<AudioSource> ();

		instance.transform.position = pos;
		instance.clip = sound;
		instance.Play ();

		Destroy (instance.gameObject, instance.clip.length + 1f);

		return instance;

	}

}
