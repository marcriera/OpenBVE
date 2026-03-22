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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using OpenBveApi.Hosts;
using OpenBveApi.Interface;
using OpenBveApi.Runtime;
using Timer = System.Timers.Timer;

namespace ZuikiInput
{
	public partial class Config : Form
	{
		/// <summary>The path to the file containing the configuration.</summary>
		private const string configFilename = "ZuikiInput.json";

		/// <summary>The list of recognised controllers.</summary>
		private Dictionary<Guid, Controller> controllers;

		/// <summary>The GUID of the selected controller.</summary>
		private Guid selectedControllerGuid = Guid.Empty;

		/// <summary>The list of controller profiles.</summary>
		public Dictionary<Guid, ControllerProfile> ControllerProfiles;

		/// <summary>Timer used to show controller input on the config form.</summary>
		private readonly Timer inputTimer;

		public Config()
		{
			InitializeComponent();

			// Load language files
			Translations.LoadLanguageFiles(OpenBveApi.Path.CombineDirectory(ZuikiInput.FileSystem.DataFolder, "Languages"));

			// Initialize the list of controllers
			controllers = new Dictionary<Guid, Controller>();

			// Initialize the list of controller profiles
			ControllerProfiles = new Dictionary<Guid, ControllerProfile>();
			LoadConfig();

			// Initialize the timer
			inputTimer = new Timer { Interval = 100 };
			inputTimer.Elapsed += timer1_Tick;
		}

		/// <summary>Loads the plugin settings from the config file.</summary>
		internal void LoadConfig()
		{
			string configFolder = OpenBveApi.Path.CombineDirectory(ZuikiInput.FileSystem.SettingsFolder, "1.5.0");
			string configFile = OpenBveApi.Path.CombineFile(configFolder, configFilename);
			if (File.Exists(configFile))
			{
				try
				{
					string json = File.ReadAllText(configFile);
					{
						JsonSerializer serializer = new JsonSerializer();
						serializer.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
						using (StreamReader sr = new StreamReader(configFile))
						using (JsonReader reader = new JsonTextReader(sr))
						{
							ControllerProfiles = serializer.Deserialize<Dictionary<Guid, ControllerProfile>>(reader);
						}
					}
				}
				catch
				{
					MessageBox.Show("An error occured whilst loading the options for ZUIKI Input Plugin from disk." + Environment.NewLine +
									"The configuration file may be corrupt.");
				}
			}
		}

		/// <summary>Saves the plugin settings to the config file.</summary>
		internal void SaveConfig()
		{
			string configFolder = OpenBveApi.Path.CombineDirectory(ZuikiInput.FileSystem.SettingsFolder, "1.5.0");
			if (!Directory.Exists(configFolder))
			{
				Directory.CreateDirectory(configFolder);
			}
			string configFile = OpenBveApi.Path.CombineFile(configFolder, configFilename);
			try
			{
				JsonSerializer serializer = new JsonSerializer();
				serializer.NullValueHandling = NullValueHandling.Ignore;
				serializer.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
				using (StreamWriter sw = new StreamWriter(configFile))
				using (JsonWriter writer = new JsonTextWriter(sw))
				{
					writer.Formatting = Formatting.Indented;
					serializer.Serialize(writer, ControllerProfiles);
				}
			}
			catch
			{
				MessageBox.Show("An error occured whilst saving the options for ZUIKI Input Plugin to disk." + Environment.NewLine +
								"Please check you have write permission.");
			}
		}

