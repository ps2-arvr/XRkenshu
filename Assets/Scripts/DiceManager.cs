using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine.SceneManagement;

public class DiceManager : MonoBehaviour
{
    //CSVReaderWriter読み込み用の変数
    private CSVReaderWriter CSVScript ;
    private GameObject CSVReaderWriter;
    // Prefab取得用の変数
    public GameObject DiceCopy ;
    // 初期の状態のPrefabを保存する変数
    public GameObject DiceOriginal;
    //　直近でクリックされたオブジェクトを格納する。
    public GameObject GetRayObject;
    public DicePrefab script ;
    //親オブジェクトとしてworldを取得
    public GameObject World;
    // 回転するかしないかを判別するbool型変数
    private bool SpinFlag = false ;
    // サイコロのPrefabをAssetから取得
    private Vector3 ClickPosition ;
    // 作ったサイコロをすべて入れる配列
    public GameObject[] Dices ;
    // 配列のオブジェクトからScriptを取り出して入れる変数
    private DicePrefab AllScripts ;
    //Keycodeを入れる
    private string InputKey ; 
    //保存先のファイルパス
    public string SaveUrl ;
    //初期設定のファイルパス
    public string SettingUrl = Application.streamingAssetsPath + "/Setting.csv";
    //コピー先のファイルパス
    public string CopyUrl;
    //ListMakeCSVから返された配列を格納する変数
    List<Vector3[]> returnList ;

    //カメラ取得
    GameObject MainCamera ; 
    GameObject BoundCamera ;
    void Start()
    {   
        CopyUrl =  Application.streamingAssetsPath + "/SaveDice.csv";
        SaveUrl =  Application.persistentDataPath + "/SaveDice.csv";
        DiceOriginal = DiceCopy;
        CSVReaderWriter = GameObject.Find("CSVReaderWriter");
        CSVScript = CSVReaderWriter.GetComponent<CSVReaderWriter>();

        CSVScript.SaveCSVExists(CopyUrl,true);
        returnList = CSVScript.ReadCSV(CSVScript.ListMakeCSV(SettingUrl,Dices));
        CloneDice(returnList);
        MainCamera = GameObject.Find("MainCamera");
        //シーンのメインカメラをアクティブにする
        MainCamera.SetActive(true);
    }



