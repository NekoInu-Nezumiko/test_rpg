using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour{
    
    //物理判定とアニメーション
    private Rigidbody2D rb;
    private Animator enemyAnim;

    //移動関連
    [SerializeField]
    private float moveSpeed, waitTime, walkTime;
    private float waitCounter, moveCounter;
    private Vector2 moveDir;

    //徘徊するエリア判定
    [SerializeField]
    private BoxCollider2D area;

    //追跡関連
    private Transform target;
    [SerializeField, Tooltip("プレイヤーを追いかけるか")]
    private bool chase;
    private bool isChasing;
    [SerializeField]
    private float chaseSpeed, rangeToChase;
    [SerializeField]
    private float waitAfterHitting;
    [SerializeField]
    private int attackDamage;

    //HP(fillAmountがfloat型なので)
    [SerializeField]
    private float maxHealth;
    private float currentHealth;

    //Knockback
    private bool isKnockingBack;
    [SerializeField]
    private float knockBackTime, knockBackForce;
    private float knockBackCounter;
    private Vector2 knockDir;

    //dropitem
    [SerializeField]
    private GameObject portion;
    [SerializeField]
    private float portionDropChance;

    //血エフェクト
    [SerializeField]
    private GameObject blood;

    //与exp
    [SerializeField]
    private int exp;
    //HP画像
    [SerializeField]
    private Image hpImage;

    //点滅
    //private Flash flash;

    void Start(){
        //1. startと同時にEnemyについているRigidbody2Dを受け取る
        rb = GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();
        //2. 移動関連に使う変数を取得
        waitCounter = waitTime;
        //3. PlayerのGameObjectを探し、その位置を取得（重い）
        target = GameObject.FindGameObjectWithTag("Player").transform;

        //HPの設定
        currentHealth = maxHealth;
        //HP_UI関数の呼び出し
        UpdateHealthImage();

        //アタッチしたFlash(点滅関連のScript)をflashに格納
        //flash = GetComponent<Flash>();

    }

    void Update(){

        //KnockBack時の処理。returnする
        if (isKnockingBack) {
            if(knockBackCounter > 0) {
                knockBackCounter -= Time.deltaTime;
                rb.velocity = knockDir * knockBackForce;
            }else {
                rb.velocity = Vector2.zero;
                isKnockingBack = false;
            }
            return;
        }

        //徘徊
        if (isChasing == false) {
            //停止(WaitCounter秒)
            if (waitCounter > 0) {
                waitCounter -= Time.deltaTime;
                rb.velocity = Vector2.zero;
                //徘徊方向を決定
                if (waitCounter <= 0) {
                    moveCounter = walkTime;
                    enemyAnim.SetBool("moving", true);
                    moveDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                    moveDir.Normalize();
                }
            //徘徊
            }else {
                moveCounter -= Time.deltaTime;
                rb.velocity = moveDir * moveSpeed;
                //停止へ移行
                if (moveCounter <= 0) {
                    enemyAnim.SetBool("moving", false);
                    waitCounter = waitTime;
                }
            }
            //追跡可能個体なら
            if (chase) {
                //距離を測る
                if(Vector3.Distance(transform.position, target.transform.position) <= rangeToChase) {
                    isChasing = true;
                }
            }
        }
        //追跡
        else {
            //WaitCounter秒停止
            if (waitCounter > 0) {
                waitCounter -= Time.deltaTime;
                rb.velocity = Vector2.zero;
                if(waitCounter <= 0) {
                    enemyAnim.SetBool("moving", true);
                }
            }
            //追跡
            else {
                moveDir = target.transform.position - transform.position;
                moveDir.Normalize();
                rb.velocity = moveDir * chaseSpeed;
            }
            //追跡続行 or 徘徊
            if(Vector3.Distance(transform.position, target.transform.position) > rangeToChase) {
                isChasing = false;
                waitCounter = waitTime;
                enemyAnim.SetBool("moving", false);
            }
        }

        //EnemyがArea外に出ていた場合は丸め込む
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, area.bounds.min.x + 1, area.bounds.max.x - 1),
            Mathf.Clamp(transform.position.y, area.bounds.min.y, area.bounds.max.y),
            transform.position.z
            );

    }

    /*
    * Playerとの衝突判定
    *   isChasing == True
    *       1.playerがKnockBack, Damage
    *       2.次の攻撃までwaitAfterHitting秒待つ
    *       3.アニメーションのBool値をfalseにする(停止)
    */
    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Player") {
            if (isChasing) {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                //KnockBack & Damage
                player.KnockBack(transform.position);
                player.DamagePlayer(attackDamage);
                waitCounter = waitAfterHitting; //攻撃間隔
                enemyAnim.SetBool("moving", false);
            }
        }
    }

    //KnockBack
    public void KnockBack(Vector3 position) {
        isKnockingBack = true;
        knockBackCounter = knockBackTime;
        //前に自身を置くと、遠ざかる
        knockDir = transform.position - position;
        knockDir.Normalize();
        enemyAnim.SetBool("moving", false);
    }

    /*
    * EnemyがDamageを与えられたとき
    *   HP == 0  blood -> drop -> destroy
    *   HP != 0  KnockBack
    *   drop : ポーションをドロップする敵で、かつportionDropChanceで当たれば
    */
    public void TakeDamage(int damage, Vector3 position) {
        currentHealth -= damage;
        //HP画像の更新
        UpdateHealthImage();
        //点滅コルーチンの呼び出し
        //flash.PlayFeedBack(); 
        if (currentHealth <= 0) {
            //死亡時、血を生成
            Instantiate(blood, transform.position, transform.rotation);
            GameManager.instance.AddExp(exp); //経験値取得
            //drop判定(portionがドロップする場合のみ)
            if(Random.Range(0,100) <= portionDropChance && portion != null) {
                //portionを同じ場所、同じ回転で生成
                Instantiate(portion, transform.position, transform.rotation); 
            }
            Destroy(gameObject); //自身を破壊
        }else {
            KnockBack(position);
        }
    }
    //HP画像の更新(Fill Amount)
    private void UpdateHealthImage(){
        hpImage.fillAmount = currentHealth / maxHealth;
    }

}
