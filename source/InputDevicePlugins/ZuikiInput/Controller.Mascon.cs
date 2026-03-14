//Copyright (c) 2026, Marc Riera, The OpenBVE Project
//
//Redistribution and use in source and binary forms, with or without
//modification, are permitted provided that the following conditions are met:
//
//1. Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
//2. Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
//ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using OpenTK.Input;

namespace ZuikiInput
{
	/// <summary>Class representing a ZUIKI Mascon controller.</summary>
	public class MasconController : Controller
	{
		/// <summary>Enumeration representing controller models.</summary>
		private enum ControllerModels
		{
			/// <summary>Unsupported controller</summary>
			Unsupported,
			/// <summary>ZUIKI Mascon</summary>
			Mascon,
			/// <summary>ZUIKI Mascon Pro</summary>
			MasconPro,
		};

		/// <summary>The controller model.</summary>
		private ControllerModels controllerModel;

		/// <summary>The min/max value for each brake notch, from Released to Emergency. Each notch consists of two values.</summary>
		private readonly double[] brakeValues;

		/// <summary>The min/max value for each power notch, from N to maximum. Each notch consists of two values.</summary>
		private readonly double[] powerValues;

		/// <summary>The controller index in OpenTK.</summary>
		private int controllerIndex;

		private MasconController(Guid guid, string name, int index, ControllerModels model, double[] brakeNotchValues, double[] powerNotchValues, bool hasReverser)
		{
			Guid = guid;
			Name = name;
			controllerIndex = index;
			controllerModel = model;
			brakeValues = brakeNotchValues;
			powerValues = powerNotchValues;
			Capabilities = new ControllerCapabilities(brakeNotchValues.Length / 2 - 2, powerNotchValues.Length / 2 - 1, hasReverser);
		}

		/// <summary>Lists the controllers supported by this class.</summary>
		internal static void GetControllers(Dictionary<Guid, Controller> controllerList)
		{
			Dictionary<Guid, Controller> controllers = new Dictionary<Guid, Controller>();

			// Check the first 10 joysticks, should be enough
			for (int i = 0; i < 10; i++)
			{
				Guid guid = Joystick.GetGuid(i);
				string name = Joystick.GetName(i);
				string controllerId = GetControllerId(guid);
				switch (controllerId)
				{
					// ZUIKI Mascon / Densha de GO! One Handle controller for Nintendo Switch
					case "0f0d:00c1":
					case "33dd:0001":
					case "33dd:0002":
					case "33dd:0003":
					case "33dd:0004":
					case "33dd:0005":
						// The controller uses buttons 7-10 and axis 2, we need those at minimum
						if (Joystick.GetCapabilities(i).ButtonCount >= 10 && Joystick.GetCapabilities(i).AxisCount >= 2)
						{
							double[] brake = { -0.0039, 0.0117, -0.2156, -0.2, -0.3254, -0.3098, -0.4352, -0.4196, -0.5372, -0.5215, -0.6470, -0.6313, -0.7568, -0.7411, -0.8588, -0.8431, -0.9686, -0.9529, -1, -0.9921 };
							double[] power = { -0.0039, 0.0117, 0.2392, 0.2549, 0.4274, 0.4431, 0.6078, 0.6235, 0.7960, 0.8117, 0.9921, 1 };
							Controller controller = new MasconController(guid, name, i, ControllerModels.Mascon, brake, power, false);
							controller.State.IsConnected = Joystick.GetState(i).IsConnected;
							if (!controllerList.ContainsKey(guid))
							{
								controllerList.Add(guid, controller);
							}
						}
						break;
					// ZUIKI Mascon Pro
					case "33dd:0006":
						// The controller uses buttons 7-10 and axis 2, we need those at minimum
						if (Joystick.GetCapabilities(i).ButtonCount >= 10 && Joystick.GetCapabilities(i).AxisCount >= 2)
						{
							double[] brake = { -0.0039, 0.0117, -0.2156, -0.2, -0.3254, -0.3098, -0.4352, -0.4196, -0.5372, -0.5215, -0.6470, -0.6313, -0.7568, -0.7411, -0.8588, -0.8431, -0.9686, -0.9529, -1, -0.9921 };
							double[] power = { -0.0039, 0.0117, 0.2392, 0.2549, 0.4274, 0.4431, 0.6078, 0.6235, 0.7960, 0.8117, 0.9921, 1 };
							Controller controller = new MasconController(guid, name, i, ControllerModels.Mascon, brake, power, true);
							controller.State.IsConnected = Joystick.GetState(i).IsConnected;
							if (!controllerList.ContainsKey(guid))
							{
								controllerList.Add(guid, controller);
							}
						}
						break;
					default:
						continue;
				}
			}
		}

		/// <summary>Updates the state of the controller.</summary>
		internal override void Update()
		{
			JoystickState state = Joystick.GetState(controllerIndex);
			UpdatePreviousState();
			State.IsConnected = state.IsConnected;

			if (State.IsConnected)
			{
				double handleAxis = Math.Round(state.GetAxis(1), 4);
				for (int i = 0; i < brakeValues.Length; i += 2)
				{
					// Each notch uses two values, minimum and maximum
					if (handleAxis >= brakeValues[i] && handleAxis <= brakeValues[i + 1])
					{
						if (i == 0)
						{
							State.BrakeNotch = 0;
						}
						else
						{
							State.BrakeNotch = i / 2;
						}
						break;
					}
				}
				for (int i = 0; i < powerValues.Length; i += 2)
				{
					// Each notch uses two values, minimum and maximum
					if (handleAxis >= powerValues[i] && handleAxis <= powerValues[i + 1])
					{
						if (i == 0)
						{
							State.PowerNotch = 0;
						}
						else
						{
							State.PowerNotch = i / 2;
						}
						break;
					}
				}
			}
		}

	}
}
