using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour{

    //回復量
    public int healthItemRecoveryValue;
    //アイテムの生存時間
    [SerializeField]
    private float lifeTime;
    //生成された瞬間に取得することを防止するため
    public float waitTime;

    // Start is called before the first frame update
    void Start(){
        Destroy(gameObject, lifeTime); //Destroyまでの時間を保存
    }

    // Update is called once per frame
    void Update(){
        if(waitTime > 0) {
            waitTime -= Time.deltaTime;
        }
    }
}
