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
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using MSCLoader;

namespace Floodlight
{
	public class FloodlightSaveData
	{
		public Vector3 Pos;
		public Quaternion Rot;
		public bool On;
		public float Pitch;
		public int Health;
		public List<Vector3> BulbPos;
		public List<Quaternion> BulbRot;

		public FloodlightSaveData()
		{
			Pos = Floodlight.DefaultPos;
			Rot = new Quaternion();
			On = false;
			Pitch = 0.0f;
			Health = 1;
			BulbPos = new List<Vector3>();
			BulbRot = new List<Quaternion>();
		}

		public FloodlightSaveData(Vector3 pos, Quaternion rot, bool on, float pitch, int health, List<Vector3> bulbPos, List<Quaternion> bulbRot)
		{
			this.Pos = pos;
			this.Rot = rot;
			this.On = on;
			this.Pitch = pitch;
			this.Health = health;
			this.BulbPos = bulbPos;
			this.BulbRot = bulbRot;
		}

		public static string SavePath;

		public static void Serialize<T>(T data, string path)
		{
			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(T));
				XmlWriter writer = XmlWriter.Create(new StreamWriter(path));
				serializer.Serialize(writer, data);
				writer.Close();
			}
			catch (Exception ex)
			{
				ModConsole.Error(ex.ToString());
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
			catch { }
			return new T();
		}
	}
}