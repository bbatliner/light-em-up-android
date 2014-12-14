using UnityEngine;
using System.Collections;

[System.Serializable]
/// <summary>
/// High-level game data class. Holds objects that contain true game data.
/// </summary>
public class GameData {

	// 10 levels; change when more are added
	private Progress progress = new Progress(10);
	private Settings settings = new Settings();

	public void setLevelStatus(int level, bool status) {
		progress.setLevelStatus(level, status);
	}

	public bool getLevelStatus(int level) {
		return progress.getLevelStatus(level);
	}

	public void resetProgress() {
		progress.reset();
	}

	public void setSetting(string name, int value) {
		settings.setSetting(name, value);
	}

	public int getSetting(string name) {
		return settings.getSetting(name);
	}

	public void resetSettings() {
		settings.resetSettings();
	}
}
