//  Project  : ACTORS
//  Contacts : Pixeye - ask@pixeye.games

using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Pixeye.Actors
{
  [Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks | Option.DivideByZeroChecks, false)]
  public class Composition : IEquatable<Composition>
  {
    public readonly indexes ComponentsIds = new indexes(8);

    internal readonly HashCode Hash;

    public Composition(HashCode hash)
    {
      Hash = hash;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool OverlapComponents(ent entity)
    {
      int match = 0;
      for (int i = 0; i < ComponentsIds.length; i++)
      {
        if (entity.Has(ComponentsIds[i]))
          match++;
      }

      return ComponentsIds.length == match;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Composition other)
    {
      return other != null && GetHashCode() == other.GetHashCode();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => Hash;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object obj)
    {
      return obj is Composition other && Equals(other);
    }

    public override string ToString()
    {
      return $"Composition {Hash} ids {ComponentsIds}";
    }
  }
}