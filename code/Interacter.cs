using Sandbox;
using System;

public sealed class Interacter : Component
{
	[Property]
	public GameObject Camera { get; set; }
	public bool hit = false;
	protected override void OnUpdate()
	{
		var interactTrace = Scene.Trace
				.FromTo( Camera.Transform.Position, Camera.Transform.Position + Camera.Transform.Rotation.Forward * 100f )
				.Size( 1f )
				.WithTag( "button" )
				.IgnoreGameObjectHierarchy( GameObject );
		if (Input.Pressed("Use"))
		{
			hit = interactTrace.Run().Hit;
		} else
		{
			hit = false;
		}

	}
}
