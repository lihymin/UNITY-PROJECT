using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public Rigidbody2D rigid;
    public float maxSpeed;

    public float maxPower;
    public SpriteRenderer spriteRenderer;

    public Animator animator;
    public CapsuleCollider2D capsuleCollider2D;

    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;
    public AudioSource audioSource;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void PlaySound(string action)
    {
        switch(action)
        {
            case "Jump":
                audioSource.clip = audioJump;
                break;
            case "Attack":
                audioSource.clip = audioAttack;
                break;
            case "Damaged":
                audioSource.clip = audioDamaged;
                break;
            case "Item":
                audioSource.clip = audioItem;
                break;
            case "Die":
                audioSource.clip = audioDie;
                break;
            case "Finish":
                audioSource.clip = audioFinish;
                break;
        }
    }

    void Update()
    {
        //점프
        if(Input.GetButtonDown("Jump") && !animator.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * maxPower, ForceMode2D.Impulse);
            animator.SetBool("isJumping", true);
            PlaySound("Jump");
            audioSource.Play();
        }
        //멈춘 후 움직임
        if(Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x, rigid.velocity.y);
        }
        //방향 전환
        if(Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }
        //애니메이션 전환
        if(Mathf.Abs(rigid.velocity.x) >= 0.3)
        {
            animator.SetBool("isWalking", true);
        }

        if(Mathf.Abs(rigid.velocity.x) < 0.3)
        {
            animator.SetBool("isWalking", false);
        }
    }

    void FixedUpdate()
    {
        //플레이어의 움직임
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(new Vector2(h,0), ForceMode2D.Impulse);
    
        if(rigid.velocity.x >= maxSpeed)
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }

        else if(rigid.velocity.x <= -maxSpeed)
        {
            rigid.velocity = new Vector2(-maxSpeed, rigid.velocity.y);
        }

        //오브젝트 탐색
        if(rigid.velocity.y < 0)
        {
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if(rayHit.collider != null)
            {   
                if(rayHit.distance <= 0.5f)
                {
                    animator.SetBool("isJumping", false);
                }
            }
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {   
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
                gameManager.scoreText.text = (gameManager.totalPoint + gameManager.stagePoint).ToString();
            }
            else
            {
                OnDamaged(collision.transform.position);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Item")
        {
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if(isBronze)
            {
                gameManager.stagePoint += 50;
            }

            else if(isSilver)
            {
                gameManager.stagePoint += 100;
            }

            else if(isGold)
            {
                gameManager.stagePoint += 300;
            }
    
            gameManager.scoreText.text = (gameManager.totalPoint + gameManager.stagePoint).ToString();
            collision.gameObject.SetActive(false);
            PlaySound("Item");
            audioSource.Play();
        }

        else if (collision.gameObject.tag == "Finish")
        {
            PlaySound("Finish");
            audioSource.Play();
            gameManager.NextStage();
        }
    }

    void OnDamaged(Vector2 targetPos)
    {
        //사운드 추가
        PlaySound("Damaged");
        audioSource.Play();
        //채력 깎임
        HealthDown();
        //레이어 변경
        gameObject.layer = 11;
        //투명 효과
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //물리 효과
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);
        //애니메이션
        animator.SetTrigger("Damaged");
        Invoke("OffDamaged", 3);
    }

    void OffDamaged()
    {
        //색깔 변경
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    void OnAttack(Transform enemy)
    {
        //사운드 추가
        PlaySound("Attack");
        audioSource.Play();
        //점수 추가
        gameManager.stagePoint += 100;
        //반응 힘
        rigid.AddForce(Vector2.up * 8, ForceMode2D.Impulse);
        //적 죽음 구현
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }

    public void HealthDown()
    {
        if(gameManager.health > 1)
        {
            gameManager.playerLife[gameManager.health - 1].color = new Color(1, 1, 1, 0.4f);
            gameManager.health--;
        }

        else
        {
            gameManager.playerLife[gameManager.health - 1].color = new Color(1, 1, 1, 0.4f);
            gameManager.health--;
            gameManager.reStartButton.SetActive(true);
            gameManager.OnDie();
            PlaySound("Die");
            audioSource.Play();
        }
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}
