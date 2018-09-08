using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using MSCLoader;
using UnityEngine;

namespace CarryMore
{
	public class MySettings
	{
		public static SettingsData Settings { get; private set; }
		public static SettingsData Temporary { get; set; }

		public static void Save(string path)
		{
			SettingsData.Serialize(Settings, path);
		}

		public static void Load(string path)
		{
			Settings = SettingsData.Deserialize(path);
		}

		public class SettingsData : ICloneable
		{
			public bool LogAll;
			public bool LogSome;
			public bool DropIfOpen;
			public bool AddIfOpen;
			public int MaxItems;

			public SettingsData()
			{
				LogAll = false; ;
				LogSome = false; ;
				DropIfOpen = true; ;
				AddIfOpen = false; ;
				MaxItems = 10;
			}

			public static void Serialize(SettingsData data, string path)
			{
				try
				{
					XmlSerializer serializer = new XmlSerializer(typeof(SettingsData));
					StreamWriter stream = new StreamWriter(path);
					XmlWriter writer = XmlWriter.Create(stream);
					serializer.Serialize(writer, data);
					writer.Close();
					stream.Close();
				}
				catch (Exception ex)
				{
					ModConsole.Error(ex.ToString());
				}
			}

			public static SettingsData Deserialize(string path)
			{
				try
				{
					XmlSerializer serializer = new XmlSerializer(typeof(SettingsData));
					StreamReader stream = new StreamReader(path);
					XmlReader reader = XmlReader.Create(stream);
					SettingsData o = serializer.Deserialize(reader) as SettingsData;
					reader.Close();
					stream.Close();
					return o;
				}
				catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException || ex is System.IO.IsolatedStorage.IsolatedStorageException)
				{
					return new SettingsData();
				}
				catch (Exception ex)
				{
					ModConsole.Error(ex.ToString());
				}
				return new SettingsData();
			}

			public object Clone()
			{
				return new SettingsData
				{
					AddIfOpen = this.AddIfOpen,
					DropIfOpen = this.DropIfOpen,
					LogAll = this.LogAll,
					LogSome = this.LogSome,
					MaxItems = this.MaxItems
				};
			}

			public void CloneFrom(SettingsData other)
			{
				this.AddIfOpen = other.AddIfOpen;
				this.DropIfOpen = other.DropIfOpen;
				this.LogAll = other.LogAll;
				this.LogSome = other.LogSome;
				this.MaxItems = other.MaxItems;
			}
		}

		public static bool GuiVisible { get; set; } = false;
		public static Rect GuiArea { get; } = new Rect(Screen.width / 2 - 200, 225, 400, 285);
		public static Rect GuiBackground { get; } = new Rect(Screen.width / 2 - 205, 200, 410, 290);
	}
}
