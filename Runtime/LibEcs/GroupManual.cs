//  Project  : ACTORS
//  Contacts : Pixeye - ask@pixeye.games

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.IL2CPP.CompilerServices;
 

namespace Pixeye.Actors
{
	class GroupManualComparer : IEqualityComparer<GroupManual>
	{
		public static GroupManualComparer Default = new GroupManualComparer();

		public bool Equals(GroupManual x, GroupManual y)
		{
			return y.id == x.id;
		}

		public int GetHashCode(GroupManual obj)
		{
			return obj.composition.hash;
		}
	}

	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks | Option.DivideByZeroChecks, false)]
	public class GroupManual : IEnumerable, IEquatable<GroupManual>, IDisposable
	{
		public ent[] entities = new ent[Framework.Settings.SizeEntities];
		public int length;
		
		public ents added = new ents(Framework.Settings.SizeEntities);
		public ents removed = new ents(Framework.Settings.SizeEntities);


		public Composition composition;

		internal int id;

		int position;


		public ref ent this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref entities[index];
		}
 

		public void Release(int index)
		{
			if (length == 0) return;
			entities[index].Release();
		}
		
		internal virtual GroupManual Initialize(Composition composition)
		{
			this.composition = composition;
			return this;
		}


		//===============================//
		// Insert
		//===============================//
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Insert(in ent entity)
		{
			var left  = 0;
			var index = 0;
			var right = length++;

			if (entity.id >= entities.Length)
			{
				Array.Resize(ref entities, entity.id << 1);
				Array.Resize(ref added.source, entity.id << 1);
				Array.Resize(ref removed.source, entity.id << 1);
			}
			else if (length >= entities.Length)
			{
				Array.Resize(ref entities, length << 1);
				Array.Resize(ref added.source, length << 1);
				Array.Resize(ref removed.source, length << 1);
			}

			var consitionSort = right - 1;
			if (consitionSort > -1 && entity.id < entities[consitionSort].id)
			{
				index = HelperArray.BinarySearch(ref entities, entity.id, 0, length-1);

				Array.Copy(entities, index, entities, index + 1, length - index);
				entities[index] = entity;
				added.source[added.length++] = entity;
			}
			else
			{
				entities[right] = entity;
				added.source[added.length++] = entity;
			}
		}

		public bool HasEntity(in ent entity, out int index)
		{
			index = -1;
			if (length == 0) return false;

			index = HelperArray.BinarySearch(ref entities, entity.id, 0, length-1);
			if (index == -1) return false;
			return true;
		}


		//===============================//
		// Try Remove
		//===============================//
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void TryRemove(int entityID)
		{
			if (length == 0) return;
		
			var i = HelperArray.BinarySearch(ref entities, entityID, 0, length-1);
			if (i == -1) return;
		
			removed.source[removed.length++] = entities[i];
		
			if (i < --length)
				Array.Copy(entities, i + 1, entities, i, length - i);
		}


		//===============================//
		// Remove
		//===============================//
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveAt(int i)
		{
			removed.source[removed.length++] = entities[i];

			if (i < --length)
				Array.Copy(entities, i + 1, entities, i, length - i);
		}


		public virtual void Dispose()
		{
			added   = new ents(Framework.Settings.SizeEntities);
			removed = new ents(Framework.Settings.SizeEntities);
		}
		
		#region EQUALS

		public bool Equals(GroupManual other)
		{
			return id == other.id;
		}

		public bool Equals(int other)
		{
			return id == other;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == GetType() && Equals((GroupManual) obj);
		}

		public override int GetHashCode()
		{
			return id;
		}

		#endregion

		#region ENUMERATOR

		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public struct Enumerator : IEnumerator<ent>
		{
			GroupManual g;
			int position;


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal Enumerator(GroupManual g)
			{
				position = -1;
				this.g   = g;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				return ++position < g.length;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Reset()
			{
				position = -1;
			}

			object IEnumerator.Current => Current;

			public ent Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get { return g.entities[position]; }
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose()
			{
				g = null;
			}
		}

		#endregion
	}
}