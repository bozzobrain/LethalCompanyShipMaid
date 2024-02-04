using LethalCompanyInputUtils.Api;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.InputSystem;

namespace ShipMaid.InputUtils
{
	internal class InputUtilsKeybinds : LcInputActions
	{
		[InputAction("<Keyboard>/j", Name = "Set Location Override")]
		public InputAction LocationOverrideKey { get; set; }

		[InputAction("<Keyboard>/m", Name = "Cleanup Ship")]
		public InputAction ShipCleanupKey { get; set; }

		[InputAction("<Keyboard>/n", Name = "Cleanup Storage")]
		public InputAction StorageCleanupKey { get; set; }
	}
}