using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove playerMove;
    public GameObject[] stages;
    public Text scoreText;
    public Text stageText;
    public Image[] playerLife;
    public GameObject reStartButton;
    public Text reStartButtonText;

    public void NextStage()
    {
        //change stage
        if(stageIndex < (stages.Length - 1)) 
        {
            stages[stageIndex].SetActive(false);
            stageIndex++;
            stages[stageIndex].SetActive(true);
            PlayerReposition();
        }
        //game clear
        else 
        {
            Time.timeScale = 0;
            Text reStartButtonText = reStartButton.GetComponentInChildren<Text>();
            reStartButtonText.text = "Game Clear!";
            reStartButton.SetActive(true);
        }
        stageText.text = "Stage " + (stageIndex + 1).ToString();
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(health > 1)
            {
                PlayerReposition();
            }
            playerMove.HealthDown();
            
        }
    }
    public void OnDie()
    {
        //투명 효과
        playerMove.spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //뒤집기 효과
        playerMove.spriteRenderer.flipY = true;
        //콜라이더 헤제
        playerMove.capsuleCollider2D.enabled = false;
        //헤치우기 효과
        playerMove.rigid.AddForce(Vector2.up * 7, ForceMode2D.Impulse);
    }

    void PlayerReposition()
    {
        playerMove.transform.position = new Vector3(-5, 2, 0);
        playerMove.VelocityZero();
    }

    public void GameReplay()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
