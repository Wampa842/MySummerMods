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
using System.Collections;
using System.Xml;
using System.Linq;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

namespace CarryMore
{
	public class CarryMore : Mod
	{
		public override string ID => "CarryMore";
		public override string Name => "Backpack";
		public override string Version => "1.4.3";
		public override string Author => "Wampa842";
		public string SavePath => System.IO.Path.Combine(ModLoader.GetModConfigFolder(this), "settings.xml");

		public static readonly Vector3 TempPosition = new Vector3(0.0f, -1000.0f, 0.0f);    // The place where objects are moved when they're "in" the backpack

		private Keybind _pickUpKey;
		private Keybind _dropAllKey;
		private Keybind _dropSelectedKey;
		private Keybind _toggleGuiKey;
		private Keybind _toggleSettingsKey;

		private FsmBool _playerInMenu;
		private GameObject _pauseMenu;
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

			// Find gameobjects and variables
			_playerInMenu = PlayMakerGlobals.Instance.Variables.FindFsmBool("PlayerInMenu");
			_pauseMenu = Resources.FindObjectsOfTypeAll<GameObject>().Single(o => o.name == "OptionsMenu");

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
					if (Items.Count > 0)
					{
						ModConsole.Error($"[Backpack] Can't apply settings - backpack is not empty ({Items.Count})");
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
						if (!_pauseMenu.activeInHierarchy)
						{
							_playerInMenu.Value = false;
						}
					}
				}
				if (GUILayout.Button("Cancel"))
				{
					MySettings.GuiVisible = false;
					if (!_pauseMenu.activeInHierarchy)
					{
						_playerInMenu.Value = false;
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.EndArea();
			}
		}
	}
}
