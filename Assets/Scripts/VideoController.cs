using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;

/// <summary>
/// Applies to any textured objet with a MovieTexture on it
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class VideoController : MonoBehaviour {

	public MovieTexture movie;
	public bool autoPlay;
	[Range(0, 1)]
	public float volume;
	public bool flipVideo;
	public bool fillScreen;
	public Color backgroundColor;
	public GameObject videoPlayerPrefab;

	GameObject vidPlayerInstance;
	RawImage rawImage;
	AudioSource audioSource;
	Vector3 videoScaleForPreserveAspect = Vector3.one;

	bool isInitialized;

	void Start() {

		if (videoPlayerPrefab != null) {

			vidPlayerInstance = Instantiate<GameObject>(videoPlayerPrefab);
			vidPlayerInstance.transform.SetParent(this.transform);

			Cursor.visible = false;
			if (movie != null) {
				Camera mainCamera = FindObjectOfType<Camera>();
				mainCamera.backgroundColor = backgroundColor;

				// Set render camera on canvas
				Canvas canvasContainer = vidPlayerInstance.GetComponent<Canvas>();
				canvasContainer.renderMode = RenderMode.ScreenSpaceCamera;
				canvasContainer.worldCamera = mainCamera;

				// Set video
				rawImage = vidPlayerInstance.GetComponentInChildren<RawImage>();
				rawImage.texture = movie;

				// Fixed width
				videoScaleForPreserveAspect = GetVideoScaleForScreen(new Vector2(movie.width, movie.height),
																							new Vector2(Screen.width, Screen.height));
				SetVideoScale(videoScaleForPreserveAspect, flipVideo, fillScreen);


				// Set audio
				audioSource = vidPlayerInstance.GetComponent<AudioSource>();
				audioSource.clip = movie.audioClip;
				audioSource.volume = volume;

				if (autoPlay) {
					Play();
				}

				isInitialized = true;
			}
		}
	}

	void SetVideoScale(Vector3 scaleToFit, bool yFlip, bool bfillScreen) {

		Vector3 vidScale;
		if (bfillScreen == false) {
			vidScale = new Vector3(scaleToFit.x, scaleToFit.y * (yFlip ? -1f : 1f), scaleToFit.z);
		} else {
			vidScale = new Vector3(1, yFlip ? -1f : 1f, 1);
		}
		rawImage.rectTransform.localScale = vidScale;
	}
	public void UpdateFromEditor() {

		if (isInitialized) {
			audioSource.volume = volume;
			//Camera mainCamera = FindObjectOfType<Camera>();
			//mainCamera.backgroundColor = backgroundColor;

			//// Set render camera on canvas
			//Canvas canvasContainer = vidPlayerInstance.GetComponent<Canvas>();
			//canvasContainer.renderMode = RenderMode.ScreenSpaceCamera;
			//canvasContainer.worldCamera = mainCamera;

			//// Fixed width
			//SetVideoScale(videoScaleForPreserveAspect, flipVideo, fillScreen);

			//if (movie.isPlaying) {
			//	audioSource.volume = volume;
			//}

		}
	}

	Vector3 GetVideoScaleForScreen(Vector2 videoSize, Vector2 screenSize) {
		float screenRatio = screenSize.x / screenSize.y;
		float videoRatio = videoSize.x / videoSize.y;
		Vector3 videoScaling;
		if (screenRatio < videoRatio) {
			videoScaling = new Vector3(1, screenRatio / videoRatio, 1);
		} else {
			videoScaling = new Vector3(videoRatio / screenRatio, 1, 1);
		}
		return videoScaling;
	}

	void Update() {
		if (movie == null)
			return;

		if (Input.GetKeyDown(KeyCode.Space)) {
			if (movie.isPlaying) {
				Pause();
			} else {
				Play();
			}
		}
	}

	void Play() {
		if (movie != null && !movie.isPlaying) {
			movie.Play();
			audioSource.Play();
		}
	}
	void Pause() {
		if (movie != null && movie.isPlaying) {
			movie.Pause();
			audioSource.Pause();
		}
	}

}

