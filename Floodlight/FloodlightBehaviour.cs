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
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using MSCLoader;
using HutongGames.PlayMaker;

namespace Floodlight
{
	public class FloodlightBehaviour : MonoBehaviour
	{
		public Settings UseBattery;
		public Settings Flicker;
		public Settings Unbreakable;

		private FsmBool _guiUse;
		private FsmString _guiText;

		private float _pitch;
		private bool _on;
		private int _health;

		private float _baseIntensity = 1.0f;
		private float _intensityMult = 1.0f;
		private float _flickerMult = 1.0f;
		private float _flickerTimer;
		private float _flickerLength;
		private float _dischargeRate = 0.1f;
		private float _dimStartCharge = 60.0f;
		private float _turnOffCharge = 30.0f;

		private GameObject _base;
		private GameObject _lamp;
		private GameObject _glass;
		private GameObject _indicator;
		private GameObject _battery;

		private MeshCollider _baseCollider;
		private MeshCollider _lampCollider;

		private AudioSource _switchOn;
		private AudioSource _switchOff;
		private AudioSource _disconnect;
		private AudioSource _break;
		private AudioSource _hum;

		private Material _glassMaterial;
		private Material _indicatorMaterial;

		private Light _light;
		private Color _lightColor => new Color(0.9f, 0.9f, 1.0f);
		private FsmFloat _batteryCharge;

		private void _flicker(float time)
		{
			_flickerTimer -= Time.deltaTime;
			bool isFlickering = time < (_health <= 1 ? _flickerLength + 2.0f : _flickerLength) && time >= 0.0f;
			if (isFlickering && !(bool)Unbreakable.GetValue())
			{
				_flickerMult = UnityEngine.Random.Range(0.5f, 1.0f);
				_hum.volume = _flickerMult;
			}
			if (_flickerTimer < 0.0f)
			{
				_flickerMult = 1.0f;
				_hum.volume = 0.3f;
				_flickerTimer = UnityEngine.Random.Range(20.0f, 60.0f);
				_flickerLength = UnityEngine.Random.Range(0.5f, 3.0f);
				if ((bool)Unbreakable.GetValue())
				{
					_health = 100;
				}
				else
				{
					--_health;
				}
			}
		}

		private bool _batteryIsInstalled(GameObject batt)
		{
			foreach (var fsm in batt.GetComponents<PlayMakerFSM>())
			{
				if (fsm.FsmName == "Use")
				{
					return fsm.FsmVariables.GetFsmBool("Installed").Value;
				}
			}
			return false;
		}

