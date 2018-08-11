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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using MSCLoader;
using HutongGames.PlayMaker;

namespace Floodlight
{
	public class LightbulbBoxBehaviour : MonoBehaviour
	{
		public static float Price => 300.0f;
		public static readonly Vector3 TeimoBuyPos = new Vector3(-1551.1f, 4.8f, 1182.8f);

		public List<GameObject> ShopList;

		private bool _bought = false;
		private bool _buying = false;
		private FsmBool _guiBuy;
		private FsmString _guiText;

		public void Activate()
		{
			transform.parent = null;
			gameObject.name = "lightbulb(Clone)";
			gameObject.layer = LayerMask.NameToLayer("Parts");
			gameObject.tag = "PART";
			gameObject.GetComponent<Rigidbody>().isKinematic = false;
		}

		public void SetBought()
		{
			_bought = true;
		}

		private void _buy()
		{
			PlayMakerFSM register = GameObject.Find("STORE/StoreCashRegister/Register").GetComponent<PlayMakerFSM>();
			register.FsmVariables.GetFsmFloat("PriceTotal").Value += Price;
			register.SendEvent("PURCHASE");
			_buying = true;
			gameObject.SetActive(false);
		}

		private void _pay()
		{
			if (_buying)
			{
				// Move to counter
				gameObject.SetActive(true);
				transform.position = TeimoBuyPos;

				// Make interactive and dynamic
				Activate();

				// Set other stuff
				ShopList.Remove(gameObject);
				_buying = false;
				_bought = true;
			}
		}

		void Awake()
		{
			Material m = new Material(Shader.Find("Standard"));
			m.mainTexture = gameObject.GetComponent<Renderer>().material.mainTexture;
			gameObject.GetComponent<Renderer>().material = m;
			_guiBuy = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIbuy");
			_guiText = PlayMakerGlobals.Instance.Variables.FindFsmString("GUIinteraction");
			GameHook.InjectStateHook(GameObject.Find("STORE/StoreCashRegister/Register"), "Purchase", () => { _pay(); });
		}

		void Update()
		{
			bool activate = Input.GetKeyDown(KeyCode.Mouse0);

			Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1.0f);
			if(hit.collider.gameObject == this.gameObject)
			{
				if(!_bought)
				{
					_guiBuy.Value = true;
					_guiText.Value = $"lightbulb ({Price} mk)";
					if (activate)
					{
						_buy();
					}
				}
			}
		}
	}
}