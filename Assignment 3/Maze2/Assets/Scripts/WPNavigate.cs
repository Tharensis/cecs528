using UnityEngine;
using System.Collections;

public class WPNavigate : MonoBehaviour 
{
    Vector3[] waypoints = null;
    int currentWP;

    Vector3 currentPos;

    public GameObject cam;
    public GUIText gt;
    public float speed { get; set;}
    public float rotationSpeed = 10.0f;
    public float accuracy = 0.01f;

	// Update is called once per frame
	void Update () 
    {
        if ( waypoints == null  || currentWP == waypoints.Length)
        {
            //animation.Play("idle");
            return;
        }

        currentPos = waypoints[currentWP];

        // If we are close enough to the current waypoint, start moving toward the next
        if (Vector3.Distance(waypoints[currentWP], transform.position) < accuracy)
        {
            currentWP++;
        }

        // If we are not at the end of the path, move to the waypoint.
        if (currentWP < waypoints.Length)
        {
            Vector3 direction = waypoints[currentWP] - transform.position;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                                        rotationSpeed * Time.deltaTime);

            transform.Translate(0, 0, speed * Time.deltaTime);        // speed = 0.5
            //Vector3 target = direction * speed + transform.position;    // speed = 2
            //transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime);
        }
	}

	public void SetWaypoints(Vector3[] points)
	{
        waypoints = points;

        currentWP = waypoints.Length;
	}

    public void SetCurrentWP(int wp)
    {
        currentWP = wp;
    }
}
