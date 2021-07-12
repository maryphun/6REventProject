using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(Animator))]
public class CharacterGraphic : MonoBehaviour
{
    SpriteRenderer renderer;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void FlipX(bool boolean)
    {
        renderer.flipX = boolean;
    }

    public bool IsFlipX()
    {
        return renderer.flipX;
    }

    public void Run(bool boolean)
    {
        animator.SetBool("Run", boolean);
    }
}
