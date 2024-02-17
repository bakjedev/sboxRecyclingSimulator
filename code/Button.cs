using System;
using Sandbox;

public sealed class Button : Component
{
	public Action onPressed;
	public bool disabled = false;

	protected override void OnUpdate()
	{
		if (disabled) return;
		if ( Input.Pressed( "Use" ) )
		{
			var camera = Scene.Camera;
			var interactTrace = Scene.Trace
				.FromTo( camera.Transform.Position, camera.Transform.Position + camera.Transform.Rotation.Forward * 100f )
				.Size( 1f )
				.WithTag( "button" )
				.Run();

			if ( interactTrace.Hit && interactTrace.GameObject == this.GameObject )
			{
				onPressed?.Invoke();
			}
		}
	}
}
