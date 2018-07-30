/*
Copyright (C) 2018 Wampa842

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using MSCLoader;
using UnityEngine;
using System;
using System.IO;
using HutongGames.PlayMaker;

namespace ListGameObjects
{
	public class ListGameObjects : Mod
	{
		public override string ID => "ListGameObjects";
		public override string Name => "ListGameObjects";
		public override string Author => "Wampa842";
		public override string Version => "1.0.0";
		public override bool UseAssetsFolder => false;

		public class ListStuffCommand : ConsoleCommand
		{
			public override string Name => "writeobjects";
			public override string Help => "write GameObjects and FSM stuff into files";
			private ListGameObjects sender;
			private string objectPath, varPath, eventPath, compPath;


			public ListStuffCommand(ListGameObjects sender)
			{
				this.sender = sender;
				this.objectPath = Path.Combine(ModLoader.GetModConfigFolder(this.sender), "GameObjects.csv");
				this.varPath = Path.Combine(ModLoader.GetModConfigFolder(this.sender), "FsmVars.csv");
				this.eventPath = Path.Combine(ModLoader.GetModConfigFolder(this.sender), "FsmEvents.csv");
				this.compPath = Path.Combine(ModLoader.GetModConfigFolder(this.sender), "FsmComponents.csv");

			}

			public override void Run(string[] args)
			{
				StreamWriter writer = null;

				// List GameObjects
				try
				{
					writer = new StreamWriter(objectPath);

					foreach (GameObject o in GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[])
					{
						writer.WriteLine($"\"{o.name}\",\"{o.tag}\",{o.layer}");
					}
				}
				catch (IOException ex)
				{
					ModConsole.Print("Couldn't write GameObjects:\n" + ex.ToString());
				}
				finally
				{
					if (writer != null)
						writer.Dispose();
				}

				// List FSM variables
				try
				{
					writer = new StreamWriter(varPath);

					foreach (var v in FsmVariables.GlobalVariables.FloatVariables)
					{
						writer.WriteLine($"\"{v.Name}\",\"{v.GetType().ToString()}\",\"{v.Value}\"");
					}
					foreach (var v in FsmVariables.GlobalVariables.IntVariables)
					{
						writer.WriteLine($"\"{v.Name}\",\"{v.GetType().ToString()}\",\"{v.Value}\"");
					}
					foreach (var v in FsmVariables.GlobalVariables.BoolVariables)
					{
						writer.WriteLine($"\"{v.Name}\",\"{v.GetType().ToString()}\",\"{v.Value}\"");
					}
					foreach (var v in FsmVariables.GlobalVariables.StringVariables)
					{
						writer.WriteLine($"\"{v.Name}\",\"{v.GetType().ToString()}\",\"{v.Value}\"");
					}
				}
				catch(IOException ex)
				{
					ModConsole.Print("Couldn't write FSM variables:\n" + ex.ToString());
				}
				finally
				{
					if (writer != null)
						writer.Dispose();
				}

				// List FSM events
				try
				{
					writer = new StreamWriter(eventPath);

				}
				catch(IOException ex)
				{
					ModConsole.Print("Couldn't write FSM components:\n" + ex.ToString());
				}
				finally
				{
					if (writer != null)
						writer.Dispose();
				}

				//List FSM components per GameObject
				try
				{
					writer = new StreamWriter(compPath);
					writer.WriteLine("\"GameObject\",\"Layer\",\"Component\",\"Variable\",\"Type\",\"Value\"");

					foreach (GameObject o in GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[])
					{
						foreach(PlayMakerFSM fsm in o.GetComponents<PlayMakerFSM>())
						{
							foreach (var v in fsm.FsmVariables.IntVariables)
							{
								writer.WriteLine($"\"{o.name}\",{o.layer},\"{fsm.FsmName}\",\"{v.Name}\",\"{v.GetType().ToString()}\",\"{v.Value}\"");
							}
							foreach (var v in fsm.FsmVariables.FloatVariables)
							{
								writer.WriteLine($"\"{o.name}\",{o.layer},\"{fsm.FsmName}\",\"{v.Name}\",\"{v.GetType().ToString()}\",\"{v.Value}\"");
							}
							foreach (var v in fsm.FsmVariables.BoolVariables)
							{
								writer.WriteLine($"\"{o.name}\",{o.layer},\"{fsm.FsmName}\",\"{v.Name}\",\"{v.GetType().ToString()}\",\"{v.Value}\"");
							}
							foreach (var v in fsm.FsmVariables.StringVariables)
							{
								writer.WriteLine($"\"{o.name}\",{o.layer},\"{fsm.FsmName}\",\"{v.Name}\",\"{v.GetType().ToString()}\",\"{v.Value}\"");
							}
						}
					}
				}
				catch (IOException ex)
				{
					ModConsole.Print("Couldn't write FSM events:\n" + ex.ToString());
				}
				finally
				{
					if (writer != null)
						writer.Dispose();
				}

				ModConsole.Print("done writing stuff - look in the config folder");
			}
		}

		public override void OnLoad()
		{
			ConsoleCommand.Add(new ListStuffCommand(this));
		}
	}
}
