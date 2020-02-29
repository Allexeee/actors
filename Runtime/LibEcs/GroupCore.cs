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
	class GroupCoreComparer : IEqualityComparer<GroupCore>
	{
		public static GroupCoreComparer Default = new GroupCoreComparer();

		public bool Equals(GroupCore x, GroupCore y)
		{
			return y.id == x.id;
		}

		public int GetHashCode(GroupCore obj)
		{
			return obj.composition.hash;
		}
	}

	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks | Option.DivideByZeroChecks, false)]
	public abstract class GroupCore : IEnumerable, IEquatable<GroupCore>, IDisposable
	{
		public ent[] entities = new ent[Framework.Settings.SizeEntities];
		public int length;
		
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

		protected void CompositionAdd<T>(Composition composition, int index)
		{
			composition.generations[index] = Storage<T>.Generation;
			composition.includeComponents[Storage<T>.componentId] = true;
		}
		
		internal virtual GroupCore Initialize(Composition composition)
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

			// todo: сделать как проверку в редакторе
			if (entity.id >= entities.Length)
			{
				Array.Resize(ref entities, entity.id << 1);
			}
			else if (length >= entities.Length)
			{
				Array.Resize(ref entities, length << 1);
			}

			var consitionSort = right - 1;
			if (consitionSort > -1 && entity.id < entities[consitionSort].id)
			{
				index = HelperArray.BinarySearch(ref entities, entity.id, 0, length-1);

				Array.Copy(entities, index, entities, index + 1, length - index);
				entities[index] = entity;
			}
			else
			{
				entities[right] = entity;
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
		// Remove
		//===============================//
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveAt(int i)
		{
			if (i < --length)
				Array.Copy(entities, i + 1, entities, i, length - i);
		}


		public virtual void Dispose()
		{
			//parallel
			if (segmentGroups != null)
				for (int i = 0; i < segmentGroups.Length; i++)
				{
					var d = segmentGroups[i];
					d.thread.Interrupt();
					d.thread.Join(5);
					syncs[i].Close();
					segmentGroups[i] = null;
				}

			segmentGroupLocal = null;
		}

		//===============================//
		// Concurrent
		//===============================//

		SegmentGroup segmentGroupLocal;
		SegmentGroup[] segmentGroups;
		ManualResetEvent[] syncs;


		int threadsAmount = Environment.ProcessorCount - 1;
		int entitiesPerThreadMin = 5000;
		int entitiesPerThread;

		HandleSegmentGroup jobAction;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void MakeConcurrent(int minEntities, int threads, HandleSegmentGroup jobAction)
		{
			this.jobAction = jobAction;

			segmentGroupLocal        = new SegmentGroup();
			segmentGroupLocal.source = this;


			segmentGroups = new SegmentGroup[threadsAmount];
			syncs         = new ManualResetEvent[threadsAmount];

			for (int i = 0; i < threadsAmount; i++)
			{
				ref var nextSegment = ref segmentGroups[i];
				nextSegment                     = new SegmentGroup();
				nextSegment.thread              = new Thread(HandleThread);
				nextSegment.thread.IsBackground = true;
				nextSegment.HasWork             = new ManualResetEvent(false);
				nextSegment.WorkDone            = new ManualResetEvent(true);
				nextSegment.source              = this;

				syncs[i] = nextSegment.WorkDone;

				nextSegment.thread.Start(nextSegment);
			}
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Execute(float delta)
		{
			if (length > 0)
			{
				var entitiesNext      = 0;
				var threadsInWork     = 0;
				var entitiesPerThread = length / (threadsAmount + 1);
				if (entitiesPerThread > entitiesPerThreadMin)
				{
					threadsInWork = threadsAmount + 1;
				}
				else
				{
					threadsInWork     = length / entitiesPerThreadMin;
					entitiesPerThread = entitiesPerThreadMin;
				}


				for (var i = 0; i < threadsInWork - 1; i++)
				{
					var nextSegmentGroup = segmentGroups[i];
					nextSegmentGroup.delta     = delta;
					nextSegmentGroup.indexFrom = entitiesNext;
					nextSegmentGroup.indexTo   = entitiesNext += entitiesPerThread;

					nextSegmentGroup.WorkDone.Reset();
					nextSegmentGroup.HasWork.Set();
				}

				segmentGroupLocal.indexFrom = entitiesNext;
				segmentGroupLocal.indexTo   = length;
				segmentGroupLocal.delta     = delta;
				jobAction(segmentGroupLocal);

				for (var i = 0; i < syncs.Length; i++)
				{
					syncs[i].WaitOne();
				}
			}
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Execute()
		{
			if (length > 0)
			{
				var entitiesNext      = 0;
				var threadsInWork     = 0;
				var entitiesPerThread = length / (threadsAmount + 1);
				if (entitiesPerThread > entitiesPerThreadMin)
				{
					threadsInWork = threadsAmount + 1;
				}
				else
				{
					threadsInWork     = length / entitiesPerThreadMin;
					entitiesPerThread = entitiesPerThreadMin;
				}


				for (var i = 0; i < threadsInWork - 1; i++)
				{
					var nextSegmentGroup = segmentGroups[i];
					nextSegmentGroup.indexFrom = entitiesNext;
					nextSegmentGroup.indexTo   = entitiesNext += entitiesPerThread;

					nextSegmentGroup.WorkDone.Reset();
					nextSegmentGroup.HasWork.Set();
				}

				segmentGroupLocal.indexFrom = entitiesNext;
				segmentGroupLocal.indexTo   = length;
				jobAction(segmentGroupLocal);

				for (var i = 0; i < syncs.Length; i++)
				{
					syncs[i].WaitOne();
				}
			}
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void HandleThread(object objSegmentGroup)
		{
			var segmentGroup = (SegmentGroup) objSegmentGroup;
			try
			{
				while (Thread.CurrentThread.IsAlive)
				{
					segmentGroup.HasWork.WaitOne();
					segmentGroup.HasWork.Reset();

					jobAction(segmentGroup);
					segmentGroup.WorkDone.Set();
				}
			}
			catch
			{
			}
		}


		#region EQUALS

		public bool Equals(GroupCore other)
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
			return obj.GetType() == GetType() && Equals((GroupCore) obj);
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
			GroupCore g;
			int position;


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal Enumerator(GroupCore g)
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


	public class Group<T> : GroupCore
	{
		internal override GroupCore Initialize(Composition composition)
		{
			var gr = base.Initialize(composition);

			composition.generations = new int[1];
			CompositionAdd<T>(composition, 0);

			return gr;
		}
	}

	public class Group<T, Y> : GroupCore
	{
		internal override GroupCore Initialize(Composition composition)
		{
			var gr = base.Initialize(composition);

			composition.generations = new int[2];
			CompositionAdd<T>(composition, 0);
			CompositionAdd<Y>(composition, 1);
			return gr;
		}
	}

	public class Group<T, Y, U> : GroupCore
	{
		internal override GroupCore Initialize(Composition composition)
		{
			var gr = base.Initialize(composition);
			composition.generations = new int[3];
			CompositionAdd<T>(composition, 0);
			CompositionAdd<Y>(composition, 1);
			CompositionAdd<U>(composition, 2);
			return gr;
		}
	}

	public class Group<T, Y, U, I> : GroupCore
	{
		internal override GroupCore Initialize(Composition composition)
		{
			var gr  = base.Initialize(composition);
			composition.generations = new int[4];
			CompositionAdd<T>(composition, 0);
			CompositionAdd<Y>(composition, 1);
			CompositionAdd<U>(composition, 2);
			CompositionAdd<I>(composition, 3);
			return gr;
		}
	}

	public class Group<T, Y, U, I, O> : GroupCore
	{
		internal override GroupCore Initialize(Composition composition)
		{
			var gr = base.Initialize(composition);
			composition.generations = new int[5];
			CompositionAdd<T>(composition, 0);
			CompositionAdd<Y>(composition, 1);
			CompositionAdd<U>(composition, 2);
			CompositionAdd<I>(composition, 3);
			CompositionAdd<O>(composition, 4);
			return gr;
		}
	}

	public class Group<T, Y, U, I, O, P> : GroupCore
	{
		internal override GroupCore Initialize(Composition composition)
		{
			var gr = base.Initialize(composition);
			composition.generations = new int[6];
			CompositionAdd<T>(composition, 0);
			CompositionAdd<Y>(composition, 1);
			CompositionAdd<U>(composition, 2);
			CompositionAdd<I>(composition, 3);
			CompositionAdd<O>(composition, 4);
			CompositionAdd<P>(composition, 5);
			return gr;
		}
	}

	public class Group<T, Y, U, I, O, P, A> : GroupCore
	{
		internal override GroupCore Initialize(Composition composition)
		{
			var gr = base.Initialize(composition);
			composition.generations = new int[7];
			CompositionAdd<T>(composition, 0);
			CompositionAdd<Y>(composition, 1);
			CompositionAdd<U>(composition, 2);
			CompositionAdd<I>(composition, 3);
			CompositionAdd<O>(composition, 4);
			CompositionAdd<P>(composition, 5);
			CompositionAdd<A>(composition, 6);
			return gr;
		}
	}

	public class Group<T, Y, U, I, O, P, A, S> : GroupCore
	{
		internal override GroupCore Initialize(Composition composition)
		{
			var gr = base.Initialize(composition);
			composition.generations = new int[8];
			CompositionAdd<T>(composition, 0);
			CompositionAdd<Y>(composition, 1);
			CompositionAdd<U>(composition, 2);
			CompositionAdd<I>(composition, 3);
			CompositionAdd<O>(composition, 4);
			CompositionAdd<P>(composition, 5);
			CompositionAdd<A>(composition, 6);
			CompositionAdd<S>(composition, 7);
			return gr;
		}
	}

	public class Group<T, Y, U, I, O, P, A, S, D> : GroupCore
	{
		internal override GroupCore Initialize(Composition composition)
		{
			var gr = base.Initialize(composition);
			composition.generations = new int[9];
			CompositionAdd<T>(composition, 0);
			CompositionAdd<Y>(composition, 1);
			CompositionAdd<U>(composition, 2);
			CompositionAdd<I>(composition, 3);
			CompositionAdd<O>(composition, 4);
			CompositionAdd<P>(composition, 5);
			CompositionAdd<A>(composition, 6);
			CompositionAdd<S>(composition, 7);
			CompositionAdd<D>(composition, 8);
			return gr;
		}
	}

	public class Group<T, Y, U, I, O, P, A, S, D, F> : GroupCore
	{
		internal override GroupCore Initialize(Composition composition)
		{
			var gr = base.Initialize(composition);
			composition.generations = new int[10];
			CompositionAdd<T>(composition, 0);
			CompositionAdd<Y>(composition, 1);
			CompositionAdd<U>(composition, 2);
			CompositionAdd<I>(composition, 3);
			CompositionAdd<O>(composition, 4);
			CompositionAdd<P>(composition, 5);
			CompositionAdd<A>(composition, 6);
			CompositionAdd<S>(composition, 7);
			CompositionAdd<D>(composition, 8);
			CompositionAdd<F>(composition, 9);
			return gr;

		}
	}
}