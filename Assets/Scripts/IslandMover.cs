using UnityEngine;

public class IslandMover : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] Transform despawnPoint;
    [SerializeField] float moveSpeed = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, despawnPoint.position, Time.deltaTime * moveSpeed);

        if(gameObject.transform.position.z >= despawnPoint.position.z)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, spawnPoint.position.z);
        }
    }
}
