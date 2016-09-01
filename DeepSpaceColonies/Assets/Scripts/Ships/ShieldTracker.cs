/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Component to keep track of how much shield a game object (like ships or structures) has
 ****************************************************/
using UnityEngine;
using System.Collections;

public class ShieldTracker : MonoBehaviour
{
    //The maximum shield this object can have
    public int maxShield = 10;
    //The current amount of shield this object has
    public int currentShield = 10;


    //Damages this shield and returns an int for how much damage passes through
	public int Damage(int damageTaken_)
    {
        int returnDamage = 0;

        //If the shield is greater than the amount of damage taken, that much damage is taken out of it
        if(damageTaken_ <= this.currentShield)
        {
            this.currentShield -= damageTaken_;
            return 0;
        }
        //Otherwise, damage gets through the shield
        else
        {
            returnDamage = damageTaken_ - this.currentShield;
            this.currentShield = 0;
        }

        return returnDamage;
    }
	

    //Adds to the current strength of this shield up to its maximum
    public void RestoreShield(int shieldAdded_)
    {
        //Does nothing if the amount added is negative
        if (shieldAdded_ < 0)
            return;

        this.currentShield += shieldAdded_;

        if (this.currentShield > this.maxShield)
            this.currentShield = this.maxShield;
    }
}
