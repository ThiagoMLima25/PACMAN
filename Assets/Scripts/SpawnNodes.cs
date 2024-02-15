using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNodes : MonoBehaviour
{
    int numberToSpaw = 28;

    public float currentSpawnOffset;
    public float spawOffset = 0.3f;

    void Start()
    {
        if (gameObject.name == "Node") {
            currentSpawnOffset = spawOffset;
            for (int i = 0; i < numberToSpaw; i++) {
                GameObject clone = Instantiate(gameObject, new Vector3(transform.position.x, transform.position.y + currentSpawnOffset, 0), Quaternion.identity);
                currentSpawnOffset += spawOffset;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
