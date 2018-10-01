/*
Copyright 2018 Wampa842
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
using UnityEngine;
using HutongGames.PlayMaker;

namespace TwentyFourClock
{
	/// <summary>
	/// In-game time mapped to 24 hours
	/// </summary>
	public class Clock24
	{
		private GameObject _sun;
		private FsmFloat _rot;
		private Transform _transH, _transM;

		/// <summary>
		/// Construct a new instance based on the Sun's rotation and the hands of the Finland-shaped clock
		/// </summary>
		public Clock24()
		{
			_sun = GameObject.Find("SUN/Pivot");
			_rot = _sun.GetComponent<PlayMakerFSM>().FsmVariables.FindFsmFloat("Rotation");
			
			_transH = GameObject.Find("SuomiClock/Clock/hour/NeedleHour").transform;
			_transM = GameObject.Find("SuomiClock/Clock/minute/NeedleMinute").transform;
		}

		/// <summary>
		/// Based on the Sun's rotation, returns whether it's the afternoon.
		/// </summary>
		public bool IsAfternoon => (_rot.Value > 330.0f || _rot.Value <= 150.0f);

		/// <summary>
		/// Hour in day, 0 to 12 float.
		/// </summary>
		public float Hour12F => ((360.0f - _transH.localRotation.eulerAngles.y) / 30.0f + 2.0f) % 12;
		/// <summary>
		/// Hour in day, 0 to 24 float.
		/// </summary>
		public float Hour24F => IsAfternoon ? Hour12F + 12.0f : Hour12F;
		/// <summary>
		/// Minute in hour, 0 to 60 float.
		/// </summary>
		public float MinuteF => (360.0f - _transM.localRotation.eulerAngles.y) / 6.0f;
		/// <summary>
		/// Second in minute, 0 to 60 float.
		/// </summary>
		public float SecondF => (MinuteF * 60) % 60;

		/// <summary>
		/// Hour in day, 0 to 11 integer.
		/// </summary>
		public int Hour12 => Mathf.FloorToInt(Hour12F);
		/// <summary>
		/// Hour in day, 0 to 23 integer.
		/// </summary>
		public int Hour24 => Mathf.FloorToInt(Hour24F);
		/// <summary>
		/// Minute in hour, 0 to 59 integer.
		/// </summary>
		public int Minute => Mathf.FloorToInt(MinuteF);
		/// <summary>
		/// Second in minute, 0 to 59 integer.
		/// </summary>
		public int Second => Mathf.FloorToInt(SecondF);

		/// <summary>
		/// The angle of the hour hand.
		/// </summary>
		public float AngleHour => _transH.localRotation.eulerAngles.y;
		/// <summary>
		/// The angle of the minute hand.
		/// </summary>
		public float AngleMinute => _transM.localRotation.eulerAngles.y;
		/// <summary>
		/// The Sun's current angle.
		/// </summary>
		public float AngleSun => _rot.Value;

		/// <summary>
		/// Convert the current 24-hour time to its string representation.
		/// </summary>
		/// <returns>The in-game time as a string, in HH:MM format.</returns>
		public override string ToString() => string.Format("{0:0}:{1:00}", Hour24, Minute);
	}
}