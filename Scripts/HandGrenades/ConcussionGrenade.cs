using Godot;
using System;
using System.Collections.Generic;



public class ConcussionGrenade : HandGrenade
{
    public static string ProjectileResource = "res://Scenes/HandGrenades/ConcussionGrenade.tscn";
    private float _blastPower = 30;

    public ConcussionGrenade()
    {
        _damage = 0;
        _inflictLength = 10;
    }

    override protected void PrimeTimeFinished()
    {
        // when lifetime ends, call explode
        this.Explode(_blastPower);
    }

    override public void Explode(float val)
    {
        object[] result = this.FindPlayersInRadius();

        foreach (Dictionary<object, object>  r in result) {
            if (r["collider"] is Player pl)
            {
                // find how far from explosion as a percentage
                float dist = this.Transform.origin.DistanceTo(pl.Transform.origin);
                dist = dist > this._areaOfEffectRadius ? (this._areaOfEffectRadius*.99f) : dist;
                float pc = ((this._areaOfEffectRadius - dist) / this._areaOfEffectRadius);

                pl.Inflict("concussiongrenade", _inflictLength, _playerOwner);
                pl.AddVelocity(this.Transform.origin, val * (1 - pc));
            }
        }
        
        this.FinishExplode();
    }
}