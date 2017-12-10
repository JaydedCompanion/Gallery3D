using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.IO;
using System.Linq;

public class ImageRegistry : MonoBehaviour {

	public static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG" };

	private static string ImageLocation = Application.dataPath;
	private static bool PlatformDependentSetup;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
#if UNITY_EDITOR
	[MenuItem("Image Registry/Load Images (Force Perform First-Time Setup)")]
#endif
	public static void UpdateImagesAndDirectory() {

		PlatformDependentSetup = false;

		UpdateImages();

	}
#if UNITY_EDITOR
	[MenuItem("Image Registry/Load Images")]
#endif
	public static void UpdateImages() {
		UpdateImages(false);
	}
#if UNITY_EDITOR
	[MenuItem("Image Registry/Load Images + Delete Inconpatibles")]
#endif
	public static void UpdateImagesRemoveIncompatibles() {
		UpdateImages(true);
	}

	public static void UpdateImages(bool RemoveIncompatibles) {

		if (!PlatformDependentSetup) {

			Debug.Log("Performing first-time setup");

			if (Application.platform == RuntimePlatform.OSXPlayer) {
				ImageLocation += "/../../";
			} else if (Application.platform == RuntimePlatform.WindowsPlayer) {
				ImageLocation += "/../";
			} else if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor) {
				ImageLocation += "/../";
			}

			ImageLocation += "/GalleryImages/";

			if (!Directory.Exists (ImageLocation)) {

				Directory.CreateDirectory(ImageLocation);

				Debug.LogWarning("Image container directory created at \"" + ImageLocation + "\". Please populate it with images to load into the gallery.");

			}

			PlatformDependentSetup = true;

		} else 
			Debug.Log("Skipping first-time setup");

		DirectoryInfo ImageContainerInfo = new DirectoryInfo(ImageLocation);
		List <FileInfo> ImageContentsInfo = ImageContainerInfo.GetFiles().ToList();

		string dataComp = "";
		for (int i = 0; i < ImageContentsInfo.Count; i++){
			
			FileInfo info = ImageContentsInfo[i];

			if (ImageExtensions.Contains(Path.GetExtension(info.FullName).ToUpperInvariant())) {
				dataComp += "isImage = true \t";
				dataComp += "File Found: " + info.Name + "\n";
			} else {
				dataComp += "isImage = false \t";
				dataComp += "File Found: " + info.Name;
				if (RemoveIncompatibles){
					Debug.LogWarning("Deleting non-image file \"" + info.Name + "\"");
					File.Delete(info.FullName);
					dataComp += "\t DELETED";
				}
				ImageContentsInfo.Remove(info);
				i--;
				dataComp += "\n";
			}

		}

		Debug.Log(dataComp);

		LevelGenerator.Instance.Paintings = FileInfoToTexture(ImageContentsInfo);

	}

	//Convert array of FileInfo to Textures
	public static Texture[] FileInfoToTexture (List <FileInfo> files) {

		Texture[] textures = new Texture[files.Count];

		for (int i = 0; i < files.Count; i ++) {

			Texture2D texture = new Texture2D(4, 4);

			FileStream fs = new FileStream(files[i].FullName, FileMode.Open, FileAccess.Read);
			byte[] imageData = new byte[fs.Length];
			fs.Read(imageData, 0, (int) fs.Length);
			texture.LoadImage(imageData);

			textures[i] = texture;

		}

		return textures;

	}

}
