﻿using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using SharpDX.XInput;

namespace DeltaEngine.Input.SharpDX
{
	/// <summary>
	/// Native implementation of the GamePad interface using XInput.
	/// </summary>
	public class SharpDXGamePad : GamePad
	{
		public SharpDXGamePad(GamePadNumber number = GamePadNumber.Any)
			: base(number)
		{
			controller = new Controller(GetUserIndexFromNumber());
			states = new State[GamePadButton.A.GetCount()];
			for (int i = 0; i < states.Length; i++)
				states[i] = State.Released;
		}

		private Controller controller;
		private readonly State[] states;

		private UserIndex GetUserIndexFromNumber()
		{
			if (Number == GamePadNumber.Any)
				return GetAnyController();
			if (Number == GamePadNumber.Two)
				return UserIndex.Two;
			if (Number == GamePadNumber.Three)
				return UserIndex.Three;
			return Number == GamePadNumber.Four ? UserIndex.Four : UserIndex.One;
		}

		private static UserIndex GetAnyController()
		{
			if (new Controller(UserIndex.Two).IsConnected)
				return UserIndex.Two;
			if (new Controller(UserIndex.Three).IsConnected)
				return UserIndex.Three;
			return new Controller(UserIndex.Three).IsConnected ? UserIndex.Four : UserIndex.One;
		}

		public override void Dispose()
		{
			controller = null;
		}

		public override bool IsAvailable
		{
			get { return controller != null && controller.IsConnected; }
			protected set { }
		}

		public override void Vibrate(float strength)
		{
			var motorSpeed = (ushort)(strength * ushort.MaxValue);
			controller.SetVibration(new Vibration
			{
				LeftMotorSpeed = motorSpeed,
				RightMotorSpeed = motorSpeed
			});
		}

		private Vector2D leftThumbStick;
		private Vector2D rightThumbStick;
		private float leftTrigger;
		private float rightTrigger;

		private static GamePadButton ConvertButtonEnum(XInputGamePadButton button)
		{
			if (IsPadDirectionButton(button))
				return ConvertPadDirections(button);
			if (IsPadButton(button))
				return ConvertButtons(button);
			if (IsPadStick(button))
				return ConvertSticks(button);
			if (IsPadShoulder(button))
				return ConvertShoulders(button);
			return ConvertStartOrBack(button);
		}

		private static bool IsPadDirectionButton(XInputGamePadButton button)
		{
			return button == XInputGamePadButton.DPadUp || button == XInputGamePadButton.DPadDown ||
				button == XInputGamePadButton.DPadLeft || button == XInputGamePadButton.DPadRight;
		}

		private static GamePadButton ConvertPadDirections(XInputGamePadButton button)
		{
			switch (button)
			{
			case XInputGamePadButton.DPadUp:
				return GamePadButton.Up;
			case XInputGamePadButton.DPadDown:
				return GamePadButton.Down;
			case XInputGamePadButton.DPadLeft:
				return GamePadButton.Left;
			case XInputGamePadButton.DPadRight:
				return GamePadButton.Right;
			}
			return (GamePadButton)(-1);
		}

		private static bool IsPadButton(XInputGamePadButton button)
		{
			return button == XInputGamePadButton.A || button == XInputGamePadButton.B ||
				button == XInputGamePadButton.X || button == XInputGamePadButton.Y;
		}

		private static GamePadButton ConvertButtons(XInputGamePadButton button)
		{
			switch (button)
			{
			case XInputGamePadButton.A:
				return GamePadButton.A;
			case XInputGamePadButton.B:
				return GamePadButton.B;
			case XInputGamePadButton.X:
				return GamePadButton.X;
			case XInputGamePadButton.Y:
				return GamePadButton.Y;
			}
			return (GamePadButton)(-1);
		}

		private static bool IsPadStick(XInputGamePadButton button)
		{
			return button == XInputGamePadButton.LeftThumb || button == XInputGamePadButton.RightThumb;
		}

		private static GamePadButton ConvertSticks(XInputGamePadButton button)
		{
			switch (button)
			{
			case XInputGamePadButton.LeftThumb:
				return GamePadButton.LeftStick;
			case XInputGamePadButton.RightThumb:
				return GamePadButton.RightStick;
			}
			return (GamePadButton)(-1);
		}

		private static bool IsPadShoulder(XInputGamePadButton button)
		{
			return button == XInputGamePadButton.LeftShoulder ||
				button == XInputGamePadButton.RightShoulder;
		}

		private static GamePadButton ConvertShoulders(XInputGamePadButton button)
		{
			switch (button)
			{
			case XInputGamePadButton.LeftShoulder:
				return GamePadButton.LeftShoulder;
			case XInputGamePadButton.RightShoulder:
				return GamePadButton.RightShoulder;
			}
			return (GamePadButton)(-1);
		}

		private static GamePadButton ConvertStartOrBack(XInputGamePadButton button)
		{
			if (button == XInputGamePadButton.Start)
				return GamePadButton.Start;
			if (button == XInputGamePadButton.Back)
				return GamePadButton.Back;
			return (GamePadButton)(-1);
		}

		private static bool IsXInputButtonPressed(int bitfield, XInputGamePadButton button)
		{
			return (bitfield & (int)button) != 0;
		}

		public override Vector2D GetLeftThumbStick()
		{
			return leftThumbStick;
		}

		public override Vector2D GetRightThumbStick()
		{
			return rightThumbStick;
		}

		public override float GetLeftTrigger()
		{
			return leftTrigger;
		}

		public override float GetRightTrigger()
		{
			return rightTrigger;
		}

		public override State GetButtonState(GamePadButton button)
		{
			return states[(int)button];
		}

		protected override void UpdateGamePadStates()
		{
			global::SharpDX.XInput.State state;
			controller.GetState(out state);
			UpdateThumbSticks(state);
			leftTrigger = NormalizeByteToFloat(state.Gamepad.LeftTrigger);
			rightTrigger = NormalizeByteToFloat(state.Gamepad.RightTrigger);
			UpdateAllButtons((int)state.Gamepad.Buttons);
		}

		private void UpdateThumbSticks(global::SharpDX.XInput.State state)
		{
			leftThumbStick.X = (int)NormalizeShortToFloat(state.Gamepad.LeftThumbX);
			leftThumbStick.Y = (int)NormalizeShortToFloat(state.Gamepad.LeftThumbY);
			rightThumbStick.X = (int)NormalizeShortToFloat(state.Gamepad.RightThumbX);
			rightThumbStick.Y = (int)NormalizeShortToFloat(state.Gamepad.RightThumbY);
		}

		private static float NormalizeShortToFloat(short value)
		{
			return value * (1f / short.MaxValue);
		}

		private static float NormalizeByteToFloat(byte value)
		{
			return value * (1f / byte.MaxValue);
		}

		private void UpdateAllButtons(int bitfield)
		{
			Array buttons = XInputGamePadButton.A.GetEnumValues();
			foreach (XInputGamePadButton button in buttons)
				UpdateButton(ConvertButtonEnum(button), IsXInputButtonPressed(bitfield, button));
		}

		private void UpdateButton(GamePadButton button, bool nowPressed)
		{
			var buttonIndex = (int)button;
			states[buttonIndex] = states[buttonIndex].UpdateOnNativePressing(nowPressed);
		}
	}
}