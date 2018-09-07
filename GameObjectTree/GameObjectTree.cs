using MSCLoader;
using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;

namespace GameObjectTree
{
	public static class XmlifyExtensions
	{
		// Vector3
		public static XmlElement Xmlify(this Vector3 v, XmlDocument doc)
		{
			XmlElement e = doc.CreateElement("Vector3");

			e.AppendChild(doc.CreateElement("X")).AppendChild(doc.CreateTextNode(v.x.ToString("0.00")));
			e.AppendChild(doc.CreateElement("Y")).AppendChild(doc.CreateTextNode(v.y.ToString("0.00")));
			e.AppendChild(doc.CreateElement("Z")).AppendChild(doc.CreateTextNode(v.z.ToString("0.00")));

			return e;
		}

		// Quaternion
		public static XmlElement Xmlify(this Quaternion q, XmlDocument doc)
		{
			return q.eulerAngles.Xmlify(doc);
		}

		// Component
		public static XmlElement Xmlify(this Component c, XmlDocument doc)
		{
			XmlElement e = doc.CreateElement("Component");
			e.SetAttribute("type", c.GetType().ToString());
			e.AppendChild(doc.CreateElement("Name")).AppendChild(doc.CreateTextNode(c.name));
			return e;
		}

		// Transform
		public static XmlElement Xmlify(this Transform t, XmlDocument doc)
		{
			XmlElement e = doc.CreateElement("Transform");

			e.AppendChild(doc.CreateElement("Position")).AppendChild(t.position.Xmlify(doc));
			e.AppendChild(doc.CreateElement("Rotation")).AppendChild(t.position.Xmlify(doc));
			e.AppendChild(doc.CreateElement("LocalPosition")).AppendChild(t.position.Xmlify(doc));
			e.AppendChild(doc.CreateElement("LocalRotation")).AppendChild(t.position.Xmlify(doc));
			e.AppendChild(doc.CreateElement("LocalScale")).AppendChild(t.position.Xmlify(doc));

			return e;
		}


		// Collider
		public static XmlElement Xmlify(this Collider coll, XmlDocument doc)
		{
			XmlElement e = doc.CreateElement("Collider");
			e.SetAttribute("type", coll.GetType().ToString());

			e.AppendChild(doc.CreateElement("IsTrigger")).AppendChild(doc.CreateTextNode(coll.isTrigger.ToString()));
			if (coll is MeshCollider)
				e.AppendChild(doc.CreateElement("IsTrigger")).AppendChild(doc.CreateTextNode(((MeshCollider)coll).convex.ToString()));

			return e;
		}

		// Rigidbody
		public static XmlElement Xmlify(this Rigidbody rb, XmlDocument doc)
		{
			XmlElement e = doc.CreateElement("Rigidbody");

			e.AppendChild(doc.CreateElement("Kinematic")).AppendChild(doc.CreateTextNode(rb.isKinematic.ToString()));
			e.AppendChild(doc.CreateElement("Mass")).AppendChild(doc.CreateTextNode(rb.mass.ToString()));

			return e;
		}

		// AudioSource
		public static XmlElement Xmlify(this AudioSource audio, XmlDocument doc)
		{
			XmlElement e = doc.CreateElement("AudioSource");

			e.AppendChild(doc.CreateElement("ClipName")).AppendChild(doc.CreateTextNode(audio.clip.name));
			e.AppendChild(doc.CreateElement("SpatialBlend")).AppendChild(doc.CreateTextNode(audio.spatialBlend.ToString()));
			e.AppendChild(doc.CreateElement("Volume")).AppendChild(doc.CreateTextNode(audio.volume.ToString()));
			e.AppendChild(doc.CreateElement("Loop")).AppendChild(doc.CreateTextNode(audio.loop.ToString()));

			return e;
		}
	}

	public class GameObjectTree : Mod
	{
		public override string ID => "GameObjectTree";
		public override string Name => "GameObjectTree";
		public override string Author => "Wampa842";
		public override string Version => "1.0.0";
		public override bool UseAssetsFolder => false;
		public string FilePath => Path.Combine(ModLoader.GetModConfigFolder(this), "hierarchy.xml");
		public string BlacklistPath => Path.Combine(ModLoader.GetModConfigFolder(this), "blacklist.txt");

		public Settings WriteButton, EditBlacklistButton;
		//public Settings WriteComponents, WriteTransforms, WriteFsm, WriteCollider, WriteRigidbody, WriteAudioSource, WriteOther;
		//public Settings MaxDepth;
		public List<string> Blacklist;


