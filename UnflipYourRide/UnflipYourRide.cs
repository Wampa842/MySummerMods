using MSCLoader;
using UnityEngine;

namespace FlipYourRide
{
	public class FlipYourRide : Mod
	{
		public override string ID => "Unflip";
		public override string Name => "Unflip";
		public override string Author => "Wampa842";
		public override string Version => "0.1.0";
		public override bool UseAssetsFolder => false;

		// GameObject names of the vehicles
		private static string[] vehicles = { "SATSUMA(557kg, 248)", "GIFU(750/450psi)", "HAYOSIKO(1500kg, 250)", "RCO_RUSCKO12(270)", "FERNDALE(1630kg)", "KEKMET(350-400psi)" };

		private bool flipIt(int number)
		{
			GameObject car = GameObject.Find(vehicles[number]);
			if (car == null)
				return false;
			car.transform.eulerAngles = new Vector3(0, 0, 0);
			return true;
		}

		public class UnflipCommand : ConsoleCommand
		{
			public override string Name => "unflip";

			public override string Help => "Usage: 'unflip <number>', or 'unflip ?' for help";

			private const string _helpString = "Usage: unflip <number>|?|help\nNumbers:\n 0: Satsuma\n 1: Gifu (truck)\n 2: Hayosiko (van)\n 3: Ruscko (ventti reward)\n 4: Ferndale (muscle car)\n 5: Kekmet (tractor)\n 6: Trailer\nWARNING: using this mod might damage your car and scatter the cargo.";
			public override void Run(string[] args)
			{
				//Check if the command can run in the first place
				if (Application.loadedLevelName != "GAME")
				{
					ModConsole.Print("Cannot execute command outside of game");
					return;
				}
				int num;
				if (args.Length != 1 || !int.TryParse(args[0], out num) || num < 0 || num >= vehicles.Length)
				{
					ModConsole.Print(_helpString);
					return;
				}

				GameObject selected = GameObject.Find(vehicles[num]);
				if (selected == null)
				{
					ModConsole.Print($"Could not find {vehicles[num]}");
					return;
				}
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
