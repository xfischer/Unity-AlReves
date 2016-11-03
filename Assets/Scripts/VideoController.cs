using UnityEngine;
using UnityEngine.UI;
using System.Collections;


/// <summary>
/// Applies to any textured objet with a MovieTexture on it
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class VideoController : MonoBehaviour {

	public MovieTexture movie;
	public bool autoPlay;
	public bool flipVideo;
	public bool fillScreen;
	public Color backgroundColor;
	public GameObject videoPlayerPrefab;

	GameObject vidPlayerInstance;
	AudioSource audioSource;

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
				RawImage rawImage = vidPlayerInstance.GetComponentInChildren<RawImage>();
				rawImage.texture = movie;

				// Fixed width
				Vector3 videoScale = Vector3.one;
				if (!fillScreen) {
					videoScale = GetVideoScaleForScreen(new Vector2(movie.width, movie.height),
																							new Vector2(Screen.width, Screen.height));
				}
				if (flipVideo) {
					videoScale.y *= -1;
				}
				rawImage.rectTransform.localScale = videoScale;

				// Set audio
				audioSource = vidPlayerInstance.GetComponent<AudioSource>();
				audioSource.clip = movie.audioClip;

				if (autoPlay) {
					movie.Play();
					audioSource.Play();
				}
			}
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
				movie.Pause();
				audioSource.Pause();
			} else {
				movie.Play();
				audioSource.Play();
			}
		}
	}

}