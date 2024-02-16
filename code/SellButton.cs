using Sandbox;

public sealed class SellButton : Component
{
	[Property]
	public SpawnButton SpawnButton { get; set; }
	[Property]
	public EconomyManager EconomyManager { get; set; }
	protected override void OnUpdate()
	{
		if ( Input.Pressed( "Use" ) )
		{
			var Camera = Scene.Camera;
			var interactTrace = Scene.Trace
					.FromTo( Camera.Transform.Position, Camera.Transform.Position + Camera.Transform.Rotation.Forward * 100f )
					.Size( 1f )
					.WithTag( "sellbutton" )
					.Run();

			if ( interactTrace.Hit )
			{
				SoundEvent buttonSound = new SoundEvent( "sounds/button1" );
				Sound.Play( buttonSound, Transform.Position );
				SpawnButton.disabled = false;
				EconomyManager.SellSellableGarbages(true);
			}
		}
	}
}
