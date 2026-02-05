//LevelData.cs
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

namespace SpaceZombie.Niveaux.Configs.V
{
	public class GameData
	{
		public List<Stage> Stages { get; set; }
	}

	public class Stage
	{
		public int StageNumber { get; set; }
		public List<Level> Levels { get; set; }
	}

	public class Level
	{
		public int LevelNumber { get; set; }
		public int NbEnemiesParLigne { get; set; }
		public bool DeplaceEnBlock { get; set; }
		public List<Line> Lines { get; set; }
	}

	public class Line
	{
		public bool Fixed { get; set; }
		public float Speed { get; set; }       // e.g., 30 or 50
		public float Direction { get; set; }   // 1 or -1
		public List<int> EnemyLevels { get; set; } // 1 = Type1, 2 = Type2, etc.
	}



	public static class ExempleDeserialisation
	{
		public static GameData Deserialize()
		{
			var path = "res://Scripts/Niveaux/ConfigNiveaux.json";
			var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
			string json = file.GetAsText();
			file.Close();
			GameData data = JsonSerializer.Deserialize<GameData>(json);
			return data;

		}
	}
}
