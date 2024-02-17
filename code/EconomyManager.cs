using Sandbox;
using System;

public sealed class EconomyManager : Component
{
	// spawning objects
	[Property]
	[Category("References")]
	public GameObject GarbageContainer { get; set; }
	[Property]
	[Category("References")]
	public GameObject WoodContainer { get; set; }
	[Property]
	[Category("References")]
	public GameObject RecyclableContainer { get; set; }
	
	
	[Property]
	[Category("References")]
	public ObjectSpawner Spawner { get; set; }
	[Property] 
	[Category("References")]
	public Button SellButton { get; set; }
	[Property] 
	[Category("References")]
	public Button SpawnButton { get; set; }
	
	[Property]
	[Category("Containers")]
	List<Vector3> Positions { get; set; }
	
	public List<GameObject> Objects = new();
	public float money = 0f;
	
	protected override void DrawGizmos()
	{
		foreach (var position in Positions)
		{
			Gizmo.Draw.Color = Color.Green;
			Gizmo.Draw.LineSphere(position, 2);
		}
	}
	
	public void SellSellableGarbages(bool destroyNonSellables)
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
					else if (destroyNonSellables)
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
		
		
		SellButton.onPressed += OnSellButton;
		SpawnButton.onPressed += OnSpawnButton;
	}

	
	private void SpawnObject(GameObject obj, Vector3 pos)
	{
		obj.Transform.Position = pos;
		Objects.Add( obj );
		obj.NetworkSpawn();
	}

	private void OnSellButton()
	{
		SoundEvent buttonSound = new SoundEvent( "sounds/button1" );
		Sound.Play( buttonSound, SellButton.Transform.Position );
		SpawnButton.disabled = false;
		SellSellableGarbages(true);
	}

	private void OnSpawnButton()
	{
		SoundEvent buttonSound = new SoundEvent( "sounds/button1" );
		Sound.Play( buttonSound, SellButton.Transform.Position );
		SpawnButton.disabled = true;
		Spawner.SpawnRandomObjects(5);
	}
}
