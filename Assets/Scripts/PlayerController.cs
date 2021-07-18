using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float baseMoveSpeed = 5f;
    [SerializeField] private LayerMask collideMask;
    [SerializeField] private Vector2 cameraOffset;

    [Header("References")]
    [SerializeField] CharacterGraphic graphic;

    private Collider2D collider;
    private CameraFollow mainCamera;
    [SerializeField] private List<Interact> interactList = new List<Interact>();
    private bool interactMode = false;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider2D>();
        interactList.Clear();
        interactMode = false;
        mainCamera = Camera.main.GetComponent<CameraFollow>();
        mainCamera.SetCameraFollowTarget(transform);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool interactKey = Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.E);

        // unmovable
        if (interactMode) movInput = Vector2.zero;

        // calculate new position
        Vector2 newPos = transform.position;
        newPos += movInput * baseMoveSpeed * Time.deltaTime;

        // check if target position is movable
        Move(newPos, movInput);

        // flip
        if (graphic.IsFlipX() && movInput.x > 0.0f)
        {
            graphic.FlipX(false);
            mainCamera.SetCameraOffsetX(cameraOffset.x + 0.5f);
        }
        else if(!graphic.IsFlipX() && movInput.x < 0.0f)
        {
            graphic.FlipX(true);
            mainCamera.SetCameraOffsetX(cameraOffset.x + -0.5f);
        }

        // camera
        mainCamera.SetCameraOffsetY(cameraOffset.y + transform.position.y / 7.5f);
        //mainCamera.SetCameraValueY(Mathf.Abs(transform.position.y));

        // animation
        graphic.Run(movInput.magnitude > 0.0f);

        // interact
        if (interactKey && interactList.Count > 0)
        {
            float range = 1000.0f;
            Interact nearestObj = interactList[0];
            foreach (Interact obj in interactList)
            {
                // compare
                float tmp = obj.GetRange(transform.position);
                if (tmp < range)
                {
                    range = tmp;
                    nearestObj = obj;
                }
            }
            nearestObj.InteractTrigger();
            interactMode = true;
}
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

    public void InteractRegister(Interact obj)
    {
        if (!interactList.Contains(obj))
        {
            interactList.Add(obj);
        }
    }

    public void InteractUnregister(Interact obj)
    {
        if (interactList.Contains(obj))
        {
            interactList.Remove(obj);
        }
    }
    public bool IsInteracting()
    {
        return interactMode;
    }

    public void SetInteractMode(bool boolean)
    {
        interactMode = boolean;
    }
}
