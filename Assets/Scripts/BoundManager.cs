using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine.SceneManagement;

public class BoundManager : MonoBehaviour
{
    // サイコロのオブジェクトを入れる変数
    public GameObject DicePrefab ;
    // クリックした時にオブジェクトに与える力
    public Vector3 GivePower = new Vector3(0.0f, 100.0f, 100.0f);
    // マウスのレイを取得する変数
    private Ray GetRay;
    // マウスの当たり判定を取得する変数
    private RaycastHit HitRay;
    // オブジェクトの物理演算
    private Rigidbody DiceRigid ;
     //カメラ取得
    private GameObject MainCamera ; 
    private GameObject BoundCamera ;   
    private BoundDicePrefab script ;
    void Start()
    {
        // オブジェクトを取得
        DicePrefab = GameObject.Find("BoundDice");
        DiceRigid = DicePrefab.GetComponent<Rigidbody> ();
        //シーンのメインカメラをアクティブにする
        BoundCamera = GameObject.Find("BoundCamera");
        BoundCamera.SetActive(true);
        script = DicePrefab.GetComponent<BoundDicePrefab>();
    }

    // Update is called once per frame
    void Update()
    {
        // 左クリックをした先にオブジェクトが存在していた場合
        if (Input.GetMouseButtonDown(0))
        {
            GetRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(GetRay, out HitRay, 1000.0f))
            {
                string objectName = HitRay.collider.gameObject.name;
                if (objectName == "BoundDice")
                {
                    DiceRigid.AddForce (GivePower, ForceMode.Impulse);
                    script.PlaySound();
                }
            }
        }else if (Input.GetKeyDown(KeyCode.Tab))
        {
            ChangeScean();
        }else if (Input.GetKeyDown(KeyCode.F12))
        {
            Application.Quit();
        }else if (Input.GetKey(KeyCode.LeftArrow))
        {
            BoundCamera.transform.Translate(-0.1f, 0.0f, 0.0f);
        }else if (Input.GetKey(KeyCode.DownArrow))
        {
            BoundCamera.transform.Translate(0.0f, 0.0f, -0.1f);
        }else if (Input.GetKey(KeyCode.UpArrow))
        {
            BoundCamera.transform.Translate(0.0f, 0.0f, 0.1f);
        }else if (Input.GetKey(KeyCode.RightArrow))
        {
            BoundCamera.transform.Translate(0.1f, 0.0f, 0.0f);
        }
    }

    //シーンの切り替えをする
    private void ChangeScean()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
