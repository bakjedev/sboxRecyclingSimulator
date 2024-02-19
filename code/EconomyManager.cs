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
	public GameObject ReturnableContainer { get; set; }
	
	
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
	private int objectsIndex = 0;
	public List<GameObject> BuyableObjects = new();
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
		bool soldAll = true;
		foreach ( GameObject obj in Spawner.objects )
		{
			var infoComponent = obj.Components.Get<Info>();
			if ( infoComponent is not null )
			{
				if ( infoComponent.Economy.IsValid )
				{
					if ( infoComponent.Economy.shouldDestroy )
					{
						if ( infoComponent.Economy.correctContainer )
						{
							money += infoComponent.Economy.price;
						}
						else if (!infoComponent.Economy.returned)
						{
							soldAll = false;
						}

						
						obj.Destroy();
					}
				}
			}
		}

		if ( !soldAll )
		{
			SoundEvent wrongSound = Cloud.SoundEvent( "dopamine.airlock_alarm" );
			wrongSound.Volume = .1f;
			Sound.Play( wrongSound, Spawner.Transform.Position );
		}
		else
		{
			SoundEvent correctSound = Cloud.SoundEvent( "exorealms.66d09d44dcbf5e0c" );
			correctSound.Volume = .1f;
			Sound.Play( correctSound, Spawner.Transform.Position );
		}
	}
	

	protected override void OnStart()
	{
		SpawnObject(GarbageContainer.Clone(), false );
		SpawnObject(WoodContainer.Clone(), false );
		SpawnObject(ReturnableContainer.Clone(), false );
		
		BuyableObjects.Add( RecyclableContainer.Clone());
		
		SellButton.onPressed += OnSellButton;
		SpawnButton.onPressed += OnSpawnButton;
	}

	
	public void SpawnObject(GameObject obj, bool removeBuyableObject)
	{
		if ( removeBuyableObject )
		{
			BuyableObjects.Remove(obj);
		}
		
		obj.Transform.Position = Positions[objectsIndex];
		var test = Transform.Position.x > 0;
		var newAngles = Transform.Rotation.Angles();
		newAngles.yaw = test ? 0 : 180;
		Transform.Rotation = newAngles.ToRotation();
		objectsIndex = (objectsIndex + 1) % Positions.Count;
		
		Objects.Add(obj);
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
