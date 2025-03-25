using System.Collections;
using System.Collections.Generic;
using Terresquall;
using UnityEngine;
using Terresquall;

public class PlayerMovement : MonoBehaviour
{
    //Movement
    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]

    public float lastVerticalVector;
    
    [HideInInspector]
    public Vector2 moveDir;
    [HideInInspector]
    public Vector2 lastMovedVector;

    [SerializeField] private GameObject joystick; // Keeps it private but still assignable in the Inspector


    //References
    Rigidbody2D rb;
    PlayerStats player;

    void Start()
    {
        player = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        lastMovedVector = new Vector2(1, 0f); // se o jogo comecar e o player nao se mover, ainda sim a faca vai sair pra direita
    }

    void Update()
    {
        InputManagement();
    }

    void FixedUpdate()
    {
        Move();
    }

    void InputManagement()
    {

        if(GameManager.instance.isGameOver || GameManager.instance.isGamePaused)
        {
            return;
        }
        float moveX, moveY;    
        if(joystick != null && joystick.gameObject.activeInHierarchy)
        {
            moveX = VirtualJoystick.GetAxisRaw("Horizontal");
            moveY = VirtualJoystick.GetAxisRaw("Vertical");
        } else
        {
            moveX = Input.GetAxisRaw("Horizontal");
            moveY = Input.GetAxisRaw("Vertical");
        }



        moveDir = new Vector2(moveX, moveY).normalized;

        if(moveDir.x != 0)
        {
            lastHorizontalVector = moveDir.x;
            lastMovedVector = new Vector2(lastHorizontalVector, 0f); //ultimo x
        }
        if(moveDir.y != 0)
        {
            lastVerticalVector = moveDir.y;
            lastMovedVector = new Vector2(0f, lastVerticalVector); //ultimo y
        }

        if(moveDir.x != 0 && moveDir.y != 0)
        {
            lastMovedVector = new Vector2(lastHorizontalVector, lastVerticalVector); //enquanto estiver se mexendo
        }
    }

    void Move()
    {
         if(GameManager.instance.isGameOver || GameManager.instance.isGamePaused)
        {
            return;
        }
        rb.linearVelocity = new Vector2(moveDir.x * player.CurrentMoveSpeed, moveDir.y * player.CurrentMoveSpeed);
    }
}