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
using System.Collections.Generic;
using System.IO;
using System.Linq;

using MSCLoader;
using UnityEngine;

namespace LightSwitchMarkers
{
	public class LightSwitchMarkers : Mod
	{
		public override string ID => "LightSwitchMarkers";
		public override string Name => "Light switch markers";
		public override string Author => "Wampa842";
		public override string Version => "1.0.0";
		public override bool UseAssetsFolder => false;
		public static Vector3 Offset => new Vector3(0.0f, 0.02f, 0.05f);
		public static Vector3 Size => new Vector3(0.05f, 0.01f, 0.005f);
		public static Color Emissive => new Color(0.0f, 1.0f, 0.5f);

		public override void OnLoad()
		{
			// Base object
			GameObject orig = GameObject.CreatePrimitive(PrimitiveType.Cube);
			orig.name = "light_switch_marker";
			Material mat = new Material(Shader.Find("Standard"));
			mat.color = Color.black;
			mat.SetColor("_EmissionColor", Emissive);
			mat.EnableKeyword("_EMISSION");
			orig.GetComponent<Renderer>().material = mat;

			// Clones
			Transform parent = GameObject.Find("LightSwitches").transform;
			int count = parent.childCount;

			for (int i = 0; i < count; ++i)
			{
				Transform p = parent.GetChild(i);
				GameObject clone = GameObject.Instantiate<GameObject>(orig);
				clone.transform.parent = p;
				clone.transform.localPosition = Offset;
				clone.transform.localScale = Size;
			}

			GameObject.Destroy(orig);
		}
	}
}
