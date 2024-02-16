using Sandbox;

public sealed class SpawnButton : Component
{
	[Property]
	public ObjectSpawner Spawner { get; set; }
	public bool disabled = false;
	protected override void OnUpdate()
	{
		if ( Input.Down( "Use" ) && !disabled )
		{
			var Camera = Scene.Camera;
			var interactTrace = Scene.Trace
					.FromTo( Camera.Transform.Position, Camera.Transform.Position + Camera.Transform.Rotation.Forward * 100f )
					.Size( 1f )
					.WithTag( "spawnbutton" )
					.Run();

			if ( interactTrace.Hit )
			{
				SoundEvent buttonSound = new SoundEvent( "sounds/button1" );
				Sound.Play( buttonSound, Transform.Position );
				disabled = true;
				Spawner.SpawnRandomObjects(5);
			}
		}
	}
}
