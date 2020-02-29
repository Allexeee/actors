using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Pixeye.Actors
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks | Option.DivideByZeroChecks, false)]
	public static partial class Entity
	{
		public static ent Create()
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

			ent entity;
			entity.id  = id;
			entity.age = age;
			Initialize(id, age);
			// EntityOperations.Set(entity, -1, EntityOperations.Action.Activate);
			return entity;
		}

		public static ent Create(string prefabID, Vector3 position = default, bool pooled = false)
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

			ent entity;
			entity.id  = id;
			entity.age = age;

			Initialize(id, age, pooled);
			Transforms[id] = pooled ? Obj.Spawn(Pool.Entities, prefabID, position) : Obj.Spawn(prefabID, position);

			EntityOperations.Set(entity, -1, EntityOperations.Action.Activate);
			return entity;
		}

		public static ent Create(string prefabID, Transform parent, Vector3 position = default, bool pooled = false)
		{
			byte age = 0;
			int  id;
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

			ent entity;
			entity.id  = id;
			entity.age = age;
			Initialize(id, age, pooled);
			Transforms[id] = pooled ? Obj.Spawn(1, prefabID, parent, position, new Quaternion()) : Obj.Spawn(prefabID, parent, position, new Quaternion());
			EntityOperations.Set(in entity, -1, EntityOperations.Action.Activate);
			return entity;
		}


		public static ent Create(GameObject prefab, Transform parent, Vector3 position = default, bool pooled = false)
		{
			byte age = 0;
			int  id;
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

			ent entity;
			entity.id  = id;
			entity.age = age;
			Initialize(id, age, pooled);
			Transforms[id] = pooled ? Obj.Spawn(1, prefab, parent, position) : Obj.Spawn(prefab, parent, position);
			EntityOperations.Set(in entity, -1, EntityOperations.Action.Activate);
			return entity;
		}
		public static ent Create(GameObject prefab, Vector3 position = default, bool pooled = false)
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


			ent entity;
			entity.id  = id;
			entity.age = age;


			Initialize(id, age, pooled);
			Transforms[id] = pooled ? Obj.Spawn(Pool.Entities, prefab, position) : Obj.Spawn(prefab, position);

			EntityOperations.Set(entity, -1, EntityOperations.Action.Activate);
			return entity;
		}

		public static ent CreateFor(GameObject obj)
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

			ent entity;
			entity.id  = id;
			entity.age = age;

			Initialize(id, age);
			Transforms[id] = obj.transform;
			EntityOperations.Set(entity, -1, EntityOperations.Action.Activate);
			return entity;
		}

		public static ent CreateFor(string name)
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

			ent entity;
			entity.id  = id;
			entity.age = age;

			Initialize(id, age);

			Transforms[id] = GameObject.Find(name).transform;
			EntityOperations.Set(entity, -1, EntityOperations.Action.Activate);
			return entity;
		}
		
	}
}