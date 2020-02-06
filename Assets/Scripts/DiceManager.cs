using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEditor;

public class DiceManager : MonoBehaviour
{
    // Prefab取得用の変数
    public GameObject DiceCopy ;
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
    private GameObject[] Dices ;
    // 配列のオブジェクトからScriptを取り出して入れる変数
    private DicePrefab AllScripts ;
    //Keycodeを入れる
    private string InputKey ; 
    private List<string[]> SettingList = new List<string[]>(); 
    // CSVファイル書き出し・読み込み用　ポジション変数
    private Vector3 SettingPosition = new Vector3();
    // CSVファイル書き出し・読み込み用　スケール変数
    private Vector3 SettingRotation ;
    // CSVファイル書き出し・読み込み用　ローテーション変数
    private Vector3 SettingScale = new Vector3();
    // CSVファイルを入れる変数
    private string ReadFile ;
    // 変更後のCSVファイルの中身を格納する配列
    private List<string[]> ReloadingList = new List<string[]>();
    // 保存先のファイルパス
    private string SaveUrl = "Assets/StreamingAssets/SaveDice.csv" ;
    //初期設定のファイルパス
    private string SettingUrl = "Assets/StreamingAssets/Setting.csv";



    
    void Start()
    {   
        if (System.IO.File.Exists(SaveUrl))
        {
            // 前回の保存データが残っている場合削除
            Debug.Log("既存のセーブデータを消しました");
            File.Delete(SaveUrl);
        }
        SettingList = ListMakeCSV(SettingUrl);
        ReadCSV(SettingList);
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
                script = hit.collider.gameObject.GetComponent<DicePrefab>();
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
                script = hit.collider.gameObject.GetComponent<DicePrefab>();
                script.SpinFlag = false;
                // ダイスを止めた角度を表示
                Debug.Log("角度"　+  hit.collider.gameObject.transform.localEulerAngles);
            }
        }    
    // 押されたキーによって処理を変える
        if (Input.GetKeyDown(KeyCode.Space))
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
        // Qキー押下でサイコロを初期配置に戻す
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // 初期設定CSVを読み込む
            SettingList = ListMakeCSV(SettingUrl);
            ReadCSV(SettingList);
        // 現在のサイコロの状態を保存する
        }else if (Input.GetKeyDown(KeyCode.E))
        {
            WriteCSV(Dices);
        // 書き出したCSVファイルを読み込み
        }else if (Input.GetKeyDown(KeyCode.R))
        {
            // ファイルが存在するかどうか確認する
            if (System.IO.File.Exists(SaveUrl))
            {
                ReloadingList = ListMakeCSV(SaveUrl);
                ReadCSV(ReloadingList);

            }else{
                // 無かった場合初期設定CSVを読み込む
                SettingList = ListMakeCSV(SettingUrl);
                ReadCSV(SettingList);
            }
        }
    }
    // 指定されたCSVからオブジェクトの座標と角度、拡大率を取得し、それに従ってオブジェクトを配置する関数
    void ReadCSV(List<string[]> TargetList)
    {
        //現存するサイコロをすべて削除する
        if (Dices != null)
        {
            foreach(GameObject dice in Dices)
            {
                Destroy(dice);
            }
        }    
        // DiceCopy内にあるオブジェクト情報を親のPrefabに戻す
        DiceCopy = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/DicePrefab.prefab") ;
        Debug.Log(TargetList.Count);
        for (int count = 1 ; count <= (TargetList.Count - 1);count++)
        {
            Debug.Log(TargetList[count][1]);
            // CSVに入っている座標・角度・拡大率を変数に格納する
            SettingPosition = new Vector3(float.Parse(TargetList[count][1]),float.Parse(TargetList[count][2]),float.Parse(TargetList[count][3]));
            SettingRotation = new Vector3(float.Parse(TargetList[count][4]),float.Parse(TargetList[count][5]),float.Parse(TargetList[count][6]));
            SettingScale = new Vector3(float.Parse(TargetList[count][7]),float.Parse(TargetList[count][8]),float.Parse(TargetList[count][9]));

            CloneDice(SettingPosition,SettingRotation,SettingScale);
        }
        TargetList.Clear();
    }
    // 現在のゲームオブジェクトの座標・角度・拡大率をCSVファイルに書き込む関数
    void WriteCSV(GameObject[] DiceList)
    {
        StreamWriter saveCSV = new StreamWriter(SaveUrl, false, Encoding.GetEncoding("Shift_JIS"));
        string writeData = "\"\"," + "x," + "y," + "z," + "x," + "y," + "z," + "x," + "y," + "z" ;
        foreach (GameObject saveDice in DiceList)
        {
            // 現在のサイコロの情報を格納する
            SettingPosition = saveDice.transform.localPosition;
            SettingRotation = saveDice.transform.localEulerAngles;
            SettingScale = saveDice.transform.localScale;

            // 一行につき一つのサイコロを書き込んでいく
            writeData += "\r\n" +"\"\","+
                        (SettingPosition.x) + "," + (SettingPosition.y) + "," + (SettingPosition.z) + "," +
                        (SettingRotation.x) + "," + (SettingRotation.y) + "," + (SettingRotation.z) + "," +
                        (SettingScale.x) + "," + (SettingScale.y) + "," + (SettingScale.z)  ;
        }
        Debug.Log(writeData);
        saveCSV.WriteLine(writeData);
        saveCSV.Close();
    }

    void CloneDice(Vector3 position, Vector3 Rotation, Vector3 Scale)
    {
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
    // 引数でCSVのファイルパスをもらい、CSVを読み込み、内容をリストに格納してリストを返すメソッド
    List<string[]> ListMakeCSV(string readPath)
    {
        List<string[]> makeList = new List<string[]>();
        // 初期設定の入ったCSVファイルを読み込み
        ReadFile = File.ReadAllText(readPath);
        // 渡されたCSVを一行ずつ読み込む
        StringReader reader = new StringReader(ReadFile);
        // 読み込む行がなくなるまでリストに格納を繰り返す
        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            makeList.Add(line.Split(','));
        }
        return makeList;
    }
}