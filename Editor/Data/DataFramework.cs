//  Project : Actors
// Contacts : Pixeye - ask@pixeye.games

#if UNITY_EDITOR
using System.IO;
using UnityEditor;


namespace Pixeye.Actors
{
	static class DataFramework
	{
		public static bool onStructs
		{
			get { return EditorPrefs.GetBool("hba.data.onStructs", false); }
			set { EditorPrefs.SetBool("hba.data.onStructs", value); }
		}

		public static string nameSpace
		{
			get { return EditorPrefs.GetString("hba.data.namespace", "Pixeye.Source"); }
			set
			{
				{
					EditorPrefs.SetString("hba.data.namespace", value);
				}
			}
		}

		public static string pathPrefabsEditor
		{
			get { return EditorPrefs.GetString("hba.path.prefabs", ""); }
			set { EditorPrefs.SetString("hba.path.prefabs", value); }
		}
	}
}
#endif