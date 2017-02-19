using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyPlayer : MonoBehaviour {

    [Header("Player Starting Attributes")]
    public int health;
    public int energy;
    public int strength;
    public int defence;
    public int maxHealth;
    public int maxEnergy;

    [Header("Player Health and Energy Sliders")]
    public Slider healthSlider; // UI slider element used to display player health
    public Slider energySlider; // UI slider element used to display player energy

    // Use this for initialization
    void Start () {
        updateSliders();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void restoreHealth()
    {
        health = maxHealth;
        updateHealthSlider();
    }

    public void addHealth(int amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        updateHealthSlider();
    }

    public void restoreEnergy()
    {
        energy = maxEnergy;
        updateEnergySlider();
    }

    public void addEnergy(int amount)
    {
        energy += amount;
        if (energy > maxEnergy)
        {
            energy = maxEnergy;
        }
        updateEnergySlider();
    }

    public void updateSliders()
    {
        updateHealthSlider();
        updateEnergySlider();
    }

    public void updateHealthSlider()
    {
        healthSlider.value = health;
    }

    public void updateEnergySlider()
    {
        energySlider.value = energy;
    }
}
