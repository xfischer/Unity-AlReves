using UnityEngine;
using UnityEngine.UI;
using System.Collections;


/// <summary>
/// Applies to any textured objet with a MovieTexture on it
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class VideoController : MonoBehaviour {

	public MovieTexture movie;
	private AudioSource audioSource;

	void Start() {

		Cursor.visible = false;
		GetComponent<RawImage>().texture = movie;
		audioSource = GetComponent<AudioSource>();
		audioSource.clip = movie.audioClip;

		movie.Play();
		audioSource.Play();
	}

	void Update() {
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