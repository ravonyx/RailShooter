using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class Frame
{
    [XmlAttribute("Frame")]
    public float timeStamp;
    public Vector3 EyePos;

}

[XmlRoot("FrameCollection")]
public class FrameContainer
{
    [XmlArray("Frames")]
    [XmlArrayItem("FrameItem")]
    public List<Frame> FrameList = new List<Frame>();
}

public class Eyemanager : MonoBehaviour {

    [SerializeField]
    TargetMove targetMoveScript;
    [SerializeField]
    string PythonPath;
    [SerializeField]
    string path;
    
   // FoveInterface.EyeRays leftEye;
    Vector3 LeftEye;
    Vector3 RightEye;

    bool acquireEyeDatas = false;
    FrameContainer framelistLeft;
    FrameContainer framelistRight;

    // Use this for initialization
    void Start () {
        framelistLeft = new FrameContainer();
        framelistRight = new FrameContainer();

       // createPlotlyCurve();
    }
	
	// Update is called once per frame
	void Update () {
        if (acquireEyeDatas)
        {
            LeftEye = FoveInterface.GetLeftEyeVector();
            RightEye = FoveInterface.GetRightEyeVector();
            acquireEyeDatas = targetMoveScript.trainingRunning;

            Frame FrameLeft = new Frame();
            FrameLeft.timeStamp = Time.time;
            FrameLeft.EyePos = LeftEye;

            Frame FrameRight = new Frame();
            FrameRight.timeStamp = Time.time;
            FrameRight.EyePos = RightEye;

            FrameLeft.EyePos = targetMoveScript.gameObject.transform.position; // :!!!!!!!

            framelistLeft.FrameList.Add(FrameLeft);
            framelistRight.FrameList.Add(FrameRight);

            /*if (!acquireEyeDatas)
                WriteEyesData();*/
        }
        if (Input.GetKeyDown(KeyCode.R))
            readEyeData();
	}

    void WriteEyesData()
    {
        XmlSerializer lefttSerializer = new XmlSerializer(typeof(FrameContainer));
        var leftStream = new FileStream(path + "leftDatas", FileMode.Create);
        lefttSerializer.Serialize(leftStream, framelistLeft);
        leftStream.Close();

        XmlSerializer rightSerializer = new XmlSerializer(typeof(FrameContainer));
        var rightStream = new FileStream(path + "rightDatas", FileMode.Create);
        rightSerializer.Serialize(rightStream, framelistRight);
        rightStream.Close();
    }
    void writeEyesDataCSV()
    {
        FileStream leftStream = new FileStream(path + "/leftDatas.csv", FileMode.Create);
        StreamWriter leftStreamWriter = new StreamWriter(leftStream);
        leftStreamWriter.WriteLine( "eyeX,eyeY,eyeZ,timestamp");
        for(int i = 0; i < framelistLeft.FrameList.Count;i++)
        {
            leftStreamWriter.WriteLine(framelistLeft.FrameList[i].EyePos.x.ToString() + ','
                + framelistLeft.FrameList[i].EyePos.y.ToString() + ','
                + framelistLeft.FrameList[i].EyePos.z.ToString() + ','
                + framelistLeft.FrameList[i].timeStamp.ToString());
           
        }
        leftStreamWriter.Close();
        leftStream.Close();

        FileStream rightStream = new FileStream(path + "/rightDatas.csv", FileMode.Create);
        StreamWriter rightStreamWriter = new StreamWriter(rightStream);
        rightStreamWriter.WriteLine("eyeX,eyeY,eyeZ,timestamp");
        for (int i = 0; i < framelistRight.FrameList.Count; i++)
        {
            rightStreamWriter.WriteLine(framelistRight.FrameList[i].EyePos.x.ToString() + ','
                + framelistRight.FrameList[i].EyePos.y.ToString() + ','
                + framelistRight.FrameList[i].EyePos.z.ToString() + ','
                + framelistRight.FrameList[i].timeStamp.ToString());
        }
        rightStreamWriter.Close();
        rightStream.Close();

        createPlotlyCurve();
    }

    void createPlotlyCurve()
    {
       
        string strCmdText = "/C " + PythonPath;
        strCmdText += " Med\\plotCurve";
        strCmdText += " " +path + "\\leftDatas.csv";
        strCmdText += " leftEyeCurve";
       // Debug.Log(strCmdText);
        System.Diagnostics.Process.Start("CMD.exe", strCmdText);

        strCmdText = "";
        strCmdText = "/C " + PythonPath;
        strCmdText += " Med\\plotCurve";
        strCmdText += " " + path + "\\rightDatas.csv";
        strCmdText += " rightEyeCurve";
        // Debug.Log(strCmdText);
        System.Diagnostics.Process.Start("CMD.exe", strCmdText);
    }
    void readEyeData()
    {

    }

    public void startEyeRecord( bool state)
    {
        acquireEyeDatas = state;
        if (!state)
            writeEyesDataCSV();
    }
}
