﻿using System;
using System.Text;
using OpenBveApi;
using OpenBveApi.Interface;
using OpenBveApi.Math;
using OpenBveApi.Objects;
using OpenBveApi.Routes;
using OpenBveApi.Textures;

namespace CsvRwRouteParser
{
	internal partial class Parser
	{
		private static void ParseStructureCommand(string Command, string[] Arguments, int[] commandIndices, Encoding Encoding, Expression Expression, ref RouteData Data, bool PreviewOnly)
		{
			switch (Command)
			{
				case "rail":
				{
					if (!PreviewOnly)
					{
						if (commandIndices[0] < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "RailStructureIndex is expected to be non-negative in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (Arguments.Length < 1)
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else if (Path.ContainsInvalidChars(Arguments[0]))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FileName " + Arguments[0] + " contains illegal characters in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{
								string f = Arguments[0];
								if (!LocateObject(ref f, ObjectPath))
								{
									Plugin.CurrentHost.AddMessage(MessageType.Error, true, "FileName " + f + " not found in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								}
								else
								{
									UnifiedObject obj;
									Plugin.CurrentHost.LoadObject(f, Encoding, out obj);
									if (obj != null)
									{
										Data.Structure.RailObjects.Add(commandIndices[0], obj, "RailStructure");
									}
								}
							}
						}
					}
				}
					break;
				case "beacon":
				{
					if (!PreviewOnly)
					{
						if (commandIndices[0] < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "BeaconStructureIndex is expected to be non-negative in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (Arguments.Length < 1)
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else if (Path.ContainsInvalidChars(Arguments[0]))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FileName " + Arguments[0] + " contains illegal characters in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{
								string f = Path.CombineFile(ObjectPath, Arguments[0]);
								if (!System.IO.File.Exists(f))
								{
									Plugin.CurrentHost.AddMessage(MessageType.Error, true, "FileName " + f + " not found in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								}
								else
								{
									UnifiedObject obj;
									Plugin.CurrentHost.LoadObject(f, Encoding, out obj);
									if (obj != null)
									{
										Data.Structure.Beacon.Add(commandIndices[0], obj, "BeaconStructure");
									}
								}
							}
						}
					}
				}
					break;
				case "pole":
				{
					if (!PreviewOnly)
					{
						if (commandIndices[0] < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "AdditionalRailsCovered is expected to be non-negative in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else if (commandIndices[1] < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "PoleStructureIndex is expected to be non-negative in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (Arguments.Length < 1)
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else if (Path.ContainsInvalidChars(Arguments[0]))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FileName " + Arguments[0] + " contains illegal characters in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{

								if (!Data.Structure.Poles.ContainsKey(commandIndices[0]))
								{
									Data.Structure.Poles.Add(commandIndices[0], new ObjectDictionary());
								}

								string f = Arguments[0];
								if (!LocateObject(ref f, ObjectPath))
								{
									Plugin.CurrentHost.AddMessage(MessageType.Error, true, "FileName " + f + " not found in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								}
								else
								{
									UnifiedObject obj;
									Plugin.CurrentHost.LoadObject(f, Encoding, out obj);
									Data.Structure.Poles[commandIndices[0]].Add(commandIndices[1], obj);
								}
							}
						}
					}
				}
					break;
				case "ground":
				{
					if (!PreviewOnly)
					{
						if (commandIndices[0] < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "GroundStructureIndex is expected to be non-negative in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (Arguments.Length < 1)
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else if (Path.ContainsInvalidChars(Arguments[0]))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FileName " + Arguments[0] + " contains illegal characters in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{
								string f = Arguments[0];
								if (!LocateObject(ref f, ObjectPath))
								{
									Plugin.CurrentHost.AddMessage(MessageType.Error, true, "FileName " + f + " not found in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								}
								else
								{
									UnifiedObject obj;
									Plugin.CurrentHost.LoadObject(f, Encoding, out obj);
									if (obj != null)
									{
										Data.Structure.Ground.Add(commandIndices[0], obj, "GroundStructure");
									}
								}
							}
						}
					}
				}
					break;
				case "walll":
				{
					if (!PreviewOnly)
					{
						if (commandIndices[0] < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "Left WallStructureIndex is expected to be non-negative in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (Arguments.Length < 1)
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else if (Path.ContainsInvalidChars(Arguments[0]))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FileName " + Arguments[0] + " contains illegal characters in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{
								string f = Arguments[0];
								if (!LocateObject(ref f, ObjectPath))
								{
									Plugin.CurrentHost.AddMessage(MessageType.Error, true, "FileName " + f + " not found in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								}
								else
								{
									UnifiedObject obj;
									Plugin.CurrentHost.LoadObject(f, Encoding, out obj);
									if (obj != null)
									{
										Data.Structure.WallL.Add(commandIndices[0], obj, "Left WallStructure");
									}
								}
							}
						}
					}
				}
					break;
				case "wallr":
				{
					if (!PreviewOnly)
					{
						if (commandIndices[0] < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "Right WallStructureIndex is expected to be non-negative in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (Arguments.Length < 1)
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else if (Path.ContainsInvalidChars(Arguments[0]))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FileName " + Arguments[0] + " contains illegal characters in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{
								string f = Arguments[0];
								if (!LocateObject(ref f, ObjectPath))
								{
									Plugin.CurrentHost.AddMessage(MessageType.Error, true, "FileName " + f + " not found in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								}
								else
								{
									UnifiedObject obj;
									Plugin.CurrentHost.LoadObject(f, Encoding, out obj);
									if (obj != null)
									{
										Data.Structure.WallR.Add(commandIndices[0], obj, "Right WallStructure");
									}
								}
							}
						}
					}
				}
					break;
				case "dikel":
				{
					if (!PreviewOnly)
					{
						if (commandIndices[0] < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "Left DikeStructureIndex is expected to be non-negative in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (Arguments.Length < 1)
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else if (Path.ContainsInvalidChars(Arguments[0]))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FileName " + Arguments[0] + " contains illegal characters in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{
								string f = Arguments[0];
								if (!LocateObject(ref f, ObjectPath))
								{
									Plugin.CurrentHost.AddMessage(MessageType.Error, true, "FileName " + f + " not found in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								}
								else
								{
									UnifiedObject obj;
									Plugin.CurrentHost.LoadObject(f, Encoding, out obj);
									if (obj != null)
									{
										Data.Structure.DikeL.Add(commandIndices[0], obj, "Left DikeStructure");
									}
								}
							}
						}
					}
				}
					break;
				case "diker":
				{
					if (!PreviewOnly)
					{
						if (commandIndices[0] < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "Right DikeStructureIndex is expected to be non-negative in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (Arguments.Length < 1)
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else if (Path.ContainsInvalidChars(Arguments[0]))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FileName " + Arguments[0] + " contains illegal characters in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{
								string f = Arguments[0];
								if (!LocateObject(ref f, ObjectPath))
								{
									Plugin.CurrentHost.AddMessage(MessageType.Error, true, "FileName " + f + " not found in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								}
								else
								{
									UnifiedObject obj;
									Plugin.CurrentHost.LoadObject(f, Encoding, out obj);
									if (obj != null)
									{
										Data.Structure.DikeR.Add(commandIndices[0], obj, "Right DikeStructure");
									}
								}
							}
						}
					}
				}
					break;
				case "forml":
				{
					if (!PreviewOnly)
					{
						if (commandIndices[0] < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "Left FormStructureIndex is expected to be non-negative in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (Arguments.Length < 1)
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else if (Path.ContainsInvalidChars(Arguments[0]))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FileName " + Arguments[0] + " contains illegal characters in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{
								string f = Arguments[0];
								if (!LocateObject(ref f, ObjectPath))
								{
									Plugin.CurrentHost.AddMessage(MessageType.Error, true, "FileName " + f + " not found in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								}
								else
								{
									UnifiedObject obj;
									Plugin.CurrentHost.LoadObject(f, Encoding, out obj);
									if (obj != null)
									{
										Data.Structure.FormL.Add(commandIndices[0], obj, "Left FormStructure");
									}
								}
							}
						}
					}
				}
					break;
				case "formr":
				{
					if (!PreviewOnly)
					{
						if (commandIndices[0] < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "Right FormStructureIndex is expected to be non-negative in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (Arguments.Length < 1)
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else if (Path.ContainsInvalidChars(Arguments[0]))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FileName " + Arguments[0] + " contains illegal characters in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{
								string f = Arguments[0];
								if (!LocateObject(ref f, ObjectPath))
								{
									Plugin.CurrentHost.AddMessage(MessageType.Error, true, "FileName " + f + " not found in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								}
								else
								{
									UnifiedObject obj;
									Plugin.CurrentHost.LoadObject(f, Encoding, out obj);
									if (obj != null)
									{
										Data.Structure.FormR.Add(commandIndices[0], obj, "Right FormStructure");
									}
								}
							}
						}
					}
				}
					break;
				case "formcl":
				{
					if (!PreviewOnly)
					{
						if (commandIndices[0] < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "Left FormCStructureIndex is expected to be non-negative in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (Arguments.Length < 1)
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else if (Path.ContainsInvalidChars(Arguments[0]))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FileName " + Arguments[0] + " contains illegal characters in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{
								string f = Arguments[0];
								if (!LocateObject(ref f, ObjectPath))
								{
									Plugin.CurrentHost.AddMessage(MessageType.Error, true, "FileName " + f + " not found in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								}
								else
								{
									StaticObject obj;
									Plugin.CurrentHost.LoadStaticObject(f, Encoding, false, out obj);
									if (obj != null)
									{
										Data.Structure.FormCL.Add(commandIndices[0], obj, "Left FormCStructure");
									}
								}
							}
						}
					}
				}
					break;
				case "formcr":
				{
					if (!PreviewOnly)
					{
						if (commandIndices[0] < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "Right FormCStructureIndex is expected to be non-negative in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (Arguments.Length < 1)
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else if (Path.ContainsInvalidChars(Arguments[0]))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FileName " + Arguments[0] + " contains illegal characters in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{
								string f = Arguments[0];
								if (!LocateObject(ref f, ObjectPath))
								{
									Plugin.CurrentHost.AddMessage(MessageType.Error, true, "FileName " + f + " not found in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								}
								else
								{
									StaticObject obj;
									Plugin.CurrentHost.LoadStaticObject(f, Encoding, false, out obj);
									if (obj != null)
									{
										Data.Structure.FormCR.Add(commandIndices[0], obj, "Right FormCStructure");
									}
								}
							}
						}
					}
				}
					break;
				case "roofl":
				{
					if (!PreviewOnly)
					{
						if (commandIndices[0] < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "Left RoofStructureIndex is expected to be non-negative in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (Arguments.Length < 1)
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else if (Path.ContainsInvalidChars(Arguments[0]))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FileName " + Arguments[0] + " contains illegal characters in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{
								if (commandIndices[0] == 0)
								{
									if (!IsRW)
									{
										Plugin.CurrentHost.AddMessage(MessageType.Error, false, "RoofStructureIndex was omitted or is 0 in " + Command + " argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
									}

									commandIndices[0] = 1;
								}

								if (commandIndices[0] < 0)
								{
									Plugin.CurrentHost.AddMessage(MessageType.Error, false, "RoofStructureIndex is expected to be non-negative in " + Command + " argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								}
								else
								{
									string f = Arguments[0];
									if (!LocateObject(ref f, ObjectPath))
									{
										Plugin.CurrentHost.AddMessage(MessageType.Error, true, "FileName " + f + " not found in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
									}
									else
									{
										UnifiedObject obj;
										Plugin.CurrentHost.LoadObject(f, Encoding, out obj);
										if (obj != null)
										{
											Data.Structure.RoofL.Add(commandIndices[0], obj, "Left RoofStructure");
										}
									}
								}
							}
						}
					}
				}
					break;
				case "roofr":
				{
					if (!PreviewOnly)
					{
						if (commandIndices[0] < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "Right RoofStructureIndex is expected to be non-negative in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (Arguments.Length < 1)
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else if (Path.ContainsInvalidChars(Arguments[0]))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FileName " + Arguments[0] + " contains illegal characters in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{
								if (commandIndices[0] == 0)
								{
									if (!IsRW)
									{
										Plugin.CurrentHost.AddMessage(MessageType.Error, false, "RoofStructureIndex was omitted or is 0 in " + Command + " argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
									}

									commandIndices[0] = 1;
								}

								if (commandIndices[0] < 0)
								{
									Plugin.CurrentHost.AddMessage(MessageType.Error, false, "RoofStructureIndex is expected to be non-negative in " + Command + " argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								}
								else
								{
									string f = Arguments[0];
									if (!LocateObject(ref f, ObjectPath))
									{
										Plugin.CurrentHost.AddMessage(MessageType.Error, true, "FileName " + f + " not found in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
									}
									else
									{
										UnifiedObject obj;
										Plugin.CurrentHost.LoadObject(f, Encoding, out obj);
										if (obj != null)
										{
											Data.Structure.RoofR.Add(commandIndices[0], obj, "Right RoofStructure");
										}
									}
								}
							}
						}
					}
				}
					break;
				case "roofcl":
				{
					if (!PreviewOnly)
					{
						if (commandIndices[0] < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "Left RoofCStructureIndex is expected to be non-negative in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (Arguments.Length < 1)
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else if (Path.ContainsInvalidChars(Arguments[0]))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FileName " + Arguments[0] + " contains illegal characters in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{
								if (commandIndices[0] == 0)
								{
									if (!IsRW)
									{
										Plugin.CurrentHost.AddMessage(MessageType.Error, false, "RoofStructureIndex was omitted or is 0 in " + Command + " argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
									}

									commandIndices[0] = 1;
								}

								if (commandIndices[0] < 0)
								{
									Plugin.CurrentHost.AddMessage(MessageType.Error, false, "RoofStructureIndex is expected to be non-negative in " + Command + " argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								}
								else
								{
									string f = Arguments[0];
									if (!LocateObject(ref f, ObjectPath))
									{
										Plugin.CurrentHost.AddMessage(MessageType.Error, true, "FileName " + f + " not found in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
									}
									else
									{
										StaticObject obj;
										Plugin.CurrentHost.LoadStaticObject(f, Encoding, false, out obj);
										if (obj != null)
										{
											Data.Structure.RoofCL.Add(commandIndices[0], obj, "Left RoofCStructure");
										}
									}
								}
							}
						}
					}
				}
					break;
				case "roofcr":
				{
					if (!PreviewOnly)
					{
						if (commandIndices[0] < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "Right RoofCStructureIndex is expected to be non-negative in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (Arguments.Length < 1)
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else if (Path.ContainsInvalidChars(Arguments[0]))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FileName " + Arguments[0] + " contains illegal characters in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{
								if (commandIndices[0] == 0)
								{
									if (!IsRW)
									{
										Plugin.CurrentHost.AddMessage(MessageType.Error, false, "RoofStructureIndex was omitted or is 0 in " + Command + " argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
									}

									commandIndices[0] = 1;
								}

								if (commandIndices[0] < 0)
								{
									Plugin.CurrentHost.AddMessage(MessageType.Error, false, "RoofStructureIndex is expected to be non-negative in " + Command + " argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								}
								else
								{
									string f = Arguments[0];
									if (!LocateObject(ref f, ObjectPath))
									{
										Plugin.CurrentHost.AddMessage(MessageType.Error, true, "FileName " + f + " not found in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
									}
									else
									{
										StaticObject obj;
										Plugin.CurrentHost.LoadStaticObject(f, Encoding, false, out obj);
										if (obj != null)
										{
											Data.Structure.RoofCR.Add(commandIndices[0], obj, "Right RoofCStructure");
										}
									}
								}
							}
						}
					}
				}
					break;
				case "crackl":
				{
					if (!PreviewOnly)
					{
						if (commandIndices[0] < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "Left CrackStructureIndex is expected to be non-negative in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (Arguments.Length < 1)
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else if (Path.ContainsInvalidChars(Arguments[0]))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FileName " + Arguments[0] + " contains illegal characters in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{
								string f = Arguments[0];
								if (!LocateObject(ref f, ObjectPath))
								{
									Plugin.CurrentHost.AddMessage(MessageType.Error, true, "FileName " + f + " not found in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								}
								else
								{
									StaticObject obj;
									Plugin.CurrentHost.LoadStaticObject(f, Encoding, true, out obj);
									if (obj != null)
									{
										Data.Structure.CrackL.Add(commandIndices[0], obj, "Left CrackStructure");
									}
								}
							}
						}
					}
				}
					break;
				case "crackr":
				{
					if (!PreviewOnly)
					{
						if (commandIndices[0] < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "Right CrackStructureIndex is expected to be non-negative in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (Arguments.Length < 1)
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else if (Path.ContainsInvalidChars(Arguments[0]))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FileName " + Arguments[0] + " contains illegal characters in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{
								string f = Arguments[0];
								if (!LocateObject(ref f, ObjectPath))
								{
									Plugin.CurrentHost.AddMessage(MessageType.Error, true, "FileName " + f + " not found in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								}
								else
								{
									StaticObject obj;
									Plugin.CurrentHost.LoadStaticObject(f, Encoding, true, out obj);
									if (obj != null)
									{
										Data.Structure.CrackR.Add(commandIndices[0], obj, "Right CrackStructure");
									}
								}
							}
						}
					}
				}
					break;
				case "freeobj":
				{
					if (!PreviewOnly)
					{
						if (commandIndices[0] < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FreeObjStructureIndex is expected to be non-negative in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (Arguments.Length < 1)
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else if (Path.ContainsInvalidChars(Arguments[0]))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FileName " + Arguments[0] + " contains illegal characters in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{
								string f = Arguments[0];
								if (!LocateObject(ref f, ObjectPath))
								{
									Plugin.CurrentHost.AddMessage(MessageType.Error, true, "FileName " + f + " could not be found in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								}
								else
								{
									UnifiedObject obj;
									Plugin.CurrentHost.LoadObject(f, Encoding, out obj);
									if (obj != null)
									{
										Data.Structure.FreeObjects.Add(commandIndices[0], obj, "FreeObject");
									}
								}
							}
						}
					}
					else
					{
						freeObjCount++;
					}
				}
					break;
				case "background":
				case "back":
				{
					if (!PreviewOnly)
					{
						if (commandIndices[0] < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "BackgroundTextureIndex is expected to be non-negative at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else if (Arguments.Length < 1)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (Path.ContainsInvalidChars(Arguments[0]))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FileName " + Arguments[0] + " contains illegal characters in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{
								if (!Data.Backgrounds.ContainsKey(commandIndices[0]))
								{
									Data.Backgrounds.Add(commandIndices[0], new StaticBackground(null, 6, false, Plugin.CurrentOptions.ViewingDistance));
								}

								string f = Path.CombineFile(ObjectPath, Arguments[0]);
								if (!System.IO.File.Exists(f) && (Arguments[0].ToLowerInvariant() == "back_mt.bmp" || Arguments[0] == "back_mthigh.bmp"))
								{
									//Default background textures supplied with Uchibo for BVE1 / BVE2, so map to something that's not totally black
									f = Path.CombineFile(Plugin.FileSystem.GetDataFolder("Compatibility"), "Uchibo\\Back_Mt.png");
								}

								if (!System.IO.File.Exists(f) && Plugin.CurrentOptions.EnableBveTsHacks)
								{
									if (Arguments[0].StartsWith("Midland Suburban Line", StringComparison.InvariantCultureIgnoreCase))
									{
										Arguments[0] = "Midland Suburban Line Objects" + Arguments[0].Substring(21);
										f = Path.CombineFile(ObjectPath, Arguments[0]);
									}
								}

								if (!System.IO.File.Exists(f))
								{
									Plugin.CurrentHost.AddMessage(MessageType.Error, true, "FileName " + f + " not found in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								}
								else
								{
									if (f.ToLowerInvariant().EndsWith(".xml"))
									{
										try
										{
											BackgroundHandle h = DynamicBackgroundParser.ReadBackgroundXML(f);
											Data.Backgrounds[commandIndices[0]] = h;
										}
										catch
										{
											Plugin.CurrentHost.AddMessage(MessageType.Error, true, f + " is not a valid background XML in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
										}
									}
									else
									{
										if (Data.Backgrounds[commandIndices[0]] is StaticBackground)
										{
											StaticBackground b = Data.Backgrounds[commandIndices[0]] as StaticBackground;
											if (b != null)
											{
												Plugin.CurrentHost.RegisterTexture(f, new TextureParameters(null, null), out b.Texture);
											}

										}
									}
								}
							}
						}
					}
				}
					break;
				case "background.x":
				case "back.x":
				{
					if (!PreviewOnly)
					{
						if (commandIndices[0] < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "BackgroundTextureIndex " + commandIndices[0] + " is expected to be non-negative at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else if (Arguments.Length < 1)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (!Data.Backgrounds.ContainsKey(commandIndices[0]))
							{
								Data.Backgrounds.Add(commandIndices[0], new StaticBackground(null, 6, false, Plugin.CurrentOptions.ViewingDistance));
							}

							int x;
							if (!NumberFormats.TryParseIntVb6(Arguments[0], out x))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "BackgroundTextureIndex " + Arguments[0] + " is invalid in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else if (x == 0)
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "RepetitionCount is expected to be non-zero in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{
								StaticBackground b = Data.Backgrounds[commandIndices[0]] as StaticBackground;
								if (b != null)
								{
									b.Repetition = x;
								}
							}
						}
					}
				}
					break;
				case "background.aspect":
				case "back.aspect":
				{
					if (!PreviewOnly)
					{
						if (commandIndices[0] < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "BackgroundTextureIndex " + commandIndices[0] + " is expected to be non-negative at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else if (Arguments.Length < 1)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (!Data.Backgrounds.ContainsKey(commandIndices[0]))
							{
								Data.Backgrounds.Add(commandIndices[0], new StaticBackground(null, 6, false, Plugin.CurrentOptions.ViewingDistance));
							}

							int aspect;
							if (!NumberFormats.TryParseIntVb6(Arguments[0], out aspect))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "BackgroundTextureIndex " + Arguments[0] + " is invalid in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else if (aspect != 0 & aspect != 1)
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "Value is expected to be either 0 or 1 in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{
								StaticBackground b = Data.Backgrounds[commandIndices[0]] as StaticBackground;
								if (b != null)
								{
									b.KeepAspectRatio = aspect == 1;
								}

							}
						}
					}
				}
					break;
			}
		}
	}
}
