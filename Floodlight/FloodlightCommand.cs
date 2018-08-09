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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using MSCLoader;
using HutongGames.PlayMaker;

namespace Floodlight
{

	public class FloodlightCommand : ConsoleCommand
	{
		public override string Name => "fl";
		public override string Help => "Floodlight options - use 'fl help' for more info";
		private FloodlightBehaviour _comp;
		private const string _helpString = "Floodlight options:\nfl info: display information about the floodlight\nfl health [number]: display or set the bulb health\nfl flicker [delay [length]]: display or set flicker options\nfl help: display this text";

		public FloodlightCommand(GameObject light)
		{
			_comp = light.GetComponent<FloodlightBehaviour>();
		}

		public override void Run(string[] args)
		{
			if (args.Length < 1)
				return;

			try
			{
				if (args[0] == "flicker")
				{
					if (args.Length == 2)
					{
						_comp.SetFlicker(float.Parse(args[1]), -1.0f);
					}
					else if (args.Length == 3)
					{
						_comp.SetFlicker(float.Parse(args[1]), float.Parse(args[2]));
					}
				}

				if (args[0] == "health")
				{
					_comp.SetHealth(int.Parse(args[1]));
				}

				if (args[0] == "info")
				{
					_comp.PrintInfo();
				}

				if (args[0] == "help")
				{
					ModConsole.Print(_helpString);
				}
			}
			catch(Exception ex)
			{
				ModConsole.Error(ex.Message);
			}
		}
	}
}
