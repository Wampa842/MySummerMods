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
		public static readonly string[] Blacklist = new string[]
		{
			"JONNEZ ES(Clone)",
			"doorl",
			"doorr",
			"doorear"
		};

		private List<GameObject> _list;
		private CarryMore _mod;

		public IEnumerator<GameObject> GetEnumerator()
		{
			return _list.GetEnumerator();
		}
		public bool Contains(GameObject o)
		{
			return _list.Contains(o);
		}
		public GameObject this[int index] => _list[index];
		public int Count => _list.Count;
		public int Capacity => _list.Capacity;
		public int SelectedIndex { get; private set; } = 0;

		public ItemList(int max, CarryMore mod)
		{
			_list = new List<GameObject>(max);
			this._mod = mod;
		}

		// Test if the GameObject can be picked up
		public bool CanPickUp(GameObject o)
		{
			// List is full
			if (_list.Count >= _list.Capacity)
			{
				if (MySettings.Settings.LogAll) ModConsole.Print($"List is full");
				return false;
			}

			// Object is not a part or item
			if (!(o.layer == 16 || o.layer == 19))
			{
				if (MySettings.Settings.LogAll) ModConsole.Print($"{o.name} is on layer {o.layer}");
				return false;
			}

			// Item doesn't have a rigid body
			if (o.GetComponent<Rigidbody>() == null)
			{
				if (MySettings.Settings.LogAll) ModConsole.Print($"{o.name} doesn't have a rigid body");
				return false;
			}

			// Item is in the blacklist
			if (Array.Exists(Blacklist, e => e == o.name))
			{
				if (MySettings.Settings.LogAll) ModConsole.Print($"{o.name} is blacklisted");
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
				PlayMakerFSM.BroadcastEvent("PROCEED Drop");
				_list.Add(o);
				o.transform.parent = null;
				o.GetComponent<Rigidbody>().isKinematic = true;
				o.transform.position = CarryMore.TempPosition;

				if (MySettings.Settings.LogAll || MySettings.Settings.LogSome) ModConsole.Print($"{o.name} added ({_list.Count} / {_list.Capacity})");
			}
			else
			{
				if (MySettings.Settings.LogAll) ModConsole.Print($"{o.name} already on the list");
			}

			// Keep selected index inside limits - can cause issues on first pickup
			if (SelectedIndex >= _list.Count)
				SelectedIndex = _list.Count - 1;
			if (SelectedIndex < 0)
				SelectedIndex = 0;

			return true;
		}

		// Attempt to drop the item at the specified index, then return the GameObject or null
		public GameObject DropAt(int index)
		{
			GameObject item = null;
			try
			{
				item = _list[index];

				item.GetComponent<Rigidbody>().isKinematic = false;
				item.transform.position = Camera.main.transform.position + (Camera.main.transform.forward * 1.0f);

				// If successful, remove it from the list
				_list.Remove(item);

				if (MySettings.Settings.LogAll || MySettings.Settings.LogSome) ModConsole.Print($"Dropped #{index} {item.name} ({_list.Count} / {_list.Capacity})");
			}
			catch
			{
				if (MySettings.Settings.LogAll) ModConsole.Print($"Can't drop #{index}");
				return null;
			}
			return item;
		}

		// Drop the last item, then return the GameObject or null
		public GameObject DropLast()
		{
			return DropAt(_list.Count - 1);
		}

		// Drop all items
		public void DropAll()
		{
			if (MySettings.Settings.LogAll || MySettings.Settings.LogSome) ModConsole.Print($"Dropping {_list.Count} items");
			for (int i = _list.Count - 1; i >= 0; --i)
			{
				DropAt(i);
			}
		}

		// Drop selected item, then return the GameObject or null
		public GameObject DropSelected()
		{
			GameObject item = DropAt(SelectedIndex);
			--SelectedIndex;

			if (SelectedIndex < 0)
				SelectedIndex = 0;
			if (SelectedIndex >= _list.Count)
				SelectedIndex = _list.Count - 1;

			return item;
		}

		// Scroll the list
		public void MoveSelection(float scroll)
		{
			// Move selected index
			if (scroll > 0)
				--SelectedIndex;
			else if (scroll < 0)
				++SelectedIndex;

			// Loop around
			if (SelectedIndex < 0)
				SelectedIndex = _list.Count - 1;
			if (SelectedIndex >= _list.Count)
				SelectedIndex = 0;
		}

		// Resize the list to a given size
		public void Realloc(int size)
		{
			if (Count > 0)
			{
				ModConsole.Error("[Backpack] Could not reallocate - backpack is not empty.");
				return;
			}
			_list = new List<GameObject>(size);
		}

		// Resize the list to the stored size
		public void Realloc()
		{
			Realloc(MySettings.Settings.MaxItems);
		}
	}
}
