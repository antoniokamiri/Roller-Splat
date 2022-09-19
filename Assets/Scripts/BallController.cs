using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 15.0f;
    public AudioClip crashSound;

    private AudioSource playerAudio;

    private bool isTraveling;
    private Vector3 travelDirection;
    private Vector3 nextCollisionPosition;

    public int minSwipeRecognition = 500;
    private Vector2 swipePositinLastFrame;
    private Vector2 swipePositinCurrentFrame;
    private Vector2 currentSwipe;

    private Color solveColor;

    private void Start()
    {
        solveColor = Random.ColorHSV(0.5f, 1); // took only bright colors.
        GetComponent<MeshRenderer>().material.color = solveColor;  // set the random color to our ball
        playerAudio = GetComponent<AudioSource>();
    }

    //swipe code
    private void FixedUpdate()
    {
        if(isTraveling)
        {
            rb.velocity = speed * travelDirection;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position - (Vector3.up / 2), 0.05f);
        int i = 0;
        while(i < hitColliders.Length)
        {
            GroundPieceController ground = hitColliders[i].transform.GetComponent<GroundPieceController>();
            if(ground && !ground.isColored)
            {
                ground.ChangeColor(solveColor); // send the random color to groundpiece change color method
            }
            i++;
        }

        if(nextCollisionPosition != Vector3.zero)
        {
            if(Vector3.Distance(transform.position, nextCollisionPosition) < 1)
            {
                playerAudio.PlayOneShot(crashSound, 0.5f);
                nextCollisionPosition = Vector3.zero;
                travelDirection = Vector3.zero;
                isTraveling = false;
            }
        }

        if (isTraveling)
            return;
        if(Input.GetMouseButton(0))
        {
            swipePositinCurrentFrame = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            
            if(swipePositinLastFrame != Vector2.zero)
            {
                currentSwipe = swipePositinCurrentFrame - swipePositinLastFrame;

                if(currentSwipe.sqrMagnitude < minSwipeRecognition) // sqrMagnitude converts the swipe distance to compare with minSwipeRecognition
                {
                    return;
                }

                currentSwipe.Normalize(); //Check the direction.

                // Go Up/Down/Right/Left

                if(currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                {
                    // Go Up or Down
                    SetDestination(currentSwipe.y > 0 ? Vector3.forward : Vector3.back);
                }

                if (currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                {
                    // Go Left or Right
                    SetDestination(currentSwipe.x > 0 ? Vector3.right : Vector3.left);
                }
            }

            swipePositinLastFrame = swipePositinCurrentFrame;
        }

        if(Input.GetMouseButtonUp(0))
        {
            swipePositinLastFrame = Vector2.zero;
            currentSwipe = Vector2.zero;
        }
    }
    private void SetDestination(Vector3 direction)
    {
        travelDirection = direction;

        RaycastHit hit; //Check which object it will collide with.

        if(Physics.Raycast(transform.position, direction, out hit, 100f))
        {
            nextCollisionPosition = hit.point;
        }

        isTraveling=true;
    }
}
