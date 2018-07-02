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
using System.Drawing;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Xml;

namespace AlivieskaGpsClient
{
	// Data received from the GPS server
	public class GpsData
	{
		private readonly MainForm _form;

		public Size Size = new Size(4200, 3350);
		public PointF Center = new PointF(-0.0585f, 0.041f);

		public PointF MapPosition
		{
			get
			{
				return new PointF(this.X / this.Size.Width + this.Center.X, -this.Z / this.Size.Height + this.Center.Y);
			}
		}

		public GpsData(MainForm form)
		{
			this._form = form;
		}

		public float X = 0;         // West-east position
		public float Y = 0;         // Height above lake Peräjärvi
		public float Z = 0;         // North-south position
		public float Heading = 0;   // Angle from north in degrees
		public float Speed = 0;     // Displayed speed of the car
		public string ResponseString;   // The raw string received from the server
		public bool Success = false;    // Indicates whether the request was successful
		public HttpStatusCode Status;   // The status code of the response

		private XmlDocument _doc = new XmlDocument();
		//<GpsData>
		//	<X>1009.916</X>
		//	<Y>-0.8313327</Y>
		//	<Z>-738.0518</Z>
		//	<Heading>10</Heading>
		//	<Speed>30</Speed>
		//	<Time>0</Time>
		//</GpsData>

		private readonly HttpClient _client = new HttpClient();
		public async Task Get(string url)
		{
			HttpResponseMessage response = await _client.GetAsync(url);
			Success = response.IsSuccessStatusCode;
			Status = response.StatusCode;
			if (response.IsSuccessStatusCode)
			{
				ResponseString = await response.Content.ReadAsStringAsync();
				_doc.LoadXml(ResponseString);
				float.TryParse(_doc.DocumentElement["X"].InnerText.Trim(), out X);
				float.TryParse(_doc.DocumentElement["Y"].InnerText.Trim(), out Y);
				float.TryParse(_doc.DocumentElement["Z"].InnerText.Trim(), out Z);
				float.TryParse(_doc.DocumentElement["Heading"].InnerText.Trim(), out Heading);
				float.TryParse(_doc.DocumentElement["Speed"].InnerText.Trim(), out Speed);
			}
			_form.UpdateGpsData();
		}
	}
}
