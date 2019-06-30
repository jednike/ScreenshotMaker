using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Screenshot
{
	[CreateAssetMenu(fileName = "Screenshots Settings", menuName = "Screenshot/Create Screenshots settings",
		order = 1)]

	public class ScreenAndLanguages : ScriptableObject
	{
		[Serializable]
		public struct PlatformDimension
		{
			public string PlatformName;
			public Vector2[] Dimensions;
		}

		[SerializeField] private PlatformDimension[] _dimensions;
		public PlatformDimension[] Dimensions
		{
			get { return _dimensions; }
		}
		
		[SerializeField] private string[] _languages;
		public string[] Languages
		{
			get { return _languages; }
		}
	}
}