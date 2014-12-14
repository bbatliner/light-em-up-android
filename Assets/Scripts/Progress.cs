using UnityEngine;
using System.Collections;

[System.Serializable]
/// <summary>
/// Wrapper class for levels completed boolean array. Provides utilty methods to set and retrieve level status.
/// </summary>
public class Progress {

	public bool[] levelsCompleted;

	// Instantiate the levels array with the specified size
	// then fill it with falses (level not complete)
	// then unlock the first level (set to true)
	public Progress(int numberOfLevels) {
		levelsCompleted = new bool[numberOfLevels];
		initializeData();
	}

	public void setLevelStatus(int level, bool status) {
		if (1 <= level && level <= levelsCompleted.Length) {
			levelsCompleted[level - 1] = status;
		}
	}

	public bool getLevelStatus(int level) {
		if (1 <= level && level <= levelsCompleted.Length) {
			return levelsCompleted[level - 1];
		}
		else {
			return false;
		}
	}

	public void reset() {
		initializeData();
	}

	private void initializeData() {
		for (int i = 0; i < levelsCompleted.Length; i++) {
			levelsCompleted[i] = false;
		}
		levelsCompleted[0] = true;
	}
}
