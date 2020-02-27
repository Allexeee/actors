//  Project : ecs
// Contacts : Pix - ask@pixeye.games

using System;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;


namespace Pixeye.Actors
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks | Option.DivideByZeroChecks, false)]
	public sealed class CacheGroup
	{
		public GroupCore[] Elements = new GroupCore[10];
		public int length;
	}

	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks | Option.DivideByZeroChecks, false)]
	static class HelperCacheGroup
	{
		public static bool Contain(this CacheGroup container, GroupCore groupCore)
		{
			var len = container.length;
			for (int i = 0; i < len; i++)
			{
				var groupAdded = container.Elements[i];
				if (groupAdded.Equals(groupCore)) return true;
			}

			return false;
		}

		public static bool TryGetValue(this CacheGroup container, Type t, Composition composition, out GroupCore group)
		{
			var len = container.length;
			for (int i = 0; i < len; i++)
			{
				var instance = container.Elements[i];
				if (t != instance.GetType()) continue;

				if (instance.composition.hash.value == composition.hash.value)
				{
					group = instance;
					return true;
				}
			}

			group = default;
			return false;
		}

		public static GroupCore Add(this CacheGroup container, GroupCore group)
		{
			ref var len     = ref container.length;
			ref var storage = ref container.Elements;

			if (len == storage.Length)
			{
				var l = container.length << 1;
				Array.Resize(ref storage, l);
			}

			storage[len++] = group;
			return group;
		}

		public static void Dispose(this CacheGroup container)
		{
			for (int i = 0; i < container.length; i++)
				container.Elements[i].Dispose();
			
			container.length = 0;
		}
	}
}