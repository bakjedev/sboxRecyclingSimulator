using Sandbox;
using System;

public sealed class Grabbable : Component
{
	[Property]
	public String Tooltip { get; set; }

	[Property]
	public bool Grabbing { get; set; } = true;
}
