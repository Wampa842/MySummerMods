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
using System.IO;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using MSCLoader;

namespace Floodlight
{
	public class Floodlight : Mod
	{
		public static Vector3 DefaultPos => new Vector3(-13.673f, 0.4f, 3.741f);
		public static readonly Vector3[] ShelfPos = new Vector3[]
		{
			new Vector3(-1548.23f, 4.63f, 1179.85f),
			new Vector3(-1548.06f, 4.63f, 1179.784f),
			new Vector3(-1548.068f, 4.63f, 1179.925f)
		};

		public override string ID => "Floodlight";
		public override string Name => "Floodlight";
		public override string Version => "2.0.2";
		public override string Author => "Wampa842";
		public override bool UseAssetsFolder => true;

		private GameObject _light;
		private GameObject _box;
		private List<GameObject> _list;

		public Settings UseBattery;
		public Settings EnableFlicker;
		public Settings Unbreakable;

		public void InitShop()
		{
			if (_list != null)
			{
				foreach (var o in _list)
				{
					GameObject.Destroy(o);
				}
			}
			_list = new List<GameObject>(3);

			for (int i = 0; i < 3; ++i)
			{
				GameObject box = GameObject.Instantiate<GameObject>(_box);
				box.tag = "Untagged";
				box.transform.position = ShelfPos[i];
				box.transform.rotation = new Quaternion();
				box.GetComponent<Rigidbody>().isKinematic = true;
				LightbulbBoxBehaviour comp = box.AddComponent<LightbulbBoxBehaviour>();
				comp.ShopList = _list;
				_list.Add(box);
			}
		}

		public Floodlight()
		{
			UseBattery = new Settings("UseBattery", "Use battery power", true);
			EnableFlicker = new Settings("EnableFlicker", "Periodic flickering", true);
			Unbreakable = new Settings("UnbreakableBulb", "Lightbulb never breaks", false);
			FloodlightSaveData.SavePath = Path.Combine(Application.persistentDataPath, "floodlight.xml");
		}

		public override void OnLoad()
		{
			try
			{

				// Load assets
				AssetBundle ab = LoadAssets.LoadBundle(this, "floodlight.unity3d");

				GameObject origLight = ab.LoadAsset<GameObject>("floodlight.prefab");
				_light = GameObject.Instantiate<GameObject>(origLight);
				_light.name = "floodlight(Clone)";
				_light.layer = LayerMask.NameToLayer("Parts");
				_light.tag = "PART";
				FloodlightBehaviour comp = _light.AddComponent<FloodlightBehaviour>();
				comp.UseBattery = UseBattery;
				comp.Flicker = EnableFlicker;
				comp.Unbreakable = Unbreakable;

				GameObject origBox = ab.LoadAsset<GameObject>("lightbulb_box.prefab");
				_box = GameObject.Instantiate<GameObject>(origBox);

				GameObject.Destroy(origLight);
				GameObject.Destroy(origBox);
				ab.Unload(false);

				// Initialize objects
				InitShop();

				// Load save
				FloodlightSaveData data = FloodlightSaveData.Deserialize<FloodlightSaveData>(FloodlightSaveData.SavePath);
				_light.GetComponent<FloodlightBehaviour>().Load(data);
				for (int i = 0; i < data.BulbPos.Count; ++i)
				{
					GameObject box = GameObject.Instantiate<GameObject>(_box);
					box.transform.position = data.BulbPos[i];
					box.transform.rotation = data.BulbRot[i];
					LightbulbBoxBehaviour c = box.AddComponent<LightbulbBoxBehaviour>();
					c.ShopList = _list;
					c.Activate();
					c.SetBought();
				}

				// Set up command
				ConsoleCommand.Add(new FloodlightCommand(_light));
			}
			catch (Exception ex)
			{
				ModConsole.Error(ex.ToString());
			}
		}

		public override void OnSave()
		{
			FloodlightSaveData saveData = _light.GetComponent<FloodlightBehaviour>().GetSaveData();
			GameObject[] boxes = GameObject.FindObjectsOfType<GameObject>().Where(e => e.GetComponent<LightbulbBoxBehaviour>() != null && !_list.Contains(e)).ToArray();
			foreach (GameObject o in boxes)
			{
				saveData.BulbPos.Add(o.transform.position);
				saveData.BulbRot.Add(o.transform.rotation);
			}
			FloodlightSaveData.Serialize<FloodlightSaveData>(saveData, FloodlightSaveData.SavePath);
		}

		public override void ModSettings()
		{
			Settings.AddCheckBox(this, UseBattery);
			Settings.AddCheckBox(this, EnableFlicker);
			Settings.AddCheckBox(this, Unbreakable);
		}
	}
}