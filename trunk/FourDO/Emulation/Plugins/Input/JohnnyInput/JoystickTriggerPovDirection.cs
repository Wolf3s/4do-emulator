﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	[Flags]
	public enum JoystickTriggerPovDirection
	{
		None = 0,
		Up = 1,
		Down = 2,
		Left = 4,
		Right = 8
	}
}
