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
using HutongGames.PlayMaker;
using System;
using System.Net;
using System.Threading;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Xml;

namespace AlivieskaGpsServer
{
	public class WebServer
	{
		/*
		The WebServer class is from here: https://codehosting.net/blog/BlogEngine/post/Simple-C-Web-Server

		The MIT License (MIT)

		Copyright (c) 2013 David's Blog (www.codehosting.net) 

		Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
		associated documentation files (the "Software"), to deal in the Software without restriction, 
		including without limitation the rights to use, copy, modify, merge, publish, distribute, 
		sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
		furnished to do so, subject to the following conditions:

		The above copyright notice and this permission notice shall be included in all copies or 
		substantial portions of the Software.

		THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
		INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
		PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
		FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
		OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
		DEALINGS IN THE SOFTWARE.
		*/

		private readonly HttpListener listener = new HttpListener();
		private readonly Action<HttpListenerRequest, HttpListenerResponse> responseMethod;

		public bool IsClosed { get; private set; } = false;
		public HttpListenerPrefixCollection Prefixes => listener.Prefixes;

		public WebServer(string[] prefixes, Action<HttpListenerRequest, HttpListenerResponse> method)
		{
			if (!HttpListener.IsSupported)
				throw new NotSupportedException("HttpListener is not supported");
			if (prefixes == null)
				throw new ArgumentException("prefixes");
			if (method == null)
				throw new ArgumentException("method");
			foreach (string s in prefixes)
				listener.Prefixes.Add(s);
			responseMethod = method;
			listener.Start();
			ModConsole.Print("Server created");
		}

		public WebServer(Action<HttpListenerRequest, HttpListenerResponse> method, params string[] prefixes) : this(prefixes, method) { }

		public void Run()
		{
			ThreadPool.QueueUserWorkItem(o =>
			{
				try
				{
					while (listener.IsListening)
					{
						ThreadPool.QueueUserWorkItem(c =>
						{
							var ctx = c as HttpListenerContext;
							try
							{
								responseMethod(ctx.Request, ctx.Response);
							}
							catch { }
							finally
							{
								ctx.Response.OutputStream.Close();
							}
						}, listener.GetContext());
					}
				}
				catch { }
			});
		}

		public void Stop()
		{
			listener.Stop();
			listener.Close();
		}
	}

	public class AlivieskaGpsServer : Mod
	{
		public override string ID => "AlivieskaGpsServer";
		public override string Name => "Alivieska GPS server";
		public override string Author => "Wampa842";
		public override string Version => "1.1.2";
		public override bool UseAssetsFolder => false;

		private string _serverConfigPath;
		private int _port = 8080;
		private bool _autoStart = true;
		private bool _outputJson = false;

		private GameObject _car;    // The Satsuma or whatever other GameObject needs to be tracked

		private void _loadConfig()
		{
			using (StreamReader reader = new StreamReader(_serverConfigPath))
			{
				while (!reader.EndOfStream)
				{
					string[] tok = reader.ReadLine().Split(' ');
					if (tok.Length > 1)
						switch (tok[0])
						{
							case "port":
								int.TryParse(tok[1], out _port);
								break;
							case "autostart":
								bool.TryParse(tok[1], out _autoStart);
								break;
							case "output":
								_outputJson = tok[1].Trim().ToLowerInvariant() == "json";
								break;
							default:
								break;
						}
				}
			}
		}

		private void _saveConfig()
		{
			using (StreamWriter writer = new StreamWriter(_serverConfigPath))
			{
				writer.WriteLine("port " + _port.ToString());
				writer.WriteLine("autostart " + _autoStart.ToString().ToLowerInvariant());
				writer.WriteLine("output " + (_outputJson ? "json" : "xml"));
			}
		}

