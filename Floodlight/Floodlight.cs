using System;
using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using System.Collections.Generic;

namespace Floodlight
{
	public class Floodlight : Mod
	{
		public class SaveData
		{
			public float PosX, PosY, PosZ, RotX, RotY, RotZ, RotW, Pitch;
			public bool On, Broken;
			public Vector3 Pos
			{
				get
				{
					return new Vector3(PosX, PosY, PosZ);
				}
				set
				{
					PosX = value.x;
					PosY = value.y;
					PosZ = value.z;
				}
			}
			public Quaternion Rot
			{
				get
				{
					return new Quaternion(RotX, RotY, RotZ, RotW);
				}
				set
				{
					RotX = value.x;
					RotY = value.y;
					RotZ = value.z;
					RotW = value.w;
				}
			}
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
					//ModConsole.Error(ex.ToString());
					throw;
				}
				return new T();
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
		}

		public override string ID => "Floodlight";
		public override string Name => "Floodlight";
		public override string Author => "Wampa842";
		public override string Version => "1.0.0-RC1";
		public override bool UseAssetsFolder => true;

		public Settings UseBattery;         // Require charged battery
		public Settings EnableFlicker;      // Enable periodic flickering (epileptics may want to disable it)

		private readonly string _savePath;  // Path to floodlight.xml

		private readonly Vector3 _defaultPos = new Vector3(-16.2f, 4.0f, 11.0f);    // Position in case the save file can't be loaded
		private Color _lightColor = new Color(0.9f, 0.9f, 1.0f);    // The color of the light and the emissive color of the glass
		private bool _on = false;                                   // Whether the light is currently on
		private float _pitch = 0.0f;                                // The lamp's angle
		private const float _baseIntensity = 2.0f;                  // The light's maximal intensity
		private float _intensity = 1.0f;                            // Light intensity multiplier
		private const float _dimStartCharge = 50.0f;                // Charge level at which the light starts to dim
		private const float _turnOffCharge = 30.0f;                 // Charge level at which the light turns off
		private const float _dischargeRate = 0.1f;					// Battery energy consumption per second

		private float _flickerTimer;                // The time in seconds until the next flicker event
		private float _flickerMultiplier = 1.0f;    // Flicker dimming
		private float _flickerLength;               // The length of the flickering

		private GameObject _base;           // The lamp stand that provides the root rigidbody and collider
		private GameObject _lamp;           // The tiltable lamp
		private GameObject _glass;          // Luminous glass
		private GameObject _switch;         // Indicator light
		private GameObject _battery;        // Connected battery
		private FsmFloat _batteryCharge;    // The charge level of the connected battery
		private MeshCollider _lampCollider; // The collider of the tiltable lamp that is detected by raycast
		private Material _glassMaterial;    // Luminous white textured
		private Material _switchMaterial;   // Luminous red
		private Light _light;               // The actual light emitter

		private FloodlightAudio _audio;     // Audio sources

		public Floodlight()
		{
			_savePath = Path.Combine(Application.persistentDataPath, "floodlight.xml");
			_audio = new FloodlightAudio();
			UseBattery = new Settings("UseBattery", "Consume battery power", true);
			EnableFlicker = new Settings("EnableFlicker", "Periodic flickering (EPILEPSY WARNING)", true);
		}

		private void _flicker(float time)
		{
			_flickerTimer -= Time.deltaTime;
			bool isFlickering = time < _flickerLength && time >= 0.0f;
			if (isFlickering)
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
				ModConsole.Print($"Floodlight: selected new flicker timing: {_flickerTimer.ToString("0.0")} delay, {_flickerLength.ToString("0.0")} length");
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
						ModConsole.Print("Floodlight: battery found");
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
					_audio.Disconnect.Play();
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
				_audio.HumLoop.Play();
			}
			else
			{
				_audio.HumLoop.Stop();
			}
		}

		public override void OnLoad()
		{
			ModConsole.Print("loading floodlight...");

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
					On = false
				};
			}

			_pitch = saveData.Pitch;
			_on = saveData.On;

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
			_light.range = 15.0f;
			_light.spotAngle = 75.0f;
			_light.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
			_light.renderMode = LightRenderMode.ForceVertex;
			_light.enabled = false;

			// Sounds
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
			}
			catch (Exception ex)
			{
				ModConsole.Error(ex.ToString());
			}
			ModConsole.Print("floodlight loaded");

			_switchLight(false);
		}

		public override void OnSave()
		{
			SaveData.Serialize(new SaveData()
			{
				Rot = _base.transform.rotation,
				Pos = _base.transform.position
			}, _savePath);
		}

		public override void ModSettings()
		{
			Settings.AddCheckBox(this, UseBattery);
			Settings.AddCheckBox(this, EnableFlicker);
		}

		public override void Update()
		{
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
							_audio.SwitchOn.Play();
						}
						else
						{
							_audio.SwitchOff.Play();
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
				}
				else
				{
					PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIuse").Value = false;
				}
			}

			// Battery
			if (_on)
			{
				if (!_checkBattery(true))
					_switchLight(false);
			}
		}
	}
}

