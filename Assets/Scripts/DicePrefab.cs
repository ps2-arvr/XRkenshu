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
    //選択されているかされていないかを判別する
    public bool SelectionNow = false;

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

      if (SelectionNow == true)
      {
        ChanegeColor(true);
      }else{
        ChanegeColor(false);
      }
    }

    void ChanegeColor(bool select)
    {
      if (select == true)
      {
        transform.GetChild(6).GetComponent<MeshRenderer>().enabled = true;
      }else{
        transform.GetChild(6).GetComponent<MeshRenderer>().enabled = false;
      }
    }
}
