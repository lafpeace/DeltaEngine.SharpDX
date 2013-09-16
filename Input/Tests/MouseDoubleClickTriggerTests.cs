﻿using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Fonts;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class MouseDoubleClickTriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void PressLeftMouseButtonToCloseWindow()
		{
			new FontText(Font.Default, "Double Click Mouse Button to close window", Rectangle.One);
			new Command(() => Resolve<Window>().CloseAfterFrame()).Add(new MouseDoubleClickTrigger());
		}
	}
}