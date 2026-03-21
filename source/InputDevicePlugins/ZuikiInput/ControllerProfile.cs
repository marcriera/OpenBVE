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
using OpenBveApi.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ZuikiInput
{
	/// <summary>Class representing a configuration profile for a controller.</summary>
	public class ControllerProfile
	{
		/// <summary>The Guid of the controller to apply the profile to.</summary>
		//public Guid Guid;

		//internal class ButtonMappings
		//{
		//	[JsonConverter(typeof(StringEnumConverter))]
		//	internal Translations.Command ButtonA;
		//	[JsonConverter(typeof(StringEnumConverter))]
		//	internal Translations.Command ButtonB;
		//	[JsonConverter(typeof(StringEnumConverter))]
		//	internal Translations.Command ButtonX;
		//	[JsonConverter(typeof(StringEnumConverter))]
		//	internal Translations.Command ButtonY;
		//	[JsonConverter(typeof(StringEnumConverter))]
		//	internal Translations.Command ButtonUp;
		//	[JsonConverter(typeof(StringEnumConverter))]
		//	internal Translations.Command ButtonDown;
		//	[JsonConverter(typeof(StringEnumConverter))]
		//	internal Translations.Command ButtonLeft;
		//	[JsonConverter(typeof(StringEnumConverter))]
		//	internal Translations.Command ButtonRight;
		//	[JsonConverter(typeof(StringEnumConverter))]
		//	internal Translations.Command ButtonMinus;
		//	[JsonConverter(typeof(StringEnumConverter))]
		//	internal Translations.Command ButtonPlus;
		//	[JsonConverter(typeof(StringEnumConverter))]
		//	internal Translations.Command ButtonL;
		//	[JsonConverter(typeof(StringEnumConverter))]
		//	internal Translations.Command ButtonR;
		//	[JsonConverter(typeof(StringEnumConverter))]
		//	internal Translations.Command ButtonZL;
		//	[JsonConverter(typeof(StringEnumConverter))]
		//	internal Translations.Command ButtonZR;
		//	[JsonConverter(typeof(StringEnumConverter))]
		//	internal Translations.Command ButtonHome;
		//	[JsonConverter(typeof(StringEnumConverter))]
		//	internal Translations.Command ButtonScreenshot;
		//}

		///// <summary>The mapping for each controller button.</summary>
		//internal ButtonMappings ButtonMapping;

		//public ControllerProfile(Guid guid)
		//{
		//	Guid = guid;
		//	ButtonMapping = new ButtonMappings();
		//}
	}
}
