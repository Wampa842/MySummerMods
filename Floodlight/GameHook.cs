/*MIT License

Copyright(c) 2017 zamp

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using HutongGames.PlayMaker;

namespace Floodlight
{
	public class GameHook
	{
		private class FsmHookAction : FsmStateAction
		{
			public Action hook;
			public override void OnEnter()
			{
				hook.Invoke();
				Finish();
			}
		}

		public static void InjectStateHook(GameObject gameObject, string stateName, Action hook)
		{
			var state = GetStateFromGameObject(gameObject, stateName);
			if (state != null)
			{
				// inject our hook action to the state machine
				var actions = new List<FsmStateAction>(state.Actions);
				var hookAction = new FsmHookAction();
				hookAction.hook = hook;
				actions.Insert(0, hookAction);
				state.Actions = actions.ToArray();
			}
		}

		private static FsmState GetStateFromGameObject(GameObject obj, string stateName)
		{
			var comps = obj.GetComponents<PlayMakerFSM>();
			foreach (var playMakerFsm in comps)
			{
				var state = playMakerFsm.FsmStates.FirstOrDefault(x => x.Name == stateName);
				if (state != null)
					return state;
			}
			return null;
		}
	}
}