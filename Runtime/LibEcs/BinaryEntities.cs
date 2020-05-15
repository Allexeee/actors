// //  Project  : ACTORS
// //  Contacts : Pixeye - ask@pixeye.games
//
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Runtime.CompilerServices;
// using System.Threading;
// using Unity.IL2CPP.CompilerServices;
//
//
// namespace Pixeye.Actors
// {
//   class BinaryEntitiesComparer : IEqualityComparer<BinaryEntities>
//   {
//     public static BinaryEntitiesComparer Default = new BinaryEntitiesComparer();
//
//     public bool Equals(GroupCore x, GroupCore y)
//     {
//       return y.id == x.id;
//     }
//
//     public int GetHashCode(GroupCore obj)
//     {
//       return obj.composition.Hash;
//     }
//   }
//
//   [Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks | Option.DivideByZeroChecks, false)]
//   public class BinaryEntities : IEnumerable, IEquatable<BinaryEntities>, IDisposable
//   {
//     public ents entities = new ents(Framework.Settings.SizeEntities);
//
//     public Composition composition;
//
//     internal int id;
//
//     int position;
//
//
//     public ref ent this[int index]
//     {
//       [MethodImpl(MethodImplOptions.AggressiveInlining)]
//       get => ref entities[index];
//     }
//     
//
//     public override string ToString()
//     {
//       return $"Group ents: {entities} composition: {composition}";
//     }
//
//
//     //===============================//
//     // Insert
//     //===============================//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void Insert(in ent entity)
//     {
//       var left  = 0;
//       var index = 0;
//       var right = entities.length + 1;
//
//       // // todo: сделать как проверку в редакторе
//       // if (entity.id >= entities.Length)
//       // {
//       //   Array.Resize(ref entities, entity.id << 1);
//       // }
//       // else if (length >= entities.Length)
//       // {
//       //   Array.Resize(ref entities, length << 1);
//       // }
//
//       var consitionSort = right - 1;
//       if (consitionSort > -1 && entity.id < entities[consitionSort].id)
//       {
//         // debug.log($"{entity.id}, {consitionSort}, {entities[consitionSort].id}, {entities}");
//         HelperArray.BinarySearch(ref entities.source, entity.id, ref left, ref right);
//         index = left;
//         Array.Copy(entities.source, index, entities.source, index + 1, entities.length - index);
//         entities[index] = entity;
//       }
//       else
//       {
//         entities[right] = entity;
//       }
//     }
//
//     public bool HasEntity(in ent entity, out int index)
//     {
//       index = -1;
//       if (length == 0) return false;
//
//       index = HelperArray.BinarySearch(ref entities, entity.id, 0, length - 1);
//       if (index == -1) return false;
//       return true;
//     }
//
//     //===============================//
//     // Remove
//     //===============================//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void RemoveAt(int i)
//     {
//       if (i < --length)
//         Array.Copy(entities, i + 1, entities, i, length - i);
//     }
//
//
//     public virtual void Dispose()
//     {
//       //parallel
//       if (segmentGroups != null)
//         for (int i = 0; i < segmentGroups.Length; i++)
//         {
//           var d = segmentGroups[i];
//           d.thread.Interrupt();
//           d.thread.Join(5);
//           syncs[i].Close();
//           segmentGroups[i] = null;
//         }
//
//       segmentGroupLocal = null;
//     }
//
//     //===============================//
//     // Concurrent
//     //===============================//
//
//     SegmentGroup       segmentGroupLocal;
//     SegmentGroup[]     segmentGroups;
//     ManualResetEvent[] syncs;
//
//
//     int threadsAmount        = Environment.ProcessorCount - 1;
//     int entitiesPerThreadMin = 5000;
//     int entitiesPerThread;
//
//     HandleSegmentGroup jobAction;
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void MakeConcurrent(int minEntities, int threads, HandleSegmentGroup jobAction)
//     {
//       this.jobAction = jobAction;
//
//       segmentGroupLocal        = new SegmentGroup();
//       segmentGroupLocal.source = this;
//
//
//       segmentGroups = new SegmentGroup[threadsAmount];
//       syncs         = new ManualResetEvent[threadsAmount];
//
//       for (int i = 0; i < threadsAmount; i++)
//       {
//         ref var nextSegment = ref segmentGroups[i];
//         nextSegment                     = new SegmentGroup();
//         nextSegment.thread              = new Thread(HandleThread);
//         nextSegment.thread.IsBackground = true;
//         nextSegment.HasWork             = new ManualResetEvent(false);
//         nextSegment.WorkDone            = new ManualResetEvent(true);
//         nextSegment.source              = this;
//
//         syncs[i] = nextSegment.WorkDone;
//
//         nextSegment.thread.Start(nextSegment);
//       }
//     }
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void Execute(float delta)
//     {
//       if (length > 0)
//       {
//         var entitiesNext      = 0;
//         var threadsInWork     = 0;
//         var entitiesPerThread = length / (threadsAmount + 1);
//         if (entitiesPerThread > entitiesPerThreadMin)
//         {
//           threadsInWork = threadsAmount + 1;
//         }
//         else
//         {
//           threadsInWork     = length / entitiesPerThreadMin;
//           entitiesPerThread = entitiesPerThreadMin;
//         }
//
//
//         for (var i = 0; i < threadsInWork - 1; i++)
//         {
//           var nextSegmentGroup = segmentGroups[i];
//           nextSegmentGroup.delta     = delta;
//           nextSegmentGroup.indexFrom = entitiesNext;
//           nextSegmentGroup.indexTo   = entitiesNext += entitiesPerThread;
//
//           nextSegmentGroup.WorkDone.Reset();
//           nextSegmentGroup.HasWork.Set();
//         }
//
//         segmentGroupLocal.indexFrom = entitiesNext;
//         segmentGroupLocal.indexTo   = length;
//         segmentGroupLocal.delta     = delta;
//         jobAction(segmentGroupLocal);
//
//         for (var i = 0; i < syncs.Length; i++)
//         {
//           syncs[i].WaitOne();
//         }
//       }
//     }
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void Execute()
//     {
//       if (length > 0)
//       {
//         var entitiesNext      = 0;
//         var threadsInWork     = 0;
//         var entitiesPerThread = length / (threadsAmount + 1);
//         if (entitiesPerThread > entitiesPerThreadMin)
//         {
//           threadsInWork = threadsAmount + 1;
//         }
//         else
//         {
//           threadsInWork     = length / entitiesPerThreadMin;
//           entitiesPerThread = entitiesPerThreadMin;
//         }
//
//
//         for (var i = 0; i < threadsInWork - 1; i++)
//         {
//           var nextSegmentGroup = segmentGroups[i];
//           nextSegmentGroup.indexFrom = entitiesNext;
//           nextSegmentGroup.indexTo   = entitiesNext += entitiesPerThread;
//
//           nextSegmentGroup.WorkDone.Reset();
//           nextSegmentGroup.HasWork.Set();
//         }
//
//         segmentGroupLocal.indexFrom = entitiesNext;
//         segmentGroupLocal.indexTo   = length;
//         jobAction(segmentGroupLocal);
//
//         for (var i = 0; i < syncs.Length; i++)
//         {
//           syncs[i].WaitOne();
//         }
//       }
//     }
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     protected void HandleThread(object objSegmentGroup)
//     {
//       var segmentGroup = (SegmentGroup) objSegmentGroup;
//       try
//       {
//         while (Thread.CurrentThread.IsAlive)
//         {
//           segmentGroup.HasWork.WaitOne();
//           segmentGroup.HasWork.Reset();
//
//           jobAction(segmentGroup);
//           segmentGroup.WorkDone.Set();
//         }
//       }
//       catch
//       {
//       }
//     }
//
//
//     #region EQUALS
//
//     public bool Equals(GroupCore other)
//     {
//       return id == other.id;
//     }
//
//     public bool Equals(int other)
//     {
//       return id == other;
//     }
//
//     public override bool Equals(object obj)
//     {
//       if (ReferenceEquals(null, obj)) return false;
//       if (ReferenceEquals(this, obj)) return true;
//       return obj.GetType() == GetType() && Equals((GroupCore) obj);
//     }
//
//     public override int GetHashCode()
//     {
//       return id;
//     }
//
//     #endregion
//
//     #region ENUMERATOR
//
//     public Enumerator GetEnumerator()
//     {
//       return new Enumerator(this);
//     }
//
//     IEnumerator IEnumerable.GetEnumerator()
//     {
//       return GetEnumerator();
//     }
//
//     public struct Enumerator : IEnumerator<ent>
//     {
//       GroupCore g;
//       int       position;
//
//
//       [MethodImpl(MethodImplOptions.AggressiveInlining)]
//       internal Enumerator(GroupCore g)
//       {
//         position = -1;
//         this.g   = g;
//       }
//
//       [MethodImpl(MethodImplOptions.AggressiveInlining)]
//       public bool MoveNext()
//       {
//         return ++position < g.length;
//       }
//
//       [MethodImpl(MethodImplOptions.AggressiveInlining)]
//       public void Reset()
//       {
//         position = -1;
//       }
//
//       object IEnumerator.Current => Current;
//
//       public ent Current
//       {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get { return g.entities[position]; }
//       }
//
//       [MethodImpl(MethodImplOptions.AggressiveInlining)]
//       public void Dispose()
//       {
//         g = null;
//       }
//     }
//
//     #endregion
//   }
//
//
//   public class Group<T> : GroupCore 
//     where T : class, new()
//   {
//     internal override GroupCore Initialize(Composition composition)
//     {
//       var gr = base.Initialize(composition);
//
//       CompositionAdd<T>();
//
//       return gr;
//     }
//   }
//
//   public class Group<T, Y> : GroupCore 
//     where T : class, new()
//     where Y : class, new()
//   {
//     internal override GroupCore Initialize(Composition composition)
//     {
//       var gr = base.Initialize(composition);
//
//       CompositionAdd<T>();
//       CompositionAdd<Y>();
//       return gr;
//     }
//   }
//
//   public class Group<T, Y, U> : GroupCore 
//     where T : class, new()
//     where Y : class, new()
//     where U : class, new()
//   {
//     internal override GroupCore Initialize(Composition composition)
//     {
//       var gr = base.Initialize(composition);
//       CompositionAdd<T>();
//       CompositionAdd<Y>();
//       CompositionAdd<U>();
//       return gr;
//     }
//   }
//
//   public class Group<T, Y, U, I> : GroupCore 
//     where T : class, new()
//     where Y : class, new()
//     where U : class, new()
//     where I : class, new()
//   {
//     internal override GroupCore Initialize(Composition composition)
//     {
//       var gr = base.Initialize(composition);
//       CompositionAdd<T>();
//       CompositionAdd<Y>();
//       CompositionAdd<U>();
//       CompositionAdd<I>();
//       return gr;
//     }
//   }
//
//   public class Group<T, Y, U, I, O> : GroupCore 
//     where T : class, new()
//     where Y : class, new()
//     where U : class, new()
//     where I : class, new()
//     where O : class, new()
//   {
//     internal override GroupCore Initialize(Composition composition)
//     {
//       var gr = base.Initialize(composition);
//       CompositionAdd<T>();
//       CompositionAdd<Y>();
//       CompositionAdd<U>();
//       CompositionAdd<I>();
//       CompositionAdd<O>();
//       return gr;
//     }
//   }
//
//   public class Group<T, Y, U, I, O, P> : GroupCore 
//     where T : class, new()
//     where Y : class, new()
//     where U : class, new()
//     where I : class, new()
//     where O : class, new()
//     where P : class, new()
//   {
//     internal override GroupCore Initialize(Composition composition)
//     {
//       var gr = base.Initialize(composition);
//       CompositionAdd<T>();
//       CompositionAdd<Y>();
//       CompositionAdd<U>();
//       CompositionAdd<I>();
//       CompositionAdd<O>();
//       CompositionAdd<P>();
//       return gr;
//     }
//   }
//
//   public class Group<T, Y, U, I, O, P, A> : GroupCore 
//     where T : class, new()
//     where Y : class, new()
//     where U : class, new()
//     where I : class, new()
//     where O : class, new()
//     where P : class, new()
//     where A : class, new()
//   {
//     internal override GroupCore Initialize(Composition composition)
//     {
//       var gr = base.Initialize(composition);
//       CompositionAdd<T>();
//       CompositionAdd<Y>();
//       CompositionAdd<U>();
//       CompositionAdd<I>();
//       CompositionAdd<O>();
//       CompositionAdd<P>();
//       CompositionAdd<A>();
//       return gr;
//     }
//   }
//
//   public class Group<T, Y, U, I, O, P, A, S> : GroupCore 
//     where T : class, new()
//     where Y : class, new()
//     where U : class, new()
//     where I : class, new()
//     where O : class, new()
//     where P : class, new()
//     where A : class, new()
//     where S : class, new()
//   {
//     internal override GroupCore Initialize(Composition composition)
//     {
//       var gr = base.Initialize(composition);
//       CompositionAdd<T>();
//       CompositionAdd<Y>();
//       CompositionAdd<U>();
//       CompositionAdd<I>();
//       CompositionAdd<O>();
//       CompositionAdd<P>();
//       CompositionAdd<A>();
//       CompositionAdd<S>();
//       return gr;
//     }
//   }
//
//   public class Group<T, Y, U, I, O, P, A, S, D> : GroupCore
//     where T : class, new()
//     where Y : class, new()
//     where U : class, new()
//     where I : class, new()
//     where O : class, new()
//     where P : class, new()
//     where A : class, new()
//     where S : class, new()
//     where D : class, new()
//   {
//     internal override GroupCore Initialize(Composition composition)
//     {
//       var gr = base.Initialize(composition);
//       CompositionAdd<T>();
//       CompositionAdd<Y>();
//       CompositionAdd<U>();
//       CompositionAdd<I>();
//       CompositionAdd<O>();
//       CompositionAdd<P>();
//       CompositionAdd<A>();
//       CompositionAdd<S>();
//       CompositionAdd<D>();
//       return gr;
//     }
//   }
//
//   public class Group<T, Y, U, I, O, P, A, S, D, F> : GroupCore
//     where T : class, new()
//     where Y : class, new()
//     where U : class, new()
//     where I : class, new()
//     where O : class, new()
//     where P : class, new()
//     where A : class, new()
//     where S : class, new()
//     where D : class, new()
//     where F : class, new()
//   {
//     internal override GroupCore Initialize(Composition composition)
//     {
//       var gr = base.Initialize(composition);
//       CompositionAdd<T>();
//       CompositionAdd<Y>();
//       CompositionAdd<U>();
//       CompositionAdd<I>();
//       CompositionAdd<O>();
//       CompositionAdd<P>();
//       CompositionAdd<A>();
//       CompositionAdd<S>();
//       CompositionAdd<D>();
//       CompositionAdd<F>();
//       return gr;
//     }
//   }
// }