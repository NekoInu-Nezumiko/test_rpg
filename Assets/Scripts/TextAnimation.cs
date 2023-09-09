using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAnimation : MonoBehaviour{

    //Textを表示する時間の管理
    //該当オブジェクトにアタッチして、既定の秒数以上経過した場合は削除する
    
    private float lifeTime = 2f;

    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0){
            Destroy(gameObject);
        }    
    }
}
