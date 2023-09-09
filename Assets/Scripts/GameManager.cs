using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    //static化して、どこからでも呼び出せるようにする
    public static GameManager instance;
    //HPバー
    [SerializeField]
    private Slider hpSlider;
    //スタミナバー
    [SerializeField]
    private Slider staminaSlider;
    //Playerオブジェクト
    [SerializeField]
    private PlayerController player;
    //ダイアログ
    public GameObject dialogBox;
    public Text dialogText;
    private string[] dialogLines;
    private int currentLine;
    private bool justStarted;

    //ステータスUI
    public GameObject statusPanel;
    [SerializeField]
    private Text hpText, stText, atText;
    [SerializeField]
    private Weapon weapon;

    //経験値(レベル)
    private int totalExp, currentLevel;
    [SerializeField, Tooltip("レベルアップに必要な経験値")] 
    private int[] requiredExp;

    //LevelUpText Canvasの配下でないとUIは表示できないのでそうする
    [SerializeField]
    private GameObject levelUpText;
    [SerializeField]
    private Canvas canvas;

    //Save関連
    private string fileName = "test_rpg.json";
    private string filePath;
    private SaveAndLoad saveAndLoad = new SaveAndLoad();


    //自分自身をinstanceに格納
    private void Awake() {
        if(instance == null) {
            instance = this;
        }
    }

    void Start()
    {
        LoadStatus();//load
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogBox.activeInHierarchy) {
            //一回目の入力では文字送りしない(justStarted)
            if (Input.GetKeyUp(KeyCode.A)) {
                SoundManager.instance.PlaySE(4);//UI

                if (!justStarted) {
                    currentLine++;
                    if(currentLine >= dialogLines.Length) {
                        dialogBox.SetActive(false);
                    }else {
                        dialogText.text = dialogLines[currentLine];
                    }
                }else {
                    justStarted = false;
                }
            }
        }
        //Eボタンが押されればステータス画面にとぶ
        if(Input.GetKeyDown(KeyCode.E)){
            ShowStatusPanel();
        }
    }

    //playerのHPをUIに反映する
    public void UpdateHealthUI() {
        hpSlider.maxValue = player.maxHealth;
        hpSlider.value = player.currentHealth;
    }
    //PlayerのStaminaをUIに反映
    public void UpdateStaminaUI() {
        staminaSlider.maxValue = player.totalStamina;
        staminaSlider.value = player.currentStamina;
    }

    //会話文UIの表示
    public void ShowDialog(string[] lines) {
        dialogLines = lines;
        currentLine = 0;
        dialogText.text = dialogLines[currentLine];
        dialogBox.SetActive(true); //表示開始
        justStarted = true;
    }

    //表示切替
    public void ShowDialogChange(bool x) {
        dialogBox.SetActive(x);
    }

    //Player死亡時
    public void Load() {
        SceneManager.LoadScene("Main");
    }

    //ステータス表示時
    public void ShowStatusPanel(){
        statusPanel.SetActive(true);
        Time.timeScale = 0; //ステータスを見ている間は、時間停止
        //UI更新用関数の呼び出し
        StatusUpdate();

    }
    //ステータス閉じる
    public void CloseStatusPanel(){
        statusPanel.SetActive(false);
        Time.timeScale = 1;
    }
    //ステータス表示更新
    public void StatusUpdate(){
        hpText.text = "体力 : " + player.maxHealth;
        stText.text = "スタミナ : " + player.totalStamina;
        atText.text = "攻撃力 : " + weapon.attackDamage;
    }

    //経験値加算用関数
    //引数はenemyが持っているexp
    public void AddExp(int exp){
        //最大レベルなら
        if (requiredExp.Length <= currentLevel){
            return;
        }
        totalExp += exp;
        //必要なexpを上回っていればレベルUP
        if(totalExp >= requiredExp[currentLevel]){
            currentLevel++;
            player.maxHealth += 5;
            player.totalStamina += 5;
            weapon.attackDamage += 2;

            //UI(Instantiateで生成 + 場所も指定)
            GameObject levelUp = Instantiate(levelUpText, player.transform.position, Quaternion.identity);
            levelUp.transform.SetParent(player.transform); //Playerの動きに合わせてCanvasを動かす
            /* old */
            //GameObject levelUp = Instantiate(levelUpText);
            //levelUp.transform.SetParent(canvas.transform); //親を指定
            //Playerの少し上に表示
            //levelUp.transform.localPosition = player.transform.position + new Vector3(0,100,0); 



        }
    }

    //Save関数とLoad関数の実装(キーを押したらsetParamを呼び出し、データを現在の物にして、SaveStatusを呼び出す感じ)
    public void SaveStatus(){
        filePath = Path.Combine(Application.persistentDataPath, fileName);
        //中身をsaveAndLoadにセット
        saveAndLoad.setParam(player.maxHealth, player.totalStamina, weapon.attackDamage, currentLevel, totalExp);
        string json = JsonUtility.ToJson(saveAndLoad, true);
        Debug.Log(json + ":save");
        File.WriteAllText(filePath, json); 

    }

    public void LoadStatus(){
        filePath = Path.Combine(Application.persistentDataPath, fileName);
        if(!File.Exists(filePath)){
            return;
        }
        var json = File.ReadAllText(filePath);
        saveAndLoad = JsonUtility.FromJson<SaveAndLoad>(json);
        //値をLoad
        player.maxHealth = saveAndLoad.HP;
        player.totalStamina = saveAndLoad.ST;
        weapon.attackDamage = saveAndLoad.AT;
        currentLevel = saveAndLoad.Level;
        totalExp = saveAndLoad.Exp;
    }
}

//Save用のクラス。JsonUtilityで管理したい
[System.Serializable]
public class SaveAndLoad{
    public int HP;
    public float ST;
    public int AT;
    public int Level;
    public int Exp;

    public void setParam(int HP, float ST, int AT, int Level, int Exp){
        this.HP = HP;
        this.ST = ST;
        this.AT = AT;
        this.Level = Level;
        this.Exp = Exp;
    }
}