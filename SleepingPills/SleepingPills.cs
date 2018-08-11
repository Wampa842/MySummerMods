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
using System.Collections.Generic;
using System.Linq;
using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

namespace SleepingPills
{
	public class PillCommand : ConsoleCommand
	{
		public override string Name => "sp";
		public override string Help => "sleeping pills - cheat";
		SleepingPills _mod;
		public PillCommand(SleepingPills mod)
		{
			_mod = mod;
		}

		public override void Run(string[] args)
		{
			if (args.Length < 1)
				return;
			try
			{
				if (args[0].ToLowerInvariant() == "set")
				{
					if (args.Length >= 3)
						SleepingPills.Swallow(float.Parse(args[1]), float.Parse(args[2]));
					else
						SleepingPills.Swallow(float.Parse(args[1]), 3.0f);
				}

				if (args[0].ToLowerInvariant() == "show")
				{
					ModConsole.Print($"\n-- Sleeping pills --\nAmount left: {SleepingPills.AmountLeft.ToString("0")}, speed: {SleepingPills.FatigueSpeed.ToString("0")}, overdose: {SleepingPills.OverdoseAmount.ToString("0")}");
				}

				if (args[0].ToLowerInvariant() == "kill")
				{
					SleepingPills.StressFsm.Value = 800.0f;
					SleepingPills.DrunkFsm.Value = 3.0f;
					ModConsole.Print($"[Pills] Overdose");
				}
			}
			catch { }
		}
	}

	public class SleepingPills : Mod
	{
		public override string ID => "SleepingPills";
		public override string Name => "Sleeping pills";
		public override string Author => "Wampa842";
		public override string Version => "1.0.1";
		public override bool UseAssetsFolder => true;

		private GameObject _bottle;
		private List<GameObject> _list;

		public static float AmountLeft = 0.0f;
		public static float FatigueSpeed = 1.0f;
		public static float OverdoseAmount = 0.0f;
		public static FsmFloat FatigueFsm;
		public static FsmFloat StressFsm;
		public static FsmFloat DrunkFsm;
		private static readonly List<Vector3> _pos = new List<Vector3>
		{
			new Vector3(-1547.536f, 4.63f, 1180.284f),
			new Vector3(-1547.452f, 4.63f, 1180.273f),
			new Vector3(-1547.502f, 4.63f, 1180.339f)
		};

		public SleepingPills()
		{
			PillSaveData.SavePath = System.IO.Path.Combine(Application.persistentDataPath, "sleeping_pills.xml");
		}

		public static bool Swallow(float amount, float speed)
		{
			AmountLeft += amount;
			FatigueSpeed = speed;
			OverdoseAmount += amount;
			ModConsole.Print($"[Pills] Took a sleeping pill: {SleepingPills.AmountLeft.ToString("0")}, speed: {SleepingPills.FatigueSpeed.ToString("0")}, overdose: {SleepingPills.OverdoseAmount.ToString("0")}");
			return true;
		}

		public void InitShop()
		{
			if (_list != null && _list.Count <= 0)
			{
				foreach (var o in _list)
				{
					GameObject.Destroy(o);
				}
			}

			_list = new List<GameObject>();
			for (int i = 0; i < 3; ++i)
			{
				GameObject pills = GameObject.Instantiate<GameObject>(_bottle);
				pills.name = "sleeping pills(Clone)";
				pills.tag = "Untagged";
				pills.layer = 0;
				pills.transform.position = _pos[i];
				pills.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
				pills.GetComponent<Rigidbody>().isKinematic = true;
				PillBehaviour comp = pills.AddComponent<PillBehaviour>();
				comp.ShopList = _list;
				_list.Add(pills);
			}
		}

		public override void OnLoad()
		{
			// Original
			AssetBundle b = LoadAssets.LoadBundle(this, "pills.unity3d");
			GameObject original = b.LoadAsset<GameObject>("pill_bottle.prefab");
			_bottle = GameObject.Instantiate<GameObject>(original);
			_bottle.name = "sleeping pills";
			Material m = new Material(Shader.Find("Standard"));
			m.mainTexture = original.GetComponent<Renderer>().material.mainTexture;
			_bottle.GetComponent<Renderer>().material = m;

			GameObject.Destroy(original);
			b.Unload(false);

			// Load save
			PillSaveData data = PillSaveData.Deserialize<PillSaveData>(PillSaveData.SavePath);
			for (int i = 0; i < data.Pos.Count; ++i)
			{
				GameObject pills = GameObject.Instantiate<GameObject>(_bottle);
				pills.transform.position = data.Pos[i];
				pills.transform.rotation = data.Rot[i];
				PillBehaviour c = pills.AddComponent<PillBehaviour>();
				c.ShopList = _list;
				c.Count = data.PillCount[i];
				c.Activate();
				c.SetBought();
			}

			// Setup
			FatigueFsm = PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerFatigue");
			StressFsm = PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerStress");
			DrunkFsm = PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerDrunk");
			InitShop();
			ConsoleCommand.Add(new PillCommand(this));
		}

		public override void OnSave()
		{
			PillSaveData data = new PillSaveData(new List<Vector3>(), new List<Quaternion>(), new List<int>(), OverdoseAmount);
			foreach (GameObject o in GameObject.FindObjectsOfType<GameObject>().Where(o => o.GetComponent<PillBehaviour>() != null && !_list.Contains(o)))
			{
				data.Pos.Add(o.transform.position);
				data.Rot.Add(o.transform.rotation);
				data.PillCount.Add(o.GetComponent<PillBehaviour>().Count);
			}
			PillSaveData.Serialize(data, PillSaveData.SavePath);
		}

		public override void Update()
		{
			if (AmountLeft > 0)
			{
				float d = FatigueSpeed * Time.deltaTime;
				FatigueFsm.Value += d;
				AmountLeft -= d;
			}
			if (OverdoseAmount > 0.0f)
			{
				OverdoseAmount -= 0.3f * Time.deltaTime;
			}
			if (OverdoseAmount >= 100.0f)
			{
				StressFsm.Value = 800.0f;
				DrunkFsm.Value = 3.0f;
				ModConsole.Print($"[Pills] Overdose");
				OverdoseAmount = 50.0f;
			}
		}
	}
}
