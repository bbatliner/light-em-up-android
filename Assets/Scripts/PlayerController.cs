using UnityEngine;
using System.Collections;

/// <summary>
/// Controls player movement with the accelerometer.
/// </summary>
public class PlayerController : MonoBehaviour {

	public float speed;

	void FixedUpdate () {
		// Read accelerometer
		float moveHorizontal = Input.acceleration.x;
		float moveVertical = Input.acceleration.y;

		// LOCAL DEVELOPMENT: to generate movement
//		moveHorizontal = Random.Range(0.85f, 0.9f);
//		moveVertical = Random.Range(-0.05f, 0.0f);

		// Create acceleration vector
		Vector3 movement = new Vector3 (moveHorizontal, 0, moveVertical);

		// Add acceleration
		rigidbody.AddForce (movement * speed * Time.deltaTime);

		// Prevent positive vertical motion
		rigidbody.velocity = new Vector3 (
			rigidbody.velocity.x, 
			Mathf.Clamp(rigidbody.velocity.y, float.NegativeInfinity, 0.0f),
		    rigidbody.velocity.z
		);
	}
}
