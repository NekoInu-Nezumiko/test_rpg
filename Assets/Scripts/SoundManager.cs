using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour{

    //シングルトン化(SoundManagerはこのプロジェクトに1つしか存在しない)
    public static SoundManager instance;

    //SE_LIST
    public AudioSource[] se;


    //SoundManagerが2つ以上存在しないようにしている
    private void Awake() {
        if(instance == null) {
            instance = this;
        }else if (instance != this) {
            Destroy(gameObject);
        }
        //シーン遷移時にGameObjectが壊れないようにする
        DontDestroyOnLoad(gameObject);
    }

    //SE再生用
    /// <summary>
    /// SEを鳴らす(0:死亡 1:被弾 2:攻撃 3:回復 4:UI)
    /// </summary>
    /// <param name="x"></param>
    public void PlaySE(int x) {
        se[x].Stop(); //重複してならないようにする
        se[x].Play(); //x番目のSEを再生
    }


}
