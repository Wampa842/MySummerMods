using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using MSCLoader;

namespace CarryMore
{
	public class MySettings
	{
		public static SettingsData Settings { get; private set; }

		public static void Save(string path)
		{
			SettingsData.Serialize(Settings, path);
		}

		public static void Load(string path)
		{
			Settings = SettingsData.Deserialize(path);
		}

		public class SettingsData
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
				catch(IOException ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
				{
					return new SettingsData();
				}
				catch(Exception ex)
				{
					ModConsole.Error(ex.ToString());
				}
				return new SettingsData();
			}
		}
	}
}
