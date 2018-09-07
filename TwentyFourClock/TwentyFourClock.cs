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
using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using Clock24Hour;

namespace Clock24Hour
{
	public class Clock24
	{
		private GameObject _sun;
		private FsmFloat _rot;
		private GameObject _needleH, _needleM;
		public Clock24()
		{
			_sun = GameObject.Find("SUN/Pivot");
			_rot = _sun.GetComponent<PlayMakerFSM>().FsmVariables.FindFsmFloat("Rotation");
			_needleH = GameObject.Find("SuomiClock/Clock/hour/NeedleHour");
			_needleM = GameObject.Find("SuomiClock/Clock/minute/NeedleMinute");
		}

		public float Hour12 => ((360.0f - _needleH.transform.localRotation.eulerAngles.y) / 30.0f + 2.0f) % 12;

		public float Hour24 => (_rot.Value > 330.0f || _rot.Value <= 150.0f) ? Hour12 + 12.0f : Hour12;

		public float Minute => (360.0f - _needleM.transform.localRotation.eulerAngles.y) / 6.0f;

		public float Second => (Minute * 60) % 60;

		public override string ToString() => $"{Mathf.Floor(Hour24).ToString("00")}:{Mathf.Floor(Minute).ToString("00")}";
	}
}

namespace TwentyFourClock
{
	public class TwentyFourClock : Mod
	{
		public override string ID => "TwentyFourClock";
		public override string Name => "24-hour clock";
		public override string Author => "Wampa842";
		public override string Version => "1.0.0";
		public override bool UseAssetsFolder => false;

		private Clock24 _clock;
		private Rect _rect;
		private GUIStyle _style;

		public override void OnLoad()
		{
			_clock = new Clock24();
			_rect = new Rect(Screen.width / 2 - 80.0f, 40.0f, 160.0f, 30.0f);
			_style = new GUIStyle();
			_style.fontSize = 20;	
			_style.normal.textColor = Color.white;
			_style.alignment = TextAnchor.MiddleCenter;
		}

		public override void OnGUI()
		{
			GUI.Label(_rect, _clock.ToString(), _style);
		}
	}
}
