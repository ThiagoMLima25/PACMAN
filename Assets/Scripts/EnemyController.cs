using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum GhostNodeState
    {
        respawing,
        leftNode,
        rightNode,
        centerNode,
        startNode,
        movingInNodes
    }

    public enum GhostType
    {
        red,
        blue,
        pink,
        orange
    }

    public bool readyToLeaveHome = false;
    public bool isFrightened;
    public bool leftHomeBefore = false;
    public bool isVisible;

    public GhostNodeState ghostNodeState;
    public GhostNodeState startGhostNodeState;
    public GhostNodeState respawnState;
    public GhostType ghostType;

    public GameObject startingNode;
    public GameObject ghostNodeStart;
    public GameObject ghostNodeCenter;
    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    
    public GameObject[] scatterNodes;
    public int scatterNodeIndex;

    public MovementController movementController;

    public GameManager gameManager;

    public SpriteRenderer ghostSprite;
    public SpriteRenderer eyesSprite;

    public Animator animator;
    public Color color;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        ghostSprite.GetComponent<SpriteRenderer>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        movementController = GetComponent<MovementController>();

        if (ghostType == GhostType.red) {
            startGhostNodeState = GhostNodeState.startNode;
            respawnState = GhostNodeState.centerNode;
            startingNode = ghostNodeStart;
        } else if (ghostType == GhostType.pink) {
            startGhostNodeState = GhostNodeState.centerNode;
            startingNode = ghostNodeCenter;
            respawnState = GhostNodeState.centerNode;
        } else if (ghostType == GhostType.blue) {
            startGhostNodeState = GhostNodeState.leftNode;
            respawnState = GhostNodeState.leftNode;
            startingNode = ghostNodeLeft;
        } else if (ghostType == GhostType.orange) {
            startGhostNodeState = GhostNodeState.rightNode;
            respawnState = GhostNodeState.rightNode;
            startingNode = ghostNodeRight;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.isPowerPelletRuning || ghostNodeState != GhostNodeState.movingInNodes) isFrightened= false;

        if (isVisible) {
            if (ghostNodeState != GhostNodeState.respawing) {
                ghostSprite.enabled = true;
            } else {
                ghostSprite.enabled = false;
            }
            eyesSprite.enabled = true;
        } else {
            ghostSprite.enabled = false;
            eyesSprite.enabled = false;
        }

        if (isFrightened) {
            animator.SetBool("frightened", true);
            eyesSprite.enabled = false;
            ghostSprite.color = new Color(255, 255, 255, 255);
        } else {
            animator.SetBool("frightened", false);
            animator.SetBool("frightenedBlinking", false);
            ghostSprite.color = color;
        }

        if (!gameManager.gameIsRunning) return;

        if (gameManager.powerPelletTimer - gameManager.currentPowerPelletTime <= 3) {
            animator.SetBool("frightenedBlinking", true);
        } else {
            animator.SetBool("frightenedBlinking", false);
        }

        animator.SetBool("moving", true);

        if (movementController.currentNode.GetComponent<NodeController>().isSideNode) {
            movementController.SetSpeed(1);
        } else {
            if (isFrightened) {
                movementController.SetSpeed(1);
            } else if (ghostNodeState == GhostNodeState.respawing) {
                movementController.SetSpeed(5);
            } else {
                movementController.SetSpeed(2);
            }
        }
    }

    public void Setup()
    {
        animator.SetBool("moving", false);
        ghostNodeState = startGhostNodeState;
        readyToLeaveHome = false;

        //Reset our ghosts back to their home position
        movementController.currentNode = startingNode;
        transform.position = startingNode.transform.position;

        movementController.direction = "";
        movementController.lastMovingDirection = "";

        //Set their scatter node index back to 0
        scatterNodeIndex = 0;

        //Set isFreightened
        isFrightened = false;
        leftHomeBefore = false;

        //Set readyToLeaveHome to be false if they are blue or pink
        if (ghostType == GhostType.red) {
            readyToLeaveHome = true;
            leftHomeBefore = true;
        } else if (ghostType == GhostType.pink) {
            readyToLeaveHome = true;
        }

        SetVisible(true);
    }

    string GetClosestDirection(Vector2 target)
    {
        float shortestDistance = 0;
        string lastMovingDirection = movementController.lastMovingDirection;
        string newDirection = "";
        NodeController nodeController = movementController.currentNode.GetComponent<NodeController>();
        
        //If we can move up and we aren`t reversing
        if (nodeController.canMoveUp && lastMovingDirection != "down") {
            GameObject nodeUp = nodeController.nodeUp;
            //Get the distance between our top node and pacman
            float distance = Vector2.Distance(nodeUp.transform.position, target);
            //If this is the shortest distance so far, set our direction
            if (distance < shortestDistance || shortestDistance == 0) {
                shortestDistance = distance;
                newDirection = "up";
            }
        }

        if (nodeController.canMoveDown && lastMovingDirection != "up") {
            GameObject nodeDown = nodeController.nodeDown;
            //Get the distance between our down node and pacman
            float distance = Vector2.Distance(nodeDown.transform.position, target);
            //If this is the shortest distance so far, set our direction
            if (distance < shortestDistance || shortestDistance == 0) {
                shortestDistance = distance;
                newDirection = "down";
            }
        }

        if (nodeController.canMoveRight && lastMovingDirection != "left") {
            GameObject nodeRight = nodeController.nodeRight;
            //Get the distance between our right node and pacman
            float distance = Vector2.Distance(nodeRight.transform.position, target);
            //If this is the shortest distance so far, set our direction
            if (distance < shortestDistance || shortestDistance == 0) {
                shortestDistance = distance;
                newDirection = "right";
            }
        }

        if (nodeController.canMoveLeft && lastMovingDirection != "right") {
            GameObject nodeLeft = nodeController.nodeLeft;
            //Get the distance between our left node and pacman
            float distance = Vector2.Distance(nodeLeft.transform.position, target);
            //If this is the shortest distance so far, set our direction
            if (distance < shortestDistance || shortestDistance == 0) {
                shortestDistance = distance;
                newDirection = "left";
            }
        }

        return newDirection;
    }

    void DetermineRedGhostDirection()
    {
        string direction = GetClosestDirection(gameManager.pacman.transform.position);
        movementController.SetDirection(direction);
    }

    void DeterminePinkGhostDirection()
    {
        string pacmanDirection = gameManager.pacman.GetComponent<MovementController>().lastMovingDirection;
        float distanceBetweenNodes = 0.35f;

        Vector2 target = gameManager.pacman.transform.position;
        if (pacmanDirection == "left") {
            target.x -= distanceBetweenNodes * 2;
        } else if (pacmanDirection == "right") {
            target.x += distanceBetweenNodes * 2;
        } else if (pacmanDirection == "up") {
            target.y += distanceBetweenNodes * 2;
        } else if (pacmanDirection == "down") {
            target.y -= distanceBetweenNodes * 2;
        }

        string direction = GetClosestDirection(target);
        movementController.SetDirection(direction);
    }

    void DetermineBlueGhostDirection()
    {
        string pacmanDirection = gameManager.pacman.GetComponent<MovementController>().lastMovingDirection;
        float distanceBetweenNodes = 0.35f;

        Vector2 target = gameManager.pacman.transform.position;
        if (pacmanDirection == "left") {
            target.x -= distanceBetweenNodes * 2;
        } else if (pacmanDirection == "right") {
            target.x += distanceBetweenNodes * 2;
        } else if (pacmanDirection == "up") {
            target.y += distanceBetweenNodes * 2;
        } else if (pacmanDirection == "down") {
            target.y -= distanceBetweenNodes * 2;
        }

        GameObject redGhost = gameManager.redGhost;
        float xDistance = target.x - redGhost.transform.position.x;
        float yDistance = target.y - redGhost.transform.position.y;

        Vector2 blueTarget = new Vector2(target.x + xDistance, target.y + yDistance);
        string direction = GetClosestDirection(blueTarget);
        movementController.SetDirection(direction);
    }

    void DetermineOrangeGhostDirection()
    {
        float distance = Vector2.Distance(gameManager.pacman.transform.position, transform.position);
        float distanceBetweenNodes = 0.35f;

        if (distance < 0) distance *= -1;
        //if we are within 8 nodes of pacman, chase him using red`s logic
        if (distance <= distanceBetweenNodes * 8) {
            DetermineBlueGhostDirection();
        }
        //Otherwise use SCATTER MODE
        else {
            DetermineGhostScatterModeDirection();
        }
    }

    void DetermineGhostScatterModeDirection()
    {
        if (transform.position.x == scatterNodes[scatterNodeIndex].transform.position.x && transform.position.y == scatterNodes[scatterNodeIndex].transform.position.y) {
            scatterNodeIndex++;

            if (scatterNodeIndex == scatterNodes.Length - 1) {
                scatterNodeIndex = 0;
            }
        }
        string direction = GetClosestDirection(scatterNodes[scatterNodeIndex].transform.position);
        movementController.SetDirection(direction);
    }

    string GetRandomDirection()
    {
        List<string> possibleDirections = new List<string>();
        NodeController nodeController = movementController.currentNode.GetComponent<NodeController>();

        if (nodeController.canMoveDown && movementController.lastMovingDirection != "up") {
            possibleDirections.Add("down");
        } else if (nodeController.canMoveUp && movementController.lastMovingDirection != "down") {
            possibleDirections.Add("up");
        } else if (nodeController.canMoveRight && movementController.lastMovingDirection != "left") {
            possibleDirections.Add("right");
        } else if (nodeController.canMoveLeft && movementController.lastMovingDirection != "right") {
            possibleDirections.Add("left");
        }

        string direction = "";
        int randomDirectionIndex = Random.Range(0, possibleDirections.Count - 1);
        direction = possibleDirections[randomDirectionIndex];
        return direction;
    }

    public void ReachedCenterOfNode(NodeController nodeController)
    {
        if (ghostNodeState == GhostNodeState.movingInNodes) {
            leftHomeBefore = true;

            //SCATTER MODE
            if (gameManager.currentGhostMode == GameManager.GhostMode.scatter) {
                DetermineGhostScatterModeDirection();
            }
            //FRIGHTENE MODE
            else if (isFrightened) {
                string direction = GetRandomDirection();
                movementController.SetDirection(direction);
            }
            //CHASE MODE
            else {
                //DETERMINE NEXT GAME NODE TO GO TO
                if (ghostType == GhostType.red) {
                    DetermineRedGhostDirection();
                } else if (ghostType == GhostType.pink) {
                    DeterminePinkGhostDirection();
                } else if (ghostType == GhostType.blue) {
                    DetermineBlueGhostDirection();
                } else if (ghostType == GhostType.orange) {
                    DetermineOrangeGhostDirection();
                }
            }
        } else if (ghostNodeState == GhostNodeState.respawing) {
            string direction = "";

            //We have reached our start node, move to the center node
            if (transform.position.x == ghostNodeStart.transform.position.x && transform.position.y == ghostNodeStart.transform.position.y) {
                direction = "down";
            }
            //We have reached our center, either finish respawn or move to the left/right node
            else if (transform.position.x == ghostNodeCenter.transform.position.x && transform.position.y == ghostNodeCenter.transform.position.y) {
                if (respawnState == GhostNodeState.centerNode) {
                    ghostNodeState = respawnState;
                } else if (respawnState == GhostNodeState.leftNode) {
                    direction = "left";
                } else if (respawnState == GhostNodeState.rightNode) {
                    direction = "right";
                }
            }
            //If our respawn state is either the left or right node and we got to that node, leave home again
            else if (transform.position.x == ghostNodeLeft.transform.position.x && transform.position.y == ghostNodeLeft.transform.position.y ||
            transform.position.x == ghostNodeRight.transform.position.x && transform.position.y == ghostNodeRight.transform.position.y) {
                ghostNodeState = respawnState;
            } else {
                // Ghost is in the board. Determine quickest direction to home
                direction = GetClosestDirection(ghostNodeStart.transform.position);
            }

            movementController.SetDirection(direction);
        } else {
            //if we are ready to leave home
            if (readyToLeaveHome) {
                //if we are in the left, move to the center
                if (ghostNodeState == GhostNodeState.leftNode) {
                    ghostNodeState = GhostNodeState.centerNode;
                    movementController.SetDirection("right");
                }
                //if we are in the right move to the center
                else if (ghostNodeState == GhostNodeState.rightNode) {
                    ghostNodeState = GhostNodeState.centerNode;
                    movementController.SetDirection("left");
                }
                //if we are in the center, move to start node
                else if (ghostNodeState == GhostNodeState.centerNode) {
                    ghostNodeState = GhostNodeState.startNode;
                    movementController.SetDirection("up");
                }
                //if we are in start, move around
                else if (ghostNodeState == GhostNodeState.startNode) {
                    ghostNodeState = GhostNodeState.movingInNodes;
                    movementController.SetDirection("left");
                }
            }
        }
    }

    public void SetVisible(bool newIsVisible)
    {
        isVisible = newIsVisible;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && ghostNodeState != GhostNodeState.respawing) {
            //Get Eaten
            if (isFrightened) {
                gameManager.GhostEaten();
                ghostNodeState = GhostNodeState.respawing;
            }
            //Eat Player
            else {
                StartCoroutine(gameManager.PlayerEaten());
            }
        }
    }

    public void setFrighened(bool newIsFrighened)
    {
        isFrightened = newIsFrighened;
    }
}
