/*
Copyright (C) <year>  <name of author>

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

namespace FlipYourRide
{
	public class FlipYourRide : Mod
	{
		public override string ID => "Unflip";
		public override string Name => "Unflip";
		public override string Author => "Wampa842";
		public override string Version => "1.0.0";
		public override bool UseAssetsFolder => false;

		// GameObject names of the vehicles
		private static readonly string[] vehicles = { "SATSUMA(557kg, 248)", "GIFU(750/450psi)", "HAYOSIKO(1500kg, 250)", "RCO_RUSCKO12(270)", "FERNDALE(1630kg)", "KEKMET(350-400psi)", "Trailer" };

		// Can't believe I can actually put to use something I've picked up in university.
		public class UnflipCommand : ConsoleCommand
		{
			public override string Name => "unflip";

			public override string Help => "Restore a flipped vehicle - see 'unflip ?' for details.";

			private const string _helpString = "Usage: unflip <0-6>|?\nExecuting this command will cause the selected vehicle's angles to reset.\nNumbers:\n 0: Satsuma\n 1: Gifu (truck)\n 2: Hayosiko (van)\n 3: Ruscko (ventti reward)\n 4: Ferndale (muscle car)\n 5: Kekmet (tractor)\n 6: Trailer\nWARNING: using this mod might damage your car and scatter the cargo.";
			public override void Run(string[] args)
			{
				// Check if the command can run in the first place
				if (Application.loadedLevelName != "GAME")
				{
					ModConsole.Print("Cannot execute command outside of game");
					return;
				}

				// If the argument is missing, not a number, or out of range, display the help text.
				int num;
				if (args.Length < 1 || !int.TryParse(args[0], out num) || num < 0 || num >= vehicles.Length)
				{
					ModConsole.Print(_helpString);
					return;
				}

				// Otherwise, try to find the vehicle
				GameObject selected = GameObject.Find(vehicles[num]);
				if (selected == null)
				{
					ModConsole.Print($"Could not find {vehicles[num]}");
					return;
				}

				// If it is found, flip it and raise it so it doesn't ALWAYS fucking clip into the ground
				selected.transform.position = selected.transform.position + new Vector3(0, 1, 0);
				selected.transform.eulerAngles = new Vector3(0, 0, 0);
			}
		}

		public override void OnLoad()
		{
			ConsoleCommand.Add(new UnflipCommand());
			base.OnLoad();
		}
	}
}
