using Godot;
using System;
using System.Collections.Generic;



public class EMPGrenade : HandGrenade
{
    public static string ProjectileResource = "res://Scenes/HandGrenades/EMPGrenade.tscn";

    public EMPGrenade()
    {
        _damage = 0;
    }

    override protected void PrimeTimeFinished()
    {
        // explode
        this.Explode();
    }

    override public void Explode()
    {
        // any players nearby
        object[] result = this.FindPlayersInRadius();

        foreach (Dictionary<object, object>  r in result) {
            if (r["collider"] is Player pl)
            {
                // calc damage per ammunition
                float damage = pl.CurrentShells * .75f + pl.CurrentRockets * 1.5f + pl.CurrentCells * .75f;

                // remove 90% of ammunition for players affected
                pl.CurrentShells = (int)Math.Floor(pl.CurrentShells * .9);
                pl.CurrentRockets = (int)Math.Floor(pl.CurrentRockets * .9);
                pl.CurrentCells = (int)Math.Floor(pl.CurrentCells * .9);

                // inflict damage
                pl.TakeDamage(this.Transform, this.GetType().ToString().ToLower(), 0, this._playerOwner, damage);
            }

            // ammunition boxes, backpacks, pipebombs, buildings
        }
        
        this.FinishExplode();
    }
}