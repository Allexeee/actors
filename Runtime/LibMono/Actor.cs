﻿//  Project  : ACTORS
//  Contacts : Pixeye - ask@pixeye.games


using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;

#endif

namespace Pixeye.Actors
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks | Option.DivideByZeroChecks, false)]
	public class Actor : MonoBehaviour, IRequireStarter
	{
		public ent entity;

		#if UNITY_EDITOR
		[FoldoutGroup("Main"), SerializeField, ReadOnly]
		internal int _entity;
		#endif

		[FoldoutGroup("Main")]
		public bool isPooled;
		

		/// <summary>
		/// Initialize entity here.
		/// </summary>
		protected virtual void Setup()
		{
		}


		//===============================//
		// Launch methods
		//===============================//
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Launch()
		{
			int  id;
			byte age = 0;

			if (ent.entStack.length > 0)
			{
				ref var pop = ref ent.entStack.source[--ent.entStack.length];
				id = pop.id;
				unchecked
				{
					age = (byte) (pop.age);
				}
			}
			else
				id = ent.lastID++;

			entity.id  = id;
			entity.age = age;


			#if UNITY_EDITOR
			_entity = id;
			#endif


			Entity.Initialize(id, age, isPooled);
			Entity.Transforms[id] = transform;

			if (isActiveAndEnabled)
			{
					Setup();
					EntityOperations.Set(entity, -1, EntityOperations.Action.Activate);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void IRequireStarter.Launch()
		{
			if (!entity.exist)
			{
				Launch();
			}
		}

		//===============================//
		// Create methods
		//===============================//

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Actor Create(GameObject prefab, Vector3 position = default, bool pooled = false)
		{
			var tr    = pooled ? Obj.Spawn(Pool.Entities, prefab, position) : Obj.Spawn(prefab, position);
			var actor = tr.AddGetActor();

			actor.isPooled = pooled;
			actor.Launch();

			return actor;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Actor Create(GameObject prefab, Transform parent, Vector3 position = default, bool pooled = false)
		{
			var tr    = pooled ? Obj.Spawn(Pool.Entities, prefab, parent, position) : Obj.Spawn(prefab, parent, position);
			var actor = tr.AddGetActor();

			actor.isPooled = pooled;
			actor.Launch();

			return actor;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Actor Create(string prefabID, Vector3 position = default, bool pooled = false)
		{
			var tr    = pooled ? Obj.Spawn(Pool.Entities, prefabID, position) : Obj.Spawn(prefabID, position);
			var actor = tr.AddGetActor();

			actor.isPooled = pooled;
			actor.Launch();

			return actor;
		}
	}

	public static class HelperActor
	{
	    public static Transform InitChilds(this Transform tr)
	    {
	      	var actors = tr.GetComponentsInChildren<Actor>();

			#if UNITY_EDITOR
	      	if (actors.Length == 0)
	      	{
				Debug.LogError("Error: the number of actor children equals to zero");
	        	return tr;
	      	}
			#endif
	
	      	var isSelfActor = actors[0].transform.GetHashCode() == tr.GetHashCode();
	      	for (int i = isSelfActor ? 1 : 0; i < actors.Length; i++)
	      	{
	        	actors[i].Launch();
	      	}

	      	return tr;
	    }

	    public static Actor InitChilds(this Actor actor)
	    {
	      	actor.transform.InitChilds();
	      	return actor;
	    }
	}
}