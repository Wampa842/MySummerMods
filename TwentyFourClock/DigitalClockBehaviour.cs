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
using System.Linq;
using MSCLoader;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.UI;

namespace TwentyFourClock
{
	public class DigitalClockBehaviour : MonoBehaviour
	{
		// Options
		private int _alarmHour;
		private int _alarmMinute;
		private bool _alarmEnabled;
		private bool _alarmArmed;
		private bool _alarmGuiShow;
		private Color _color;

		// Children and components
		private TextMesh _hourMinText;
		private TextMesh _secText;
		private TextMesh _alarmTimeText;
		private AudioSource _alarmSound;
		private Renderer _dotRenderer;
		private Renderer _alarmTimeRenderer;
		private Renderer _alarmIconRenderer;
		private GameObject _textParent;

		private Clock24 _clock;

		// Game interaction
		private GameObject _pauseMenu;
		private FsmBool _playerInMenu;
		private FsmBool _guiUse;
		private FsmString _guiText;

		// Public access
		public bool AlarmEnabled => _alarmEnabled;
		public int AlarmHour => _alarmHour;
		public int AlarmMinute => _alarmMinute;
		public Clock24 Clock => _clock;
		public Color DisplayColor => _color;

		// Load saved data
		public void Setup(ClockSaveData save)
		{
			_alarmEnabled = save.AlarmEnabled;
			_alarmHour = save.AlarmHour;
			_alarmMinute = save.AlarmMinute;
			_color = save.DisplayColor;
		}

		// Enable alarm at the specified time
		public void Arm(int h, int m)
		{
			_alarmHour = h % 24;
			_alarmMinute = m % 60;
			_alarmEnabled = true;
			Debug.Log(string.Format("Alarm set: {0}:{1}", h, m));
		}

		// Disable alarm
		public void Disarm()
		{
			_alarmSound.Stop();
			_alarmEnabled = false;
		}

		void Awake()
		{
			// Game items
			_playerInMenu = PlayMakerGlobals.Instance.Variables.FindFsmBool("PlayerInMenu");
			_guiUse = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIuse");
			_guiText = PlayMakerGlobals.Instance.Variables.FindFsmString("GUIinteraction");
			_pauseMenu = Resources.FindObjectsOfTypeAll<GameObject>().Single(o => o.name == "OptionsMenu");
			_clock = new Clock24();

			// Child components
			_textParent = gameObject.transform.Find("digitalclock_text").gameObject;
			_hourMinText = gameObject.transform.Find("digitalclock_text/digitalclock_hourmin").gameObject.GetComponent<TextMesh>();
			_secText = gameObject.transform.Find("digitalclock_text/digitalclock_sec").gameObject.GetComponent<TextMesh>();
			_alarmTimeText = gameObject.transform.Find("digitalclock_text/digitalclock_alarm_time").gameObject.GetComponent<TextMesh>();

			_dotRenderer = gameObject.transform.Find("digitalclock_text/digitalclock_blink").gameObject.GetComponent<Renderer>();
			_alarmIconRenderer = gameObject.transform.Find("digitalclock_text/digitalclock_alarm_icon").gameObject.GetComponent<Renderer>();
			_alarmTimeRenderer = gameObject.transform.Find("digitalclock_text/digitalclock_alarm_time").gameObject.GetComponent<Renderer>();

			_alarmSound = gameObject.transform.Find("digitalclock_beep").gameObject.GetComponent<AudioSource>();
		}

		void Update()
		{
			// Interaction
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1.0f) && hit.collider.gameObject == this.gameObject)
			{
				bool use = cInput.GetKeyDown("Use");
				_guiUse.Value = true;

				if (_alarmSound.isPlaying)
				{
					_guiText.Value = "Mute alarm";
					if (use)
					{
						_alarmSound.Stop();
					}
				}
				else
				{
					_guiText.Value = "Set alarm";
					if (use)
					{
						_alarmGuiShow = true;
					}
				}
			}

			// Timekeeping
			_hourMinText.text = _clock.ToString();
			_secText.text = _clock.Second.ToString("00");
			_dotRenderer.enabled = (Time.time % 1.0f) < 0.5f;
			_alarmIconRenderer.enabled = _alarmTimeRenderer.enabled = _alarmEnabled;
			if(_alarmEnabled)
			{
				_alarmTimeText.text = string.Format("{0:0}:{1:00}", _alarmHour, _alarmMinute);
			}

			// Alarm
			if(_alarmArmed && _clock.Hour24 == _alarmHour && _clock.Minute == _alarmMinute)
			{
				_alarmSound.Play();
			}
			_alarmArmed = !(_clock.Hour24 == _alarmHour && _clock.Minute == _alarmMinute);
		}

		void OnGUI()
		{
			if (_alarmGuiShow)
			{
				GUILayout.BeginArea(new Rect(Screen.width / 2 - 100, Screen.height - 200, 220, 170), GUI.skin.box);

				// Time
				GUILayout.BeginVertical();
				GUILayout.Label("Hour");
				int.TryParse(GUILayout.TextField(_alarmHour.ToString()), out _alarmHour);
				_alarmHour %= 24;
				GUILayout.Label("Minute");
				int.TryParse(GUILayout.TextField(_alarmMinute.ToString()), out _alarmMinute);
				_alarmMinute %= 60;
				GUILayout.Label(string.Format("Color ({0:0.0}, {1:0.0}, {2:0.0}", _color.r, _color.g, _color.b));
				GUILayout.BeginHorizontal();
				_color.r = GUILayout.HorizontalSlider(_color.r, 0, 1);
				_color.g = GUILayout.HorizontalSlider(_color.g, 0, 1);
				_color.b = GUILayout.HorizontalSlider(_color.b, 0, 1);
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();

				// Apply the color
				foreach (MeshRenderer c in _textParent.GetComponentsInChildren<MeshRenderer>())
				{
					c.material.SetColor("_Color", _color);
					c.material.SetColor("_Emissive", _color);
				}

				// Buttons
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Arm"))
				{
					Arm(_alarmHour, _alarmMinute);
					_alarmGuiShow = false;
				}
				if (GUILayout.Button("Disarm"))
				{
					Disarm();
					_alarmGuiShow = false;
				}
				if (GUILayout.Button("Cancel"))
				{
					_alarmGuiShow = false;
				}
				GUILayout.EndHorizontal();
				GUILayout.EndArea();
			}
		}
	}
}
