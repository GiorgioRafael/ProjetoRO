using UnityEngine;
using Terresquall;

public class PlayerMovement : Sortable
{
    //define a velocidade de movimento padrão
    public const float DEFAULT_MOVESPEED = 5f;

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

    protected override void Start()
    {
        base.Start();
        player = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        lastMovedVector = new Vector2(1, 0f); // se o jogo comecar e o player nao se mover, ainda sim a faca vai sair pra direita
    }

    void Update()
    {
        if(!GameManager.instance.gameHasStarted) return;
        InputManagement();
    }

    void FixedUpdate()
    {
        if(!GameManager.instance.gameHasStarted) return;
        Move();
    }

    void InputManagement()
{
    if(!GameManager.instance.gameHasStarted) return;

    if (GameManager.instance.isGameOver || GameManager.instance.isGamePaused)
    {
        return;
    }

    float moveX = 0f;
    float moveY = 0f;

    // Keyboard input
    moveX += Input.GetAxisRaw("Horizontal");
    moveY += Input.GetAxisRaw("Vertical");

    // Joystick input (only if it's active)
    if (joystick != null && joystick.gameObject.activeInHierarchy)
    {
        moveX += VirtualJoystick.GetAxisRaw("Horizontal");
        moveY += VirtualJoystick.GetAxisRaw("Vertical");
    }

    moveDir = new Vector2(moveX, moveY).normalized;

    // Save last direction moved
    if (moveDir.x != 0)
    {
        lastHorizontalVector = moveDir.x;
        lastMovedVector = new Vector2(lastHorizontalVector, 0f);
    }
    if (moveDir.y != 0)
    {
        lastVerticalVector = moveDir.y;
        lastMovedVector = new Vector2(0f, lastVerticalVector);
    }
    if (moveDir.x != 0 && moveDir.y != 0)
    {
        lastMovedVector = new Vector2(lastHorizontalVector, lastVerticalVector);
    }
}


    void Move()
    {
         if(GameManager.instance.isGameOver || GameManager.instance.isGamePaused)
        {
            return;
        }

         rb.linearVelocity = moveDir * DEFAULT_MOVESPEED * player.Stats.moveSpeed;
        
    }
}