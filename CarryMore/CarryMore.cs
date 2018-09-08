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
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

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

	public class CarryMore : Mod
	{
		public override string ID => "CarryMore";
		public override string Name => "Backpack";
		public override string Version => "1.4.1";
		public override string Author => "Wampa842";
		public string SavePath => System.IO.Path.Combine(ModLoader.GetModConfigFolder(this), "settings.xml");

		public static readonly Vector3 TempPosition = new Vector3(0.0f, -1000.0f, 0.0f);    // The place where objects are moved when they're "in" the backpack

		private Keybind _pickUpKey;
		private Keybind _dropAllKey;
		private Keybind _dropSelectedKey;
		private Keybind _toggleGuiKey;
		private Keybind _toggleSettingsKey;

		private FsmBool _playerInMenu;
		private bool _resize = false;
		private bool _listVisible;
		private GUIStyle _listStyle;
		private const float _listPosLeft = 150.0f;
		private const float _listPosBottom = 50.0f;
		private const float _listLineHeight = 20.0f;

		public ItemList Items;

		public CarryMore()
		{
			// Add keybinds
			_pickUpKey = new Keybind("PickUp", "Pick up targeted item", KeyCode.E);
			_dropAllKey = new Keybind("DropAll", "Drop all items", KeyCode.Y, KeyCode.LeftControl);
			_dropSelectedKey = new Keybind("DropSelected", "Drop selected item (or last item if the list is hidden)", KeyCode.Y);
			_toggleGuiKey = new Keybind("ToggleGUI", "Toggle inventory list", KeyCode.X);
			_toggleSettingsKey = new Keybind("ToggleSettingsGUI", "Toggle settings", KeyCode.X, KeyCode.LeftControl);

			// Load settings
			MySettings.Load(SavePath);
			//_alertVisible = false;

			// Initialize the item list
			Items = new ItemList(MySettings.Settings.MaxItems, this);

			// Initialize list UI
			_listVisible = false;
			_listStyle = new GUIStyle();
			_listStyle.fontSize = 12;
			_listStyle.normal.textColor = Color.white;
		}

		public override void OnLoad()
		{
			// Add keybinds
			Keybind.Add(this, _pickUpKey);
			Keybind.Add(this, _dropAllKey);
			Keybind.Add(this, _dropSelectedKey);
			Keybind.Add(this, _toggleGuiKey);
			Keybind.Add(this, _toggleSettingsKey);

			// Find Playmaker variables
			_playerInMenu = PlayMakerGlobals.Instance.Variables.FindFsmBool("PlayerInMenu");

			// Welcome text
			ModConsole.Print("[Backpack] Loaded!");
			ModConsole.Print("[Backpack] Settings can be accessed by pressing Ctrl + X.");
		}

		public override void OnSave()
		{
			Items.DropAll();
			MySettings.Save(SavePath);
		}

		public override void Update()
		{
			if (Application.loadedLevelName == "GAME")
			{
				// Reallocate if needed
				if (_resize)
				{
					Items.Realloc();
					_resize = false;
				}

				// Drop everything
				if (_dropAllKey.IsDown())
				{
					if (!MySettings.Settings.DropIfOpen || _listVisible)
						Items.DropAll();
				}

				// Drop the last thing
				if (_dropSelectedKey.IsDown())
				{
					if (!MySettings.Settings.DropIfOpen || _listVisible)
					{
						if (_listVisible)
							Items.DropSelected();
						else
							Items.DropLast();
					}
				}

				// Pick up
				if (_pickUpKey.IsDown() && (!MySettings.Settings.AddIfOpen || _listVisible))
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

				// Toggle list
				if (_toggleGuiKey.IsDown())
				{
					_listVisible = !_listVisible;
				}

				// Scroll list
				if (_listVisible)
				{
					float scroll = Input.GetAxis("Mouse ScrollWheel");
					if (scroll != 0.0f)
						Items.MoveSelection(scroll);
				}

				// Show settings
				if (_toggleSettingsKey.IsDown())
				{
					MySettings.Temporary = MySettings.Settings.Clone() as MySettings.SettingsData;
					MySettings.GuiVisible = true;
				}
			}
		}

		public override void OnGUI()
		{
			if (_listVisible)
			{
				GUI.Label(new Rect(_listPosLeft - 20.0f, Screen.height - _listPosBottom - _listLineHeight, Screen.width, _listLineHeight), $"Backpack ({Items.Count} / {Items.Capacity})");
				for (int i = 0; i < Items.Count; ++i)
				{
					float top = Screen.height - _listPosBottom - (_listLineHeight * ((Items.Count - i) + 1));
					string name = Items[i].name.Replace("(Clone)", "").Replace("(itemx)", "").Replace("(xxxxx)", "");
					if (i == Items.SelectedIndex)
						GUI.Label(new Rect(_listPosLeft - 20.0f, top, 20.0f, _listLineHeight), ">");
					GUI.Label(new Rect(_listPosLeft, top, Screen.width, _listLineHeight), name);
				}
			}

			if (MySettings.GuiVisible)
			{
				_playerInMenu.Value = true;
				GUI.Box(MySettings.GuiBackground, "Settings");

				GUILayout.BeginArea(MySettings.GuiArea);

				GUILayout.BeginVertical();
				MySettings.Temporary.LogAll = GUILayout.Toggle(MySettings.Temporary.LogAll, "Log everything");
				MySettings.Temporary.LogSome = GUILayout.Toggle(MySettings.Temporary.LogSome, "Log pick-up and drop events");
				MySettings.Temporary.AddIfOpen = GUILayout.Toggle(MySettings.Temporary.AddIfOpen, "Pick up only if the list is visible");
				MySettings.Temporary.DropIfOpen = GUILayout.Toggle(MySettings.Temporary.DropIfOpen, "Drop only if the list is visible");
				GUILayout.Label(string.Format("\nCapacity: {0}", MySettings.Temporary.MaxItems));
				MySettings.Temporary.MaxItems = Mathf.RoundToInt(GUILayout.HorizontalSlider(MySettings.Temporary.MaxItems, 1.0f, 50.0f));
				GUILayout.Label("WARNING:\nBackpack must be emptied in order to apply the settings.");
				GUILayout.EndVertical();
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("OK"))
				{
					if (Items.Count <= 0)
					{
						ModConsole.Error("[Backpack] Can't apply settings - backpack is not empty.");
					}
					else
					{
						MySettings.Settings.AddIfOpen = MySettings.Temporary.AddIfOpen;
						MySettings.Settings.DropIfOpen = MySettings.Temporary.DropIfOpen;
						MySettings.Settings.LogAll = MySettings.Temporary.LogAll;
						MySettings.Settings.LogSome = MySettings.Temporary.LogSome;
						MySettings.Settings.MaxItems = MySettings.Temporary.MaxItems;
						Items.Realloc();
						MySettings.Save(SavePath);
						MySettings.GuiVisible = false;
					}
				}
				if (GUILayout.Button("Cancel"))
				{
					MySettings.GuiVisible = false;
				}
				GUILayout.EndHorizontal();
				GUILayout.EndArea();
			}
			else
			{
				_playerInMenu.Value = false;
			}
		}
	}
}