    // Update is called once per frame
    void Update()
    {
        Dices = GameObject.FindGameObjectsWithTag("TagDice");
        // ASWDが押下されたとき、最後にクリックされた、もしくは作られたオブジェクトが移動する
        if (Input.GetKey(KeyCode.A))
        {
            DiceCopy.transform.Translate(-0.1f, 0.0f, 0.0f);
        }else if (Input.GetKey(KeyCode.S))
        {
            DiceCopy.transform.Translate(0.0f, 0.0f, -0.1f);
        }else if (Input.GetKey(KeyCode.W))
        {
            DiceCopy.transform.Translate(0.0f, 0.0f, 0.1f);
        }else if (Input.GetKey(KeyCode.D))
        {
            DiceCopy.transform.Translate(0.1f, 0.0f, 0.0f);
        }

        // クリックされたとき、クリックされたサイコロを回転・停止させる
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 左クリックされたとき
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                // クリックされたダイスを入れる
                DiceCopy = hit.collider.gameObject;
                script = DiceCopy.GetComponent<DicePrefab>();
                script.SpinFlag = true;
            }else 
            {
                // クリックした位置にオブジェクトがなかった場合　サイコロを複製する
                // クリックした位置を格納
                ClickPosition = Input.mousePosition;
                // Z座標を調整
                ClickPosition.z = 10.0f;
                // 作ったダイスを入れる
                DiceCopy = Instantiate(DiceCopy, Camera.main.ScreenToWorldPoint(ClickPosition), Quaternion.identity);
                DiceCopy.tag = "TagDice";
                DiceCopy.name = "DiceCopy";
                DiceCopy.transform.parent = World.transform;
                // ダイスを作った場所をデバッグで表示
                Debug.Log("座標"　+　Camera.main.ScreenToWorldPoint(ClickPosition));
            }
        // 右クリックされたとき
        }else if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                DiceCopy = hit.collider.gameObject;
                script = DiceCopy.GetComponent<DicePrefab>();
                script.SpinFlag = false;
                // ダイスを止めた角度を表示
                Debug.Log("角度"　+  hit.collider.gameObject.transform.localEulerAngles);
            }
        }    
    // 押されたキーによって処理を変える
        if (Input.GetKeyDown(KeyCode.Space))
        {                
            AllSpins();
        }
        
        // Qキー押下でサイコロを初期配置に戻す
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // 初期設定CSVを読み込む
            returnList = CSVScript.ReadCSV(CSVScript.ListMakeCSV(SettingUrl,Dices));
            CloneDice(returnList);
        // 現在のサイコロの状態を保存する
        }else if (Input.GetKeyDown(KeyCode.E))
        {
            CSVScript.WriteCSV(Dices,SaveUrl,CopyUrl);
        // 書き出したCSVファイルを読み込み
        }else if (Input.GetKeyDown(KeyCode.R))
        {
            // ファイルが存在するかどうか確認する
            if (CSVScript.SaveCSVExists(SaveUrl))
            {
                returnList = CSVScript.ReadCSV(CSVScript.ListMakeCSV(CopyUrl,Dices));
                CloneDice(returnList);
            }else{
                // 無かった場合初期設定CSVを読み込む
                Debug.Log("保存データ　null");
                returnList =  CSVScript.ReadCSV(CSVScript.ListMakeCSV(SettingUrl,Dices));
                CloneDice(returnList);
            }
        }else if (Input.GetKeyDown(KeyCode.F12))
        {
            Application.Quit();
        }else if (Input.GetKeyDown(KeyCode.Tab))
        {
            ChangeScean();
        }

        SelectionDise();
    }
    public void CloneDice(List<Vector3[]> list)
    {
        foreach (Vector3[] List in returnList)
        {
            Vector3 position = List[0] ;
            Vector3 Rotation = List[1] ;
            Vector3 Scale = List[2] ;
            // DiceCopy内にあるオブジェクト情報を親のPrefabに戻す
            DiceCopy = DiceOriginal ;
            // サイコロのPrefabを生成する
            DiceCopy = Instantiate(DiceCopy,new Vector3(0, 0, 0), Quaternion.identity);
            // サイコロにサイコロ用のタグをつける
            DiceCopy.tag = "TagDice";
            // サイコロの名前
            DiceCopy.name = "DiceCopy";
            // サイコロをWorldクラスの子クラスに設定する
            DiceCopy.transform.parent = World.transform;
            // 座標・角度・拡大率をそれぞれ入れていく
            DiceCopy.transform.localScale = Scale;
            DiceCopy.transform.localPosition = position;
            DiceCopy.transform.localEulerAngles = Rotation;
        }
    }

    private void AllSpins()
    {
        // タグがTagDiceのものをすべて入れる
        if (SpinFlag == false)
        {
            // 入れられたオブジェクトを全て回転させる
            foreach(GameObject dice in Dices)
            {
                AllScripts = dice.GetComponent<DicePrefab>();
                AllScripts.SpinFlag = true ;
                // マネージャのSpinFlagをtrueにする
                this.SpinFlag = true;
            }
        }else {
             // 入れられたオブジェクトを全て停止させる
            foreach(GameObject dice in Dices)
            {
                AllScripts = dice.GetComponent<DicePrefab>();
                AllScripts.SpinFlag = false ;
                // マネージャのSpinFlagをfalseにする
                this.SpinFlag = false;
            }      
        }
    }

    private void SelectionDise()
    {
        foreach(GameObject select in Dices)
        {
            DicePrefab selectScript ;
            selectScript = select.GetComponent<DicePrefab>();
            if (select == DiceCopy)
            {
                selectScript.SelectionNow = true;
            }else{
                selectScript.SelectionNow = false;
            }
        }

    }

    //シーンの切り替えをする
    private void ChangeScean()
    {
        SceneManager.LoadScene("BoundDiceScene");
    }
}