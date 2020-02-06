using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DicePrefab : MonoBehaviour
{
    // GameManagerを格納する変数.
    public GameObject GameManager;
     // GameManagerのDiceManager(Script)を格納する変数.
    public DiceManager Script;
    // 回転するかしないかを判別するbool型変数.
    public bool SpinFlag = false;

    // Start is called before the first frame update.
    void Start()
    {
      GameManager = GameObject.Find("GameManager");
      Script = GameManager.GetComponent<DiceManager>();
    }

    // Update is called once per frame.
    void Update()
    {
      if (this.SpinFlag == true)
      {
        this.transform.Rotate(new Vector3(3, 3, 3));
      }else {
        this.transform.Rotate(new Vector3(0, 0, 0));
      }
    }
}
