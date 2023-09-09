using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour{

    //ButtonClickでMainへ
    public void GameStart() {
        SceneManager.LoadScene("Main");
        SoundManager.instance.PlaySE(4);//UI
    }
}
