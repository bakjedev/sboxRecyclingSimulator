using Sandbox;
[Icon("person")]
public sealed class Player : Component
{
	[Property]
	public GameObject Camera { get; set; }
	[Property]
	public CharacterController characterController { get; set; }
	[Property]
	public float Speed { get; set; } = 300f;
	[Property]
	public float WalkSpeed { get; set; } = 150f;
	[Property]
	public float JumpStrength { get; set; } = 250f;
	[Property]
	public Vector3 EyePosition { get; set; }
	public Angles EyeAngles { get; set; }

	Transform initialCameraTransform;


	protected override void DrawGizmos()
	{
		if ( !Gizmo.IsSelected ) return;
		Gizmo.Draw.Color = Color.Blue;
		Gizmo.Draw.LineSphere( EyePosition, 3f );
		Gizmo.Draw.Color = Color.Red;
	}
	protected override void OnStart()
	{
		if(Camera != null)
		{
			initialCameraTransform = Camera.Transform.Local;
		}
	}
	protected override void OnUpdate()
	{
		EyeAngles += Input.AnalogLook;
		EyeAngles = EyeAngles.WithPitch( MathX.Clamp( EyeAngles.pitch, -89f, 89f ) );
		Transform.Rotation = Rotation.FromYaw( EyeAngles.yaw );

		if( Camera != null )
		{
			Camera.Transform.Local = initialCameraTransform.RotateAround( EyePosition, EyeAngles.WithYaw(0f) );
		}
	}

	protected override void OnFixedUpdate()
	{
		if ( characterController == null ) return;

		var wishSpeed = Input.Down( "Walk" ) ? WalkSpeed : Speed;
		var wishVel = Input.AnalogMove.Normal * wishSpeed * Transform.Rotation;

		characterController.Accelerate( wishVel );

		if ( characterController.IsOnGround )
		{
			characterController.Acceleration = 10f;
			characterController.ApplyFriction( 5f );

			if(Input.Pressed("Jump"))
			{
				characterController.Punch( Vector3.Up * JumpStrength );
			}
		} 
		else
		{
			characterController.Acceleration = 1f;
			characterController.Velocity += Scene.PhysicsWorld.Gravity * Time.Delta;
		}

		characterController.Move();
	}
}
