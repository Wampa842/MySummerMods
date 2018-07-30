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

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

namespace CarryMore
{
	public class ItemList
	{
		public static string[] Blacklist = new string[]
		{
			"JONNEZ ES(Clone)"
		};

		private List<GameObject> _list;
		public CarryMore Mod;

		public ItemList(int max, CarryMore mod)
		{
			_list = new List<GameObject>(max);
			this.Mod = mod;
		}

		// Test if the GameObject can be picked up
		public bool CanPickUp(GameObject o)
		{
			// List is full
			if (_list.Count >= _list.Capacity)
			{
				if ((bool)Mod.FullLogging.Value) ModConsole.Print($"List is full");
				return false;
			}

			// Object is not a part or item
			if (!(o.layer == 16 || o.layer == 19))
			{
				if ((bool)Mod.FullLogging.Value) ModConsole.Print($"{o.name} is on layer {o.layer}");
				return false;
			}

			// Item doesn't have a rigid body
			if (o.GetComponent<Rigidbody>() == null)
			{
				if ((bool)Mod.FullLogging.Value) ModConsole.Print($"{o.name} doesn't have a rigid body");
				return false;
			}

			// Item is in the blacklist
			if(Array.Exists(Blacklist, e => e == o.name))
			{
				if ((bool)Mod.FullLogging.Value) ModConsole.Print($"{o.name} is blacklisted");
				return false;
			}

			// Item is installed on or bolted to the car
			// Loop through all PlayMakerFSM components
			foreach (PlayMakerFSM c in o.GetComponents<PlayMakerFSM>())
			{
				// Part is installed if component is "Data" or "Use" and "Installed" is true
				if (c.FsmName == "Data" || c.FsmName == "Use")
				{
					FsmBool v = c.FsmVariables.FindFsmBool("Installed");
					if (v != null && v.Value)
						return false;
				}

				// Part is bolted if component is "BoltCheck" and "Tightness" is greater than 0
				if (c.FsmName == "BoltCheck")
				{
					FsmFloat v = c.FsmVariables.FindFsmFloat("Tightness");
					if (v != null && v.Value > 0.0f)
						return false;
				}
			}

			// Otherwise, the item can be picked up
			return true;
		}

		// Attempt to pick up an item
		public bool PickUp(GameObject o)
		{
			// Test if it's possible to pick up
			if (!CanPickUp(o))
				return false;

			if (!_list.Contains(o))
			{
				_list.Add(o);
				o.SetActive(false);
				if ((bool)Mod.SomeLogging.Value || (bool)Mod.FullLogging.Value) ModConsole.Print($"{o.name} added ({_list.Count} / {_list.Capacity})");
			}
			else
			{
				if ((bool)Mod.FullLogging.Value) ModConsole.Print($"{o.name} already on the list");
			}

			return true;
		}

		// Attempt to drop the item at the specified index
		public void DropAt(int index)
		{
			try
			{
				GameObject item = _list[index];

				// Enable the item and place it in front of the player
				item.SetActive(true);
				item.transform.position = Camera.main.transform.position + (Camera.main.transform.forward * 1.0f);
				item.transform.eulerAngles = Camera.main.transform.eulerAngles;

				// If successful, remove it from the list
				_list.Remove(item);

				if ((bool)Mod.SomeLogging.Value || (bool)Mod.FullLogging.Value) ModConsole.Print($"Dropped #{index} {item.name} ({_list.Count} / {_list.Capacity})");
			}
			catch
			{
				if ((bool)Mod.FullLogging.Value) ModConsole.Print($"Can't drop #{index}");
			}
		}

		// Drop the last item
		public void DropLast()
		{
			DropAt(_list.Count - 1);
		}

		// Drop all items
		public void DropAll()
		{
			if ((bool)Mod.SomeLogging.Value || (bool)Mod.FullLogging.Value) ModConsole.Print($"Dropping {_list.Count} items");
			for (int i = _list.Count - 1; i >= 0; --i)
			{
				DropAt(i);
			}
		}
	}

	public class CarryMore : Mod
	{
		public override string ID => "CarryMore";
		public override string Name => "Carry more stuff";
		public override string Version => "1.1.0";
		public override string Author => "Wampa842";

		private Keybind _pickUpKey;
		private Keybind _dropAllKey;
		private Keybind _dropLastKey;
		public Settings MaxItems;
		public Settings SomeLogging;    // Log only pick-up and drop events
		public Settings FullLogging;    // Log pick-up rejection events

		public ItemList Items;

		public CarryMore()
		{
			_pickUpKey = new Keybind("PickUp", "Pick up targeted item", KeyCode.E);
			_dropAllKey = new Keybind("DropAll", "Drop all items", KeyCode.Y, KeyCode.LeftControl);
			_dropLastKey = new Keybind("DropLast", "Drop last item", KeyCode.Y);

			MaxItems = new Settings("MaxItems", "Max items", 10, () =>
			{
				Items.DropAll();
				Items = new ItemList((int)MaxItems.Value, this);
			});
			SomeLogging = new Settings("LogSome", "Log pick-up/drop events", true);
			FullLogging = new Settings("LogEverything", "Log everything", false);

			Items = new ItemList((int)MaxItems.Value, this);
		}

		public override void OnLoad()
		{
			Keybind.Add(this, _pickUpKey);
			Keybind.Add(this, _dropAllKey);
			Keybind.Add(this, _dropLastKey);
		}

		public override void ModSettings()
		{
			Settings.AddSlider(this, MaxItems, 1, 50);
			Settings.AddCheckBox(this, SomeLogging);
			Settings.AddCheckBox(this, FullLogging);
		}

		public override void OnSave()
		{
			// Drop all items
			Items.DropAll();
		}

		public override void Update()
		{
			if (Application.loadedLevelName == "GAME")
			{
				if (_dropAllKey.IsDown())
				{
					Items.DropAll();
				}
				if (_dropLastKey.IsDown())
				{
					Items.DropLast();
				}
				if (_pickUpKey.IsDown())
				{
					RaycastHit[] raycastHits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 1.0f);
					for (int i = 0; i < raycastHits.Length; ++i)
					{
						GameObject item = raycastHits[i].transform.gameObject;
						if (Items.PickUp(item))
						{
							break;
						}
					}
				}
			}
		}
	}
}
