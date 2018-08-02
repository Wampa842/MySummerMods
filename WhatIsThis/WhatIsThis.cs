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
using MSCLoader;
using UnityEngine;

namespace WhatIsThis
{
	public class WhatIsThis : Mod
	{
		public override string ID => "WhatIsThis";
		public override string Name => "What is this?";
		public override string Version => "1.0.0";
		public override string Author => "Wampa842";

		private Keybind Check;

		public WhatIsThis()
		{
			Check = new Keybind("CheckItem", "Check targeted item", KeyCode.E, KeyCode.LeftControl);
		}

		public override void OnLoad()
		{
			Keybind.Add(this, Check);
		}

		public override void Update()
		{
			if(Application.loadedLevelName == "GAME")
			{
				if(Check.IsDown())
				{
					RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 1.0f);
					ModConsole.Print("\n--- What is this? ---");
					for(int i = 0; i < hits.Length; ++i)
					{
						GameObject o = hits[i].transform.gameObject;
						ModConsole.Print($"{o.name}, {o.layer}, {o.tag}, {o.transform.parent.name}");
					}
				}
			}
		}
	}
}
