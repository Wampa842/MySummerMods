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
		private readonly Func<HttpListenerRequest, string> responseMethod;

		public bool IsClosed { get; private set; } = false;
		public HttpListenerPrefixCollection Prefixes => listener.Prefixes;

		public WebServer(string[] prefixes, Func<HttpListenerRequest, string> method)
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

		public WebServer(Func<HttpListenerRequest, string> method, params string[] prefixes) : this(prefixes, method) { }

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
								string responseString = responseMethod(ctx.Request);
								byte[] buffer = Encoding.UTF8.GetBytes(responseString);
								ctx.Response.ContentType = "application/json";
								ctx.Response.Headers.Add("Access-Control-Allow-Origin", ctx.Request.Headers["Origin"]);
								ctx.Response.ContentLength64 = buffer.Length;
								ctx.Response.OutputStream.Write(buffer, 0, buffer.Length);
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
		public override string Version => "1.0.0";
		public override bool UseAssetsFolder => false;

		private string _serverConfigPath;
		private int _port = 8080;
		private bool _autoStart = true;

		private void _loadConfig()
		{
			using (StreamReader reader = new StreamReader(_serverConfigPath))
			{
				while (!reader.EndOfStream)
				{
					string[] tok = reader.ReadLine().Split(' ');
					switch (tok[0])
					{
						case "port":
							int.TryParse(tok[1], out _port);
							break;
						case "autostart":
							bool.TryParse(tok[1], out _autoStart);
							break;
						default:
							break;
					}
				}
			}
		}

		// The Satsuma or whatever other GameObject needs to be tracked
		private GameObject _car;

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
		public string GetJsonContent(HttpListenerRequest request)
		{
			return GetJsonContent();
		}

		public WebServer Server;

		public void StartServer(bool loadConfigFile = true)
		{
			if(loadConfigFile)
			{
				_loadConfig();
			}
			if (Server == null)
			{
				try
				{
					Server = new WebServer(GetJsonContent, $"http://*:{_port}/");
					ModConsole.Print("Creating server...");
				}
				catch (HttpListenerException ex)
				{
					ModConsole.Print("Could not create server:\n" + ex.ToString());
				}
			}
			Server.Run();
			ModConsole.Print("Server is running");
		}

		public void StopServer()
		{
			ModConsole.Print("Stopping server");
			if (Server != null)
			{
				Server.Stop();
				Server = null;
				ModConsole.Print("Server stopped");
			}
			ModConsole.Print("Server is not running");
		}

		private class GpsCommand : ConsoleCommand
		{
			public override string Name => "gps";
			public override string Help => "GPS server - see 'gps help' for details";

			AlivieskaGpsServer _mod;

			private readonly string _usageString = "Usage: gps [-p <port>] start|stop|restart|write|help";
			private readonly string _helpString = "gps - controls a web server that provides positioning information on 'http://localhost:8080/'.";

			public GpsCommand(AlivieskaGpsServer sender)
			{
				this._mod = sender;
			}

			public override void Run(string[] args)
			{
				// Make sure the command is executed with arguments.
				if (args.Length < 1)
				{
					ModConsole.Print(_usageString);
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
					if ((p = Array.IndexOf(args, "-p")) >= 0)
					{
						if (args.Length >= p + 1 && int.TryParse(args[p + 1], out int portNumber) && portNumber <= 65535 && portNumber >= 1024)
						{
							_mod._port = portNumber;
						}
						else ModConsole.Print(_usageString);
					}
				}

				// Restart
				if(Array.Exists(args, e => e.ToLowerInvariant() == "restart"))
				{
					_mod.StopServer();
					_mod.StartServer();
					return;
				}

				// Start
				if(Array.Exists(args, e => e.ToLowerInvariant() == "start"))
				{
					_mod.StartServer();
					return;
				}

				// Stop
				if(Array.Exists(args, e => e.ToLowerInvariant() == "stop"))
				{
					_mod.StopServer();
					return;
				}

				// Write to file
				if(Array.Exists(args, e => e.ToLowerInvariant() == "write"))
				{
					using (StreamWriter writer = new StreamWriter(Path.Combine(ModLoader.GetModConfigFolder(_mod), "out.json")))
					{
						writer.Write(_mod.GetJsonContent());
					}
				}
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
