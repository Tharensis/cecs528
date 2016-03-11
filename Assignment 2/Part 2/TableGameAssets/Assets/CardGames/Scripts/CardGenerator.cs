using UnityEngine;
using System.Collections;

public class CardGenerator : MonoBehaviour {

	public Material[] cardTypes;

	private GameObject originalCard;
	private Vector3 newCardPosition;

	private float xPos;
	private float yPos;
	private float zPos;

	private int x,y;

	// Use this for initialization
	void Start () {
		originalCard = GameObject.Find ("Original Card");

		xPos = gameObject.transform.position.x;
		yPos = gameObject.transform.position.y;
		zPos = gameObject.transform.position.z;

		newCardPosition = new Vector3 (xPos, yPos, zPos);
		x = 0;
		y = 0;
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
		{
			//originalCard.GetComponent<MeshRenderer>().material = cardTypes[Random.Range(0, cardTypes.Length)];
			GameObject newCard = (GameObject)Instantiate (originalCard, originalCard.transform.position, originalCard.transform.rotation);
			newCard.transform.parent = transform;
			newCard.GetComponent<MeshRenderer>().material = cardTypes[Random.Range(0, cardTypes.Length)];

			StartCoroutine (MoveTo (newCard.transform, newCardPosition));

			// Setting grid position for next card
			x++;
			if(x > 12)
			{
				x = 0;
				y++;
			}
			newCardPosition.x = xPos - 0.1f * x;
			newCardPosition.z = zPos + 0.1f * y;
		}
	}

	//Move an object to a new position
	IEnumerator MoveTo(Transform objectToMove, Vector3 targetPosition)
	{
    //Initialize a velocity for smooth damp
    var velocity = Vector3.zero;
    //While we are not near to the target
    while((objectToMove.position - targetPosition).sqrMagnitude > 0.01 * 0.01)
    {
        //Use smooth damp to move to the new position
        objectToMove.position = Vector3.SmoothDamp(objectToMove.position, targetPosition, ref velocity, 1);
        //Yield until the next frame
        yield return null;
    }
}
}
