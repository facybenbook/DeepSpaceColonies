/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Component to keep track of how much health a game object (like ships or structures) have
 ****************************************************/
using UnityEngine;
using System.Collections;

public class HealthTracker : MonoBehaviour
{
    //The reputation this object has toward the player
    public Reputation reputationStanding = Reputation.PlayerControlled;
    //The maximum health this object can have
    public int maxHealth = 10;
    //The current amount of health this object has
    public int currentHealth = 10;
	


    //Function called externally. Causes damage to this object and will destroy it if its health goes to 0
    public void Damage(int damageTaken_, GameObject attacker_)
    {
        //Can't inflict negative damage
        if (damageTaken_ < 0)
            return;

        //Subtracts damage from this owner's shield if they have one
        if (GetComponent<ShieldTracker>() != null)
        {
            //Any remaining damage is returned and dealt to this object's health
            this.currentHealth -= this.GetComponent<ShieldTracker>().Damage(damageTaken_);
        }
        //If there is no shield, damage is dealt directly to HP
        else
        {
            this.currentHealth -= damageTaken_;
        }

        //When the health drops below 0 it's destroyed
        if(this.currentHealth <= 0)
        {
            this.currentHealth = 0;

            //Dispatches an event saying that this object was destroyed and who the attacker was
            EVTData destroyedEVT = new EVTData();
            destroyedEVT.objectDestroyed = new ObjectDestroyedEVT(attacker_, this.gameObject);
            EventManager.TriggerEvent("ObjectWasDestroyed", destroyedEVT);
        }
    }


    //Function called externally. Restores health to this object up to its maximum
    public void Heal(int damageHealed_)
    {
        //Can't heal negative health
        if (damageHealed_ < 0)
            return;

        //Heals this object up to its max health value
        this.currentHealth += damageHealed_;

        if (this.currentHealth > this.maxHealth)
            this.currentHealth = this.maxHealth;
    }
}


//Enum that determines an object's hostility toward the player
public enum Reputation
{
    PlayerControlled,
    Hostile,
    Chaotic
}