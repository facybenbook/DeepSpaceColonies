/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Component that allows ships, structures, planets, and moons to hold resources
    - Provides functions to add resources to this component's inventory
 ****************************************************/
using UnityEngine;
using System.Collections;

public class Inventory : MonoBehaviour
{
    //How much energy this object can hold
    public int maxEnergy = 0;
    public int currentEnergy = 0;

    //How many minerals this object can hold
    public int maxMinerals = 0;
    public int currentMinerals = 0;

    //How many colonies this object can hold
    public int maxColonies = 0;
    public int currentColonies = 0;



    //Function called on the first frame
    private void Start()
    {
        //Makes sure the current values for resources are within acceptable bounds
        this.CheckMinMax();
    }


    //Function called from FillInventory and externally. Returns the amount of energy that was taken
    public int AddEnergy(int energy_ = 0)
    {
        int takenEnergy = 0;

        //Prevents us from taking a negative value
        if(energy_ < 0)
        {
            Debug.LogWarning("WARNING: Inventory.AddEnergy, only positive integers can be added");
            return takenEnergy;
        }

        //Only adds energy up the the max
        if(energy_ + this.currentEnergy > this.maxEnergy)
        {
            takenEnergy = this.maxEnergy - this.currentEnergy;
            this.currentEnergy += this.maxEnergy - this.currentEnergy;
        }
        //Adds all of the energy if there's enough room
        else
        {
            takenEnergy = energy_;
            this.currentEnergy += energy_;
        }

        //Returns the amount of energy taken
        return takenEnergy;
    }


    //Function called from FillInventory and externally. Returns the amount of minerals taken
    public int AddMinerals(int minerals_ = 0)
    {
        int takenMinerals = 0;

        //Prevents us from taking a negative value
        if (minerals_ < 0)
        {
            Debug.LogWarning("WARNING: Inventory.AddMinerals, only positive integers can be added");
            return takenMinerals;
        }

        //Only adds minerals up to the max
        if (minerals_ + this.currentMinerals > this.maxMinerals)
        {
            takenMinerals = this.maxMinerals - this.currentMinerals;
            this.currentMinerals += this.maxMinerals - this.currentMinerals;
        }
        //Adds all of the minerals if there's enough room
        else
        {
            takenMinerals = minerals_;
            this.currentMinerals += minerals_;
        }

        //Returns the amount of minerals taken
        return takenMinerals;
    }


    //Function called from FillInventory and externally. Returns the amount of colonies taken
    public int AddColonies(int colonies_ = 0)
    {
        int takenColonies = 0;

        //Prevents us from taking a negative value
        if (colonies_ < 0)
        {
            Debug.LogWarning("WARNING: Inventory.AddColonies, only positive integers can be added");
            return takenColonies;
        }

        //Only adds colonies up to the max
        if (colonies_ + this.currentColonies > this.maxColonies)
        {
            takenColonies = this.maxColonies - this.currentColonies;
            this.currentColonies += this.maxColonies - this.currentColonies;
        }
        //Adds all of the colonies if there's enough room
        else
        {
            takenColonies = colonies_;
            this.currentColonies += colonies_;
        }

        //Returns the amount of colonies taken
        return takenColonies;
    }


    //Function called from Start. Prevents all of the supplies from dropping below 0 and going above their maximum
    private void CheckMinMax()
    {
        //Checking Energy values
        if (this.currentEnergy > this.maxEnergy)
            this.currentEnergy = this.maxEnergy;
        else if (this.currentEnergy < 0)
            this.currentEnergy = 0;

        //Checking Mineral values
        if (this.currentMinerals > this.maxMinerals)
            this.currentMinerals = this.maxMinerals;
        else if (this.currentMinerals < 0)
            this.currentMinerals = 0;

        //Checking Colony values
        if (this.currentColonies > this.maxColonies)
            this.currentColonies = this.maxColonies;
        else if (this.currentColonies < 0)
            this.currentColonies = 0;
    }


    //Function called externally. Gives the target inventory object as many supplies as possible from this inventory
    public void FillInventory(Supplies supplyType_, Inventory objectToReceive_)
    {
        switch(supplyType_)
        {
            //Gives the other object as much energy as possible
            case Supplies.Energy:
                this.currentEnergy -= objectToReceive_.AddEnergy(this.currentEnergy);
                break;

            //Gives the other object as many minerals as possible
            case Supplies.Minerals:
                this.currentMinerals -= objectToReceive_.AddMinerals(this.currentMinerals);
                break;

            //Gives the other object as many colonies as possible
            case Supplies.Colonies:
                this.currentColonies -= objectToReceive_.AddColonies(this.currentColonies);
                break;
            
            //Gives the other object as many resources as possible
            case Supplies.All:
                this.currentEnergy -= objectToReceive_.AddEnergy(this.currentEnergy);
                this.currentMinerals -= objectToReceive_.AddMinerals(this.currentMinerals);
                this.currentColonies -= objectToReceive_.AddColonies(this.currentColonies);
                break;
        }
    }
}


//Enum for the different types of supplies to be held in Inventory.cs
public enum Supplies
{
    All,
    Energy,
    Minerals,
    Colonies
}