		/// <summary>Configures the correct mappings for a controller according to the controller profile.</summary>
		internal void ConfigureMappings(VehicleSpecs specs, Controller controller)
		{
			// Get train capabilities
			Controller.ControllerCapabilities capabilities = controller.Capabilities;

			// Get controller profile; if there is no profile, create one
			if (!ControllerProfiles.ContainsKey(controller.Guid))
			{
				ControllerProfiles.Add(controller.Guid, new ControllerProfile());
			}
			ControllerProfile profile = ControllerProfiles[controller.Guid];

			if (!profile.ConvertNotches)
			{
				// The notches are not supposed to be converted
				// Brake notches
				if (profile.MapHoldBrake && specs.HasHoldBrake)
				{
					profile.BrakeControls[0].Command = Translations.Command.BrakeAnyNotch;
					profile.BrakeControls[0].Option = 0;
					profile.BrakeControls[1].Command = Translations.Command.HoldBrake;
					for (int i = 2; i <= capabilities.BrakeNotches + 1; i++)
					{
						profile.BrakeControls[i].Command = Translations.Command.BrakeAnyNotch;
						profile.BrakeControls[i].Option = i - 1;
					}
				}
				else
				{
					for (int i = 0; i <= capabilities.BrakeNotches + 1; i++)
					{
						profile.BrakeControls[i].Command = Translations.Command.BrakeAnyNotch;
						profile.BrakeControls[i].Option = i;
					}
				}
				// Emergency brake, only if the train has the same or less notches than the controller
				if (specs.BrakeNotches <= capabilities.BrakeNotches)
				{
					profile.BrakeControls[(int)ControllerState.BrakeNotches.Emergency].Command = Translations.Command.BrakeEmergency;
				}
				// Power notches
				for (int i = 0; i <= capabilities.PowerNotches; i++)
				{
					profile.PowerControls[i].Command = Translations.Command.PowerAnyNotch;
					profile.PowerControls[i].Option = i;
				}
			}
			else
			{
				// The notches are supposed to be converted
				// Brake notches
				if (profile.MapHoldBrake && specs.HasHoldBrake)
				{
					double brakeStep = (specs.BrakeNotches - 1) / (double)(capabilities.BrakeNotches - 1);
					profile.BrakeControls[0].Command = Translations.Command.BrakeAnyNotch;
					profile.BrakeControls[0].Option = 0;
					profile.BrakeControls[1].Command = Translations.Command.HoldBrake;
					for (int i = 2; i < capabilities.BrakeNotches + 1; i++)
					{
						profile.BrakeControls[i].Command = Translations.Command.BrakeAnyNotch;
						profile.BrakeControls[i].Option = (int)Math.Round(brakeStep * (i - 1), MidpointRounding.AwayFromZero);
						if (i > 0 && profile.BrakeControls[i].Option == 0)
						{
							profile.BrakeControls[i].Option = 1;
						}
						if (profile.KeepMinMax && i == 2)
						{
							profile.BrakeControls[i].Option = 1;
						}
						if (profile.KeepMinMax && i == capabilities.BrakeNotches)
						{
							profile.BrakeControls[i].Option = specs.BrakeNotches - 1;
						}
					}
				}
				else
				{
					double brakeStep = specs.BrakeNotches / (double)capabilities.BrakeNotches;
					for (int i = 0; i < capabilities.BrakeNotches + 1; i++)
					{
						profile.BrakeControls[i].Command = Translations.Command.BrakeAnyNotch;
						profile.BrakeControls[i].Option = (int)Math.Round(brakeStep * i, MidpointRounding.AwayFromZero);
						if (i > 0 && profile.BrakeControls[i].Option == 0)
						{
							profile.BrakeControls[i].Option = 1;
						}
						if (profile.KeepMinMax && i == 1)
						{
							profile.BrakeControls[i].Option = 1;
						}
						if (profile.KeepMinMax && i == capabilities.BrakeNotches)
						{
							profile.BrakeControls[i].Option = specs.BrakeNotches;
						}
					}
				}
				// Emergency brake
				profile.BrakeControls[(int)ControllerState.BrakeNotches.Emergency].Command = Translations.Command.BrakeEmergency;
				// Power notches
				double powerStep = specs.PowerNotches / (double)capabilities.PowerNotches;
				for (int i = 0; i < capabilities.PowerNotches + 1; i++)
				{
					profile.PowerControls[i].Command = Translations.Command.PowerAnyNotch;
					profile.PowerControls[i].Option = (int)Math.Round(powerStep * i, MidpointRounding.AwayFromZero);
					if (i > 0 && profile.PowerControls[i].Option == 0)
					{
						profile.PowerControls[i].Option = 1;
					}
					if (profile.KeepMinMax && i == 1)
					{
						profile.PowerControls[i].Option = 1;
					}
					if (profile.KeepMinMax && i == capabilities.PowerNotches)
					{
						profile.PowerControls[i].Option = specs.PowerNotches;
					}
				}
			}

			if (specs.BrakeType == BrakeTypes.AutomaticAirBrake)
			{
				// Trains with an air brake are mapped differently
				double brakeStep = 3 / (double)(capabilities.BrakeNotches);
				for (int i = 1; i < capabilities.BrakeNotches + 1; i++)
				{
					profile.BrakeControls[i].Command = Translations.Command.BrakeAnyNotch;
					int notch = ((int)Math.Round(brakeStep * i, MidpointRounding.AwayFromZero) - 1);
					profile.BrakeControls[i].Option = notch >= 0 ? notch : 0;
				}
			}

			for (int i = 0; i < profile.ReverserControls.Length; i++)
			{
				profile.ReverserControls[i].Command = Translations.Command.ReverserAnyPosition;
				profile.ReverserControls[i].Option = i - 1;
			}
		}

