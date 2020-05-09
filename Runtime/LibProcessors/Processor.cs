//  Project  : ACTORS
//  Contacts : Pixeye - ask@pixeye.games

using System;

namespace Pixeye.Actors
{
  public abstract class Processor : IDisposable
  {
    protected Processor()
    {
      if (Framework.Processors.length == Framework.Processors.storage.Length)
        Array.Resize(ref Framework.Processors.storage, Framework.Processors.length << 1);

      Framework.Processors.storage[Framework.Processors.length++] = this;
      ProcessorUpdate.AddProc(this);
      ProcessorGroups.Setup(this);
      Toolbox.disposables.Add(this);
    }

    public void Dispose()
    {
      ProcessorUpdate.RemoveProc(this);
      OnDispose();
    }


    //===============================//
    // Events
    //===============================//

    public virtual void HandleEvents()
    {
    }

    protected virtual void OnDispose()
    {
    }
  }

  #region PROCESSORS

  public abstract class Processor<T> : Processor
    where T : class, new()
  {
    [InnerGroupAttribute] public Group<T> source = default;
  }

  public abstract class Processor<T, Y> : Processor
    where T : class, new()
    where Y : class, new()
  {
    [InnerGroupAttribute] public Group<T, Y> source = default;
  }

  public abstract class Processor<T, Y, U> : Processor
    where T : class, new()
    where Y : class, new()
    where U : class, new()
  {
    [InnerGroupAttribute] public Group<T, Y, U> source = default;
  }

  public abstract class Processor<T, Y, U, I> : Processor
    where T : class, new()
    where Y : class, new()
    where U : class, new()
    where I : class, new()
  {
    [InnerGroupAttribute] public Group<T, Y, U, I> source = default;
  }

  public abstract class Processor<T, Y, U, I, O> : Processor
    where T : class, new()
    where Y : class, new()
    where U : class, new()
    where I : class, new()
    where O : class, new()

  {
    [InnerGroupAttribute] public Group<T, Y, U, I, O> source = default;
  }

  public abstract class Processor<T, Y, U, I, O, P> : Processor
    where T : class, new()
    where Y : class, new()
    where U : class, new()
    where I : class, new()
    where O : class, new()
    where P : class, new()

  {
    [InnerGroupAttribute] public Group<T, Y, U, I, O, P> source = default;
  }

  public abstract class Processor<T, Y, U, I, O, P, A> : Processor
    where T : class, new()
    where Y : class, new()
    where U : class, new()
    where I : class, new()
    where O : class, new()
    where P : class, new()
    where A : class, new()

  {
    [InnerGroupAttribute] public Group<T, Y, U, I, O, P, A> source = default;
  }

  public abstract class Processor<T, Y, U, I, O, P, A, S> : Processor
    where T : class, new()
    where Y : class, new()
    where U : class, new()
    where I : class, new()
    where O : class, new()
    where P : class, new()
    where A : class, new()
    where S : class, new()

  {
    [InnerGroupAttribute] public Group<T, Y, U, I, O, P, A, S> source = default;
  }

  public abstract class Processor<T, Y, U, I, O, P, A, S, D> : Processor
    where T : class, new()
    where Y : class, new()
    where U : class, new()
    where I : class, new()
    where O : class, new()
    where P : class, new()
    where A : class, new()
    where S : class, new()
    where D : class, new()
  {
    [InnerGroupAttribute] public Group<T, Y, U, I, O, P, A, S, D> source = default;
  }

  public abstract class Processor<T, Y, U, I, O, P, A, S, D, F> : Processor
    where T : class, new()
    where Y : class, new()
    where U : class, new()
    where I : class, new()
    where O : class, new()
    where P : class, new()
    where A : class, new()
    where S : class, new()
    where D : class, new()
    where F : class, new()
  {
    [InnerGroupAttribute] public Group<T, Y, U, I, O, P, A, S, D, F> source = default;
  }

  #endregion

  class InnerGroupAttribute : Attribute
  {
  }
}