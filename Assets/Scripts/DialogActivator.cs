using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogActivator : MonoBehaviour{

    [SerializeField, Header("会話文章"), Multiline(3)]
    private string[] lines;
    private bool canActivator;

    //SavePoint判定
    [SerializeField]
    private bool savePoint;

    void Start(){
    }

    void Update(){
        //A - discuss(dialogが表示されていないときのみ)
        if (Input.GetKeyDown(KeyCode.A) && canActivator==true && !GameManager.instance.dialogBox.activeInHierarchy) {
            GameManager.instance.ShowDialog(lines);

            if(savePoint){
                GameManager.instance.SaveStatus();//save
            }
        }
    }

    //CollsionEnter Player & Dialog
    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Player") {
            canActivator = true;
        }
    }

    //CollisionExit Player & Dialog
    private void OnCollisionExit2D(Collision2D collision) {
        if(collision.gameObject.tag == "Player") {
            canActivator = false;
            GameManager.instance.ShowDialogChange(canActivator);
        }
    }
}
