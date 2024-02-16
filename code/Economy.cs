using Sandbox;

public sealed class Economy : Component
{
	/// <summary>
	/// In euros
	/// </summary>
	[Property]
	public float price { get; set; } = 1f;
	public bool shouldSell { get; set; } = false;
}
