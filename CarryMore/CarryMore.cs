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
				if ((bool)_mod.FullLogging.Value) ModConsole.Print($"List is full");
				return false;
			}

			// Object is not a part or item
			if (!(o.layer == 16 || o.layer == 19))
			{
				if ((bool)_mod.FullLogging.Value) ModConsole.Print($"{o.name} is on layer {o.layer}");
				return false;
			}

			// Item doesn't have a rigid body
			if (o.GetComponent<Rigidbody>() == null)
			{
				if ((bool)_mod.FullLogging.Value) ModConsole.Print($"{o.name} doesn't have a rigid body");
				return false;
			}

			// Item is in the blacklist
			if (Array.Exists(Blacklist, e => e == o.name))
			{
				if ((bool)_mod.FullLogging.Value) ModConsole.Print($"{o.name} is blacklisted");
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

				if ((bool)_mod.SomeLogging.Value || (bool)_mod.FullLogging.Value) ModConsole.Print($"{o.name} added ({_list.Count} / {_list.Capacity})");
			}
			else
			{
				if ((bool)_mod.FullLogging.Value) ModConsole.Print($"{o.name} already on the list");
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

				if ((bool)_mod.SomeLogging.Value || (bool)_mod.FullLogging.Value) ModConsole.Print($"Dropped #{index} {item.name} ({_list.Count} / {_list.Capacity})");
			}
			catch
			{
				if ((bool)_mod.FullLogging.Value) ModConsole.Print($"Can't drop #{index}");
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
			if ((bool)_mod.SomeLogging.Value || (bool)_mod.FullLogging.Value) ModConsole.Print($"Dropping {_list.Count} items");
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
			DropAll();
			_list = new List<GameObject>(size);
		}

		// Resize the list to the settings value
		public void Realloc()
		{
			try
			{
				int val = int.Parse(_mod.MaxItems.GetValue().ToString());   // what the fuck
				Realloc(val);
			}
			catch (Exception ex)
			{
				ModConsole.Error(ex.Message);
			}
		}
	}

	public class CarryMore : Mod
	{
		public override string ID => "CarryMore";
		public override string Name => "Carry more stuff";
		public override string Version => "1.3.1";
		public override string Author => "Wampa842";

		public readonly string SaveFilePath;

		public static readonly Vector3 TempPosition = new Vector3(0.0f, -1000.0f, 0.0f);    // The place where objects are moved when they're "in" the backpack

		private Keybind _pickUpKey;
		private Keybind _dropAllKey;
		private Keybind _dropSelectedKey;
		private Keybind _toggleGuiKey;
		public Settings MaxItems;
		public Settings MaxItemsApply;
		public Settings SomeLogging;    // Log only pick-up and drop events
		public Settings FullLogging;    // Log pick-up rejection events
		public Settings DropIfListVisible;
		public Settings PickUpIfListVisible;
		public Settings EnableSaving;

		private bool _guiVisible;
		private GUIStyle _guiStyle;
		private const float _guiPosLeft = 150.0f;
		private const float _guiPosBottom = 50.0f;
		private const float _guiHeight = 20.0f;

		public ItemList Items;

		public CarryMore()
		{
			SaveFilePath = System.IO.Path.Combine(ModLoader.GetModConfigFolder(this), "SaveData.xml");

			// Add keybinds
			_pickUpKey = new Keybind("PickUp", "Pick up targeted item", KeyCode.E);
			_dropAllKey = new Keybind("DropAll", "Drop all items", KeyCode.Y, KeyCode.LeftControl);
			_dropSelectedKey = new Keybind("DropSelected", "Drop selected item (or last item if the list is hidden)", KeyCode.Y);
			_toggleGuiKey = new Keybind("ToggleGUI", "Toggle inventory list", KeyCode.X);

			// Add settings
			MaxItems = new Settings("MaxItems", "Max items", 10)
			{
				type = SettingsType.Slider
			};
			MaxItemsApply = new Settings("MaxItemsApply", "Apply max items", new Action(() =>
			{
				Items.Realloc();
			}));
			SomeLogging = new Settings("LogSome", "Log pick-up/drop events", true);
			FullLogging = new Settings("LogEverything", "Log everything", false);
			DropIfListVisible = new Settings("DropIfListVisible", "Don't drop items if the list is hidden", true);
			PickUpIfListVisible = new Settings("PickUpIfListVisible", "Don't pick up items if the list is hidden", false);
			EnableSaving = new Settings("EnableSaving", "Save backpack contents (EXPERIMENTAL)", false);

			// Initialize the item list
			Items = new ItemList((int)MaxItems.Value, this);

			// Initialize GUI
			_guiVisible = false;
			_guiStyle = new GUIStyle();
			_guiStyle.fontSize = 12;
			_guiStyle.normal.textColor = Color.white;
		}

		public override void OnLoad()
		{
			// Add keybinds
			Keybind.Add(this, _pickUpKey);
			Keybind.Add(this, _dropAllKey);
			Keybind.Add(this, _dropSelectedKey);
			Keybind.Add(this, _toggleGuiKey);

			// Load save data
			if ((bool)EnableSaving.Value)
			{
				try
				{
					GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();

					XmlDocument doc = new XmlDocument();
					doc.Load(SaveFilePath);
					XmlElement root = doc.DocumentElement;

					// Loop through child nodes - each one contains a GameObject name
					foreach (XmlNode n in root.ChildNodes)
					{
						// Find objects of that name
						GameObject[] match = objects.Where(o => o.name == n.InnerText).ToArray();
						if ((bool)FullLogging.Value) ModConsole.Print(match.Length.ToString() + " matches found for " + n.InnerText);

						foreach (GameObject o in match)
						{
							// If they're found, see if there are any within the region where they're stored while in the backpack
							if ((bool)FullLogging.Value) ModConsole.Print($"{o.name} is at {o.transform.position.x}, {o.transform.position.y}, {o.transform.position.z}");
							if (o.transform.position.y <= (TempPosition.y + 100.0f) && (Mathf.Abs(o.transform.position.x - TempPosition.x) < 10.0f) && (Mathf.Abs(o.transform.position.z - TempPosition.z) < 100.0f) && !Items.Contains(o))
							{
								Items.PickUp(o);
							}
						}
					}
				}
				catch (Exception ex)
				{
					ModConsole.Error("Error while loading backpack saves:\n" + ex.Message);
				}
			}
		}

		public override void ModSettings()
		{
			Settings.AddSlider(this, MaxItems, 1, 50);
			Settings.AddButton(this, MaxItemsApply, "Drop all items and resize the backpack");
			Settings.AddCheckBox(this, SomeLogging);
			Settings.AddCheckBox(this, FullLogging);
			Settings.AddCheckBox(this, DropIfListVisible);
			Settings.AddCheckBox(this, PickUpIfListVisible);
			Settings.AddCheckBox(this, EnableSaving);
		}

		public override void OnSave()
		{
			if((bool)EnableSaving.Value)
			{
				if ((bool)SomeLogging.Value || (bool)FullLogging.Value) ModConsole.Print("Saving backpack contents...");

				// Save item names to XML
				XmlDocument doc = new XmlDocument();
				doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));
				XmlElement root = doc.CreateElement("CarryMoreSave");
				doc.AppendChild(root);
				XmlElement node;

				for (int i = 0; i < Items.Count; ++i)
				{
					GameObject item = Items[i];
					node = doc.CreateElement("BackpackItem");
					node.AppendChild(doc.CreateTextNode(item.name));
					root.AppendChild(node);

					item.GetComponent<Rigidbody>().isKinematic = true;
					item.transform.position = CarryMore.TempPosition;
				}

				doc.Save(SaveFilePath);
			}
			else
			{
				Items.DropAll();
			}
		}

		public override void Update()
		{
			if (Application.loadedLevelName == "GAME")
			{
				// Drop everything
				if (_dropAllKey.IsDown())
				{
					if (!(bool)DropIfListVisible.Value || _guiVisible)
						Items.DropAll();
				}

				// Drop the last thing
				if (_dropSelectedKey.IsDown())
				{
					if (!(bool)DropIfListVisible.Value || _guiVisible)
					{
						if (_guiVisible)
							Items.DropSelected();
						else
							Items.DropLast();
					}
				}

				// Pick up
				if (_pickUpKey.IsDown() && (!(bool)PickUpIfListVisible.Value || _guiVisible))
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

				// Toggle GUI
				if (_toggleGuiKey.IsDown())
				{
					_guiVisible = !_guiVisible;
				}

				// Scroll GUI
				if (_guiVisible)
				{
					float scroll = Input.GetAxis("Mouse ScrollWheel");
					if (scroll != 0.0f)
						Items.MoveSelection(scroll);
				}
			}
		}

		public override void OnGUI()
		{
			if (_guiVisible)
			{
				if (Items.Count > 0)
				{
					for (int i = 0; i < Items.Count; ++i)
					{
						float top = Screen.height - _guiPosBottom - (_guiHeight * (Items.Count - i));
						string name = Items[i].name.Replace("(Clone)", "").Replace("(itemx)", "").Replace("(xxxxx)", "");
						if (i == Items.SelectedIndex)
							GUI.Label(new Rect(_guiPosLeft - 20.0f, top, 20.0f, _guiHeight), ">");
						GUI.Label(new Rect(_guiPosLeft, top, Screen.width, _guiHeight), name);
					}
				}
				else
				{
					GUI.Label(new Rect(_guiPosLeft, Screen.height - _guiPosBottom - _guiHeight, Screen.width, _guiHeight), "Backpack is empty");
				}
			}
		}
	}
}
