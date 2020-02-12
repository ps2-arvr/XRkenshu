using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEditor;


public class CSVReaderWriter : MonoBehaviour
{
    //ゲームオブジェクト管理用
    private GameObject DiceCopy ;
    // CSVファイル書き出し・読み込み用　ポジション変数
    private Vector3 SettingRotation ;
    // CSVファイル書き出し・読み込み用　ローテーション変数
    private Vector3 SettingPosition = new Vector3();
    // CSVファイル書き出し・読み込み用　スケール変数
    private Vector3 SettingScale = new Vector3();
    // CSVファイルを入れる変数
    private string ReadFile ;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
    public List<Vector3[]> ReadCSV(List<string[]> TargetList)
    {
        List<Vector3[]> ReloadingList = new List<Vector3[]>();
        Debug.Log("リスト内の数：" + TargetList.Count);
        for (int count = 1 ; count <= (TargetList.Count - 1);count++)
        {
            Vector3[] List = new Vector3[3];
            Debug.Log("リストの中身position x：" + TargetList[count][1]);
            // CSVに入っている座標・角度・拡大率を変数に格納する
            SettingPosition = new Vector3(float.Parse(TargetList[count][1]),float.Parse(TargetList[count][2]),float.Parse(TargetList[count][3]));
            SettingRotation = new Vector3(float.Parse(TargetList[count][4]),float.Parse(TargetList[count][5]),float.Parse(TargetList[count][6]));
            SettingScale = new Vector3(float.Parse(TargetList[count][7]),float.Parse(TargetList[count][8]),float.Parse(TargetList[count][9]));
            Debug.Log(SettingPosition +":"+ SettingRotation +":"+ SettingScale);
            List[0] = SettingPosition;
            List[1] = SettingRotation;
            List[2] = SettingScale;
            ReloadingList.Add(List);
        }
        TargetList.Clear();
        return ReloadingList;
    }

    // 現在のゲームオブジェクトの座標・角度・拡大率をCSVファイルに書き込む関数
    public void WriteCSV(GameObject[] DiceList,string saveUrl,string copyUrl)
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

        StreamWriter sw;
        sw = new StreamWriter(saveUrl, false, Encoding.GetEncoding("UTF-8"));
        sw.WriteLine(writeData);
        sw.Close();
        SaveCSVExists(copyUrl,true);
        File.Copy(saveUrl, copyUrl);
    }

    // 引数でCSVのファイルパスをもらい、CSVを読み込み、内容をリストに格納してリストを返すメソッド
    public List<string[]> ListMakeCSV(string readPath,GameObject[] Dices)
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
        return makeList;    
    }
}
