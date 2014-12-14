using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Controls moving between scenes, drawing the GUIs, and high level object management.
/// </summary>
public class GameManager : MonoBehaviour {
	
	// Prefabs and other things
	public GameObject grassTile;
	public GameObject player;
	public Material[] playerMaterials;
	public Sprite titleSprite;
	public Sprite exitSprite;

	// Custom GUI
	public GUISkin customGUISkin;

	// Because the pivot points of the GrassTile sprite
	// and the Block texture are different, these handle
	// offsetting the Block transform position to be on
	// the same coordinate plane as the GrassTiles
	public float blockXOffset;
	public float blockYOffset;

	// Instances of prefabs
	private List<GameObject> myGrassTiles = new List<GameObject>();
	private List<GameObject> myPlayers = new List<GameObject>();

	// Private variables
	private int lastLevel;
	private string lastLevelName;
	private List<Vector3> playerStartPositions = new List<Vector3>();

	// When the script loads
	void Awake() {
		// Persist the GameManager through levels
		DontDestroyOnLoad (gameObject);
	}

	// Use this for initialization
	void Start () {
		// Prevent the screen from dimming
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		// Load current game data
		SaveLoad.load();
		// Adjust player texture (will then be handled by OnLevelWasLoaded())
		changePlayerMaterial();
	}

	// When a new scene is loaded, re-setup the level
	void OnLevelWasLoaded() {
		if (Application.loadedLevelName.Contains("Level")) {
			setupLevel();
		}
		if (!Application.loadedLevelName.Equals("Choose Skin")) {
			changePlayerMaterial();
		}
	}
	
	// Update is called once per frame
	void Update () {
		// If the level is done,
		if (Application.loadedLevelName.Contains("Level")) {
			if (levelIsComplete ()) {
				// Unset references to variables
				clearReferences();
				// Unlock the next level!
				int completedLevelNumber = int.Parse(lastLevelName.Substring(6, 1));
				SaveLoad.setLevelStatus(completedLevelNumber + 1, true);
				SaveLoad.save();
				// Complete the level
				Application.LoadLevel("Complete");
			}
			// If the player fell off the screen
			if (playerIsDead()) {
				// Reset all the tiles
				foreach (GameObject tile in myGrassTiles) {
					if (tile != null) {
						TileAnimator animator = tile.GetComponent<TileAnimator>();
						if (animator != null) {
							animator.removeCountdown();
						}
					}
				}
				// Move the player(s) back to start
				foreach (GameObject myPlayer in myPlayers) {
					myPlayer.transform.position = playerStartPositions.GetRange(myPlayers.IndexOf(myPlayer), 1)[0];
					myPlayer.rigidbody.velocity = Vector3.zero;
					myPlayer.rigidbody.rotation = Quaternion.identity;
					myPlayer.transform.rotation = Quaternion.identity;
				}
			}
		}
	}

	void setupLevel() {
		// Store level names
		lastLevel = Application.loadedLevel;
		lastLevelName = Application.loadedLevelName;

		// Find the GameObjects and store references to them
		myGrassTiles.AddRange(GameObject.FindGameObjectsWithTag("GrassTile"));
//		myBlocks.AddRange(GameObject.FindGameObjectsWithTag("Block"));
		myPlayers.AddRange(GameObject.FindGameObjectsWithTag("Player"));

		// Store the player(s)'s starting point(s)
		foreach (GameObject myPlayer in myPlayers) {
			playerStartPositions.Add(myPlayer.transform.position);
		}
	}
	
