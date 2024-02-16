using Sandbox;
using System;
using System.Buffers.Text;
using System.Numerics;

public sealed class Grabber : Component
{
	[Property]
	public GameObject Camera { get; set; }
	[Property]
	public float Range { get; set; } = 100f;
	public PhysicsBody grabbedBody { get; set; }
	public Transform grabbedOffset { get; set; }
	GameObject HitObject;

	protected override void DrawGizmos()
	{
		if ( !Gizmo.IsSelected || Camera == null) return;
		Gizmo.Draw.Color = Color.Red;
		Gizmo.Draw.Line( Camera.Transform.Local.Position, Camera.Transform.Local.Position + Camera.Transform.Local.Rotation.Forward * Range );
	}
	protected override void OnUpdate()
	{
		if ( Camera == null) return;

		Transform aimTransform = Camera.Transform.World;

		// If an object is currently grabbed
		if ( grabbedBody is not null)
		{
			// Calculate the target transform for the grabbed object based on the camera's transform
			var targetTx = aimTransform.ToWorld( grabbedOffset );

			var infoComponent = grabbedBody.GetGameObject().Components.Get<Info>();

			if ( infoComponent.IsValid)
			{
				if ( !infoComponent.Grabbable.Grabbing )
				{
					grabbedOffset = default;
					grabbedBody = default;
					return;
				}
			}

			// If the grab input is released, reset grabbed state
			if ( !Input.Down( "Grab" ) )
			{
				grabbedOffset = default;
				grabbedBody = default;
			}
			else
			{
				grabbedBody.SmoothMove( targetTx, 0.4f, Time.Delta );
				return;
			}
		}

		// Perform a trace to detect objects in front of the camera within the grab range
		var grabTrace = Scene.Trace
			.FromTo( Camera.Transform.Position, Camera.Transform.Position + Camera.Transform.Rotation.Forward * Range )
			.Size( 1f )
			.WithoutTags( "player", "nametag" )
			.IgnoreGameObjectHierarchy( GameObject )
			.Run();

		// If no object is hit or the hit object is not dynamic, hide the nametag of the previously hit object
		if ( !grabTrace.Hit || grabTrace.Body is null || grabTrace.Body.BodyType != PhysicsBodyType.Dynamic )
		{
			if ( HitObject is not null)
			{
				var nametagObject = HitObject.Children.FirstOrDefault();

				if ( nametagObject is not null )
				{
					var nametagComponent = nametagObject.Components.Get<WorldPanel>(true);
					if ( nametagComponent is not null )
					{
						nametagComponent.RenderScale = 0f;
					}
				}
				HitObject = null;
			}
			return;
		}
		// If the currently hit object is different from the previously hit object
		if ( HitObject != grabTrace.GameObject )
		{
			// Hide the nametag of the previously hit object
			if ( HitObject is not null )
			{
				var prevNametagObject = HitObject.Children.FirstOrDefault();
				if ( prevNametagObject is not null )
				{
					var prevNametagComponent = prevNametagObject.Components.Get<WorldPanel>( true );
					if ( prevNametagComponent is not null )
					{
						prevNametagComponent.RenderScale = 0f;
					}
				}
			}

			// Show the nametag of the currently hit object
			HitObject = grabTrace.GameObject;
			var nametagObject = HitObject.Children.FirstOrDefault();
			if ( nametagObject is not null )
			{
				var nametagComponent = nametagObject.Components.Get<WorldPanel>( true );
				if ( nametagComponent is not null )
				{
					nametagComponent.RenderScale = 1f;
				}
			}
		}

		// If the grab input is pressed and the hit object is grabbable, grab the object
		if ( Input.Down( "Grab" ) )
		{
			var infoComponent = grabTrace.GameObject.Components.Get<Info>();

			if ( infoComponent == null || infoComponent.Grabbable.IsValid && infoComponent.Grabbable.Grabbing )
			{
				grabbedBody = grabTrace.Body;
				grabbedOffset = aimTransform.ToLocal( grabTrace.Body.Transform );
				grabbedBody.MotionEnabled = true;
			}
		}
	}
}
