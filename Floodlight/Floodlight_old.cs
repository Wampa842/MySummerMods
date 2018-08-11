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
using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using System.Collections.Generic;

namespace FloodlightOld
{

	public class FloodlightOld : Mod
	{
		public class FloodlightCommand : ConsoleCommand
		{
			public override string Name => "fl";
			public override string Help => "Floodlight options - use 'fl help' for more info";
			private FloodlightOld _mod;
			private const string _helpString = "Floodlight options:\nfl info: display information about the floodlight\nfl health [number]: display or set the bulb health\nfl flicker [delay [length]]: display or set flicker options\nfl help: display this text";

			public FloodlightCommand(FloodlightOld mod)
			{
				_mod = mod;
			}

			public override void Run(string[] args)
			{
				if (args.Length < 1)
					return;

				if (args[0] == "flicker")
				{
					if (args.Length == 2)
					{
						_mod._flickerTimer = float.Parse(args[1]);
						_mod._flickerLength = 1.0f;
					}
					else if (args.Length == 3)
					{
						_mod._flickerTimer = float.Parse(args[1]);
						_mod._flickerLength = float.Parse(args[2]);
					}
					ModConsole.Print($"Timer: {_mod._flickerTimer.ToString("0")}, length: {_mod._flickerLength.ToString("0")}");
				}

				if (args[0] == "health")
				{
					if (args.Length == 2)
					{
						_mod._bulbHealth = int.Parse(args[1]);
					}
					ModConsole.Print($"Bulb health: {_mod._bulbHealth}");
				}

				if(args[0] == "info")
				{
					ModConsole.Print("\n-- Floodlight info --");
					ModConsole.Print(_mod._battery == null ? "Battery is disconnected" : $"Battery is connected, {_mod._batteryCharge.Value.ToString("0.0")} charge");
					ModConsole.Print($"Light is {(_mod._on ? "on" : "off")}, tilted {(-_mod._pitch).ToString("0")}°");
					ModConsole.Print($"Bulb health is {_mod._bulbHealth}, costs {_mod._bulbHealth}");
					ModConsole.Print($"Flickering after {_mod._flickerTimer}, for {_mod._flickerLength} seconds");
				}

				if(args[0] == "help")
				{
					ModConsole.Print(_helpString);
				}
			}
		}

		public class SaveData
		{
			public float Pitch;
			public bool On;
			public int BulbHealth;
			public Vector3 Pos;
			public Quaternion Rot;
			public SaveData(FloodlightOld mod)
			{
				Pitch = mod._pitch;
				On = mod._on;
				BulbHealth = mod._bulbHealth;
				Pos = mod._base.transform.position;
				Rot = mod._base.transform.rotation;
			}
			public SaveData() { }
			public static void Serialize<T>(T data, string path)
			{
				try
				{
					XmlSerializer serializer = new XmlSerializer(typeof(T));
					XmlWriter writer = XmlWriter.Create(new StreamWriter(path));
					serializer.Serialize(writer, data);
					writer.Close();
				}
				catch
				{
					throw;
				}
			}
			public static T Deserialize<T>(string path) where T : new()
			{
				try
				{
					XmlSerializer serializer = new XmlSerializer(typeof(T));
					XmlReader reader = XmlReader.Create(new StreamReader(path));
					return (T)serializer.Deserialize(reader);
				}
				catch
				{
					throw;
				}
			}
		}

		public class FloodlightAudio
		{
			public string SwitchOnFile => "audio\\switch_on.ogg";
			public string SwitchOffFile => "audio\\switch_off.ogg";
			public string HumLoopFile => "audio\\hum.ogg";
			public string DisconnectFile => "audio\\disconnect.ogg";
			public string BreakFile => "audio\\break.ogg";
			public AudioSource SwitchOn { get; set; }
			public AudioSource SwitchOff { get; set; }
			public AudioSource HumLoop { get; set; }
			public AudioSource Disconnect { get; set; }
			public AudioSource Break { get; set; }
			public bool HasLoaded = false;
			public FloodlightAudio()
			{
				this.HasLoaded = false;
				this.SwitchOn = null;
				this.SwitchOff = null;
				this.HumLoop = null;
				this.Disconnect = null;
				this.Break = null;
			}
			public static void Play(AudioSource src)
			{
				if (src != null)
					src.Play();
			}
			public static void Stop(AudioSource src)
			{
				if (src != null)
					src.Stop();
			}
		}

