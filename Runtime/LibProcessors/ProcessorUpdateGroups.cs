﻿using System.Collections.Generic;
using Source.Runtime;
using UnityEngine;

namespace Pixeye.Actors
{
  public class ProcessorUpdateGroups : Processor, ITick
  {
    // public GroupManual source = new GroupManual();
    public static readonly List<ent> SourceAdd     = new List<ent>(256);
    public static readonly List<ent> SourceRemove  = new List<ent>(256);
    public static readonly List<ent> SourceRelease = new List<ent>(256);

    private readonly List<ent> _sourceAdded    = new List<ent>(256);
    private readonly List<ent> _sourceRemoved  = new List<ent>(256);
    private readonly List<ent> _sourceReleased = new List<ent>(256);

    public void Tick(float delta)
    {
      foreach (var entity in _sourceAdded)
      {
        for (int i = 0; i < groups.Added.length; i++)
        {
          var group = groups.Added.Elements[i];
          if (group.HasEntity(entity, out var index))
          {
            debug.log($"Processor Update Group Clear Add ent: {entity} {group}");
            group.RemoveAt(index);
          }
        }
      }

      foreach (var entity in _sourceRemoved)
      {
        for (int i = 0; i < groups.Removed.length; i++)
        {
          var group = groups.Removed.Elements[i];
          if (group.HasEntity(entity, out var index))
            group.RemoveAt(index);
        }
      }

      foreach (var entity in _sourceReleased)
      {
        for (int i = 0; i < groups.Released.length; i++)
        {
          var group = groups.Released.Elements[i];
          if (group.HasEntity(entity, out var index))
            group.RemoveAt(index);
        }
      }

      _sourceAdded.Clear();
      _sourceRemoved.Clear();
      _sourceReleased.Clear();


      foreach (var entity in SourceRelease)
      {
        for (int l = 0; l < groups.globalsLen; l++)
        {
          var group = groups.globals[l];
          if (group.HasEntity(entity, out var index))
            group.RemoveAt(index);
        }

        for (int i = 0; i < groups.Released.length; i++)
        {
          var group = groups.Released.Elements[i];
          if (!group.HasEntity(entity, out var index))
            if (group.composition.OverlapComponents(entity.id))
            {
              group.Insert(entity);
              _sourceReleased.Add(entity);
            }
        }
      }

      foreach (var entity in SourceAdd)
      {
        for (int l = 0; l < groups.globalsLen; l++)
        {
          var group = groups.globals[l];
          if (!group.HasEntity(entity, out var index))
          {
            if (group.composition.OverlapComponents(entity.id))
            {
              group.Insert(entity);
            }
          }
        }

        for (int i = 0; i < groups.Added.length; i++)
        {
          var group = groups.Added.Elements[i];
          if (!entity.wasInGroupAdded.Has(group.id))
          {
            if (!group.HasEntity(entity, out var index))
              if (group.composition.OverlapComponents(entity.id))
              {
                entity.wasInGroupAdded.Add(group.id);
                group.Insert(entity);
                _sourceAdded.Add(entity);
              }
          }
        }
      }

      foreach (var entity in SourceRemove)
      {
        for (int l = 0; l < groups.globalsLen; l++)
        {
          var group = groups.globals[l];
          if (group.HasEntity(entity, out var index))
            if (!group.composition.OverlapComponents(entity.id))
              group.RemoveAt(index);
        }

        for (int i = 0; i < groups.Removed.length; i++)
        {
          var group = groups.Removed.Elements[i];
          if (!entity.wasInGroupAdded.Has(group.id))
          {
            if (!group.HasEntity(entity, out var index))
              if (group.composition.OverlapComponents(entity.id))
              {
                entity.wasInGroupRemoved.Add(group.id);
                group.Insert(entity);
                _sourceRemoved.Add(entity);
              }
          }
        }
      }

      SourceRemove.Clear();
      SourceRelease.Clear();
      SourceAdd.Clear();
    }
  }
}