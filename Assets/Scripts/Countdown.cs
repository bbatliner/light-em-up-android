using UnityEngine;
using System.Collections;

/// <summary>
/// Draws the countdown text for a Tile.
/// </summary>
public class Countdown : MonoBehaviour {

	// The number of seconds to start the countdown at
	public int countdownTime;

	// Countdown every 1.0 seconds
	private float countdownInterval = 1.0f;
	private float nextCountdown = 0;

	// Use this for initialization
	void Start () {
		guiText.fontSize = (int) (Screen.height * 0.08f);
		resetCountdownInterval ();
	}
	
	// Update is called once per frame
	void Update () {
		// Move text to the center of the tile
		transform.position = Camera.main.WorldToViewportPoint (
			new Vector3(transform.parent.position.x + 1.0f, 0.0f, transform.parent.position.z + 1.0f)
		);

		// Countdown if it can
		if (Time.time > nextCountdown) {
			countdownTime--;
			resetCountdownInterval ();
		}

		// Destroy the counter if countdown is done
		if (countdownTime == 0) {
			Destroy(gameObject);
		}
		else {
			// Otherwise update display text
			guiText.text = countdownTime.ToString ();
		}
	}

	// Reset the current countdown timer (so it counts the full interval)
	public void resetCountdownInterval() {
		nextCountdown = Time.time + countdownInterval;
	}
}
