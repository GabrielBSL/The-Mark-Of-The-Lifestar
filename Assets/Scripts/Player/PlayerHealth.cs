﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerHealth : MonoBehaviour, IHitable
    {
        PlayerController playerController;
        Rigidbody2D rb;

        [Header("Health")]
        [SerializeField] private float maxHealth;

        [Header("Stun")]
        [SerializeField] private float stunTime;
        [SerializeField] private float stunHandleLimit;
        [SerializeField] private float stunValueDecreaseOverSecond;

        private float stunValue = 0;
        private float stunTimer = 0;

        private float currentHealth;
        private bool isAlive = true;
        private bool isStunned = false;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            rb = GetComponent<Rigidbody2D>();
            currentHealth = maxHealth;
        }

        private void Update()
        {
            if (isStunned)
            {
                if (stunTimer < 0) isStunned = false;
                else stunTimer -= Time.deltaTime;
            }

            if (stunValue > 0) stunValue -= stunValueDecreaseOverSecond * Time.deltaTime;
        }

        public void RegisterHit(float damage, float stun, float direction)
        {
            if (!isAlive) return;

            isStunned = false;
            stunTimer = 0;

            currentHealth -= damage;
            stunValue = playerController.GetFacingDirection() == direction ? stunValue + stun * 2 : stunValue + stun;

            if (currentHealth <= 0)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                playerController.PlayAnimation(PlayerAnimationsList.p_death);
                playerController.EnablePlayerInputs(false);
                isAlive = false;
            }
            else if (stunValue >= stunHandleLimit)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                playerController.PlayAnimation(PlayerAnimationsList.p_hurt);
                isStunned = true;
                stunValue = 0;
                stunTimer = stunTime;
            }
        }

        //-----------------------------------------------------------------
        //**********                Get Functions                **********
        //-----------------------------------------------------------------

        public bool GetIsAlive()
        {
            return isAlive;
        }
    }
}
