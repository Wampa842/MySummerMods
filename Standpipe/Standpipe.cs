using System;
using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

namespace Standpipe
{
	public class Standpipe : Mod
	{
		public override string ID => "Standpipe";
		public override string Name => "Standpipes";
		public override string Author => "Wampa842";
		public override string Version => "1.0.0";
		public override bool UseAssetsFolder => true;

		// Global positions
		private readonly Vector3[] _positions = new Vector3[]
		{
			new Vector3(51, -1.46f, -71.5f),	// Home
			new Vector3(-849.5f, -3.16f, 502),	// Island
			new Vector3(-1532.5f, 2.96f, 1185),	// Peräjärvi
			new Vector3(-1217, 0.33f, -621),	// Strawberry patch
			new Vector3(1540, 4.44f, 711),		// Loppe
			new Vector3(463.3f, 9.32f, 1320.5f)	// Dance pavilion
		};

		// Bearing angles
		private readonly float[] _rotations = new float[]
		{
			0.0f,
			240.0f,
			150.0f,
			40.0f,
			65.0f,
			90.0f
		};

		public override void OnLoad()
		{
			// Load the assets and create the standpipe prefab
			AssetBundle ab = LoadAssets.LoadBundle(this, "standpipe.unity3d");
			GameObject o = ab.LoadAsset<GameObject>("standpipe.prefab");
			GameObject prefab = GameObject.Instantiate<GameObject>(o);
			prefab.name = "standpipe";

			// Duplicate the tap
			GameObject tap = GameObject.Instantiate<GameObject>(GameObject.Find("KitchenWaterTap"));
			tap.transform.SetParent(prefab.transform);

			// Destroy base mesh
			GameObject.DestroyImmediate(tap.transform.Find("kitchen_tap_base").gameObject);

			// Set up lever
			Transform leverMesh = prefab.transform.Find("standpipe_lever");
			Transform leverParent = tap.transform.Find("Handle");
			Vector3 handlePos = leverMesh.localPosition;
			leverParent.SetParent(prefab.transform, false);
			leverParent.localPosition = handlePos;
			leverParent.localRotation = new Quaternion();
			leverMesh.SetParent(leverParent.Find("Pivot"));
			leverMesh.localPosition = new Vector3();

			// Set up trigger
			Transform trig = tap.transform.Find("Trigger");
			trig.SetParent(prefab.transform);
			trig.localPosition = new Vector3(0.0f, 0.892f, 0.035f);
			SphereCollider coll = trig.GetComponent<SphereCollider>();
			coll.radius = 0.15f;
			
			// Set up particles
			Transform particle = tap.transform.Find("Particle");
			particle.SetParent(prefab.transform, false);
			particle.localPosition = new Vector3(0.0f, 0.7887f, 0.215f);

			// Set up component
			prefab.AddComponent<StandpipeBehaviour>();
			particle.GetComponent<AudioSource>().clip = ab.LoadAsset<AudioClip>("assets/audio/waterflow.ogg");

			// Carve out the remains
			GameObject.DestroyImmediate(leverParent.Find("Pivot/kitchen_tap_handle").gameObject);
			GameObject.DestroyImmediate(tap);

			// Set up multiple standpipes
			GameObject parent = new GameObject("STANDPIPES");
			for (int i = 0; i < _positions.Length; ++i)
			{
				GameObject instance = GameObject.Instantiate<GameObject>(prefab);
				instance.transform.SetParent(parent.transform);
				instance.transform.position = _positions[i];
				instance.transform.rotation = Quaternion.Euler(0.0f, _rotations[i], 0.0f);
			}

			// Destroy the prefabs and unload the bundle
			GameObject.Destroy(prefab);
			GameObject.Destroy(o);
			ab.Unload(false);
		}
	}
}
