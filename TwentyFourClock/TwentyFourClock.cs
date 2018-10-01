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
using System.IO;
using MSCLoader;
using UnityEngine;

namespace TwentyFourClock
{
	public class TwentyFourClock : Mod
	{
		public override string ID => "TwentyFourClock";
		public override string Name => "24-hour clock";
		public override string Author => "Wampa842";
		public override string Version => "1.0.0";
		public override bool UseAssetsFolder => true;

		private readonly string _savePath;

		private GameObject _clock;

		public TwentyFourClock()
		{
			_savePath = Path.Combine(Application.persistentDataPath, "digitalclock.xml");
		}

		public override void OnSave()
		{
			DigitalClockBehaviour c = _clock.GetComponent<DigitalClockBehaviour>();

			ClockSaveData.Serialize(new ClockSaveData()
			{
				Position = this._clock.transform.position,
				Rotation = this._clock.transform.rotation,
				AlarmEnabled = c.AlarmEnabled,
				AlarmHour = c.AlarmHour,
				AlarmMinute = c.AlarmMinute,
				DisplayColor = c.DisplayColor
			}, this._savePath);
		}

		public override void OnLoad()
		{
			AssetBundle ab = LoadAssets.LoadBundle(this, "digitalclock.unity3d");
			GameObject original = ab.LoadAsset<GameObject>("digitalclock.prefab");
			
			_clock = GameObject.Instantiate<GameObject>(original);
			_clock.name = "'Atomnyje' clock(Clone)";
			_clock.layer = LayerMask.NameToLayer("Parts");
			_clock.tag = "PART";
			DigitalClockBehaviour c = _clock.AddComponent<DigitalClockBehaviour>();

			ClockSaveData save = ClockSaveData.Deserialize(_savePath);
			_clock.transform.position = save.Position;
			_clock.transform.rotation = save.Rotation;
			c.Setup(save);

			GameObject.Destroy(original);
			ab.Unload(false);

			ConsoleCommand.Add(new ClockCommand(this, c));

			ModConsole.Print(string.Format("[Clock24] 24-hour clock has loaded.\nThe current time is {0:0}:{1:00}", c.Clock.Hour24, c.Clock.Minute));
		}
	}
}