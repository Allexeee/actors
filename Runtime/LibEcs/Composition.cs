//  Project  : ACTORS
//  Contacts : Pixeye - ask@pixeye.games


using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;


namespace Pixeye.Actors
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks | Option.DivideByZeroChecks, false)]
	public unsafe class Composition : IEquatable<Composition>
	{
		public int[] generations = new int[0];
		// public int[] ids = new int[0];

		public bool[] includeComponents = new bool[Framework.Settings.SizeComponents];

		internal HashCode hash;

		// [MethodImpl(MethodImplOptions.AggressiveInlining)]
		// internal bool OverlapComponents(in CacheEntity cache)
		// {
		// 	int match = 0;
		// 	for (int i = 0; i < cache.componentsAmount; i++)
		// 	{
		// 		if (includeComponents[cache.componentsIds[i]])
		// 			match++;
		// 	}
		//
		// 	return ids.Length == match;
		// }
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool OverlapComponents(int entityId)
		{
			var cache = Entity.entities[entityId];
			int match = 0;
			for (int i = 0; i < cache.componentsAmount; i++)
			{
				if (includeComponents[cache.componentsIds[i]])
					match++;
			}

			return generations.Length == match;
		}
		
		// [MethodImpl(MethodImplOptions.AggressiveInlining)]
		// internal bool CanProceed(int entityID)
		// {
		// 	for (int ll = 0; ll < ids.Length; ll++)
		// 		if ((Entity.Generations[entityID, generations[ll]] & ids[ll]) != ids[ll])
		// 			return false;
		//
		// 	return true;
		// }
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(Composition other)
		{
			return GetHashCode() == other.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode() => hash;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object obj)
		{
			var other = obj as Composition;
			return other != null && Equals(other);
		}
	}
}