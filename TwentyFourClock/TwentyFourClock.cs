/*
Copyright <YEAR> <COPYRIGHT HOLDER>
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
documentation files (the "Software"), to deal in the Software without restriction, including without limitation
the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
DEALINGS IN THE SOFTWARE.
*/
using System;
using System.IO;
using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

namespace TwentyFourClock
{
	public class Clock24
	{
		private GameObject _sun;
		private FsmFloat _rot;
		private Quaternion _rotH, _rotM;
		public Clock24()
		{
			_sun = GameObject.Find("SUN/Pivot");
			_rot = _sun.GetComponent<PlayMakerFSM>().FsmVariables.FindFsmFloat("Rotation");
			_rotH = GameObject.Find("SuomiClock/Clock/hour/NeedleHour").transform.localRotation;
			_rotM = GameObject.Find("SuomiClock/Clock/minute/NeedleMinute").transform.localRotation;
		}

		public float Hour12 => ((360.0f - _rotH.eulerAngles.y) / 30.0f + 2.0f) % 12;

		public float Hour24 => (_rot.Value > 330.0f || _rot.Value <= 150.0f) ? Hour12 + 12.0f : Hour12;

		public float Minute => (360.0f - _rotM.eulerAngles.y) / 6.0f;

		public float Second => (Minute * 60) % 60;

		public override string ToString() => $"{Mathf.Floor(Hour24).ToString("00")}:{Mathf.Floor(Minute).ToString("00")}";
	}

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
