using Sandbox;
using System;

public enum InfoType
{
	/// <summary>
	/// Nothing
	/// </summary>
	[Icon( "filter_none" )]
	None,
	/// <summary>
	/// The player
	/// </summary>
	[Icon( "person" )]
	Player,
	/// <summary>
	/// The garbage items the player can grab
	/// </summary>
	[Icon( "delete" )]
	Garbage,
	/// <summary>
	/// Wooden objects
	/// </summary>
	[Icon( "forest" )]
	Wood,
	/// <summary>
	/// Recyclable objects
	/// </summary>
	[Icon( "recycling" )]
	Recyclable
}
[Icon( "info" )]
public sealed class Info : Component
{
	[Property]
	public InfoType Team { get; set; }

	[Property]
	public Mortal Mortal { get; set; }

	[Property]
	public Economy Economy { get; set; }
	[Property]
	public Grabbable Grabbable { get; set; }
	
}
