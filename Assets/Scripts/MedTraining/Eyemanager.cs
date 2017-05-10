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
            FrameLeft.timeStamp = Time.time;
            FrameLeft.EyePos = RightEye;

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
    void readEyeData()
    {

    }

    public void startEyeRecord( bool state)
    {
        acquireEyeDatas = state;
        if (!state)
            WriteEyesData();
    }
}
