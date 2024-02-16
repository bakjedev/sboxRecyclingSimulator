using Sandbox;
using System;

public sealed class EconomyManager : Component
{
	[Property]
	public ObjectSpawner Spawner;
	public float money = 0f;
	[Property]
	List<Vector3> Positions { get; set; }
	public List<GameObject> Objects = new();

	[Property]
	public GameObject GarbageContainer { get; set; }
	[Property]
	public GameObject WoodContainer { get; set; }
	[Property]
	public GameObject RecyclableContainer { get; set; }

	protected override void DrawGizmos()
	{
		foreach (var position in Positions)
		{
			Gizmo.Draw.Color = Color.Green;
			Gizmo.Draw.LineSphere(position, 2);
		}
	}
	public void SellSellableGarbages(bool destroyNonSellable)
	{
		bool failedToSellAll = false;
		foreach ( GameObject obj in Spawner.objects )
		{
			var infoComponent = obj.Components.Get<Info>();
			if ( infoComponent is not null )
			{
				if ( infoComponent.Economy.IsValid )
				{
					if ( infoComponent.Economy.shouldSell )
					{
						money += infoComponent.Economy.price;
						obj.Destroy();
					} 
					else if (destroyNonSellable)
					{
						failedToSellAll = true;
						obj.Destroy();
					}
				}
			}
		}
		if ( failedToSellAll )
		{
			SoundEvent wrongSound = Cloud.SoundEvent( "dopamine.airlock_alarm" );
			wrongSound.Volume = .1f;
			Sound.Play( wrongSound, Spawner.Transform.Position );
		} else
		{
			SoundEvent correctSound = Cloud.SoundEvent( "exorealms.66d09d44dcbf5e0c" );
			correctSound.Volume = .1f;
			Sound.Play( correctSound, Spawner.Transform.Position );
		}
	}

	protected override void OnStart()
	{
		SpawnObject(GarbageContainer.Clone(), Positions[0] );
		SpawnObject(WoodContainer.Clone(), Positions[1] );
		SpawnObject(RecyclableContainer.Clone(), Positions[2] );

	}

	
	private void SpawnObject(GameObject obj, Vector3 pos)
	{
		obj.Transform.Position = pos;
		Objects.Add( obj );
		obj.NetworkSpawn();
	}
}
