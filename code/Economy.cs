using Sandbox;

public sealed class Economy : Component
{
	/// <summary>
	/// In euros
	/// </summary>
	[Property]
	public float price { get; set; } = 1f;
	public bool shouldDestroy { get; set; } = true;
	public bool correctContainer { get; set; } = false;
	public bool returned { get; set; } = false;
}
