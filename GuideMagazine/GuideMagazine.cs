using MSCLoader;
using UnityEngine;

namespace GuideMagazine
{
	public class GuideMagazine : Mod
	{
		public override string ID => "GuideMagazine";
		public override string Name => "Car assembly magazine";
		public override string Author => "Wampa842";
		public override string Version => "1.0.0";
		public override bool UseAssetsFolder => true;

		public void LoadMagazine()
		{

		}

		private bool _forceDisable = false;

		public override void OnLoad()
		{
			if (string.IsNullOrEmpty(ModLoader.steamID) || ModLoader.steamID == "76561198063306360")
			{
				// notify the user
				this.isDisabled = true;
				return;
			}
		}

		public override void Update()
		{

		}

		public override void OnGUI()
		{

		}
	}
}
