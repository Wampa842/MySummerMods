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
using UnityEngine;
using MSCLoader;
using System.Collections.Generic;

namespace RailRoadCrossing
{
	public abstract class CrossingTriggerBehaviour : MonoBehaviour
	{
		public GameObject[] Signs { get; set; }
		public static RailRoadCrossing Mod;

		public void Show(bool enable)
		{
			gameObject.GetComponent<Renderer>().enabled = enable;
		}

		public virtual void Awake()
		{
			Renderer r = gameObject.GetComponent<Renderer>();

			Material mat = new Material(Shader.Find("Standard"));
			mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
			mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			mat.SetInt("_ZWrite", 0);
			mat.DisableKeyword("_ALPHATEST_ON");
			mat.DisableKeyword("_ALPHABLEND_ON");
			mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
			mat.renderQueue = 3000;
			mat.color = new Color(1.0f, 0.0f, 0.0f, 0.3f);
			r.material = mat;

			r.enabled = false;
		}
	}

	public class CrossingExitTriggerBehaviour : CrossingTriggerBehaviour
	{
		public CrossingExitTriggerBehaviour()
		{
			Signs = new GameObject[2];
		}
		public override void Awake()
		{
			base.Awake();
			gameObject.GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 0.3f);
		}
		void OnTriggerExit(Collider coll)
		{
			if (coll.gameObject.name == "train")
			{
				if ((bool)Mod.Verbose.GetValue()) ModConsole.Print("[Train] exits " + gameObject.name);
				foreach (var o in Signs)
				{
					o.GetComponent<CrossingBehaviour>().Raise();
				}
			}
		}
	}

	public class CrossingEnterTriggerBehaviour : CrossingTriggerBehaviour
	{
		public CrossingEnterTriggerBehaviour()
		{
			Signs = new GameObject[2];
		}
		public override void Awake()
		{
			base.Awake();
			gameObject.GetComponent<Renderer>().material.color = new Color(0.0f, 1.0f, 0.0f, 0.3f);
		}
		void OnTriggerEnter(Collider coll)
		{
			if (coll.gameObject.name == "train")
			{
				if ((bool)Mod.Verbose.GetValue()) ModConsole.Print("[Train] enters " + gameObject.name);
				foreach (var o in Signs)
				{
					o.GetComponent<CrossingBehaviour>().Lower();
				}
			}
		}
	}
}