		/// <summary>Adds the available controllers to the device dropdown list.</summary>
		private void ListControllers()
		{
			// Clear the internal and visible lists
			deviceBox.Items.Clear();
			controllers.Clear();
			MasconController.GetControllers(controllers);

			if (controllers.Count > 0)
			{
				selectedControllerGuid = controllers.Keys.First();
			}

			foreach (KeyValuePair<Guid, Controller> controller in controllers)
			{
				deviceBox.Items.Add(controller.Value.Name);
				if (!ControllerProfiles.ContainsKey(controller.Key))
				{
					// If there is no profile for the active controller, create one
					ControllerProfiles.Add(controller.Key, new ControllerProfile());

				}
			}

			// Adjust the width of the device dropdown to prevent truncation
			deviceBox.DropDownWidth = deviceBox.Width;
			foreach (var item in deviceBox.Items)
			{
				int currentItemWidth = (int)deviceBox.CreateGraphics().MeasureString(item.ToString(), deviceBox.Font).Width;
				if (currentItemWidth > deviceBox.DropDownWidth)
				{
					deviceBox.DropDownWidth = currentItemWidth;
				}
			}
		}

		/// <summary>Updates the interface to reflect the input from the controller.</summary>
		private void UpdateInterface()
		{
			ControllerState.BrakeNotches brakeNotch = controllers[selectedControllerGuid].State.BrakeNotch;
			ControllerState.PowerNotches powerNotch = controllers[selectedControllerGuid].State.PowerNotch;
			ControllerState.ReverserPositions reverserPosition = controllers[selectedControllerGuid].State.ReverserPosition;

			switch (brakeNotch)
			{
				case ControllerState.BrakeNotches.Released:
					label_brake.Text = Translations.GetInterfaceString(HostApplication.OpenBve, new[] { "zuiki", "label_brake" }).Replace("[notch]", Translations.QuickReferences.HandleBrakeNull);
					break;
				case ControllerState.BrakeNotches.Emergency:
					label_brake.Text = Translations.GetInterfaceString(HostApplication.OpenBve, new[] { "zuiki", "label_brake" }).Replace("[notch]", Translations.QuickReferences.HandleEmergency);
					break;
				default:
					label_brake.Text = Translations.GetInterfaceString(HostApplication.OpenBve, new[] { "zuiki", "label_brake" }).Replace("[notch]", Translations.QuickReferences.HandleBrake + (int)brakeNotch);
					break;
			}

			switch (powerNotch)
			{
				case ControllerState.PowerNotches.N:
					label_power.Text = Translations.GetInterfaceString(HostApplication.OpenBve, new[] { "zuiki", "label_power" }).Replace("[notch]", Translations.QuickReferences.HandlePowerNull);
					break;
				default:
					label_power.Text = Translations.GetInterfaceString(HostApplication.OpenBve, new[] { "zuiki", "label_power" }).Replace("[notch]", Translations.QuickReferences.HandlePower + (int)powerNotch);
					break;
			}

			switch (reverserPosition)
			{
				case ControllerState.ReverserPositions.Forward:
					label_reverser.Text = Translations.GetInterfaceString(HostApplication.OpenBve, new[] { "zuiki", "label_reverser" }).Replace("[notch]", Translations.QuickReferences.HandleForward);
					break;
				case ControllerState.ReverserPositions.Backward:
					label_reverser.Text = Translations.GetInterfaceString(HostApplication.OpenBve, new[] { "zuiki", "label_reverser" }).Replace("[notch]", Translations.QuickReferences.HandleBackward);
					break;
				default:
					label_reverser.Text = Translations.GetInterfaceString(HostApplication.OpenBve, new[] { "zuiki", "label_reverser" }).Replace("[notch]", Translations.QuickReferences.HandleNeutral);
					break;
			}
		}