	void OnGUI() {
		// USEFUL RECT: new Rect((Screen.width - w) / 2, (Screen.height - h) / 2, w, h)
		// This centers a GUI object, both horizontally and vertically, with a given width and height

		// Set the GUI skin
		GUI.skin = customGUISkin;

		// Create a custom Label style from the current one
		GUIStyle customLabelStyle = GUI.skin.GetStyle("Label");
		customLabelStyle.alignment = TextAnchor.UpperCenter;

		// Create a custom Button style from the current one
		GUIStyle customButtonStyle = GUI.skin.GetStyle("Button");

		// If inside a level,
		if (Application.loadedLevelName.Contains("Level")) {
			// Draw an exit button to return to the level select screen
			Texture exit = exitSprite.texture;
			float aspectRatio = exit.width / exit.height;
			// Scale the width of the image (so it doens't run off the edges)
			float w = Screen.width * 0.05f;
			// Then preserve the aspect ratio
			float h = w * 1.0f/aspectRatio;
			customButtonStyle.fixedWidth = w;
			customButtonStyle.fixedHeight = h;
			customButtonStyle.padding = new RectOffset(2, 2, 2, 2);
			if (GUI.Button(new Rect(0.92f * Screen.width, 0.05f * Screen.height, w, h), exit)) {
				// Clear any GameObject references
				clearReferences();
				Application.LoadLevel("Select");
			}
			// Reset the padding
			customButtonStyle.padding = GUI.skin.button.padding;
		}


		// Find the GUI name and set it up accordingly
		if (Application.loadedLevelName.Equals("Main Menu")) {
			// Main menu has the following:
			// Title image
			// Start game button
			// Help button
			// Change skin button

			Texture title = titleSprite.texture;
			float aspectRatio = title.width / title.height;
			// Scale the width of the image (so it doesn't run off the edges)
			float w = Screen.width * 0.65f;
			// Then preserve the aspect ratio
			float h = w * 1.0f/aspectRatio;
			GUI.DrawTexture(new Rect((Screen.width - w) / 2, 1.5f * Screen.height / 5 - h / 2, w, h), title);

			w = Screen.width * 0.33f;
			h = Screen.height * 0.09f;
			customButtonStyle.fixedWidth = w;
			customButtonStyle.fixedHeight = h;
			customButtonStyle.fontSize = (int) (Screen.height * 0.055f);
			customButtonStyle.normal.textColor = Color.black;
			customButtonStyle.active.textColor = Color.black;
			customButtonStyle.hover.textColor = Color.black;
			if (GUI.Button (new Rect((Screen.width - w) / 2, 3.4f * Screen.height / 5 - h / 2, w, h), "Start Game", customButtonStyle)) {
				Application.LoadLevel("Select");
			}
			if (GUI.Button (new Rect((Screen.width - w) / 2, 3.95f * Screen.height / 5 - h / 2, w, h), "How to Play", customButtonStyle)) {
				Application.LoadLevel("Help");
			}
			if (GUI.Button (new Rect((Screen.width - w) / 2, 4.5f * Screen.height / 5 - h / 2, w, h), "Choose Skin", customButtonStyle)) {
				Application.LoadLevel("Choose Skin");
			}
		}
		else if (Application.loadedLevelName.Equals("Select")) {
			// Select has the following:
			// Clickable box for each level

			float w = Screen.width * 0.12f;
			float h = w;
			float padding = Screen.width * 0.03f;
			float fromLeft = Screen.width * 0.14f;

			customButtonStyle.fixedWidth = w;
			customButtonStyle.fixedHeight = h;
			customButtonStyle.fontSize = (int) (Screen.height * 0.1f);
			customButtonStyle.normal.textColor = Color.red;
			customButtonStyle.active.textColor = Color.red;
			customButtonStyle.hover.textColor = Color.red;

			// Hard-coded locations for each button? Ughhhh
			// Should probably upgrade to Unity 4.6 at some point :P
			if (GUI.Button(new Rect(fromLeft + 0 * (w + padding), Screen.height * 0.25f, w, h), SaveLoad.getLevelStatus(1) ? "1" : "X", customButtonStyle)) {
				if (SaveLoad.getLevelStatus(1)) {
					Application.LoadLevel("Level 1");
				}
			}
			if (GUI.Button(new Rect(fromLeft + 1 * (w + padding), Screen.height * 0.25f, w, h), SaveLoad.getLevelStatus(2) ? "2" : "X", customButtonStyle)) {
				if (SaveLoad.getLevelStatus(2)) {
					Application.LoadLevel("Level 2");
				}
			}
			if (GUI.Button(new Rect(fromLeft + 2 * (w + padding), Screen.height * 0.25f, w, h), SaveLoad.getLevelStatus(3) ? "3" : "X", customButtonStyle)) {
				if (SaveLoad.getLevelStatus(3)) {
					Application.LoadLevel("Level 3");
				}
			}
			if (GUI.Button(new Rect(fromLeft + 3 * (w + padding), Screen.height * 0.25f, w, h), SaveLoad.getLevelStatus(4) ? "4" : "X", customButtonStyle)) {
				if (SaveLoad.getLevelStatus(4)) {
					Application.LoadLevel("Level 4");
				}
			}
			if (GUI.Button(new Rect(fromLeft + 4 * (w + padding), Screen.height * 0.25f, w, h), SaveLoad.getLevelStatus(5) ? "5" : "X", customButtonStyle)) {
				if (SaveLoad.getLevelStatus(5)) {
					Application.LoadLevel("Level 5");
				}
			}
			if (GUI.Button(new Rect(fromLeft + 0 * (w + padding), Screen.height * 0.48f, w, h), SaveLoad.getLevelStatus(6) ? "6" : "X", customButtonStyle)) {
				if (SaveLoad.getLevelStatus(6)) {
					Application.LoadLevel("Level 6");
				}
			}
			if (GUI.Button(new Rect(fromLeft + 1 * (w + padding), Screen.height * 0.48f, w, h), SaveLoad.getLevelStatus(7) ? "7" : "X", customButtonStyle)) {
				if (SaveLoad.getLevelStatus(7)) {
					Application.LoadLevel("Level 7");
				}
			}
			if (GUI.Button(new Rect(fromLeft + 2 * (w + padding), Screen.height * 0.48f, w, h), SaveLoad.getLevelStatus(8) ? "8" : "X", customButtonStyle)) {
				if (SaveLoad.getLevelStatus(8)) {
					Application.LoadLevel("Level 8");
				}
			}
			if (GUI.Button(new Rect(fromLeft + 3 * (w + padding), Screen.height * 0.48f, w, h), SaveLoad.getLevelStatus(9) ? "9" : "X", customButtonStyle)) {
				if (SaveLoad.getLevelStatus(9)) {
					Application.LoadLevel("Level 9");
				}
			}
			if (GUI.Button(new Rect(fromLeft + 4 * (w + padding), Screen.height * 0.48f, w, h), SaveLoad.getLevelStatus(10) ? "10" : "X", customButtonStyle)) {
				if (SaveLoad.getLevelStatus(10)) {
					Application.LoadLevel("Level 10");
				}
			}

			// Return to main menu button
			w = Screen.width * 0.25f;
			h = Screen.height * 0.1f;
			customButtonStyle.fixedWidth = w;
			customButtonStyle.fixedHeight = h;
			customButtonStyle.fontSize = (int) (Screen.height * 0.055f);
			customButtonStyle.normal.textColor = Color.black;
			customButtonStyle.active.textColor = Color.black;
			customButtonStyle.hover.textColor = Color.black;
			if (GUI.Button(new Rect(Screen.width / 2 + 0.1f*w, 0.75f * Screen.height, w, h), "Main Menu")) {
				Application.LoadLevel("Main Menu");
				Destroy (gameObject);
			}

			// Reset save data button
			w = Screen.width * 0.25f;
			h = Screen.height * 0.1f;
			customButtonStyle.fixedWidth = w;
			customButtonStyle.fixedHeight = h;
			customButtonStyle.fontSize = (int) (Screen.height * 0.043f);
			if (GUI.Button(new Rect(Screen.width / 2 - 1.1f*w, 0.75f * Screen.height, w, h), "Reset Progress")) {
				SaveLoad.resetProgress();
				SaveLoad.save();
			}
		}
		else if (Application.loadedLevelName.Equals("Help")) {
			// Help has the following:
			// how to play text
			customLabelStyle.alignment = TextAnchor.MiddleLeft;
			customLabelStyle.fontSize = (int) (Screen.height * 0.055f);
			customLabelStyle.normal.textColor = Color.black;
			GUI.Label(new Rect(Screen.width * 0.05f, Screen.height * 0.005f, Screen.width * 0.9f, Screen.height * 0.9f),
			          "Objective: Activate all of the green tiles on each level.\n\n" +
			          "• Roll onto a green tile to activate it.\n" +
			          "• Green tiles only stay activated for a certain amount of time.\n" +
			          "• Rolling over a green tile while it is counting down will reset its timer.\n" +
			          "• Blue tiles are holes in the level - don't fall or the level will reset!\n" +
			          "• Black tiles are solid and act as barriers in the level."
			          );
			float w = Screen.width * 0.28f;
			float h = Screen.height * 0.1f;
			customButtonStyle.fixedWidth = w;
			customButtonStyle.fixedHeight = h;
			customButtonStyle.fontSize = (int) (Screen.height * 0.05f);
			if (GUI.Button(new Rect((Screen.width - w) / 2, Screen.height * 0.825f, w, h), "Back to Menu")) {
				Application.LoadLevel("Main Menu");
				Destroy (gameObject);
			}
		}
		else if (Application.loadedLevelName.Equals("Choose Skin")) {
			// Choose skin has the following:
			// Button to choose each skin
			// Main menu button

			float w = Screen.width * 0.28f;
			float h = Screen.height * 0.08f;
			customButtonStyle.fixedWidth = w;
			customButtonStyle.fixedHeight = h;
			customButtonStyle.fontSize = (int) (Screen.height * 0.05f);
			if (GUI.Button(new Rect((Screen.width - w) / 2, Screen.height * 0.86f, w, h), "Back to Menu")) {
				Application.LoadLevel("Main Menu");
				Destroy (gameObject);
			}
			w = Screen.width * 0.24f;
			h = Screen.height * 0.08f;
			customButtonStyle.fixedWidth = w;
			customButtonStyle.fixedHeight = h;
			customButtonStyle.fontSize = (int) (Screen.height * 0.04f);
			if (GUI.Button(new Rect(Screen.width * 0.11f, Screen.height * 0.66f, w, h), "Select this skin!")) {
				SaveLoad.setSetting("playerTexture", 0);
				SaveLoad.save();
			}
			if (GUI.Button(new Rect(Screen.width * 0.38f, Screen.height * 0.66f, w, h), "Select this skin!")) {
				SaveLoad.setSetting("playerTexture", 1);
				SaveLoad.save();
			}
			if (GUI.Button(new Rect(Screen.width * 0.65f, Screen.height * 0.66f, w, h), "Select this skin!")) {
				SaveLoad.setSetting("playerTexture", 2);
				SaveLoad.save();
			}
		}
		else if (Application.loadedLevelName.Equals("Complete")) {
			// Complete has the following:
			// Level complete text
			// Next level button
			// Main menu button

			// Font size = 14.5% of screen height
			customLabelStyle.fontSize = (int) (Screen.height * 0.145f);
			customLabelStyle.normal.textColor = Color.red;
			float w = Screen.width * 0.5f;
			float h = Screen.height * 0.33f;
			GUI.Label(new Rect((Screen.width - w) / 2, 1.4f * Screen.height / 5 - h / 2, w, h), lastLevelName + " complete!", customLabelStyle);

			customButtonStyle.normal.textColor = Color.black;
			customButtonStyle.active.textColor = Color.black;
			customButtonStyle.hover.textColor = Color.black;
			customButtonStyle.fontSize = (int) (Screen.height * 0.055f);
			w = Screen.width * 0.3f;
			h = Screen.height * 0.1f;
			customButtonStyle.fixedWidth = w;
			customButtonStyle.fixedHeight = h;
			if (GUI.Button(new Rect((Screen.width - 2.3f*w) / 2, 3.5f * Screen.height / 5 - h / 2, w, h), "Next Level!")) {
				Application.LoadLevel(lastLevel + 1);
			}
			if (GUI.Button(new Rect((Screen.width + 0.3f*w) / 2, 3.5f * Screen.height / 5 - h / 2, w, h), "Select Level")) {
				Application.LoadLevel("Select");
			}
		}
	}

	bool levelIsComplete() {
		bool isComplete = true;

		// If there are no tiles, the level cannot possibly be complete
		if (myGrassTiles.Count == 0) {
			return false;
		}
		
		foreach (GameObject gO in myGrassTiles) {
			if (gO != null && !gO.GetComponent<Animator>().GetBool("IsActive")) {
				isComplete = false;
				break;
			}
		}
		
		return isComplete;
	}

	bool playerIsDead() {
		bool isDead = false;
		foreach (GameObject myPlayer in myPlayers) {
			if (myPlayer.transform.position.y < -2.0f) {
				isDead = true;
				break;
			}
		}
		return isDead;
	}

	void changePlayerMaterial() {
		// Try to change the textures of any players on the level
		int playerTexture = SaveLoad.getSetting("playerTexture");
		foreach (GameObject myPlayer in GameObject.FindGameObjectsWithTag("Player")) {
			// Display the correct player texture according to the user settings
			myPlayer.renderer.material = playerMaterials[playerTexture];
		}
	}

	void clearReferences() {
		// Remove references to GameObjects and other things
		myGrassTiles.Clear();
//		myBlocks.Clear();
		myPlayers.Clear();
		playerStartPositions.Clear();
	}
}
