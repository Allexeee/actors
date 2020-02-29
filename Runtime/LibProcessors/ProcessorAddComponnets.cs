using System.Collections.Generic;
using Source.Runtime;
using UnityEngine;

namespace Pixeye.Actors
{
  public class ProcessorAddComponents : Processor<ComponentAddComponents>, ITick
  {
    public void Tick(float delta)
    {
      foreach (var entity in source)
      {
        for (int l = 0; l < groups.globalsLen; l++)
        {
          var group = groups.globals[l];
          if (!group.HasEntity(entity, out var index))
            if (group.composition.OverlapComponents(entity))
              group.Insert(entity);
        }
      }

      source.length = 0;
    }
  }
}