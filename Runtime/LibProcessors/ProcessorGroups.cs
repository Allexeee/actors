//  Project  : ACTORS
//  Contacts : Pixeye - ask@pixeye.games

using System;
using System.Reflection;
using Unity.IL2CPP.CompilerServices;


namespace Pixeye.Actors
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks | Option.DivideByZeroChecks, false)]
	sealed class ProcessorGroups
	{
		public static void Setup(object b)
		{
			var type         = b.GetType();
			var objectFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
			int length       = objectFields.Length;

			var groupType = typeof(GroupCore);
			var groupByProcAttribute      = Attribute.GetCustomAttribute(type, typeof(GroupByAttribute)) as GroupByAttribute;
			var groupExcludeProcAttribute = Attribute.GetCustomAttribute(type, typeof(ExcludeAttribute)) as ExcludeAttribute;
			var groupBindProcAttribute    = Attribute.GetCustomAttribute(type, typeof(BindAttribute)) as BindAttribute;


			for (int i = 0; i < length; i++)
			{
				var myFieldInfo = objectFields[i];

				if (myFieldInfo.FieldType.IsSubclassOf(groupType))
				{
					// check if the group located inside of the base processor
					var inner = Attribute.GetCustomAttribute(myFieldInfo, typeof(InnerGroupAttribute)) as InnerGroupAttribute;

					// if group is located inside of the base processor use processor filtering 
					var groupByAttribute      = inner != null ? groupByProcAttribute : Attribute.GetCustomAttribute(myFieldInfo, typeof(GroupByAttribute)) as GroupByAttribute;
					var groupExcludeAttribute = inner != null ? groupExcludeProcAttribute : Attribute.GetCustomAttribute(myFieldInfo, typeof(ExcludeAttribute)) as ExcludeAttribute;
					var bindAttribute         = inner != null ? groupBindProcAttribute : Attribute.GetCustomAttribute(myFieldInfo, typeof(BindAttribute)) as BindAttribute;


					var excludeCompFilter = new int[0];

					if (groupExcludeAttribute != null)
					{
						excludeCompFilter = groupExcludeAttribute.filterType;
					}

					var composition = new Composition();
					composition.AddTypesExclude(excludeCompFilter);


					composition.hash = HashCode.OfEach(myFieldInfo.FieldType.GetGenericArguments()).And(31).AndEach(excludeCompFilter);

					var group = SetupGroup(myFieldInfo.FieldType, composition, myFieldInfo.GetValue(b));
					myFieldInfo.SetValue(b, group);

					if (bindAttribute != null)
					{
						if (bindAttribute.id >= groups.globals.Length)
							Array.Resize(ref groups.globals, bindAttribute.id + 5);

						groups.globals[bindAttribute.id] = group;
					}
				}
			}
		}

		internal static GroupCore SetupGroup(Type groupType, Composition composition, object fieldObj)
		{
			if (groups.All.TryGetValue(groupType, composition, out GroupCore group))
			{
				return group;
			}

			if (fieldObj != null)
			{
				group = fieldObj as GroupCore;
				return groups.globals[groups.globalsLen++] = groups.All.Add(CreateGroup(groupType, composition));
			}

			return groups.globals[groups.globalsLen++] = groups.All.Add(CreateGroup(groupType, composition));
		}

		internal static GroupCore CreateGroup(Type groupType, Composition composition)
		{
			var gr = (Activator.CreateInstance(groupType, true) as GroupCore).Initialize(composition);
			gr.id = groups.All.length;
			return gr;
		}
	}
}