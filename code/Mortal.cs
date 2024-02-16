using Sandbox;
using System;

public sealed class Mortal : Component
{
	[Property]
	public float MaxHealth { get; set; } = 100f;
	[Property]
	public float HealthRegen { get; set; } = .5f;
	[Property]
	public float HealthRegenDelay { get; set; } = 3f;

	public float HealthRegenInterval { get; set; } = 1f;
	public float Health { get; private set; }
	public bool Alive { get; private set; } = true;

	TimeSince lastDamage;
	TimeUntil nextHeal;

	protected override void OnStart()
	{
		Health = MaxHealth;
	}

	protected override void OnUpdate()
	{
		if ( lastDamage >= HealthRegenDelay && Health != MaxHealth && Alive )
		{
			if ( nextHeal )
			{
				Damage( -HealthRegen );
				nextHeal = HealthRegenInterval;
			}
		}
	}

	public void Damage( float damage )
	{
		if ( !Alive ) return;

		Health = Math.Clamp( Health - damage, 0f, MaxHealth );

		if ( damage > 0 )
			lastDamage = 0f;

		if ( Health <= 0 )
			Kill();
	}

	public void Kill()
	{
		Health = 0f;
		Alive = false;

		GameObject.Destroy();
	}
}