		public override string ID => "Floodlight";
		public override string Name => "Floodlight";
		public override string Author => "Wampa842";
		public override string Version => "1.0.1";
		public override bool UseAssetsFolder => true;

		public Settings UseBattery;									// Require charged battery
		public Settings EnableFlicker;								// Enable periodic flickering (epileptics may want to disable it)
		public Settings Unbreakable;								// Lightbulb never breaks

		private readonly string _savePath;							// Path to floodlight.xml

		private bool _on = false;                       // Whether the light is currently on
		private int _bulbHealth = 30;                   // The health of a new bulb
		private float _pitch = 0.0f;                    // The lamp's angle
		private float _intensity = 1.0f;                // Light intensity multiplier
		private float _bulbCost = 300.0f;               // The cost of replacing a busted lightbulb
		private const float _baseIntensity = 2.0f;      // The light's maximal intensity
		private const float _dimStartCharge = 50.0f;	// Charge level at which the light starts to dim
		private const float _turnOffCharge = 30.0f;		// Charge level at which the light turns off
		private const float _dischargeRate = 0.1f;		// Battery energy consumption per second
		private string _bulbText => $"replace lightbulb ({_bulbCost.ToString("0")} mk)";

		private readonly Vector3 _defaultPos = new Vector3(-13.673f, 0.4f, 3.741f);            // Position in case the save file can't be loaded
		private readonly Vector3 _teimoPos = new Vector3(-1548.23f, 4.644f, 1179.85f);      // New lightbulb's position at Teimo's
		private readonly Vector3 _fleetariPos = new Vector3(1553.51f, 5.475f, 740.57f);     // New lightbulb's position at Fleetari's

		private Color _lightColor = new Color(0.9f, 0.9f, 1.0f);    // The color of the light and the emissive color of the glass

		private float _flickerTimer;                // The time in seconds until the next flicker event
		private float _flickerMultiplier = 1.0f;    // Flicker dimming
		private float _flickerLength;               // The length of the flickering

		private GameObject _base;                   // The lamp stand that provides the root rigidbody and collider
		private GameObject _lamp;                   // The tiltable lamp
		private GameObject _glass;                  // Luminous glass
		private GameObject _switch;                 // Indicator light
		private GameObject _battery;                // Connected battery
		private FsmFloat _batteryCharge;            // The charge level of the connected battery
		private Material _glassMaterial;            // Luminous white textured
		private Material _switchMaterial;           // Luminous red
		private Light _light;                       // The actual light emitter
		private MeshCollider _lampCollider;         // The collider of the tiltable lamp that is detected by raycast
		private MeshCollider _boxColliderTeimo;     // Collider of the lightbulb box at Teimo's
		private MeshCollider _boxColliderFleetari;  // Same at Fleetari's
		private WWW _load;

		private FloodlightAudio _audio;				// Audio sources

		public FloodlightOld()
		{
			_savePath = Path.Combine(Application.persistentDataPath, "floodlight.xml");
			_audio = new FloodlightAudio();
			UseBattery = new Settings("UseBattery", "Consume battery power", true);
			EnableFlicker = new Settings("EnableFlicker", "Periodic flickering (EPILEPSY WARNING)", true);
			Unbreakable = new Settings("UnbreakableBulb", "Lightbulb never breaks", false);
		}

		private void _flicker(float time)
		{
			_flickerTimer -= Time.deltaTime;
			bool isFlickering = time < (_bulbHealth <= 1 ? _flickerLength + 2.0f : _flickerLength) && time >= 0.0f;
			if (isFlickering && !(bool)Unbreakable.GetValue())
			{
				_flickerMultiplier = UnityEngine.Random.Range(0.5f, 1.0f);
				_audio.HumLoop.volume = _flickerMultiplier;
			}
			if (_flickerTimer < 0.0f)
			{
				_flickerMultiplier = 1.0f;
				_audio.HumLoop.volume = 0.3f;
				_flickerTimer = UnityEngine.Random.Range(20.0f, 60.0f);
				_flickerLength = UnityEngine.Random.Range(0.5f, 3.0f);
				if ((bool)Unbreakable.GetValue())
				{
					_bulbHealth = 100;
				}
				else
				{
					--_bulbHealth;
				}
				ModConsole.Print($"Floodlight: selected new flicker timing: {_flickerTimer.ToString("0.0")} delay, {_flickerLength.ToString("0.0")} length, {_bulbHealth} health");
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
						ModConsole.Print($"Floodlight: battery found, {o.GetComponents<PlayMakerFSM>().Single(c => c.FsmName == "Use").FsmVariables.FindFsmFloat("Charge").Value.ToString("0.00")} charge");
					}
				}
				if (_battery == null)
				{
					ModConsole.Print("Floodlight: battery not found");
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
								_intensity = _batteryCharge.Value < _dimStartCharge
									? (_batteryCharge.Value - _turnOffCharge) / (_dimStartCharge - _turnOffCharge) / 2.0f + 0.5f
									: 1.0f;
								if ((bool)EnableFlicker.GetValue())
									_flicker(_flickerTimer);
							}
							_light.intensity = _baseIntensity * _intensity * _flickerMultiplier;
						}