		// FSM
		public static XmlElement ReadFsm(PlayMakerFSM fsm, XmlDocument doc)
		{
			XmlElement e = doc.CreateElement("Fsm");
			XmlElement n;

			e.AppendChild(doc.CreateElement("Name")).AppendChild(doc.CreateTextNode(fsm.FsmName));

			if (fsm.FsmVariables.BoolVariables.Length > 0)
			{
				n = doc.CreateElement("FsmBool");
				foreach (FsmBool v in fsm.FsmVariables.BoolVariables)
				{
					n.AppendChild(doc.CreateElement(v.Name)).AppendChild(doc.CreateTextNode(v.Value.ToString()));
				}
				e.AppendChild(n);
			}
			if (fsm.FsmVariables.IntVariables.Length > 0)
			{
				n = doc.CreateElement("FsmInt");
				foreach (var v in fsm.FsmVariables.IntVariables)
				{
					n.AppendChild(doc.CreateElement(v.Name)).AppendChild(doc.CreateTextNode(v.Value.ToString()));
				}
				e.AppendChild(n);
			}
			if (fsm.FsmVariables.FloatVariables.Length > 0)
			{
				n = doc.CreateElement("FsmFloat");
				foreach (var v in fsm.FsmVariables.FloatVariables)
				{
					n.AppendChild(doc.CreateElement(v.Name)).AppendChild(doc.CreateTextNode(v.Value.ToString()));
				}
				e.AppendChild(n);
			}
			if (fsm.FsmVariables.StringVariables.Length > 0)
			{
				n = doc.CreateElement("FsmString");
				foreach (var v in fsm.FsmVariables.BoolVariables)
				{
					n.AppendChild(doc.CreateElement(v.Name)).AppendChild(doc.CreateTextNode(v.Value.ToString()));
				}
				e.AppendChild(n);
			}

			return e;
		}

		public XmlElement ReadComponent(XmlDocument doc, Component c)
		{
			XmlElement e = doc.CreateElement("Component");
			e.SetAttribute("type", c.GetType().ToString());
			if (!string.IsNullOrEmpty(c.name))
			{
				XmlElement name = doc.CreateElement("Name");
				name.AppendChild(doc.CreateTextNode(c.name));
				e.AppendChild(name);
			}

			if (c is PlayMakerFSM)
			{
				e.AppendChild(ReadFsm((PlayMakerFSM)c, doc));
			}
			//else if (c is Collider)
			//{
			//	e.AppendChild(((Collider)c).Xmlify(doc));
			//}
			//else if (c is Rigidbody)
			//{
			//	e.AppendChild(((Rigidbody)c).Xmlify(doc));
			//}
			//else if (c is AudioSource)
			//{
			//	e.AppendChild(((AudioSource)c).Xmlify(doc));
			//}
			//else
			//{
			//	e.AppendChild(c.Xmlify(doc));
			//}

			return e;
		}

		public XmlElement ReadGameObject(XmlDocument doc, GameObject o, int depth)
		{
			XmlElement e = doc.CreateElement("GameObject");

			// Object properties
			XmlElement name = doc.CreateElement("Name");
			name.AppendChild(doc.CreateTextNode(o.name));
			e.AppendChild(name);

			// Components
			Component[] components = o.GetComponents<Component>().Where(c => !(c is Transform)).ToArray();
			if (components != null && components.Length > 0)
			{
				XmlElement ce = doc.CreateElement("Components");
				int count = 0;
				//foreach (Component c in components)
				//{
				//	//if (c is Transform && !(bool)WriteTransforms.GetValue())
				//	//	continue;
				//	//if (c is PlayMakerFSM && !(bool)WriteFsm.GetValue())
				//	//	continue;
				//	if (c is PlayMakerFSM)
				//	{
				//		ce.AppendChild(ReadComponent(doc, c));
				//		++count;
				//	}
				//}
				ce.SetAttribute("count", count.ToString());
				e.AppendChild(ce);
			}

			// Child objects
			if (o.transform.childCount > 0)
			{
				XmlElement ce = doc.CreateElement("Children");
				ce.SetAttribute("count", o.transform.childCount.ToString());
				for (int i = 0; i < o.transform.childCount; ++i)
				{
					GameObject child = o.transform.GetChild(i).gameObject;
					ce.AppendChild(ReadGameObject(doc, child, depth + 1));
				}
				e.AppendChild(ce);
			}

			return e;
		}

		public void WriteAllGameObjects()
		{
			ModConsole.Print("[GOT] start");
			XmlDocument doc = new XmlDocument();
			XmlElement root = doc.CreateElement("MSCGameObjects");
			foreach (GameObject o in GameObject.FindObjectsOfType<GameObject>().Where(e => e.transform.parent == null))
			{
				root.AppendChild(ReadGameObject(doc, o, 0));
			}
			doc.AppendChild(root);
			doc.Save(FilePath);
			ModConsole.Print("[GOT] end");
		}

		public GameObjectTree()
		{
			WriteButton = new Settings("write", "Write to file", () => { WriteAllGameObjects(); });
			EditBlacklistButton = new Settings("blacklist", "Edit blacklist", () => { System.Diagnostics.Process.Start(BlacklistPath); });

			//WriteComponents = new Settings("components", "Write components", true);
			//WriteTransforms = new Settings("transforms", "Write transforms", true);
			//WriteFsm = new Settings("fsm", "Write PlayMaker components", true);
			//WriteCollider = new Settings("colliders", "Write colliders", true);
			//WriteRigidbody = new Settings("rigidbodies", "Write rigid bodies", true);
			//WriteAudioSource = new Settings("audiosources", "Write audio sources", true);
			//WriteOther = new Settings("others", "Write other components", true);
			//MaxDepth = new Settings("depth", "Hierarchy depth (0 for infinite)", (int)0);
		}

		public override void ModSettings()
		{
			Settings.AddButton(this, WriteButton);
		}
	}
}