		/// <summary>Retranslates the configuration interface.</summary>
		private void UpdateTranslation()
		{
			Text = Translations.GetInterfaceString(HostApplication.OpenBve, new[] {"zuiki","config_title"});
			deviceInputBox.Text = Translations.GetInterfaceString(HostApplication.OpenBve, new[] {"zuiki","input_section"});
			label_device.Text = Translations.GetInterfaceString(HostApplication.OpenBve, new[] {"zuiki","device"});
			handleMappingBox.Text = Translations.GetInterfaceString(HostApplication.OpenBve, new[] {"zuiki","handle_section"});
			convertnotchesCheck.Text = Translations.GetInterfaceString(HostApplication.OpenBve, new[] {"zuiki","option_convert"});
			minmaxCheck.Text = Translations.GetInterfaceString(HostApplication.OpenBve, new[] {"zuiki","option_keep_minmax"});
			holdbrakeCheck.Text = Translations.GetInterfaceString(HostApplication.OpenBve, new[] {"zuiki","option_holdbrake"});
			buttonSave.Text = Translations.GetInterfaceString(HostApplication.OpenBve, new[] {"zuiki","save_button"});
			buttonCancel.Text = Translations.GetInterfaceString(HostApplication.OpenBve, new[] {"zuiki","cancel_button"});
		}

		private void Config_Shown(object sender, EventArgs e)
		{
			// Add connected devices to device list
			ListControllers();

			// Try to select the selected device
			if (selectedControllerGuid != Guid.Empty)
			{
				deviceBox.SelectedIndex = controllers.Keys.ToList().IndexOf(selectedControllerGuid);
			}

			// Start timer
			inputTimer.Enabled = true;

			// Translate the interface to the current language
			UpdateTranslation();
		}

		private void Config_FormClosed(Object sender, FormClosedEventArgs e)
		{
			// Reload the previous config and close the config dialog
			LoadConfig();
			inputTimer.Enabled = false;
		}

		private void deviceBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			selectedControllerGuid = controllers.Keys.ToList()[deviceBox.SelectedIndex];
			controllers[selectedControllerGuid].Update();

			// Enable boxes
			deviceInputBox.Enabled = true;
			handleMappingBox.Enabled = true;

			// Set checkboxes
			convertnotchesCheck.Checked = ControllerProfiles[selectedControllerGuid].ConvertNotches;
			minmaxCheck.Checked = ControllerProfiles[selectedControllerGuid].KeepMinMax;
			holdbrakeCheck.Checked = ControllerProfiles[selectedControllerGuid].MapHoldBrake;
			minmaxCheck.Enabled = ControllerProfiles[selectedControllerGuid].ConvertNotches;
		}

		private void convertnotchesCheck_CheckedChanged(object sender, EventArgs e)
		{
			ControllerProfiles[selectedControllerGuid].ConvertNotches = convertnotchesCheck.Checked;
			minmaxCheck.Enabled = ControllerProfiles[selectedControllerGuid].ConvertNotches;
		}

		private void minmaxCheck_CheckedChanged(object sender, EventArgs e)
		{
			ControllerProfiles[selectedControllerGuid].KeepMinMax = minmaxCheck.Checked;
		}

		private void holdbrakeCheck_CheckedChanged(object sender, EventArgs e)
		{
			ControllerProfiles[selectedControllerGuid].MapHoldBrake = holdbrakeCheck.Checked;
		}

		private void buttonSave_Click(object sender, EventArgs e)
		{
			// Save the config and close the config dialog
			SaveConfig();
			Close();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			// Reload the previous config and close the config dialog
			LoadConfig();
			Close();
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			controllers[selectedControllerGuid].Update();

			//grab a random control, as we need something from the UI thread to check for invoke
			//WinForms is a pain
			if (buttonCancel.InvokeRequired)
			{
				buttonCancel.Invoke((MethodInvoker) UpdateInterface);
			}
			else
			{
				UpdateInterface();	
			}
			
		}

	}
}
