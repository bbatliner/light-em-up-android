using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the animation states of the Tile object.
/// </summary>
public class TileAnimator : MonoBehaviour {

	// The reference to the Countdown prefab
	public Countdown countdown;
	// The time that this Tile's Countdowns will begin at
	public int countdownTime;

	// This tile's Animator component
	private Animator animator;
	// The reference to this tile's Countdown
	private Countdown myCountdown;

	// When the script loads
	void Awake () {
		// The reason this is here and not in Start() is
		// because I was having issues with `animator` being
		// "null" when OnTriggerEnter() is fired. This happens
		// when the Player is instantiated ON a tile, and a trigger
		// event is called before the first Update can call Start().
		// Moving this to Awake() ensures that `animator` is defined
		// from the beginning of the script's existence.
		animator = GetComponent<Animator> ();
	}

	// Update is called once per frame
	void Update() {
		// Change the state of the tile depending on
		// whether or not it is currently counting down
		if (!hasCountdown()) {
			animator.SetBool ("IsActive", false);
		}
		else {
			animator.SetBool ("IsActive", true);
		}
	}

	// When the player enters the tile
	void OnCollisionEnter() {
		// Start animation to Active state
		animator.SetBool("IsActive", true);

		// If the tile is free
		if (!hasCountdown()) {
			// Create a new child Countdown
			myCountdown = (Countdown) Instantiate(countdown);
			myCountdown.transform.parent = transform;
			myCountdown.countdownTime = countdownTime;
		}
		else {
			// Otherwise reset the timer of the existing Countdown
			myCountdown.countdownTime = countdownTime;
			myCountdown.resetCountdownInterval();
		}
	}

	// If the player stays on the tile
	void OnCollisionStay() {
		// If there's a countdown here,
		if (hasCountdown ()) {
			// Reset its timer
			myCountdown.countdownTime = countdownTime;
			myCountdown.resetCountdownInterval();
		}
	}

	public bool hasCountdown() {
		return transform.childCount != 0;
	}

	public void removeCountdown() {
		if (hasCountdown()) {
			Destroy(myCountdown.gameObject);
		}
	}
}
