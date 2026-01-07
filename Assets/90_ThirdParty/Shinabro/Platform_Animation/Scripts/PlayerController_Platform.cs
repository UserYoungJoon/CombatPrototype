using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController_Platform : MonoBehaviour
{
    Animator anim;

    [Header("Rotation speed")]
    public float speed_rot;

    [Header("Movement speed during jump")]
    public float speed_move;

    [Header("Time available for combo")]
    public int term;

    public bool isJump;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        Rotate();
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

        if (!isJump)
        {
            Attack();
            Dodge();
            Jump();
            Block();
            Crouch();
            Skill1();
            Skill2();
            Skill3();
            Skill4();
            Skill5();
            Skill6();
            Skill7();
            Skill8();
        }
    }

    Quaternion rot;
    bool isRun;


    void Rotate()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.dKey.isPressed)
        {
            Move();
            rot = Quaternion.LookRotation(Vector3.right);
        }


        else if (keyboard.aKey.isPressed)
        {
            Move();
            rot = Quaternion.LookRotation(Vector3.left);
        }

        else
        {
            anim.SetBool("Run", false);
                anim.SetBool("Walk", false);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, rot, speed_rot * Time.deltaTime);

    }


    void Move()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (isJump)
        {
            transform.position += transform.forward * speed_move * Time.deltaTime;
            anim.SetBool("Run", false);
                anim.SetBool("Walk", false);

        }
        else
        {
            anim.SetBool("Run", true);
                anim.SetBool("Walk", keyboard.leftShiftKey.isPressed);
        }
    }

    int clickCount;
    float timer;
    bool isTimer;


    void Attack()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;


        if (isTimer)
        {
            timer += Time.deltaTime;
        }


        if (mouse.leftButton.wasPressedThisFrame)
        {
            switch (clickCount)
            {

                case 0:

                    anim.SetTrigger("Attack1");

                    isTimer = true;

                    clickCount++;
                    break;


                case 1:

                    if (timer <= term)
                    {
                        anim.SetTrigger("Attack2");

                        clickCount++;
                    }


                    else
                    {
                        anim.SetTrigger("Attack1");

                        clickCount = 1;
                    }


                    timer = 0;
                    break;


                case 2:

                    if (timer <= term)
                    {
                        anim.SetTrigger("Attack3");

                        clickCount = 0;

                        isTimer = false;
                    }


                    else
                    {
                        anim.SetTrigger("Attack1");

                        clickCount = 1;
                    }

                    timer = 0;
                    break;
            }
        }
    }


    void Dodge()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;


        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            anim.SetTrigger("Dodge");
        }
    }

    void Block()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.rightButton.wasPressedThisFrame)
        {
            anim.SetBool("Block", true);
        }
        if (mouse.rightButton.wasReleasedThisFrame)
        {
            anim.SetBool("Block", false);
        }
    }

    void Crouch()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.sKey.wasPressedThisFrame)
        {
            anim.SetBool("Crouch", true);
        }
        if (keyboard.sKey.wasReleasedThisFrame)
        {
            anim.SetBool("Crouch", false);
        }
    }


    void Jump()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;


        if (keyboard.wKey.wasPressedThisFrame)
        {
            anim.SetBool("Block", false);
            anim.SetBool("Crouch", false);
            anim.SetTrigger("Jump");

            isJump = true;
        }
    }


    void JumpEnd()
    {
        isJump = false;
    }

    // Skill1
    void Skill1()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.digit1Key.wasPressedThisFrame)
        {
            // Play Skill1 animation
            anim.SetTrigger("Skill1");
        }
    }
    // Skill2
    void Skill2()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.digit2Key.wasPressedThisFrame)
        {
            // Play Skill2 animation
            anim.SetTrigger("Skill2");
        }
    }
    // Skill3
    void Skill3()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.digit3Key.wasPressedThisFrame)
        {
            // Play Skill3 animation
            anim.SetTrigger("Skill3");
        }
    }
    // Skill4
    void Skill4()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.digit4Key.wasPressedThisFrame)
        {
            // Play Skill4 animation
            anim.SetTrigger("Skill4");
        }
    }
    // Skill5
    void Skill5()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.digit5Key.wasPressedThisFrame)
        {
            // Play Skill5 animation
            anim.SetTrigger("Skill5");
        }
    }
    // Skill6
    void Skill6()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.digit6Key.wasPressedThisFrame)
        {
            // Play Skill6 animation
            anim.SetTrigger("Skill6");
        }
    }
    // Skill7
    void Skill7()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.digit7Key.wasPressedThisFrame)
        {
            // Play Skill7 animation
            anim.SetTrigger("Skill7");
        }
    }
    // Skill8
    void Skill8()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.digit8Key.wasPressedThisFrame)
        {
            // Play Skill8 animation
            anim.SetTrigger("Skill8");
        }
    }
}