		public string GetJsonContent()
		{
			float x = _car.transform.position.x;
			float y = _car.transform.position.y;
			float z = _car.transform.position.z;
			float speed = FsmVariables.GlobalVariables.FindFsmFloat("SpeedKMH").Value;
			float heading = _car.transform.eulerAngles.y;
			float time = FsmVariables.GlobalVariables.FindFsmFloat("GlobalTime").Value;

			return string.Concat("{", string.Format(@"""x"":{0},""y"":{1},""z"":{2},""time"":{3},""speed"":{4},""heading"":{5}", x, y, z, time, speed, heading), "}");
		}

		public string GetXmlContent()
		{
			XmlDocument doc = new XmlDocument();
			doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", "no"));
			doc.AppendChild(doc.CreateElement("GpsData"));

			XmlElement node;
			node = doc.CreateElement("X");
			node.InnerText = _car.transform.position.x.ToString();
			doc.DocumentElement.AppendChild(node);

			node = doc.CreateElement("Y");
			node.InnerText = _car.transform.position.y.ToString();
			doc.DocumentElement.AppendChild(node);

			node = doc.CreateElement("Z");
			node.InnerText = _car.transform.position.z.ToString();
			doc.DocumentElement.AppendChild(node);

			node = doc.CreateElement("Heading");
			node.InnerText = _car.transform.eulerAngles.y.ToString();
			doc.DocumentElement.AppendChild(node);

			node = doc.CreateElement("Speed");
			node.InnerText = FsmVariables.GlobalVariables.FindFsmFloat("SpeedKMH").Value.ToString();
			doc.DocumentElement.AppendChild(node);

			node = doc.CreateElement("Time");
			node.InnerText = FsmVariables.GlobalVariables.FindFsmFloat("GlobalTime").Value.ToString();
			doc.DocumentElement.AppendChild(node);

			return doc.OuterXml;
		}

		public void GetContent(HttpListenerRequest request, HttpListenerResponse response)
		{
			string responseString;
			if(Array.Exists(request.Url.Segments, e => e.ToLowerInvariant().Contains("json")))
			{
				responseString = GetJsonContent();
				response.ContentType = "application/json";
			}
			else if(Array.Exists(request.Url.Segments, e => e.ToLowerInvariant().Contains("xml")))
			{
				responseString = GetXmlContent();
				response.ContentType = "application/xml";
			}
			else
			{
				if(_outputJson)
				{
					responseString = GetJsonContent();
					response.ContentType = "application/json";
				}
				else
				{
					responseString = GetXmlContent();
					response.ContentType = "application/xml";
				}
			}
			byte[] buffer = Encoding.UTF8.GetBytes(responseString);
			response.Headers.Add("Access-Control-Allow-Origin", request.Headers["Origin"]);
			response.ContentLength64 = buffer.Length;
			response.OutputStream.Write(buffer, 0, buffer.Length);
		}

		public WebServer Server;

		public void StartServer(bool loadConfigFile = true)
		{
			if (loadConfigFile)
			{
				_loadConfig();
			}
			if (Server == null)
			{
				try
				{
					ModConsole.Print("Creating server...");
					Server = new WebServer(GetContent, $"http://*:{_port}/");
					Server.Run();
				}
				catch (HttpListenerException ex)
				{
					ModConsole.Error("Could not create server:\n" + ex.ToString());
				}
			}
			ModConsole.Print("Server is running");
		}

		public void StopServer()
		{
			try
			{
				ModConsole.Print("Stopping server");
				if (Server != null)
				{
					Server.Stop();
					Server = null;
				}
				ModConsole.Print("Server is not running");
			}
			catch (Exception ex)
			{
				ModConsole.Error("Could not stop server:\n" + ex.ToString());
			}
			ModConsole.Print("Server stopped");
		}

		private class GpsCommand : ConsoleCommand
		{
			public override string Name => "gps";
			public override string Help => "GPS server - see 'gps help' for details";

			private AlivieskaGpsServer _mod;

			private readonly string _helpString =
				"AlivieskaGpsServer\n" +
				"Copyright (c) Wampa842 2018 (github.com/wampa842)\n\n" +
				"Usage:\n" +
				"  gps [-p <port>|--port <port>] <command>\n" +
				"where <command> is:" +
				"  start: starts listening on the default or specified port\n" +
				"  stop: halts the server\n" +
				"  restart: stops and then starts the server while reloading the configuration file\n" +
				"  write [xml] [json]: writes the response string to a file, in XML or JSON format\n" +
				"  help: displays this message\n\n" +
				"This mod runs a small HTTP web server that displays the current position and heading of the Satsuma, either in XML or in JSON format.\n" +
				"The data is served on localhost's specified port, or 8080 by default.";

			public GpsCommand(AlivieskaGpsServer sender)
			{
				this._mod = sender;
			}

			public override void Run(string[] args)
			{
				// Make sure the command is executed with arguments.
				if (args.Length < 1)
				{
					ModConsole.Error("Incorrect number of arguments. See 'gps help' for details.");
					return;
				}

				// Print the help text
				if (Array.Exists(args, e => e.ToLowerInvariant() == "help" || e == "-?"))
				{
					ModConsole.Print(_helpString);
					return;
				}

				// When starting or restarting, try to find -p and try to validate the following argument as a port number
				if (Array.Exists(args, e => e.ToLowerInvariant() == "start" || e.ToLowerInvariant() == "restart"))
				{
					int p;
					if ((p = Array.IndexOf(args, "-p")) >= 0 || (p = Array.IndexOf(args, "--port")) >= 0)
					{
						if (args.Length >= p + 1 && int.TryParse(args[p + 1], out int portNumber) && portNumber <= 65535 && portNumber >= 1024)
						{
							_mod._port = portNumber;
						}
						else ModConsole.Print(_helpString);
					}
				}

				// Restart
				if (Array.Exists(args, e => e.ToLowerInvariant() == "restart"))
				{
					_mod.StopServer();
					_mod.StartServer();
					return;
				}

				// Start
				if (Array.Exists(args, e => e.ToLowerInvariant() == "start"))
				{
					_mod.StartServer();
					return;
				}

				// Stop
				if (Array.Exists(args, e => e.ToLowerInvariant() == "stop"))
				{
					_mod.StopServer();
					return;
				}

				// Write to file
				if (Array.Exists(args, e => e.ToLowerInvariant() == "write"))
				{
					if (Array.Exists(args, e => e.ToLowerInvariant() == "json"))
					{
						using (StreamWriter writer = new StreamWriter(Path.Combine(ModLoader.GetModConfigFolder(_mod), "out.json")))
						{
							writer.Write(_mod.GetJsonContent());
						}
					}
					else if (Array.Exists(args, e => e.ToLowerInvariant() == "xml"))
					{
						using (StreamWriter writer = new StreamWriter(Path.Combine(ModLoader.GetModConfigFolder(_mod), "out.xml")))
						{
							writer.Write(_mod.GetXmlContent());
						}
					}
					else
					{
						using (StreamWriter writer = new StreamWriter(Path.Combine(ModLoader.GetModConfigFolder(_mod), "out.json")))
						{
							writer.Write(_mod.GetJsonContent());
						}
						using (StreamWriter writer = new StreamWriter(Path.Combine(ModLoader.GetModConfigFolder(_mod), "out.xml")))
						{
							writer.Write(_mod.GetXmlContent());
						}
					}
					ModConsole.Print("Completed writing - check the mod's config folder.");
					return;
				}

				// If this point is reached, there were no valid commands to execute.
				ModConsole.Error("Invalid arguments. See 'gps help' for details.");
			}
		}

		public override void OnLoad()
		{
			_serverConfigPath = Path.Combine(ModLoader.GetModConfigFolder(this), "server.cfg");
			_loadConfig();
			ConsoleCommand.Add(new GpsCommand(this));
			_car = GameObject.Find("SATSUMA(557kg, 248)");
			StartServer();
		}

		public override void OnSave()
		{
			StopServer();
		}
	}
}
