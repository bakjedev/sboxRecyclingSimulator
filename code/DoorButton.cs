using Sandbox;
using Sandbox.UI;

public sealed class DoorButton : Component
{
	[Property]
	public Menu Menu { get; set; }

	protected override void OnUpdate()
	{
		var Camera = Scene.Camera;
		if ( Input.Pressed( "Use" ) )
		{
			var interactTrace = Scene.Trace
					.FromTo( Camera.Transform.Position, Camera.Transform.Position + Camera.Transform.Rotation.Forward * 100f )
					.Size( 1f )
					.WithTag( "doorbutton" )
					.Run();
			if ( interactTrace.Hit )
			{
				Menu.InMenu = !Menu.InMenu;
			}
		}

		if (Menu.InMenu)
		{
			var distanceFromPlayer = Vector3.DistanceBetween( Transform.Position, Camera.Transform.Position );

			if (distanceFromPlayer > 100f)
			{
				Menu.InMenu = false;
			}
		}
	}
}
