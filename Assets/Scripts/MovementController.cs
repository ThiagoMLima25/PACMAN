using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class MovementController : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject currentNode;
    public float speed = 4f;

    public string direction = "";
    public string lastMovingDirection = "";

    public bool canWarp = true;
    public bool isGhost = false;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.gameIsRunning) return;

        NodeController currentNodeController = currentNode.GetComponent<NodeController>();
        transform.position = Vector2.MoveTowards(transform.position, currentNode.transform.position, speed * Time.deltaTime);
        bool reverseDirection = false;
        if ((direction == "left" && lastMovingDirection == "right") ||
            (direction == "right" && lastMovingDirection == "left") ||
            (direction == "up" && lastMovingDirection == "down") ||
            (direction == "down" && lastMovingDirection == "up")) reverseDirection = true;

        //Figure out if we`re at the center of our current node
        if ((transform.position.x == currentNode.transform.position.x && transform.position.y == currentNode.transform.position.y) || reverseDirection) {
            if (isGhost) {
                GetComponent<EnemyController>().ReachedCenterOfNode(currentNodeController);
            }

            //If we reached the center of the left warp, warp to the right warp
            if (currentNodeController.isWarpLeftNode && canWarp) {
                currentNode = gameManager.RightWarpNode;
                direction = "left";
                lastMovingDirection = "left";
                transform.position = currentNode.transform.position;
                canWarp = false;
            }
            //If we reached the center of the right warp, warp to the left warp
            else if (currentNodeController.isWarpRightNode && canWarp) {
                currentNode = gameManager.LeftWarpNode;
                direction = "right";
                lastMovingDirection = "right";
                transform.position = currentNode.transform.position;
                canWarp = false;
            } else {
                //If we are not a ghost that is respawing and we are on the start node and we are trying to move down, stop
                if (currentNodeController.isGhostStartingNode && direction == "down" 
                && (!isGhost || GetComponent<EnemyController>().ghostNodeState != EnemyController.GhostNodeState.respawing)) {
                    direction = lastMovingDirection;
                }

                //Get the next node from our node controller using our current direction
                GameObject newNode = currentNodeController.GetNodeFromDirection(direction);

                //if we can move in the desired direction
                if (newNode != null) {
                    currentNode = newNode;
                    lastMovingDirection = direction;
                }

                //if we can`t move in the desired direction, keep going in the last direction
                else {
                    direction = lastMovingDirection;
                    newNode = currentNodeController.GetNodeFromDirection(direction);

                    if (newNode != null) {
                        currentNode = newNode;
                    }
                }
            }
        } else {
            canWarp = true;
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void SetDirection(string newDirection)
    {
        direction = newDirection;
    }
}
