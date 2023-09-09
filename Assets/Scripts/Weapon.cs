using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour{

    //Playerの攻撃力
    public int attackDamage;

    //敵と衝突判定(引数のcollisionがTriggerにチェックがついているものと衝突したか)
    public void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "Enemy") {
            //攻撃用の関数を呼ぶ(武器の攻撃力,自身の位置)
            collision.gameObject.GetComponent<EnemyController>().TakeDamage(attackDamage, transform.position);
            SoundManager.instance.PlaySE(2);//攻撃
        }
    }

}
