using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float jumpTime = 0.75f;
    public float jumpHeight = 3;
    public float acceleration = 10;
    private float dashCD = .2f;
    private int dashCount = 0;
    private float dashLength = .15f;
    public float dashImpulse = 800;
    private bool lastButtonLeft = false;
    private bool isDashing = false;
    private bool hasLanded = false;
    private bool hasJumped = false;

    float gravity;
    float jumpImpulse;

    bool isGrounded = false;

    Vector3 velocity = new Vector3();
    PawnAABB pawn;

    void Start() {
        pawn = GetComponent<PawnAABB>();
        DeriveJumpValues();
    }
    void DeriveJumpValues()
    {
        gravity = (jumpHeight * 2) / (jumpTime * jumpTime);
        jumpImpulse = gravity * jumpTime;
    }

    void Update()
    {
        HandleInput();

        // Do the Move move:
        PawnAABB.CollisionResults results = pawn.Move(velocity * Time.deltaTime);
        if (results.hitTop || results.hitBottom) velocity.y = 0;
        if (results.hitLeft || results.hitRight) velocity.x = 0;

        isGrounded = results.hitBottom;

        if (isGrounded)
        {
            hasLanded = true;
            hasJumped = false;
        }

        if (!isDashing)
        {
            if (velocity.x > 10) velocity.x = 10;
            if (velocity.x < -10) velocity.x = -10;
        }

        transform.position += results.distance;
    }

    private void HandleInput()
    {
        // Gravity
        velocity.y -= gravity * Time.deltaTime;

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = jumpImpulse;
        }
        if (Input.GetButtonDown("Jump") && !isGrounded && !hasJumped)
        {
            velocity.y = jumpImpulse * .8f;
            hasJumped = true;
        }

        // Sideways movement
        float axisH = Input.GetAxisRaw("Horizontal");

        DoDash();

        if (axisH == 0)
        {
            DecelerateX(acceleration * 2);
        }
        else
        {
            AccelerateX(axisH * acceleration);
        }
        CheckDash();
    }

    private void DecelerateX(float amount)
    {
        // Slow down the player
        if (velocity.x > 0)   // Moving right
        {
            AccelerateX(-amount);
            if (velocity.x <= 0) velocity.x = 0;
        }
        else if (velocity.x < 0)   // Moving left
        {
            AccelerateX(amount);
            if (velocity.x >= 0) velocity.x = 0;
        }
    }

    private void AccelerateX(float amount)
    {
        if (velocity.x > 0 && amount < 0 || velocity.x < 0 && amount > 0)
        {
            velocity.x += amount * Time.deltaTime * 2;
        }
        else {
            velocity.x += amount * Time.deltaTime;
        }
    }

    private void DoDash()
    {
        
        if (Input.GetButtonDown("Left") || Input.GetButtonDown("Right"))
        {
            if (Input.GetButtonDown("Left")) lastButtonLeft = true;
            else lastButtonLeft = false;
            
            if (dashCount == 0)
            {
                if (lastButtonLeft && Input.GetButtonDown("Left"))
                {
                    dashCD = .2f;
                    dashCount += 1;
                }
                if (!lastButtonLeft && Input.GetButtonDown("Right"))
                {
                    dashCD = .2f;
                    dashCount += 1;
                }
            }
            else if (dashCD > 0 && dashCount == 1 && hasLanded)
            {
                isDashing = true;
            }
        }
    }

    private void CheckDash()
    {
        if (dashCD > 0)
        {
            dashCD -= Time.deltaTime;
        }
        else
        {
            dashCount = 0;
        }
        if (isDashing)
        {
            dashLength -= Time.deltaTime;
            if (dashLength >= .12f)
            {
                if (!lastButtonLeft) AccelerateX(dashImpulse);
                else AccelerateX(-dashImpulse);
            }
            if (dashLength > 0)
            {
                velocity.x *= .99f;
            }
            else
            {
                DecelerateX(velocity.x - 10);
                isDashing = false;
                dashLength = .15f;
                dashCD = .2f;
                if (!isGrounded) hasLanded = false;
            }
        }
    }
}
