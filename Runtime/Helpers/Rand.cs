//  Project : ecs
// Contacts : Pix - ask@pixeye.games

using System;

namespace Pixeye.Framework
{
	public static class Rand
	{

		public static Random Source = new Random(DateTime.Today.Second);
		
		public static float Get(float minimum, float maximum)
		{
			return (float) Source.NextDouble() * (maximum - minimum) + minimum;
		}
	}
}