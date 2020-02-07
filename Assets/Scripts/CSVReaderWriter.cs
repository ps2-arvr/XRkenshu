using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEditor;


public class CSVReaderWriter : MonoBehaviour
{
    //ゲームマネージャ呼び出し用
    public GameObject GameManager;
    // GameManagerのDiceManager(Script)を格納する変数.
    public DiceManager Script;
    //ゲームオブジェクト管理用
    private GameObject DiceCopy ;
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

    // Start is called before the first frame update
    void Start()
    {
        //GameManager = GameObject.Find("GameManager");
        Script = GameManager.GetComponent<DiceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Script.Dices = GameObject.FindGameObjectsWithTag("TagDice");
    }

    public bool SaveCSVExists(string checkPath, bool delete = false)
    {
        if (System.IO.File.Exists(checkPath))
        {
            // 前回の保存データが残っている場合削除
            Debug.Log("既存のデータがあります");
            if (delete == true)
            {
                File.Delete(checkPath);
            }
            return true;
        }
        return false;
    }

    // 指定されたCSVからオブジェクトの座標と角度、拡大率を取得し、それに従ってオブジェクトを配置する関数
    public void ReadCSV(List<string[]> TargetList)
    {

        Debug.Log("リスト内の数：" + TargetList.Count);
        for (int count = 1 ; count <= (TargetList.Count - 1);count++)
        {
            Debug.Log("リストの中身position x：" + TargetList[count][1]);
            // CSVに入っている座標・角度・拡大率を変数に格納する
            SettingPosition = new Vector3(float.Parse(TargetList[count][1]),float.Parse(TargetList[count][2]),float.Parse(TargetList[count][3]));
            SettingRotation = new Vector3(float.Parse(TargetList[count][4]),float.Parse(TargetList[count][5]),float.Parse(TargetList[count][6]));
            SettingScale = new Vector3(float.Parse(TargetList[count][7]),float.Parse(TargetList[count][8]),float.Parse(TargetList[count][9]));

            Script.CloneDice(SettingPosition,SettingRotation,SettingScale);
        }
        TargetList.Clear();
    }

    // 現在のゲームオブジェクトの座標・角度・拡大率をCSVファイルに書き込む関数
    public void WriteCSV(GameObject[] DiceList)
    {
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
        Debug.Log(Script.CopyUrl);
        Debug.Log(Script.SaveUrl);
        StreamWriter sw;
        sw = new StreamWriter(Script.SaveUrl, false, Encoding.GetEncoding("UTF-8"));
        sw.WriteLine(writeData);
        sw.Close();
        SaveCSVExists(Script.CopyUrl,true);
        File.Copy(Script.SaveUrl, Script.CopyUrl);
    }

        // 引数でCSVのファイルパスをもらい、CSVを読み込み、内容をリストに格納してリストを返すメソッド
    public void ListMakeCSV(string readPath,GameObject[] Dices)
    {
        List<string[]> makeList = new List<string[]>();
        // 初期設定の入ったCSVファイルを読み込み
        //ReadFile = File.ReadAllText(readPath);
        Encoding enc;
        enc = Encoding.GetEncoding("UTF-8");
        string path = readPath;
        string csvText = File.ReadAllText(path,enc);
        Debug.Log(path);

        // 渡されたCSVを一行ずつ読み込む
        StringReader reader = new StringReader(csvText);
        // 読み込む行がなくなるまでリストに格納を繰り返す
        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            makeList.Add(line.Split(','));
        }
        //現存するサイコロをすべて削除する
        if (Dices != null)
        {
            foreach(GameObject dice in Dices)
            {
                Destroy(dice);
            }
        }    
        ReadCSV(makeList);
    }
}
