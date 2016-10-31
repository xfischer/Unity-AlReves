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
	public bool autoPlay;
	public Color backgroundColor;

	Camera mainCamera;
	RawImage rawImage;
	AudioSource audioSource;

	void Start()
	{

		Cursor.visible = false;
		if (movie != null)
		{
			Camera mainCamera = FindObjectOfType<Camera>();
			mainCamera.backgroundColor = backgroundColor;

			// Set render camera on canvas
			Canvas canvasContainer = GetComponent<Canvas>();
			canvasContainer.renderMode = RenderMode.ScreenSpaceCamera;
			canvasContainer.worldCamera = mainCamera;

			// Set video
			rawImage = GetComponentInChildren<RawImage>();
			rawImage.texture = movie;

			// Fixed width
			int sourceWidth = movie.width;
			int sourceHeight = movie.height;

			float nPercent = 0;
			float nPercentW = 0;
			float nPercentH = 0;

			nPercentW = ((float)Screen.width / (float)sourceWidth);
			nPercentH = ((float)Screen.height / (float)sourceHeight);
			if (nPercentH < nPercentW)
			{
				nPercentH = 1;
			}
			else
			{
				nPercentW = 1;
			}

			int destWidth = (int)(sourceWidth * nPercentW);
			int destHeight = (int)(sourceHeight * nPercentH);

			rawImage.rectTransform.localScale = new Vector3(nPercentW, nPercentH, 1);



			print("Movie size" + movie.width + " x " + movie.height + " (ratio = " + movie.width/(float)movie.height);
			print("Camera ratio" + mainCamera.aspect);

			// Set audio
			audioSource = GetComponent<AudioSource>();
			audioSource.clip = movie.audioClip;

			if (autoPlay)
			{
				movie.Play();
				audioSource.Play();
			}
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