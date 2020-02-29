//  Project : ecs
// Contacts : Pix - ask@pixeye.games

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;


namespace Pixeye.Actors
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks | Option.DivideByZeroChecks, false)]
	public static unsafe partial class Entity
	{
		public const bool Pooled = true;

		static readonly int sizeEntityCache = UnsafeUtility.SizeOf<CacheEntity>();

		public static Transform[] Transforms;
		
		public static CacheEntity* entities;

		internal static int lengthTotal;
		public static int[,] Generations;

		internal static ents alive;

 
		
		//===============================//
		// Initialize 
		//===============================//

		#if UNITY_EDITOR
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		#else
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
		#endif
		internal static void Start()
		{
			var t = Resources.Load<TextAsset>("SettingsFramework");
			if (t != null)
				JsonUtility.FromJsonOverwrite(t.text, Framework.Settings);

			Framework.Settings.SizeGenerations = Framework.Settings.SizeComponents / 32;


			lengthTotal = Framework.Settings.SizeEntities;
			Generations = new int[Framework.Settings.SizeEntities, Framework.Settings.SizeGenerations];
			Transforms  = new Transform[Framework.Settings.SizeEntities];

			entities = (CacheEntity*) UnmanagedMemory.Alloc(sizeEntityCache * Framework.Settings.SizeEntities);

			for (int i = 0; i < Framework.Settings.SizeEntities; i++)
			{
				entities[i] = new CacheEntity(6);
			}

			alive = new ents(Framework.Settings.SizeEntities);

			#if UNITY_EDITOR
			Toolbox.OnDestroyAction += Dispose;
			#endif
		}

		// Use for other libraries
		public static int GetLiveEntities()
		{
			return alive.length;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void Initialize(int id, byte age, bool isPooled = false, bool isNested = false)
		{
			if (id >= lengthTotal)
			{
				var l = id << 1;
				HelperArray.ResizeInt(ref Generations, l, Framework.Settings.SizeGenerations);
				Array.Resize(ref Transforms, l);

				entities = (CacheEntity*) UnmanagedMemory.ReAlloc(entities, sizeEntityCache * l);

				for (int i = lengthTotal; i < l; i++)
				{
					entities[i] = new CacheEntity(5);
				}

				lengthTotal = l;
			}

			entities[id].componentsAmount = 0;

			var ptrCache = &entities[id];
			// ptrCache->age      = age;
			ptrCache->isNested = isNested;
			ptrCache->isPooled = isPooled;
			// todo: need to refactor in future
			// ptrCache->isDirty  = true;
			// ptrCache->isAlive  = true;

			ent e;
			e.id  = id;
			// e.age = age;
			alive.Add(e);
		}


		//===============================//
		// Naming
		//===============================//
		static FastString fstr = new FastString(500);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void RenameGameobject(this ent entity)
		{
			var tr = Transforms[entity.id];
			if (tr != null)
			{
				var name             = tr.name;
				var index            = tr.name.LastIndexOf(':');
				if (index > -1) name = tr.name.Remove(0, index + 1);
				var id               = entity.id;

				name = name.Trim();
				fstr.Clear();
				fstr.Append($"{id.ToString().PadLeft(4, '0')}: ");

				if (Framework.Settings.DebugNames)
				{
					fstr.Append("[ ");
					for (int j = 0; j < entities[entity.id].componentsAmount; j++)
					{
						var storage = Storage.All[entities[entity.id].componentsIds[j]];
						var lex     = j < entities[entity.id].componentsAmount - 1 ? " " : "";
						fstr.Append($"{storage.GetComponentType().Name.Remove(0, 9)}{lex}");
					}

					fstr.Append(" ]: ");
				}

				fstr.Append(name);

				tr.name = fstr.ToString();
			}
		}
		
		static void Dispose()
		{
			for (int i = 0; i < lengthTotal; i++)
				Marshal.FreeHGlobal((IntPtr) entities[i].componentsIds);

			UnmanagedMemory.Cleanup();
		}
	}
}