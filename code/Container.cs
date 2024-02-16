using Sandbox;
using Sandbox.UI;



public sealed class Container : Component, Component.ITriggerListener
{
	[Property]
	public InfoType containerTeam { get; set; }
	public float upgradeCost = 100f;
	public int level = 1;


	public void Upgrade() {
		upgradeCost += 100f;
		level++;
	}

	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		TagSet ignores = new TagSet();
		ignores.Add( "crate" );
		ignores.Add( "floor" );
		ignores.Add( "player" );
		if ( other.Tags.HasAny(ignores)) return;
		var infoComponent = other.GameObject.Components.Get<Info>();
		var nametag = other.GameObject.Children.FirstOrDefault();

		if ( nametag is not null)
		{
			var screenComponent = nametag.Components.Get<Nametag>();
			if ( screenComponent is not null )
			{
				if ( infoComponent.Team == containerTeam )
				{
					screenComponent.state = 1;
					SoundEvent correctSound = Cloud.SoundEvent( "exorealms.66d09d44dcbf5e0c" );
					correctSound.Volume = .1f;
					Sound.Play( correctSound, other.Transform.Position );
					if (infoComponent.Economy.IsValid)
						infoComponent.Economy.shouldSell = true;
				}
				else
				{
					screenComponent.state = 2;
					SoundEvent wrongSound = Cloud.SoundEvent( "dopamine.airlock_alarm" );
					wrongSound.Volume = .1f;
					Sound.Play( wrongSound, other.Transform.Position );
				}
			}
		}

	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
		TagSet ignores = new TagSet();
		ignores.Add( "crate" );
		ignores.Add( "floor" );
		ignores.Add( "player" );
		if ( other.Tags.HasAny( ignores ) ) return;
		var infoComponent = other.GameObject.Components.Get<Info>();
		var nametag = other.GameObject.Children.FirstOrDefault();

		if ( infoComponent != null )
		{
			if ( infoComponent.Economy.IsValid )
				infoComponent.Economy.shouldSell = false;
		}
		if ( nametag is not null )
		{
			var screenComponent = nametag.Components.Get<Nametag>();
			if ( screenComponent is not null )
			{
				screenComponent.state = 0;
			}
		}

	}
}
