using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public MovementController movementController;

    public SpriteRenderer sprite;
    public Animator animator;

    public GameObject startNode;

    public Vector2 startPosition;

    public GameManager gameManager;
    
    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        startPosition = new Vector2(-0.03f, -0.67f);
        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        movementController = GetComponent<MovementController>();
        startNode = movementController.currentNode;
    }

    public void Setup()
    {
        movementController.currentNode = startNode;
        movementController.lastMovingDirection = "left";
        movementController.direction = "left";
        movementController.SetSpeed(2);
        sprite.flipX = false;
        transform.position = startPosition;
     
        animator.SetBool("moving", false);
        animator.SetBool("dead", false);
        animator.speed= 1;
    }

    public void StopAnimation()
    {
        animator.speed= 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.gameIsRunning) {
            return;
        }

        animator.SetBool("moving", true);
        if (Input.GetKey(KeyCode.LeftArrow)) {
            movementController.SetDirection("left");
        } else if (Input.GetKey(KeyCode.RightArrow)) {
            movementController.SetDirection("right");
        } else if (Input.GetKey(KeyCode.UpArrow)) {
            movementController.SetDirection("up");
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            movementController.SetDirection("down");
        }

        bool flipX = false;
        bool flipY = false;
        if (movementController.lastMovingDirection == "left") {
            animator.SetInteger("direction", 0);
        } else if (movementController.lastMovingDirection == "right") {
            animator.SetInteger("direction", 0);
            flipX = true;
        } else if (movementController.lastMovingDirection == "up") {
            animator.SetInteger("direction", 1);
        } else if (movementController.lastMovingDirection == "down") {
            animator.SetInteger("direction", 1);
            flipY = true;
        }

        sprite.flipX= flipX;
        sprite.flipY= flipY;
    }

    public void Death()
    {
        animator.speed = 1;
        animator.SetBool("moving", false);
        animator.SetBool("dead", true);
    }
}
