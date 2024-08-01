using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;

    public int nextMove;

    float nextThinkTime;

    Animator animator;

    SpriteRenderer spriteRenderer;
    
    CapsuleCollider2D capsuleCollider2D;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        nextThinkTime = Random.Range(2f, 5f);
        Think();
    }

    void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //Platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove, rigid.position.y);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if(rayHit.collider == null && spriteRenderer.flipY != true)
        {   
            
            nextMove = -nextMove;
            spriteRenderer.flipX = nextMove == 1;
            CancelInvoke();
            Invoke("Think", nextThinkTime);
        }
    }

    //재귀 함수
    void Think()
    {
        //Set Next Active
        nextMove = Random.Range(-1, 2);
        float nextThinkTime = Random.Range(2f, 5f);

        //Sprite Animation
        animator.SetInteger("RunSpeed", nextMove);

        //Flip Sprite
        if(nextMove != 0)
        {
            spriteRenderer.flipX = nextMove == 1;
        }
        //Recursive
        Invoke("Think", nextThinkTime);
    }

    public void OnDamaged()
    {
        //투명 효과
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //뒤집기 효과
        spriteRenderer.flipY = true;
        //콜라이더 헤제
        capsuleCollider2D.enabled = false;
        //헤치우기 효과
        rigid.AddForce(Vector2.up * 7, ForceMode2D.Impulse);
        //헤체
        Invoke("DeActive", 5);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }

}
