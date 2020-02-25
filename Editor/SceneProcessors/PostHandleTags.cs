using System.Linq;
using UnityEditor;

namespace Pixeye.Actors
{
	public static class PostHandleTags
	{
#if !ACTORS_COMPONENTS_STRUCTS
		[MenuItem("Tools/Actors/Set Struct Components", false, 2)]
		static public void SetStructsChecks()
		{
			DataFramework.onStructs = true;
			string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			var    allDefines    = definesString.Split(';').ToList();

			var index = allDefines.FindIndex(d => d.Contains("ACTORS_COMPONENTS_STRUCTS"));
			if (index == -1)
			{
				allDefines.Add("ACTORS_COMPONENTS_STRUCTS");
			}


			PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines.ToArray()));
		}
		#else
		[MenuItem("Tools/Actors/Remove Struct Components", false, 2)]
		static public void SetNoStructsChecks()
		{
			DataFramework.onStructs = false;
			string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			var    allDefines = definesString.Split(';').ToList();

			var index = allDefines.FindIndex(d => d.Contains("ACTORS_COMPONENTS_STRUCTS"));
			if (index != -1)
			{
				allDefines[index] = string.Empty;
			}


			PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines.ToArray()));
		}
		#endif

		#if !ACTORS_DEBUG
		[MenuItem("Tools/Actors/Set Debug", false, 4)]
		static public void SetDebug()
		{
			string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			var    allDefines = definesString.Split(';').ToList();

			var index = allDefines.FindIndex(d => d.Contains("ACTORS_DEBUG"));
			if (index == -1)
			{
				allDefines.Add("ACTORS_DEBUG");
			}


			PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines.ToArray()));
		}
		#else
		[MenuItem("Tools/Actors/Set Release", false, 4)]
		static public void SetNoDebug()
		{
			string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			var    allDefines    = definesString.Split(';').ToList();

			var index = allDefines.FindIndex(d => d.Contains("ACTORS_DEBUG"));
			if (index != -1)
			{
				allDefines[index] = string.Empty;
			}


			PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines.ToArray()));
		}
		#endif
	}
}