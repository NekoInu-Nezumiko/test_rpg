using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour{

    //Player animation + Physics calculation
    [SerializeField]
    private Animator playerAnim;
    public Rigidbody2D rb;

    //武器アニメーション
    [SerializeField]
    private Animator weaponAnim;

    //Speed(Not dash)
    [SerializeField, Tooltip("移動スピード")]
    private int moveSpeed;

    //HP
    [System.NonSerialized]
    public int currentHealth;
    public int maxHealth;
    /*
     * スタミナ(max,回復速度,現在値,ダッシュ速度、ダッシュ長さ、スタミナ消費量、
     * タイマー、基本速度)
     */
    public float totalStamina, recoverySpeedStamina;
    [System.NonSerialized]
    public float currentStamina;
    [SerializeField]
    private float dashSpeed, dashLength, dashCost;
    private float dashCounter, activeMoveSpeed;

    //吹っ飛び判定
    private bool isKnockingBack;
    private Vector2 knockDir;
    [SerializeField]
    private float knockBackTime, knockBackForce;
    private float knockBackCounter;
    //無敵判定
    [SerializeField]
    private float invincibilityTime;
    private float invincibilityCounter;
    //点滅関連
    //private Flash flash;


    // Start is called before the first frame update
    void Start(){
        currentHealth = maxHealth;
        GameManager.instance.UpdateHealthUI();
        activeMoveSpeed = moveSpeed;
        currentStamina = totalStamina;
        GameManager.instance.UpdateStaminaUI();
        //flash = GetComponent<Flash>(); //点滅関連
    }

    // Specによって呼ばれる間隔が異なるのでfpsはほかで制御する
    // .normalizedでベクトルを正規化(1の大きさに)するので斜め移動対策
    void Update(){

        //メニューを開いているときは動けないように
        if(GameManager.instance.statusPanel.activeInHierarchy){
            return;
        }

        //無敵時間の処理
        if (invincibilityCounter > 0) {
            invincibilityCounter -= Time.deltaTime;
        }
        //KnockBack処理
        if (isKnockingBack) {
            knockBackCounter -= Time.deltaTime;
            rb.velocity = knockDir * knockBackForce;

            if(knockBackCounter <= 0) {
                isKnockingBack = false;
            }
            else {
                //update処理は実行しない(KnockBack中の場合)
                return;
            }
        }
        //入力に応じて移動
        rb.velocity = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
            ).normalized * activeMoveSpeed;

        //移動している場合(移動していないときはanimatorをoffにする)
        if (rb.velocity != Vector2.zero){
            playerAnim.enabled = true;
            if (Input.GetAxisRaw("Horizontal") != 0) { 
                if(Input.GetAxisRaw("Horizontal") > 0){
                    playerAnim.SetFloat("X", 1f);
                    playerAnim.SetFloat("Y", 0);
                    weaponAnim.SetFloat("X", 1f);
                    weaponAnim.SetFloat("Y", 0);
                }
                else{
                    playerAnim.SetFloat("X", -1f);
                    playerAnim.SetFloat("Y", 0);
                    weaponAnim.SetFloat("X", -1f);
                    weaponAnim.SetFloat("Y", 0);
                }
            }else if (Input.GetAxisRaw("Vertical") > 0 ){
                playerAnim.SetFloat("X", 0);
                playerAnim.SetFloat("Y", 1f);
                weaponAnim.SetFloat("X", 0);
                weaponAnim.SetFloat("Y", 1f);
            }
            else if (Input.GetAxisRaw("Vertical") < 0) {
                playerAnim.SetFloat("X", 0);
                playerAnim.SetFloat("Y", -1f);
                weaponAnim.SetFloat("X", 0);
                weaponAnim.SetFloat("Y", -1f);
            }
        }else{
            playerAnim.enabled = false; //animator_off
        }

        //Attack - Z
        if (Input.GetKeyDown(KeyCode.Z)){
            weaponAnim.SetTrigger("Attack");
        }
        //Dash - X (Stamina enough)
        if(dashCounter <= 0) {
            if (Input.GetKeyDown(KeyCode.X) && currentStamina >= dashCost) {
                activeMoveSpeed = dashSpeed;
                dashCounter = dashLength;

                currentStamina -= dashCost;
                GameManager.instance.UpdateStaminaUI();
            }
        }else {
            dashCounter -= Time.deltaTime;
            if(dashCounter <= 0) {
                activeMoveSpeed = moveSpeed;
            }
        }
        //Stamina recovery
        if (currentStamina != totalStamina) {
            currentStamina = Mathf.Clamp(currentStamina + recoverySpeedStamina * Time.deltaTime, 0, totalStamina);
            GameManager.instance.UpdateStaminaUI();
        }
        
    }

    /// <summary>
    /// 吹き飛ばされ用の関数
    /// </summary>
    /// <param name="position"></param>
    public void KnockBack(Vector3 position) {
        knockBackCounter = knockBackTime;
        isKnockingBack = true;
        //ポジションの方角から遠ざかる
        knockDir = transform.position - position;
        knockDir.Normalize();
    }
    /// <summary>
    /// Damage判定とUIの調整
    /// </summary>
    /// <param name="damage"></param>
    public void DamagePlayer(int damage) {
        //無敵でない場合は攻撃を食らう
        if(invincibilityCounter <= 0) {
            //flash.PlayFeedBack(); //点滅
            currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
            invincibilityCounter = invincibilityTime;
            SoundManager.instance.PlaySE(1);//被弾

            //HPが0ならば物理判定から消滅する
            if (currentHealth == 0) {
                gameObject.SetActive(false);
                GameManager.instance.Load(); //画面Load
                SoundManager.instance.PlaySE(0);//死亡
            }
            //HPコライダーの調整
            GameManager.instance.UpdateHealthUI();
        }
        
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        //portionとの衝突判定(最大HP != 現在HP, 取得時間OK)
        if(collision.tag == "Portion" && maxHealth != currentHealth && collision.GetComponent<Items>().waitTime < 0) {
            Items items = collision.GetComponent<Items>();

            SoundManager.instance.PlaySE(3);//recovery

            currentHealth = Mathf.Clamp(currentHealth + items.healthItemRecoveryValue, 0, maxHealth);
            GameManager.instance.UpdateHealthUI(); //UIに判定
            Destroy(collision.gameObject); //アイテム破棄
        }
    }

}
