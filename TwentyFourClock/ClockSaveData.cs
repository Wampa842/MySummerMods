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
using UnityEngine;
using MSCLoader;

namespace TwentyFourClock
{
	public class ClockSaveData
	{
		public Vector3 Position;
		public Quaternion Rotation;
		public bool AlarmEnabled;
		public int AlarmHour;
		public int AlarmMinute;
		public Color DisplayColor;

		public ClockSaveData()
		{
			Position = new Vector3(-1.862581f, 0.627786f, 8.647138f);
			Rotation = Quaternion.Euler(0.0f, 270.0f, 0.0f);
			AlarmEnabled = false;
			AlarmHour = 10;
			AlarmMinute = 0;
			DisplayColor = Color.red;
		}

		public static void Serialize(ClockSaveData data, string path)
		{
			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(ClockSaveData));
				XmlWriter writer = XmlWriter.Create(new StreamWriter(path));
				serializer.Serialize(writer, data);
				writer.Close();
			}
			catch (Exception ex)
			{
				ModConsole.Error(ex.ToString());
			}
		}

		public static ClockSaveData Deserialize(string path)
		{
			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(ClockSaveData));
				XmlReader reader = XmlReader.Create(new StreamReader(path));
				return (ClockSaveData)serializer.Deserialize(reader);
			}
			catch (Exception ex)
			{
				if (!(ex is FileNotFoundException || ex is System.IO.IsolatedStorage.IsolatedStorageException))
					ModConsole.Error(ex.ToString());
			}
			return new ClockSaveData();
		}
	}
}