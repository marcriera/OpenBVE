//Simplified BSD License (BSD-2-Clause)
//
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
using System.Reflection;
using System.Windows.Forms;
using OpenBveApi.FileSystem;
using OpenBveApi.Hosts;
using OpenBveApi.Interface;
using OpenBveApi.Runtime;

namespace ZuikiInput
{
	/// <summary>Input Device Plugin class for controllers by ZUIKI</summary>
	public class ZuikiInput : ITrainInputDevice
	{
		/// <summary>The number of brake controls (maximum brake notches + released + emergency)</summary>
		internal const int BrakeControlsCount = 10;

		/// <summary>The number of power controls (maximum power notches + neutral)</summary>
		internal const int PowerControlsCount = 6;

		/// <summary>The number of reverser controls (forward + backward + neutral)</summary>
		internal const int ReverserControlsCount = 3;

		/// <summary>The number of button controls</summary>
		internal const int ButtonControlsCount = 16;


		public event EventHandler<InputEventArgs> KeyDown;
		public event EventHandler<InputEventArgs> KeyUp;

		internal static HostInterface CurrentHost;

		public InputControl[] Controls
		{
			get; private set;
		}

		internal static FileSystem FileSystem;

		/// <summary>The plugin's configuration.</summary>
		private Config config;

		/// <summary>Whether the input plugin has just started running.</summary>
		private bool loading = true;

		/// <summary>Whether the input plugin is running in-game.</summary>
		internal bool Ingame;

		/// <summary>The specs of the driver's train.</summary>
		internal VehicleSpecs TrainSpecs = new VehicleSpecs(5, BrakeTypes.ElectricCommandBrake, 8, false, 1);

		/// <summary>The list of recognised controllers.</summary>
		private Dictionary<Guid, Controller> controllers;

		/// <summary>The GUID of the active controller.</summary>
		private Guid activeControllerGuid = Guid.Empty;


		/// <summary>A function call when the Config button is pressed.</summary>
		/// <param name="owner">The owner of the window</param>
		public void Config(IWin32Window owner)
		{
			config.ShowDialog(owner);
		}

		/// <summary>A function called when the plugin is loaded.</summary>
		/// <param name="fileSystem">The instance of FileSytem class</param>
		/// <returns>Whether the plugin has been loaded successfully.</returns>
		public bool Load(FileSystem fileSystem)
		{
			FileSystem = fileSystem;
			//HACK: In order to avoid meddling with a shipped interface (or making this field public and increasing the mess), let's grab it via reflection
			CurrentHost = (HostInterface)typeof(FileSystem).GetField("currentHost", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(fileSystem);

			// Create the config form
			config = new Config();

			// Initialize controls
			Controls = new InputControl[BrakeControlsCount + PowerControlsCount + ReverserControlsCount + ButtonControlsCount];

			// Initialize the list of controllers
			controllers = new Dictionary<Guid, Controller>();

			return true;
		}

		/// <summary>A function called when the plugin is unloaded.</summary>
		public void Unload()
		{
			config.Dispose();
		}

		/// <summary>A function called on each frame.</summary>
		public void OnUpdateFrame()
		{
			// Release controls 
			foreach (InputControl control in Controls)
			{
				KeyUp(this, new InputEventArgs(control));
			}

			if (!controllers.ContainsKey(activeControllerGuid) || !controllers[activeControllerGuid].State.IsConnected)
			{
				// Try to find an active controller
				FindActiveController();
			}
			else
			{
				// There is an active controller, update the state
				Controller controller = controllers[activeControllerGuid];
				ControllerProfile profile = config.ControllerProfiles[activeControllerGuid];
				controller.Update();

				// Apply brake handle
				if (controller.State.BrakeNotch != controller.PreviousState.BrakeNotch || loading)
				{
					KeyDown(this, new InputEventArgs(profile.BrakeControls[(int)controller.State.BrakeNotch]));
				}
				// Apply power handle
				if (controller.State.PowerNotch != controller.PreviousState.PowerNotch || loading)
				{
					KeyDown(this, new InputEventArgs(profile.PowerControls[(int)controller.State.PowerNotch]));
				}
				// Apply reverser
				if (controller.State.ReverserPosition != controller.PreviousState.ReverserPosition || loading)
				{
					KeyDown(this, new InputEventArgs(profile.ReverserControls[(int)controller.State.ReverserPosition + 1]));
				}
				// Apply buttons
				//foreach (Controller.ControllerButtons button in Enum.GetValues(typeof(Controller.ControllerButtons)))
				//{
				//	int index = Array.IndexOf(Enum.GetValues(typeof(Controller.ControllerButtons)), button);
				//	Console.WriteLine(index);
				//}
			}

			loading = false;
		}

		/// <summary>A function notifying the plugin about the train's existing status.</summary>
		/// <param name="data">Data</param>
		public void SetElapseData(ElapseData data)
		{
			Translations.CurrentLanguageCode = data.CurrentLanguageCode;

			// HACK: The number of stations cannot be zero in-game
			if (data.Stations.Count > 0)
			{
				Ingame = true;
			}
		}

		public void SetMaxNotch(int powerNotch, int brakeNotch)
		{
		}

		/// <summary>A function notifying the plugin about the train's specifications.</summary>
		/// <param name="specs">The train's specifications.</param>
		public void SetVehicleSpecs(VehicleSpecs specs)
		{
			TrainSpecs = specs;
			FindActiveController();
		}

		/// <summary>Is called when the state of the doors changes.</summary>
		/// <param name="oldState">The old state of the doors.</param>
		/// <param name="newState">The new state of the doors.</param>
		public void DoorChange(DoorStates oldState, DoorStates newState)
		{
		}

		/// <summary>Is called when the aspect in the current or in any of the upcoming sections changes, or when passing section boundaries.</summary>
		/// <remarks>The signal array is guaranteed to have at least one element. When accessing elements other than index 0, you must check the bounds of the array first.</remarks>
		public void SetSignal(SignalData[] signal)
		{
		}

		/// <summary>Is called when the train passes a beacon.</summary>
		/// <param name="data">The beacon data.</param>
		public void SetBeacon(BeaconData data)
		{
		}

		/// <summary>Passes controls to the main program.</summary>
		/// <param name="guid">The controller Guid.</param>
		internal void PassControls(Guid guid)
		{
			ControllerProfile profile = config.ControllerProfiles[guid];
			profile.BrakeControls.CopyTo(Controls, 0);
			profile.PowerControls.CopyTo(Controls, BrakeControlsCount);
			profile.ReverserControls.CopyTo(Controls, BrakeControlsCount + PowerControlsCount);
			profile.ButtonControls.CopyTo(Controls, BrakeControlsCount + PowerControlsCount + ReverserControlsCount);
		}

		/// <summary>Looks for the first connected controller that is supported by the plugin.</summary>
		internal void FindActiveController()
		{
			// When loading the plugin, get all the supported controllers
			// The plugin will not try to find new controllers after this
			if (loading)
			{
				MasconController.GetControllers(controllers);
			}

			// Set the first connected controller as the active controller
			foreach (KeyValuePair<Guid, Controller> controller in controllers)
			{
				controller.Value.Update();
				if (controller.Value.State.IsConnected)
				{
					activeControllerGuid = controller.Key;
					config.ConfigureMappings(TrainSpecs, controller.Value);
					PassControls(activeControllerGuid);
					return;
				}
			}
		}
	}
}
