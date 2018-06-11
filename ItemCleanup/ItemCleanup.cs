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
using System.Collections.Generic;

namespace ItemCleanup
{
	public class ItemCleanup : Mod
	{
		public override string ID => "ItemCleanup";
		public override string Name => "Item cleanup";
		public override string Author => "Wampa842";
		public override string Version => "0.1.0";
		public override bool UseAssetsFolder => false;

		private class ItemCleanupCommand : ConsoleCommand
		{
			public override string Name => "cleanup";

			public override string Help => "Removes items from the game, enter 'cleanup ?' for details";

			private const string _helpString = "Usage: cleanup <item1 [item2...]>\nRemoves the items matching the arguments from the game.\nKeywords:\n 'empty': empty containers (except juice bottles)\n 'allempty': empty containers including juice\n 'food': all food items and cigarettes\n 'caritems': consumables (coolant, brake fluid, oil, etc) but not parts\n 'carparts': car parts that can be worn out (tyres, fan belt, etc)";
			private string[] _workingKeywords = { "empty" };
			private string[] _notWorkingKeywords = { "allempty", "food", "caritems", "carparts" };

			public override void Run(string[] args)
			{
				// Display help text
				if(args.Length < 1 || args[0] == "?" || args[0] == "help")
				{
					ModConsole.Print(_helpString);
					return;
				}

				// List of all possible keywords
				Dictionary<string, string[]> keywordsAvailable = new Dictionary<string, string[]>
				{
					{ "empty", new string[]{ "empty(itemx)", "bottle_empty", "can_empty" } }
				};
				HashSet<string> keywordsActive = new HashSet<string>();

				// Check if all arguments are a known keyword, then add them to the active list - probably will remove this check and make unknown keywords try to identify game objects by name
				foreach (string key in args)
				{
					if (!(Array.Exists(_workingKeywords, s => s == key) || Array.Exists(_notWorkingKeywords, s => s == key)))
					{
						ModConsole.Print($"'{key}' is not a known keyword.");
						return;
					}
					keywordsActive.UnionWith(keywordsAvailable[key]);
				}


				// Iterate through all game objects - is this a good idea? How much of the game am I fucking up?
				foreach (GameObject o in (GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[]))
				{
					// TODO
				}
			}
		}

		public override void OnLoad()
		{
			ConsoleCommand.Add(new ItemCleanupCommand());
			base.OnLoad();
		}
	}
}
