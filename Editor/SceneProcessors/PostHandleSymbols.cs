//  Project : ecs
// Contacts : Pix - ask@pixeye.games

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Pixeye.Actors
{
	/// <summary>
	/// Adds the given define symbols to PlayerSettings define symbols.
	/// Just add your own define symbols to the Symbols property at the below.
	/// </summary>
	[InitializeOnLoad]
	public class PostHandleSymbols : Editor
	{
		/// <summary>
		/// Add define symbols as soon as Unity gets done compiling.
		/// </summary>
		static PostHandleSymbols()
		{
			string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			var    allDefines    = definesString.Split(';').ToList();

			var index = allDefines.FindIndex(d => d.Contains("ACTORS_COMPONENTS_STRUCTS"));

			var str = DataFramework.onStructs ? "ACTORS_COMPONENTS_STRUCTS" : string.Empty;
			if (index > -1)
			{
				allDefines[index] = str;
			}
			else
			{
				allDefines.Add(str);
			}
			
			PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines.ToArray()));
		}
	}
}