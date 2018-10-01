using UnityEditor;
using UnityEngine;
using System.IO;

public class CreateAssetBundles
{
	private const string _path = "Assets/AssetBundles";
	private const string _modpath = @"G:\Steam\steamapps\common\My Summer Car\Mods\Assets\TwentyFourClock";

	[MenuItem("Assets/Build AssetBundles")]
	static void BuildAllAssetBundles()
	{
		if (!Directory.Exists(_path))
			Directory.CreateDirectory(_path);
		AssetBundleManifest m = BuildPipeline.BuildAssetBundles(_path, BuildAssetBundleOptions.ForceRebuildAssetBundle, BuildTarget.StandaloneOSXUniversal);
		
		if (!Directory.Exists(_modpath))
			Directory.CreateDirectory(_modpath);

		foreach(string s in m.GetAllAssetBundles())
		{
			File.Copy(Path.Combine(_path, s), Path.Combine(_modpath, s), true);
			Debug.Log(string.Format("Copied {0} to {1}", s, _modpath));
		}
	}
}