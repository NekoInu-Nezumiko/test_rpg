using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour{
    
    //画像
    private SpriteRenderer spriteRenderer;
    //Animator
    private Animator animator;
    //不可視時間
    [SerializeField]
    private float invisibleTime;
    //可視時間
    [SerializeField]
    private float visibleTime;

    private void Awake(){
        //変数に該当オブジェクトにアタッチしているコンポーネントを格納
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    //点滅開始関数(別ファイルからコルーチンを呼ぶ)
    public void PlayFeedBack(){
        StartCoroutine("FlashCoroutine"); //コルーチンの呼び方
    }

    //実際に点滅させる関数(コルーチン) -> 非同期なのでyield
    //SpriteRendererのalphaを変える事で点滅を再現する
    private IEnumerator FlashCoroutine(){
        for (int i=0; i < 1; i++){
            //PlayerのAnimatorを切る
            animator.enabled = false;
            Color spriteColor = spriteRenderer.color; //現在のspriteRendererの色を変数に格納
            spriteColor.a = 0; //alphaを調節
            spriteRenderer.color = spriteColor; //spriteRendererに反映

            yield return new WaitForSeconds(invisibleTime); //invisibleTimeだけ表示させない（待つ）

            animator.enabled = true;
            spriteColor.a = 1;
            spriteRenderer.color = spriteColor;
            yield return new WaitForSeconds(visibleTime); //visibleTimeだけ表示

        }

        yield break;
    }
}
