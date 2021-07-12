using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float baseMoveSpeed = 5f;
    [SerializeField] private LayerMask collideMask;

    [Header("References")]
    [SerializeField] CharacterGraphic graphic;

    Collider2D collider;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 movInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // calculate new position
        Vector2 newPos = transform.position;
        newPos += movInput * baseMoveSpeed * Time.deltaTime;

        // check if target position is movable
        Move(newPos, movInput);

        if (graphic.IsFlipX() && movInput.x > 0.0f)
        {
            graphic.FlipX(false);
        }
        else if(!graphic.IsFlipX() && movInput.x < 0.0f)
        {
            graphic.FlipX(true);
        }

        graphic.Run(movInput.magnitude > 0.0f);
    }

    private void Move(Vector2 newPosition, Vector2 direction)
    {
        Vector2 mag = newPosition - new Vector2(transform.position.x, transform.position.y);
        Vector2 colliderPos = new Vector2(transform.position.x, transform.position.y) + collider.offset;
        Vector2 adjustedPos = new Vector2(colliderPos.x, colliderPos.y) + mag;

        // collision check before actually move the player
        Vector2 destinationTop = new Vector2(adjustedPos.x + (direction.x * collider.bounds.extents.x), colliderPos.y + collider.bounds.extents.y - 0.05f);
        Vector2 destinationMid = new Vector2(adjustedPos.x + (direction.x * collider.bounds.extents.x), colliderPos.y);
        Vector2 destinationBottom = new Vector2(adjustedPos.x + (direction.x * collider.bounds.extents.x), colliderPos.y - collider.bounds.extents.y + 0.05f);

        // assign new position to the character (x)
        if (!CollisionCheck(destinationTop, collideMask)     // collision with wall
            && !CollisionCheck(destinationBottom, collideMask)
            && !CollisionCheck(destinationMid, collideMask))
        {
            transform.position = new Vector2(newPosition.x, transform.position.y);
        }


        // collision check before actually move the player
        Vector2 destinationLeft = new Vector2(colliderPos.x - collider.bounds.extents.x, adjustedPos.y + (direction.y * collider.bounds.extents.y));
        Vector2 destinationMiddle = new Vector2(colliderPos.x, (adjustedPos.y + (direction.y * collider.bounds.extents.y)));
        Vector2 destinationRight = new Vector2(colliderPos.x + collider.bounds.extents.x, adjustedPos.y + (direction.y * collider.bounds.extents.y));

        // assign new position to the character (y)
        if (!CollisionCheck(destinationLeft, collideMask)     // collision with wall
            && !CollisionCheck(destinationRight, collideMask)
            && !CollisionCheck(destinationMiddle, collideMask))
        {
            transform.position = new Vector2(transform.position.x, newPosition.y);
        }

        Debug.DrawLine(adjustedPos, destinationMiddle);
    }

    private bool CollisionCheck(Vector2 point, LayerMask layerMask)
    {
        return (Physics2D.OverlapPoint(point, layerMask));
    }
}
