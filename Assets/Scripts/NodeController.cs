using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class NodeController : MonoBehaviour
{
    public bool canMoveUp = false;
    public bool canMoveLeft = false;
    public bool canMoveRight = false;
    public bool canMoveDown = false;

    public GameObject nodeUp;
    public GameObject nodeLeft;
    public GameObject nodeRight;
    public GameObject nodeDown;

    public bool isWarpRightNode = false;
    public bool isWarpLeftNode = false;

    //If the node contains  a pellet when the game starts
    public bool isPelletNode = false;
    //If the node still has a pellet
    public bool hasPellet = false;

    public SpriteRenderer pelletSprite;

    public GameManager gameManager;

    public bool isGhostStartingNode = false;
    public bool isSideNode = false;
    public bool isPowerPellet = false;

    public float powerPelletBlinkingTimer = 0;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (transform.childCount > 0) {
            gameManager.GotPelletFromNodeController(this);
            hasPellet = true;
            isPelletNode = true;
            pelletSprite = GetComponentInChildren<SpriteRenderer>();
        }

        RaycastHit2D[] hitsUp;
        //Shoot raycast(line) going up
        hitsUp = Physics2D.RaycastAll(transform.position, Vector2.up);

        for (int i = 0; i < hitsUp.Length; i++) {
            float distance = Mathf.Abs(hitsUp[i].point.y - transform.position.y);
            if (distance < 0.4f && hitsUp[i].collider.tag == "Node") {
                canMoveUp = true;
                nodeUp = hitsUp[i].collider.gameObject;
            }
        }

        RaycastHit2D[] hitsLeft;
        //Shoot raycast(line) going left
        hitsLeft = Physics2D.RaycastAll(transform.position, Vector2.left);

        for (int i = 0; i < hitsLeft.Length; i++) {
            float distance = Mathf.Abs(hitsLeft[i].point.x - transform.position.x);
            if (distance < 0.4f && hitsLeft[i].collider.tag == "Node") {
                canMoveLeft = true;
                nodeLeft = hitsLeft[i].collider.gameObject;
            }
        }

        RaycastHit2D[] hitsRight;
        //Shoot raycast(line) going right
        hitsRight = Physics2D.RaycastAll(transform.position, Vector2.right);

        for (int i = 0; i < hitsRight.Length; i++) {
            float distance = Mathf.Abs(hitsRight[i].point.x - transform.position.x);
            if (distance < 0.4f && hitsRight[i].collider.tag == "Node") {
                canMoveRight = true;
                nodeRight = hitsRight[i].collider.gameObject;
            }
        }

        RaycastHit2D[] hitsDown;
        //Shoot raycast(line) going down
        hitsDown = Physics2D.RaycastAll(transform.position, -Vector2.up);

        for (int i = 0; i < hitsDown.Length; i++) {
            float distance = Mathf.Abs(hitsDown[i].point.y - transform.position.y);
            if (distance < 0.4f && hitsDown[i].collider.tag == "Node") {
                canMoveDown = true;
                nodeDown = hitsDown[i].collider.gameObject;
            }
        }

        if (isGhostStartingNode) {
            canMoveDown = true;
            nodeDown = gameManager.ghostNodeCenter;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.gameIsRunning) return;

        if (isPowerPellet && hasPellet ) {
            powerPelletBlinkingTimer += Time.deltaTime;
            if (powerPelletBlinkingTimer >= 0.1f) {
                powerPelletBlinkingTimer = 0;
                pelletSprite.enabled = !pelletSprite.enabled;
            }
        }
    }

    public void RespawnPeliet()
    {
        if (isPelletNode) { 
            hasPellet = true;
            pelletSprite.enabled = true;
        }
    }

    public GameObject GetNodeFromDirection(string direction)
    {
        if (direction == "left" && canMoveLeft) {
            return nodeLeft;
        }

        if (direction == "right" && canMoveRight) {
            return nodeRight;
        }

        if (direction == "up" && canMoveUp) {
            return nodeUp;
        }

        if (direction == "down" && canMoveDown) {
            return nodeDown;
        }

        return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && hasPellet) {
            hasPellet = false;
            pelletSprite.enabled = false;
            StartCoroutine(gameManager.CollectedPellet(this));
        }
    }
}
