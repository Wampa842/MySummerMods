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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MSCLoader;
using HutongGames.PlayMaker;

namespace RailRoadCrossing
{
	class CrossingBehaviour : MonoBehaviour
	{
		private enum BarrierStatus { Up, Down, Warning, Rising, Lowering };
		private BarrierStatus _status;
		private float _barrierTargetAngle
		{
			get
			{
				return _status == BarrierStatus.Down || _status == BarrierStatus.Lowering ? -90.0f : 0.0f;
			}
		}
		private float _barrierAngle = 0.0f;
		private float _timer = 0.0f;
		private bool _soundEnabled = true;

		private GameObject _barrier;
		private GameObject _sign;

		private Material _signMaterial;
		private Material _baseMaterial;
		private Material _barrierMaterial;
		private Material _lightRedLMaterial;
		private Material _lightRedRMaterial;
		private Material _lightWhiteMaterial;
		private Material _lightBarrierMaterial;

		private AudioSource _bellLoopSound;
		private AudioSource _bellSound;
		private AudioSource _motorSound;

		public void Raise()
		{
			_status = BarrierStatus.Rising;
		}

		public void Lower()
		{
			_status = BarrierStatus.Warning;
			_timer = 0.0f;
		}

		public void UpdateSettings(bool sound, bool barrier)
		{
			_soundEnabled = sound;
			_barrier.SetActive(barrier);
		}

		void Awake()
		{
			// Find components
			_barrier = gameObject.transform.FindChild("railway_barrier").gameObject;
			_sign = gameObject.transform.FindChild("railway_sign").gameObject;

			// Find textures
			Texture baseTex = gameObject.GetComponent<Renderer>().material.mainTexture;
			Texture signTex = _sign.GetComponent<Renderer>().material.mainTexture;
			Texture barrTex = _barrier.GetComponent<Renderer>().material.mainTexture;
			Texture barrEmissiveTex = _barrier.transform.FindChild("barrier_light_1").gameObject.GetComponent<Renderer>().material.GetTexture("_EmissionMap");
			Texture signEmissiveTex = _sign.transform.FindChild("railway_sign_white").gameObject.GetComponent<Renderer>().material.GetTexture("_EmissionMap");

			// Create and assign new materials
			_baseMaterial = new Material(Shader.Find("Standard"));
			_baseMaterial.mainTexture = baseTex;
			gameObject.GetComponent<Renderer>().material = _baseMaterial;

			_signMaterial = new Material(Shader.Find("Standard"));
			_signMaterial.mainTexture = signTex;
			_sign.GetComponent<Renderer>().material = _signMaterial;

			_barrierMaterial = new Material(Shader.Find("Standard"));
			_barrierMaterial.mainTexture = barrTex;
			_barrier.GetComponent<Renderer>().material = _barrierMaterial;

			// Assign emissive materials
			_lightWhiteMaterial = Material.Instantiate<Material>(_signMaterial);
			_lightWhiteMaterial.SetTexture("_EmissionMap", signEmissiveTex);
			_lightWhiteMaterial.SetColor("_EmissionColor", Color.white);
			_lightWhiteMaterial.SetFloat("_EmissionScaleUI", 10.0f);

			_lightRedLMaterial = Material.Instantiate<Material>(_lightWhiteMaterial);
			_lightRedLMaterial.SetColor("_EmissionColor", Color.red);
			_lightRedLMaterial.SetFloat("_EmissionScaleUI", 10.0f);

			_lightRedRMaterial = Material.Instantiate<Material>(_lightRedLMaterial);
			_lightBarrierMaterial = Material.Instantiate<Material>(_barrierMaterial);
			_lightBarrierMaterial.SetTexture("_EmissionMap", barrEmissiveTex);
			_lightBarrierMaterial.SetColor("_EmissionColor", Color.red);
			_lightBarrierMaterial.SetFloat("_EmissionScaleUI", 10.0f);

			_sign.transform.FindChild("railway_sign_left").gameObject.GetComponent<Renderer>().material = _lightRedLMaterial;
			_sign.transform.FindChild("railway_sign_right").gameObject.GetComponent<Renderer>().material = _lightRedRMaterial;
			_sign.transform.FindChild("railway_sign_white").gameObject.GetComponent<Renderer>().material = _lightWhiteMaterial;
			_barrier.transform.FindChild("barrier_light_1").gameObject.GetComponent<Renderer>().material = _lightBarrierMaterial;
			_barrier.transform.FindChild("barrier_light_2").gameObject.GetComponent<Renderer>().material = _lightBarrierMaterial;

			// Find audio sources
			_bellLoopSound = _sign.transform.FindChild("bell_loop_sound").gameObject.GetComponent<AudioSource>();
			_bellSound = _sign.transform.FindChild("bell_sound").gameObject.GetComponent<AudioSource>();
			_motorSound = transform.FindChild("motor_sound").gameObject.GetComponent<AudioSource>();
		}

		void Update()
		{
			// Movement
			if (_status == BarrierStatus.Warning)
			{
				_timer += Time.deltaTime;
				if (_timer >= 3.0f)
				{
					_status = BarrierStatus.Lowering;
				}
			}
			if (_status == BarrierStatus.Rising)
			{
				if (_barrierAngle < _barrierTargetAngle)
				{
					_barrierAngle += 30.0f * Time.deltaTime;
					_barrier.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, _barrierAngle);
				}
				else
				{
					_status = BarrierStatus.Up;
				}
			}
			if (_status == BarrierStatus.Lowering)
			{
				if (_barrierAngle > _barrierTargetAngle)
				{
					_barrierAngle -= 30.0f * Time.deltaTime;
					_barrier.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, _barrierAngle);
				}
				else
				{
					_status = BarrierStatus.Down;
				}
			}

			// Lights
			bool blink = (Time.time % 1.0f) < 0.5f;
			if (_status == BarrierStatus.Up)
			{
				if (blink)
				{
					_lightWhiteMaterial.EnableKeyword("_EMISSION");
				}
				else
				{
					_lightWhiteMaterial.DisableKeyword("_EMISSION");
				}

				_lightRedLMaterial.DisableKeyword("_EMISSION");
				_lightRedRMaterial.DisableKeyword("_EMISSION");
				_lightBarrierMaterial.DisableKeyword("_EMISSION");
			}
			else
			{
				if (blink)
				{
					_lightRedLMaterial.EnableKeyword("_EMISSION");
					_lightRedRMaterial.DisableKeyword("_EMISSION");
					_lightBarrierMaterial.EnableKeyword("_EMISSION");
				}
				else
				{
					_lightRedLMaterial.DisableKeyword("_EMISSION");
					_lightRedRMaterial.EnableKeyword("_EMISSION");
					_lightBarrierMaterial.DisableKeyword("_EMISSION");
				}

				_lightWhiteMaterial.DisableKeyword("_EMISSION");
			}

			// Sounds
			if ((_status == BarrierStatus.Lowering || _status == BarrierStatus.Rising) && _soundEnabled)
			{
				if (!_motorSound.isPlaying)
				{
					_motorSound.Play();
				}
			}
			else if (_motorSound.isPlaying)
			{
				_motorSound.Stop();
			}

			if ((_status == BarrierStatus.Lowering || _status == BarrierStatus.Warning) && !_bellLoopSound.isPlaying && _soundEnabled)
			{
				_bellLoopSound.Play();
			}

			if (!(_status == BarrierStatus.Lowering || _status == BarrierStatus.Warning) && _bellLoopSound.isPlaying)
			{
				_bellLoopSound.Stop();
				if (_soundEnabled)
					_bellSound.Play();
			}
		}
	}
}
