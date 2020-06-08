using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanel : MonoBehaviour
{
    SerialCommunication _myserial;

    ///<topbar params>
    public InputField IF_portname;
    public InputField IF_baudrate;
    public Button btn_connect;
    public Text text_state;
    ///</topbar params>

    
    public void connectSerial(Button btn)
    {
        string button_text = btn.GetComponentInChildren<Text>().text;
        int.TryParse(IF_baudrate.text, out int baudrate);
        if (baudrate < 9600 || baudrate > 120000)
        {
            baudrate = 9600;
        }

        if (button_text.Equals("Connect"))
        {
            try
            {
                _myserial = new SerialCommunication(IF_portname.text, baudrate);
                btn.GetComponentInChildren<Text>().text = "Disconnect";
                text_state.text = "Connected";
            }
            catch (UnityException e)
            {
                Debug.LogWarning("error when initial serial port! " + e);
                text_state.text = "Connect error";
            }
        }
        else
        {
            _myserial.MySerial.Close();
            btn.GetComponentInChildren<Text>().text = "Connect";
            text_state.text = "Disconnected";

        }
    }


    /// <motor params>
    float motor_x = 0f;
    float motor_y = 0f;
    int motor_frq = 0;
    public InputField IF_x;
    public InputField IF_y;
    public InputField IF_frq;
    /// </motor params>
    
    public void sendMotorCmd()
    {
        float.TryParse(IF_x.text, out motor_x);
        float.TryParse(IF_y.text, out motor_y);
        int.TryParse(IF_frq.text, out motor_frq);

        if(_myserial != null)
        {
            sendMotor(_myserial, motor_x, motor_y, motor_frq);
        }
    }

    /// <LED paprms>
    int[] R = new int[4];
    int[] G = new int[4];
    int[] B = new int[4];
    /// </LED paprms>
    public void setLEDcolor(string slider_name, float v)
    {
        int value = (int)v;
        //update RGB list
        int len = slider_name.Length;
        string c = slider_name.Substring(len-2, 1);
        int.TryParse(slider_name.Substring(len-1, 1), out int idx);
        //Debug.Log(c + idx);
        switch (c)
        {
            case "R":
                R[idx - 1] = value;
                break;
            case "G":
                G[idx - 1] = value;
                break;
            case "B":
                B[idx - 1] = value;
                break;
        }

        //foreach(int i in R)
        //{
        //    Debug.Log(i);
        //}

        if (_myserial!=null && _myserial.MySerial.IsOpen && !IsDebugOn)
        {
            _myserial.setLed(R, G, B);
        }
    }


    /// <debug params>
    bool IsDebugOn;
    public InputField IF_debuginfor;
    /// </debug params>

    public void debugOn(bool b)
    {
        if (b && _myserial!=null)
        {
            IsDebugOn = true;
            _myserial.onDebug();
        }else if(!b && _myserial != null)
        {
            IsDebugOn = false;
            _myserial.offDebug();
        }
    }

    /// <normal msg params>
    public Text pow_text;
    public Text oid_text;
    /// </normal msg params>
    
    public void getPower()
    {
        if(_myserial!=null && _myserial.MySerial.IsOpen && !IsDebugOn)
        {
            pow_text.text = _myserial.power.ToString() + "V";
        }
    }
    public void getOid()
    {
        if (_myserial != null && _myserial.MySerial.IsOpen && !IsDebugOn)
        {
            double[] pos = _myserial.posAvg(5);
            int deg = _myserial.angAvg(5);
            oid_text.text = "pos(" + pos[0].ToString() + ", " + pos[1].ToString() + ")" + " deg:" + deg.ToString();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        btn_connect.onClick.AddListener(() => connectSerial(btn_connect));
    }

    // Update is called once per frame
    void Update()
    {
        if(IsDebugOn && _myserial.MySerial.IsOpen)
        {
            string s = _myserial.getDebugInfor();
            IF_debuginfor.text = s;
        }
    }





    void sendMotor(ISerialMotor imotor, float x, float y, int f)
    {
        imotor.setG91(x,y,f);
    }


    private void OnApplicationQuit()
    {
        if(_myserial!=null && _myserial.MySerial.IsOpen)
        {
            _myserial.MySerial.Close();
        }
    }
}
