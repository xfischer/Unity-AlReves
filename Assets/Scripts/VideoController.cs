using UnityEngine;
using UnityEngine.UI;
using System.Collections;


/// <summary>
/// Applies to any textured objet with a MovieTexture on it
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class VideoController : MonoBehaviour
{

	public MovieTexture movie;

	Camera mainCamera;
	RawImage rawImage;
	AudioSource audioSource;

	void Start()
	{

		Cursor.visible = false;
		if (movie != null)
		{
			Camera mainCamera = FindObjectOfType<Camera>();
			Canvas canvasContainer = GetComponent<Canvas>();
			canvasContainer.renderMode = RenderMode.ScreenSpaceCamera;
			canvasContainer.worldCamera = mainCamera;

			rawImage = GetComponentInChildren<RawImage>();
			rawImage.texture = movie;
			audioSource = GetComponent<AudioSource>();
			audioSource.clip = movie.audioClip;

			movie.Play();
			audioSource.Play();
		}
	}

	void Update()
	{
		if (movie == null)
			return;

		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (movie.isPlaying)
			{
				movie.Pause();
				audioSource.Pause();
			}
			else {
				movie.Play();
				audioSource.Play();
			}
		}
	}

}