		private bool _checkBattery(bool connect)
		{
			// Skip the whole shebang if battery usage is disabled
			if (!(bool)UseBattery.GetValue())
				return true;

			// Disconnect the battery if needed
			if (!connect)
			{
				_battery = null;
				_batteryCharge = null;
				return false;
			}

			// Try to find a battery
			if (_battery == null)
			{
				foreach (GameObject o in GameObject.FindObjectsOfType<GameObject>().Where(e => e.name == "battery(Clone)"))
				{
					if ((o.transform.position - _base.transform.position).sqrMagnitude < 1.0f)
					{
						if (_batteryIsInstalled(o))
							continue;
						_battery = o;
						ModConsole.Print($"[Floodlight] Battery found, {o.GetComponents<PlayMakerFSM>().Single(c => c.FsmName == "Use").FsmVariables.FindFsmFloat("Charge").Value.ToString("0.00")} charge");
					}
				}
				if (_battery == null)
				{
					ModConsole.Print("[Floodlight] Battery not found");
				}
			}

			// Check if the battery is still connected
			if (_battery != null)
			{
				// Check if the battery is in range and not installed in the car
				if ((_battery.transform.position - _base.transform.position).sqrMagnitude < 1.0f && !_batteryIsInstalled(_battery))
				{
					// Check the charge level
					if (_batteryCharge == null)
						_batteryCharge = _battery.GetComponents<PlayMakerFSM>().Single(c => c.FsmName == "Use").FsmVariables.FindFsmFloat("Charge");
					if (_batteryCharge != null && _batteryCharge.Value >= _turnOffCharge)
					{
						// If the charge level is above minimum, drain energy
						if (_on)
						{
							_batteryCharge.Value -= _dischargeRate * Time.deltaTime;
							if (_batteryCharge.Value > _turnOffCharge)
							{
								//_intensity = (_batteryCharge.Value < _dimStartCharge ? (_batteryCharge.Value - _turnOffCharge) / (_dimStartCharge - _turnOffCharge) : 1.0f);
								_intensityMult = _batteryCharge.Value < _dimStartCharge
									? (_batteryCharge.Value - _turnOffCharge) / (_dimStartCharge - _turnOffCharge) / 2.0f + 0.5f
									: 1.0f;
								if ((bool)Flicker.GetValue())
									_flicker(_flickerTimer);
							}
							_light.intensity = _baseIntensity * _intensityMult * _flickerMult;
						}

						return true;
					}
					else
					{
						// If not, disconnect
						ModConsole.Print("[Floodlight] Battery depleted");
						_battery = null;
						_batteryCharge = null;
						return false;
					}
				}
				else
				{
					// If not, disconnect and play the disconnect sound clip
					ModConsole.Print("[Floodlight] Battery disconnected");
					_batteryCharge = null;
					_battery = null;
					_disconnect.Play();
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		private void _switchLight(bool on)
		{
			_on = on;

			// Light
			_light.enabled = _on;
			_light.intensity = _baseIntensity * _intensityMult;

			if (_on)
			{
				_indicatorMaterial.SetColor("_EmissionColor", Color.red);
				_indicatorMaterial.SetFloat("_EmissionScaleUI", 100.0f);

				_glassMaterial.EnableKeyword("_EMISSION");
			}
			else
			{
				_indicatorMaterial.SetColor("_EmissionColor", Color.red * 0.3f);
				_indicatorMaterial.SetFloat("_EmissionScaleUI", 20.0f);

				_glassMaterial.DisableKeyword("_EMISSION");
			}

			// Sound
			if (_on)
			{
				_hum.Play();
			}
			else
			{
				_hum.Stop();
			}
		}

		// MonoBehaviour messages
		void OnCollisionEnter(Collision collision)
		{
			GameObject o = collision.gameObject;
			if (_health <= 0 && o.GetComponent<LightbulbBoxBehaviour>() != null)
			{
				_health = UnityEngine.Random.Range(60, 100);
				ModConsole.Print($"[Floodlight] New lightbulb: {_health}");
				GameObject.Destroy(collision.gameObject);
			}
		}

		void Awake()
		{
			_base = this.gameObject;
			_lamp = this.transform.Find("Floodlight_lamp").gameObject;
			_glass = _lamp.transform.Find("Floodlight_glass").gameObject;
			_indicator = _lamp.transform.Find("indicator").gameObject;

			_baseCollider = _base.GetComponent<MeshCollider>();
			_lampCollider = _lamp.GetComponent<MeshCollider>();

			_switchOn = _lamp.transform.Find("switch_on_sound").GetComponent<AudioSource>();
			_switchOff = _lamp.transform.Find("switch_off_sound").GetComponent<AudioSource>();
			_disconnect = _lamp.transform.Find("disconnect_sound").GetComponent<AudioSource>();
			_break = _lamp.transform.Find("break_sound").GetComponent<AudioSource>();
			_hum = _lamp.transform.Find("hum_sound").GetComponent<AudioSource>();

			_light = _lamp.GetComponent<Light>();

			// Materials
			Material m = new Material(Shader.Find("Standard"));
			m.mainTexture = _base.GetComponent<Renderer>().material.mainTexture;
			_base.GetComponent<Renderer>().material = m;
			_lamp.GetComponent<Renderer>().material = m;

			_glassMaterial = new Material(Shader.Find("Standard"));
			_glassMaterial.mainTexture = _base.GetComponent<Renderer>().material.mainTexture;
			_glassMaterial.SetColor("_EmissionColor", Color.white);
			_glassMaterial.SetFloat("_EmissionScaleUI", 100.0f);
			_glass.GetComponent<MeshRenderer>().material = _glassMaterial;

			_indicatorMaterial = new Material(Shader.Find("Standard"));
			_indicatorMaterial.color = Color.red * 0.3f;
			_indicatorMaterial.SetColor("_EmissionColor", Color.red * 0.3f);
			_indicatorMaterial.SetFloat("_EmissionScaleUI", 100.0f);
			_indicatorMaterial.EnableKeyword("_EMISSION");
			_indicator.GetComponent<MeshRenderer>().material = _indicatorMaterial;

			_guiUse = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIuse");
			_guiText = PlayMakerGlobals.Instance.Variables.FindFsmString("GUIinteraction");

			_flickerTimer = UnityEngine.Random.Range(20.0f, 60.0f);
			_flickerLength = UnityEngine.Random.Range(1.0f, 3.0f);
		}

		void Update()
		{
			// Get inputs
			bool use = cInput.GetButtonDown("Use");
			float scroll = Input.GetAxis("Mouse ScrollWheel");

			// Interaction
			Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 2.0f);
			if(hit.collider == _lampCollider)
			{
				_guiUse.Value = true;
				_guiText.Value = _on ? "Light on" : "Light off";

				if (use)
				{
					_switchLight(!_on);
					if (_on)
					{
						_switchOn.Play();
					}
					else
					{
						_switchOff.Play();
					}
				}
				if (scroll > 0)
				{
					_pitch -= 5.0f;
					if (_pitch < -60.0f)
						_pitch = -60.0f;
					_lamp.transform.localRotation = Quaternion.Euler(_pitch, 0.0f, 0.0f);
				}
				else if (scroll < 0)
				{
					_pitch += 5.0f;
					if (_pitch > 15.0f)
						_pitch = 15.0f;
					_lamp.transform.localRotation = Quaternion.Euler(_pitch, 0.0f, 0.0f);
				}
			}

			// Conditions to switch off
			if (_on)
			{
				if (!_checkBattery(true))
					_switchLight(false);
				if (_health <= 0)
				{
					if (_health == 0)
						_break.Play();
					--_health;
					_switchLight(false);
				}
			}
		}

		// Save/load
		public void Load(FloodlightSaveData saveData)
		{
			_base.transform.position = saveData.Pos;
			_base.transform.rotation = saveData.Rot;
			_pitch = saveData.Pitch;
			_health = saveData.Health;
			_switchLight(saveData.On);
		}

		public FloodlightSaveData GetSaveData()
		{
			return new FloodlightSaveData(_base.transform.position, _base.transform.rotation, _on, _pitch, _health, new System.Collections.Generic.List<Vector3>(), new System.Collections.Generic.List<Quaternion>());
		}

		// Access methods for the console command
		public void SetHealth(int health)
		{
			_health = health;
			ModConsole.Print($"[Floodlight] Health: {_health}");
		}

		public void SetFlicker(float time, float duration)
		{
			_flickerTimer = time;
			if (duration > 0)
				_flickerLength = duration;
			ModConsole.Print($"[Floodlight] Flicker: {_flickerTimer.ToString("0.0")} delay, {_flickerLength.ToString("0.0")} duration");
		}

		public void PrintInfo()
		{
			ModConsole.Print("\n-- Floodlight info --");
			ModConsole.Print(_battery == null ? "Battery is disconnected" : $"Battery is connected, {_batteryCharge.Value.ToString("0.0")} charge");
			ModConsole.Print($"Light is {(_on ? "on" : "off")}, tilted {(-_pitch).ToString("0")} degrees");
			ModConsole.Print($"Bulb health is {_health}, costs {LightbulbBoxBehaviour.Price.ToString("0")}");
			ModConsole.Print($"Flickering after {_flickerTimer.ToString("0.0")} sec, for {_flickerLength.ToString("0.0")} sec");
		}
	}
}