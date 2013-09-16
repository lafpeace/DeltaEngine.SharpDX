﻿using DeltaEngine.Core;
using DeltaEngine.Multimedia;
using DeltaEngine.Platforms;

namespace Breakout
{
	/// <summary>
	/// Simple breakout game with more bricks to destroy each level you advance.
	/// </summary>
	public class Program : App
	{
		public Program()
		{
			Resolve<Settings>().UpdatesPerSecond = 60;
			new Game(Resolve<Window>(), Resolve<SoundDevice>());
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}