using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameManager gameManager;
    public AudioManager audioManager;

    public float speed;
    public float jumpForce;
    public float launchForce;
    public float enemyKillForce;

    public int extraJumps;
    public int extraLaunches;

    public GameObject playerDeathParticle;

    private Rigidbody2D rb;
    private Vector3 velocity;

    private bool isGrounded;
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    private bool canLaunch;

    private int extraJumpCounter;
    private int extraLaunchCounter;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        if (gameManager.isGameActive)
        {
            Movement();
        }
    }
    void Update()
    {
        if (gameManager.isGameActive)
        {
            Jump();
            Launch();
            if (Input.GetButtonDown("Cancel") && gameObject.activeSelf)
            {
                Death();
            }
        }
    }
    private void Movement()
    {
        float dt = Time.deltaTime;

        float horizontalInput = Input.GetAxis("Horizontal");

        velocity = new Vector3(horizontalInput, 0) * speed * dt;

        transform.position += velocity;
        
        if (horizontalInput != 0)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }
    private void Jump()
    {
        float dt = Time.deltaTime;

        if (isGrounded || extraJumpCounter < extraJumps)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= dt;
        }
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= dt;
        }
        if (coyoteTimeCounter > 0 && jumpBufferCounter > 0)
        {
            if (rb.velocity.y < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpBufferCounter = 0;
            if (!isGrounded)
            {
                extraJumpCounter++;
            }
            isGrounded = false;
        }
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            coyoteTimeCounter = 0;
        }
    }
    private void Launch()
    {
        if (Input.GetButtonDown("Fire1") && (canLaunch || extraLaunchCounter < extraLaunches))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 launchDirection = mousePos - new Vector2(transform.position.x, transform.position.y);
            launchDirection.Normalize();

            if (rb.velocity.y < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
            rb.velocity = new Vector2(0, rb.velocity.y);
            rb.AddForce(launchDirection * launchForce, ForceMode2D.Impulse);
            if (!canLaunch)
            {
                extraLaunchCounter++;
            }
            canLaunch = false;
        }
    }
    private void Death()
    {
        Instantiate(playerDeathParticle, transform.position, transform.rotation);
        gameObject.SetActive(false);
        gameManager.PlayerDeath();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            canLaunch = true;
            extraJumpCounter = 0;
            extraLaunchCounter = 0;
            rb.velocity = Vector2.zero;
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            isGrounded = true;
            canLaunch = true;
            extraJumpCounter = 0;
            extraLaunchCounter = 0;
            if (rb.velocity.y < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
            rb.AddForce(Vector2.up * enemyKillForce, ForceMode2D.Impulse);
            gameManager.score += 2;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Lava"))
        {
            Death();
        }
    }
}
