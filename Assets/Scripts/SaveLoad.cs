using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;

/// <summary>
/// Static implementation of save-load. Provides static utility methods to set and retrieve game data.
/// </summary>
public static class SaveLoad {
	
	private static GameData gameData = new GameData();

	public static void setLevelStatus(int level, bool status) {
		gameData.setLevelStatus(level, status);
	}

	public static bool getLevelStatus(int level) {
		return gameData.getLevelStatus(level);
	}

	public static void resetProgress() {
		gameData.resetProgress();
	}

	public static void setSetting(string name, int value) {
		gameData.setSetting(name, value);
	}

	public static int getSetting(string name) {
		return gameData.getSetting(name);
	}

	public static void resetSettings() {
		gameData.resetSettings();
	}

	public static void save() {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/progress.gd");
		bf.Serialize(file, SaveLoad.gameData);
		file.Close();
	}

	public static void load() {
		if(File.Exists(Application.persistentDataPath + "/progress.gd")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/progress.gd", FileMode.Open);
			gameData = (GameData)bf.Deserialize(file);
			file.Close();
		}
	}
}
