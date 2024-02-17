using Sandbox;
using System;

public sealed class RigidPlayer : Component
{
	[Property]
	public GameObject Camera { get; set; }
	[Property]
	public Rigidbody body { get; set; }
	[Property]
	public CapsuleCollider collider { get; set; }
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

	protected override void OnStart()
	{
		if ( Camera != null )
		{
			initialCameraTransform = Camera.Transform.Local.WithPosition( Camera.Transform.Local.Position.WithZ( collider.End.z ) );
		}
	}

	protected override void OnUpdate()
	{
		EyeAngles += Input.AnalogLook;
		EyeAngles = EyeAngles.WithPitch( MathX.Clamp( EyeAngles.pitch, -89f, 89f ) );
		Transform.Rotation = Rotation.FromYaw( EyeAngles.yaw );

		if ( Camera != null )
		{
			Camera.Transform.Local = initialCameraTransform.RotateAround( EyePosition, EyeAngles.WithYaw( 0f ) );
		}
	}

	protected override void OnFixedUpdate()
	{
		// Get input
		var wishVel = Input.AnalogMove.Normal * Transform.Rotation;

		float acceleration;
		acceleration = OnGround() ? 8000000 : 2000000;

		// Calculate slope stuff
		var slopeNormal = OnSlope();
		var gravityComponent = Scene.PhysicsWorld.Gravity.Dot( slopeNormal );

		// If were standing almost still counter act the slope down movement.
		var slopeAngle = MathF.Acos(slopeNormal.z);
		if ( slopeAngle < MathX.DegreeToRadian( 45 ))
		{
			if ( Math.Abs( body.Velocity.Length ) < 20 )
			{
				body.Velocity += slopeNormal * gravityComponent * Time.Delta;
			}

			// make it so if moving on slope your wishvel is taking in the normal of the slope
			var wishVelOnSlope = wishVel - wishVel.Dot( slopeNormal ) * slopeNormal;

			wishVel = wishVelOnSlope.Normal;
		}
		body.ApplyForce( wishVel * acceleration );

		if ( (Input.Pressed( "Jump" ) || Input.MouseWheel.y != 0) && OnGround() )
		{
			// setting z velocity to 0 to make jumps on slopes not retarded
			body.Velocity = body.Velocity.WithZ( 0 );
			body.ApplyImpulse( Vector3.Up * JumpStrength * 4000 );
		}
		
		// limit velocity
		var maxSpeed = Input.Down( "Walk" ) ? WalkSpeed : Speed;

		float vVel = body.Velocity.z;
		Vector3 hVel = body.Velocity.WithZ( 0 );
		hVel = hVel.ClampLength( maxSpeed );
		body.Velocity = hVel.WithZ( vVel );
	}



	private float GetHSpeed()
	{
		return Math.Abs(body.Velocity.WithZ(0).Length);
	}

	private Vector3 OnSlope()
	{
		var feet = Transform.Position.WithZ( Transform.Position.z - 12 );
		var halfHeight = collider.End.z / 3;
		var slopeTrace = Scene.Trace
			.FromTo( feet, feet + Transform.Rotation.Down * halfHeight )
			.Size( 1f )
			.WithoutTags( "player" )
			.Run();

		if (slopeTrace.Hit)
		{
			if (slopeTrace.Normal != Vector3.Up)
				return slopeTrace.Normal;
		}

		return Vector3.Zero;
	}

	private bool OnGround()
	{
		var groundTrace = Scene.Trace
			.FromTo( Transform.Position.WithZ( Transform.Position.z - 24 ), Transform.Position.WithZ( Transform.Position.z - 24 ) + Transform.Rotation.Down * 2 )
			.Size( 10f )
			.WithoutTags( "player" )
			.IgnoreGameObjectHierarchy( GameObject )
			.Run();
		return groundTrace.Hit;
	}
}
