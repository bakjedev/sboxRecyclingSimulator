using Sandbox;
using System;
using System.Runtime.CompilerServices;

public sealed class ObjectSpawner : Component
{
	[Property]
	[Category(" Spawning objects ")]
	public GameObject garbageBag { get; set; }
	[Property]
	[Category( " Spawning objects " )]
	public GameObject table { get; set; }
	[Property]
	[Category( " Spawning objects " )]
	public GameObject cardboardBox { get; set; }
	[Property]
	[Category( " testing " )]
	public Vector3 spawnPos { get; set; } = Vector3.Zero;
	public List<GameObject> objects = new();
	protected override void DrawGizmos()
	{
		Gizmo.Draw.WorldText( "Spawn Pos", new Transform(spawnPos, Rotation.FromRoll(90f), 0.3f));
	}


	public void SpawnObject(GameObject obj, Vector3 pos)
	{
		var o = obj.Clone();
		o.Transform.Position = pos;

		o.NetworkSpawn();
		objects.Add( o );
	}

	public void SpawnRandomObjects(int amount)
	{
		Random random = new Random();

		for ( int i = 0; i < amount; i++ )
		{
			int randomIndex = random.Next( 0, 3 );

			switch ( randomIndex )
			{
				case 0:
					SpawnObject( garbageBag, spawnPos + Vector3.Up * 100f * i );
					break;
				case 1:
					SpawnObject( table, spawnPos + Vector3.Up * 100f * i );
					break;
				case 2:
					SpawnObject( cardboardBox, spawnPos + Vector3.Up * 100f * i );
					break;
				default:
					Log.Info( "Invalid object to spawn" );
					break;
			}
		}
	}
}
