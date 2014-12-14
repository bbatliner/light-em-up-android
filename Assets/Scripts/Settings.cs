using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
/// <summary>
/// Wrapper class for settings Dictionary. Provides utility methods to set and retrieve settings.
/// </summary>
public class Settings {
	
	public Dictionary<string, int> settings = new Dictionary<string, int>();

	public Settings() {
		initializeDefaultSettings();
	}

	public int getSetting(string name) {
		if (settings.ContainsKey(name)) {
			return settings[name];
		}
		else {
			return -1;
		}
	}

	public void setSetting(string name, int value) {
		settings[name] = value;
	}

	public void resetSettings() {
		initializeDefaultSettings();
	}

	private void initializeDefaultSettings() {
		settings = new Dictionary<string, int>();
		settings["playerTexture"] = 0;
	}
}
