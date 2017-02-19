using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Devdog.General;
using Devdog.General.UI;
using Devdog.InventoryPro;
using Devdog.InventoryPro.UI;
using UnityEngine;

namespace Devdog.General
{
    [DisallowMultipleComponent]
    public partial class MyPlayer2D : Player
    {

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

        protected override void SetTriggerHandler()
        {
            var obj = new GameObject("_TriggerHandler2D");
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            obj.gameObject.layer = 2; // Ignore raycasts

            var handler = obj.AddComponent<PlayerTriggerHandler2D>();
            handler.player = this;
            handler.selector = triggerSelector;

            triggerHandler = handler;
        }

        protected override void Start()
        {
            updateSliders();
        }

        public void restoreHealth()
        {
            health = maxHealth;
            updateHealthSlider();
        }

        public void addHealth (int amount)
        {
            health += amount;
            if (health > maxHealth) {
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
}