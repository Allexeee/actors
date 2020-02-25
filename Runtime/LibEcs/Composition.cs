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
		public int[] ids = new int[0];

		public bool[] includeComponents = new bool[Framework.Settings.SizeComponents];
		internal bool[] excludeComponents = new bool[Framework.Settings.SizeComponents];

		internal HashCode hash;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool OverlapComponents(in CacheEntity cache)
		{
			int match = 0;
			for (int i = 0; i < cache.componentsAmount; i++)
			{
				if (includeComponents[cache.componentsIds[i]])
					match++;
			}

			return ids.Length == match;
		}

		internal void AddTypesExclude(int[] types)
		{
			if (types != null)
				for (int i = 0; i < types.Length; i++)
				{
					var t = Storage.typeNames[types[i]];

					excludeComponents[t] = true;
				}
		}

		internal void SetupExcludeTypes(GroupCore g)
		{
			for (int i = 0; i < Storage.lastID; i++)
			{
				var t = excludeComponents[i];
				if (t) Storage.All[i].groups.Add(g);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool Check(int entityID)
		{
			for (int ll = 0; ll < ids.Length; ll++)
				if ((Entity.Generations[entityID, generations[ll]] & ids[ll]) != ids[ll])
				{
					return false;
				}

			ref var components = ref Entity.entities[entityID];

			for (int i = 0; i < components.componentsAmount; i++)
			{
				if (excludeComponents[components.componentsIds[i]])
				{
					return false;
				}
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool CanProceed(int entityID)
		{
			for (int ll = 0; ll < ids.Length; ll++)
				if ((Entity.Generations[entityID, generations[ll]] & ids[ll]) != ids[ll])
					return false;

			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool ExcludeTypes(int entityID)
		{
			ref var components = ref Entity.entities[entityID];

			for (int i = 0; i < components.componentsAmount; i++)
			{
				if (excludeComponents[components.componentsIds[i]])
				{
					return true;
				}
			}

			return false;
		}

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