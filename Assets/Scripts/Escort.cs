using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escort : MonoBehaviour
{
    [SerializeField] Transform[] pathPoints;
    [SerializeField] float moveSpeed = 5;
    [SerializeField] float playerDistanceMax = 10;
    int currentPoint;
    bool isMoving;
    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(isMoving && Vector3.Distance(transform.position, player.position) <= playerDistanceMax)
        {
            print(Vector3.Distance(transform.position, player.position));
            if (Vector3.Distance(pathPoints[currentPoint].position, transform.position) <= 2f)
            {
                if (++currentPoint >= pathPoints.Length)
                    currentPoint = 0;
            }

            transform.LookAt(pathPoints[currentPoint].position);
            //HandleRotation(pathPoints[currentPoint].position);
            transform.Translate(transform.forward * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    void HandleRotation(Vector3 targetPos)
    {
        //float angle = Mathf.Atan2(transform.position.z - targetPos.z, transform.position.x - targetPos.x) * Mathf.Rad2Deg;
        Vector3 direction = (targetPos - transform.position).normalized;
        float angle = Vector3.Angle(direction, transform.forward);
        //print("Mouse position: " + mousePosition + ", angle: " + angle);
        transform.rotation = Quaternion.Euler(new Vector3(0, angle + 90f, 0));
    }

    public void StartMoving()
    {
        isMoving = true;
    }

    public void StopMoving()
    {
        isMoving = false;
    }
}