						return true;
					}
					else
					{
						// If not, disconnect
						ModConsole.Print("Floodlight: battery depleted");
						_battery = null;
						_batteryCharge = null;
						return false;
					}
				}
				else
				{
					// If not, disconnect and play the disconnect sound clip
					ModConsole.Print("Floodlight: battery disconnected");
					_batteryCharge = null;
					_battery = null;
					FloodlightAudio.Play(_audio.Disconnect);
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
			_light.intensity = _baseIntensity * _intensity;
			_glassMaterial.SetColor("_EmissionColor", _lightColor * (_on ? 100 : 0));
			_glassMaterial.SetFloat("_EmissionScaleUI", _on ? 100.0f : 0.0f);
			_switchMaterial.color = Color.red * (_on ? 1.0f : 0.2f);
			_switchMaterial.SetColor("_EmissionColor", Color.red);
			_switchMaterial.SetFloat("_EmissionScaleUI", _on ? 50.0f : 0.0f);

			if (_on)
			{
				_glassMaterial.EnableKeyword("_EMISSION");
				_switchMaterial.EnableKeyword("_EMISSION");
			}
			else
			{
				_glassMaterial.DisableKeyword("_EMISSION");
				_switchMaterial.DisableKeyword("_EMISSION");
			}

			// Sound
			if (_on)
			{
				FloodlightAudio.Play(_audio.HumLoop);
			}
			else
			{
				FloodlightAudio.Stop(_audio.HumLoop);
			}
		}

		private void _initLight(SaveData saveData)
		{
			MeshCollider coll;
			MeshRenderer renderer;
			Texture2D tex_diff = LoadAssets.LoadTexture(this, "model\\floodlight_d.png");

			// Floodlight base
			_base = LoadAssets.LoadOBJ(this, "model\\floodlight_base.obj", false, false);
			_base.name = "floodlight(Clone)";
			_base.layer = LayerMask.NameToLayer("Parts");
			_base.tag = "PART";
			_base.transform.position = saveData.Pos;
			_base.transform.rotation = saveData.Rot;
			coll = _base.AddComponent<MeshCollider>();
			coll.convex = true;
			coll.name = "floodlight(Clone)";
			Rigidbody rb = _base.AddComponent<Rigidbody>();
			rb.mass = 5.0f;
			rb.detectCollisions = true;
			rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
			rb.isKinematic = false;
			renderer = _base.GetComponent<MeshRenderer>();
			renderer.material.mainTexture = tex_diff;

			// Lamp cover
			_lamp = LoadAssets.LoadOBJ(this, "model\\floodlight_cover.obj", false, false);
			_lamp.name = "floodlight cover(Clone)";
			_lamp.transform.parent = _base.transform;
			_lamp.transform.localPosition = new Vector3(0.0f, 0.22f, 0.0f);
			_lamp.transform.localEulerAngles = new Vector3(_pitch, 0.0f, 0.0f);
			_lampCollider = _lamp.AddComponent<MeshCollider>();
			_lampCollider.convex = true;
			_lampCollider.isTrigger = true;
			renderer = _lamp.GetComponent<MeshRenderer>();
			renderer.material.mainTexture = tex_diff;

			// Glass
			_glass = LoadAssets.LoadOBJ(this, "model\\floodlight_glass.obj", false, false);
			_glass.name = "floodlight glass(Clone)";
			_glass.transform.parent = _lamp.transform;
			_glass.transform.localPosition = new Vector3();
			_glass.transform.localRotation = new Quaternion();
			_glassMaterial = _glass.GetComponent<MeshRenderer>().material;
			_glassMaterial.mainTexture = tex_diff;
			_glassMaterial.shader = Shader.Find("Standard");

			// Switch
			_switch = GameObject.CreatePrimitive(PrimitiveType.Cube);
			_switch.name = "floodlight indicator(Clone)";
			_switch.transform.parent = _lamp.transform;
			_switch.transform.localScale = new Vector3(0.03f, 0.01f, 0.01f);
			_switch.transform.localPosition = new Vector3(0.0f, 0.09f, 0.04f);
			_switch.transform.localRotation = new Quaternion();
			_switch.AddComponent<Renderer>();
			_switchMaterial = _switch.GetComponent<Renderer>().material;
			_switchMaterial.color = Color.red * 0.3f;
			GameObject.Destroy(_switch.GetComponent<Rigidbody>());
			GameObject.Destroy(_switch.GetComponent<Collider>());

			// Light
			_light = _lamp.AddComponent<Light>();
			_light.type = LightType.Spot;
			_light.color = _lightColor;
			_light.intensity = _baseIntensity;
			_light.range = 20.0f;
			_light.spotAngle = 80.0f;
			_light.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
			_light.renderMode = LightRenderMode.ForceVertex;
			_light.enabled = false;
		}

		private void _initLightWithBundle(SaveData saveData)
		{
			AssetBundle bundle = LoadAssets.LoadBundle(this, "floodlight.unity3d");
			_base = bundle.LoadAsset<GameObject>("floodlight.prefab");
		}

		private void _initShop()
		{
			// Load the lightbulb box
			GameObject original = LoadAssets.LoadOBJ(this, "model\\box.obj", false, false);
			original.name = "new_lightbulb";
			original.GetComponent<MeshRenderer>().material.mainTexture = LoadAssets.LoadTexture(this, "model\\box_d.png");
			AudioSource audio = original.AddComponent<AudioSource>();
			audio.clip = GameObject.Find("cash_register_2").GetComponent<AudioSource>().clip;
			audio.transform.parent = original.transform;
			audio.spatialBlend = 1.0f;
			audio.maxDistance = 10.0f;

			GameObject fBox = GameObject.Instantiate<GameObject>(original);

			original.transform.position = _teimoPos;
			original.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
			MeshCollider coll = original.AddComponent<MeshCollider>();
			coll.name = "replace lightbulb (300mk)(Clone)";
			coll.convex = true;
			coll.isTrigger = true;
			_boxColliderTeimo = coll;

			fBox.transform.position = _fleetariPos;
			coll = fBox.AddComponent<MeshCollider>();
			coll.name = "replace lightbulb (300mk)(Clone)";
			coll.convex = true;
			coll.isTrigger = true;
			_boxColliderFleetari = coll;
		}

		private void _initAudioAsync()
		{
			if (_audio.SwitchOn == null)
			{
				if (_load == null)
					_load = new WWW("file:///" + Path.Combine(ModLoader.GetModAssetsFolder(this), _audio.SwitchOnFile));
				if (!_load.isDone) return;
				_audio.SwitchOn = _lamp.AddComponent<AudioSource>();
				_audio.SwitchOn.clip = _load.GetAudioClip(true);
				_audio.SwitchOn.transform.parent = _lamp.transform;
				_audio.SwitchOn.spatialBlend = 1.0f;
				_audio.SwitchOn.maxDistance = 5.0f;
				_load = null;
				ModConsole.Print("Loaded SwitchOn");
			}
			if (_audio.SwitchOff == null)
			{
				if (_load == null)
					_load = new WWW("file:///" + Path.Combine(ModLoader.GetModAssetsFolder(this), _audio.SwitchOffFile));
				if (!_load.isDone) return;
				_audio.SwitchOff = _lamp.AddComponent<AudioSource>();
				_audio.SwitchOff.clip = _load.GetAudioClip(true);
				_audio.SwitchOff.transform.parent = _lamp.transform;
				_audio.SwitchOff.spatialBlend = 1.0f;
				_audio.SwitchOff.maxDistance = 5.0f;
				_load = null;
				ModConsole.Print("Loaded SwitchOff");
			}
			if (_audio.HumLoop == null)
			{
				if (_load == null)
					_load = new WWW("file:///" + Path.Combine(ModLoader.GetModAssetsFolder(this), _audio.HumLoopFile));
				if (!_load.isDone) return;
				_audio.HumLoop = _lamp.AddComponent<AudioSource>();
				_audio.HumLoop.clip = _load.GetAudioClip(true);
				_audio.HumLoop.loop = true;
				_audio.HumLoop.volume = 0.3f;
				_audio.HumLoop.transform.parent = _lamp.transform;
				_audio.HumLoop.spatialBlend = 1.0f;
				_audio.HumLoop.maxDistance = 5.0f;
				_load = null;
				ModConsole.Print("Loaded Hum");
			}
			if (_audio.Disconnect == null)
			{
				if (_load == null)
					_load = new WWW("file:///" + Path.Combine(ModLoader.GetModAssetsFolder(this), _audio.DisconnectFile));
				if (!_load.isDone) return;
				_audio.Disconnect = _lamp.AddComponent<AudioSource>();
				_audio.Disconnect.clip = _load.GetAudioClip(true);
				_audio.Disconnect.transform.parent = _lamp.transform;
				_audio.Disconnect.spatialBlend = 1.0f;
				_audio.Disconnect.maxDistance = 10.0f;
				_load = null;
				ModConsole.Print("Loaded Disconnect");
			}
			if (_audio.Break == null)
			{
				if (_load == null)
					_load = new WWW("file:///" + Path.Combine(ModLoader.GetModAssetsFolder(this), _audio.BreakFile));
				if (!_load.isDone) return;
				_audio.Break = _lamp.AddComponent<AudioSource>();
				_audio.Break.clip = _load.GetAudioClip(true);
				_audio.Break.transform.parent = _lamp.transform;
				_audio.Break.spatialBlend = 1.0f;
				_audio.Break.maxDistance = 30.0f;
				_load = null;
				ModConsole.Print("Loaded Break");
			}
			_audio.HasLoaded = true;
		}

		/*private void _initAudio()
		{
			try
			{
				WWW load = new WWW("file:///" + Path.Combine(ModLoader.GetModAssetsFolder(this), _audio.SwitchOnFile));
				while (!load.isDone) ;
				_audio.SwitchOn = _lamp.AddComponent<AudioSource>();
				_audio.SwitchOn.clip = load.GetAudioClip(true);
				_audio.SwitchOn.transform.parent = _lamp.transform;
				_audio.SwitchOn.spatialBlend = 1.0f;
				_audio.SwitchOn.maxDistance = 5.0f;

				load = new WWW("file:///" + Path.Combine(ModLoader.GetModAssetsFolder(this), _audio.SwitchOffFile));
				while (!load.isDone) ;
				_audio.SwitchOff = _lamp.AddComponent<AudioSource>();
				_audio.SwitchOff.clip = load.GetAudioClip(true);
				_audio.SwitchOff.transform.parent = _lamp.transform;
				_audio.SwitchOff.spatialBlend = 1.0f;
				_audio.SwitchOff.maxDistance = 5.0f;

				load = new WWW("file:///" + Path.Combine(ModLoader.GetModAssetsFolder(this), _audio.HumLoopFile));
				while (!load.isDone) ;
				_audio.HumLoop = _lamp.AddComponent<AudioSource>();
				_audio.HumLoop.clip = load.GetAudioClip(true);
				_audio.HumLoop.loop = true;
				_audio.HumLoop.volume = 0.3f;
				_audio.HumLoop.transform.parent = _lamp.transform;
				_audio.HumLoop.spatialBlend = 1.0f;
				_audio.HumLoop.maxDistance = 5.0f;

				load = new WWW("file:///" + Path.Combine(ModLoader.GetModAssetsFolder(this), _audio.DisconnectFile));
				while (!load.isDone) ;
				_audio.Disconnect = _lamp.AddComponent<AudioSource>();
				_audio.Disconnect.clip = load.GetAudioClip(true);
				_audio.Disconnect.transform.parent = _lamp.transform;
				_audio.Disconnect.spatialBlend = 1.0f;
				_audio.Disconnect.maxDistance = 10.0f;

				load = new WWW("file:///" + Path.Combine(ModLoader.GetModAssetsFolder(this), _audio.BreakFile));
				while (!load.isDone) ;
				_audio.Break = _lamp.AddComponent<AudioSource>();
				_audio.Break.clip = load.GetAudioClip(true);
				_audio.Break.transform.parent = _lamp.transform;
				_audio.Break.spatialBlend = 1.0f;
				_audio.Break.maxDistance = 30.0f;
			}
			catch (Exception ex)
			{
				ModConsole.Error(ex.ToString());
			}
		}*/

		public override void OnLoad()
		{
			// Command
			ConsoleCommand.Add(new FloodlightCommand(this));
			ModConsole.Print("loading floodlight...");

			// Load save
			SaveData saveData;
			try
			{
				saveData = SaveData.Deserialize<SaveData>(_savePath);
			}
			catch
			{
				saveData = new SaveData()
				{
					Pos = _defaultPos,
					Rot = new Quaternion(),
					Pitch = 0.0f,
					On = false,
					BulbHealth = 60
				};
			}


			// Light
			_initLightWithBundle(saveData);

			// Shop
			_initShop();

			// Initialize
			_pitch = saveData.Pitch;
			_bulbHealth = saveData.BulbHealth;
			_flickerTimer = UnityEngine.Random.Range(20.0f, 60.0f);
			_flickerLength = UnityEngine.Random.Range(1.0f, 3.0f);

			_switchLight(saveData.On);
			_lamp.transform.localRotation = Quaternion.Euler(_pitch, 0.0f, 0.0f);
		}

		public override void OnSave()
		{
			SaveData.Serialize(new SaveData(this), _savePath);
		}

		public override void ModSettings()
		{
			Settings.AddCheckBox(this, UseBattery);
			Settings.AddCheckBox(this, EnableFlicker);
			Settings.AddCheckBox(this, Unbreakable);
		}

		public override void Update()
		{
			//// Load audio
			//if (!_audio.HasLoaded)
			//{
			//	try
			//	{
			//		_initAudioAsync();
			//	}
			//	catch (Exception ex)
			//	{
			//		ModConsole.Error(ex.ToString());
			//	}
			//}

			// Get inputs
			float scroll = Input.GetAxis("Mouse ScrollWheel");
			bool use = cInput.GetButtonDown("Use");

			// Raycast
			RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 1.0f);
			for (int i = 0; i < hits.Length; ++i)
			{
				if (hits[i].collider == _lampCollider)
				{
					PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIuse").Value = true;
					PlayMakerGlobals.Instance.Variables.FindFsmString("GUIinteraction").Value = _on ? "light on" : "light off";
					if (use)
					{
						// Toggle the light
						_switchLight(!_on);
						if (_on)
						{
							FloodlightAudio.Play(_audio.SwitchOn);
						}
						else
						{
							FloodlightAudio.Play(_audio.SwitchOff);
							_battery = null;
							_batteryCharge = null;
						}
					}

					if (scroll > 0)
					{
						// Pitch up
						_pitch -= 5.0f;
						if (_pitch < -60.0f)
							_pitch = -60.0f;
						_lamp.transform.localEulerAngles = new Vector3(_pitch, 0.0f, 0.0f);
					}
					if (scroll < 0)
					{
						// Pitch down
						_pitch += 5.0f;
						if (_pitch > 10.0f)
							_pitch = 10.0f;
						_lamp.transform.localEulerAngles = new Vector3(_pitch, 0.0f, 0.0f);
					}

					break;
				}
				else if (hits[i].collider == _boxColliderTeimo || hits[i].collider == _boxColliderFleetari)
				{
					PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIbuy").Value = true;
					PlayMakerGlobals.Instance.Variables.FindFsmString("GUIinteraction").Value = _bulbText;
					if (Input.GetKeyDown(KeyCode.Mouse0))
					{
						if (_bulbHealth <= 0)
						{
							FsmFloat wealth = PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerMoney");
							if (wealth.Value >= _bulbCost)
							{
								wealth.Value -= _bulbCost;
								hits[i].collider.transform.gameObject.GetComponent<AudioSource>().Play();
								_bulbHealth = UnityEngine.Random.Range(60, 100);
								ModConsole.Print($"Floodlight: lightbulb replaced, {_bulbHealth} health");
							}
							else
							{
								ModConsole.Print($"Floodlight: not enough money ({wealth.Value.ToString("0")} wealth, {_bulbCost.ToString("0")} cost)");
							}
						}
						else
						{
							ModConsole.Print($"Floodlight: lightbulb is not broken ({_bulbHealth} health)");
						}
					}

					break;
				}
			}

			// Conditions to switch off
			if (_on)
			{
				if (!_checkBattery(true))
					_switchLight(false);
				if (_bulbHealth <= 0)
				{
					if (_bulbHealth == 0)
						FloodlightAudio.Play(_audio.Break);
					--_bulbHealth;
					_switchLight(false);
				}
			}
		}
	}
}
