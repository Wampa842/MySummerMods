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

			string objectPath, varPath, eventPath;


			public ListStuffCommand(ListGameObjects sender)
			{
				this.sender = sender;
				this.objectPath = Path.Combine(ModLoader.GetModConfigFolder(this.sender), "GameObjects.csv");
				this.varPath = Path.Combine(ModLoader.GetModConfigFolder(this.sender), "FsmVars.csv");
				this.eventPath = Path.Combine(ModLoader.GetModConfigFolder(this.sender), "FsmEvents.csv");

			}

			public override void Run(string[] args)
			{
				StreamWriter writer = null;
				try
				{
					writer = new StreamWriter(objectPath);

					foreach (GameObject o in GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[])
					{
						writer.WriteLine($"\"{o.name}\",\"{o.tag}\"");
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
				try
				{
					writer = new StreamWriter(eventPath);

					foreach (var v in FsmEvent.EventList)
					{
						writer.WriteLine($"\"{v.Name}\",\"{v.Path}\"");
					}
				}
				catch(IOException ex)
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
