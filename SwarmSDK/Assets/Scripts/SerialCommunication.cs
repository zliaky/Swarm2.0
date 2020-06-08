using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class SerialCommunication : MonoBehaviour, ISerialLed, ISerialDebug, ISerialInfor, ISerialMotor
{
    private SerialPort _serial;

    public SerialCommunication(string port_name, int baud_rate)
    {
        _serial = new SerialPort(port_name, baud_rate);
        _serial.Open();
    }

    public float power {
        get {
            _serial.WriteLine("VOLREAD");
            string temps = _serial.ReadLine();//返回格式为VMM=x.xxxxV
            float fpow = float.Parse(temps.Split('=')[1]);
            return fpow;
        }
    }

    public string getDebugInfor()
    {
        return _serial.ReadLine();
    }

    public void offDebug()
    {
        _serial.WriteLine("DEBUGOFF");
    }

    public void onDebug()
    {
        _serial.WriteLine("DEBUGON");
    }

    public double[] posAvg(int sample_rate)
    {
        _serial.WriteLine("OIDREAD" + sample_rate.ToString());
        //返回格式：OIDPOS1 X=xxx.xxxx, Y=xxx.xxxx, D=xxx;
        double sum_x = 0d;
        double sum_y = 0d;
        for(int i = 0; i < sample_rate; i++)
        {
            string temp_oid = _serial.ReadLine();
            string[] splited = temp_oid.Split(' ');
            string sx = splited[1].Split('=')[1];
            string sy = splited[2].Split('=')[1];
            sx = sx.Substring(0, sx.Length - 2);
            sy = sy.Substring(0, sy.Length - 2);
            double.TryParse(sx, out double dx);
            double.TryParse(sy, out double dy);
            sum_x += dx;
            sum_y += dy;
        }

        double[] pos = { sum_x / sample_rate, sum_y / sample_rate };
        return pos;
    }

    public int angAvg(int sample_rate)
    {
        _serial.WriteLine("OIDREAD" + sample_rate.ToString());
        //返回格式：OIDPOS1 X=xxx.xxxx, Y=xxx.xxxx, D=xxx;
        int sum_d = 0;
        for (int i = 0; i < sample_rate; i++)
        {
            string temp_oid = _serial.ReadLine();
            string[] splited = temp_oid.Split(' ');
            string sd = splited[1].Split('=')[3];
            sd = sd.Substring(0, sd.Length - 2);
            int.TryParse(sd, out int deg);
            sum_d += deg;
        }

        return sum_d / sample_rate;
    }

    public void setG91(float x, float y, int frq)
    {
        _serial.WriteLine("MDRIVEG91X" + x.ToString() + "Y" + y.ToString() + "F" + frq.ToString());
    }

    public void setLed(int[] r, int[] g, int[] b)
    {
        string cmd = "LED";
        for (int i = 0; i < r.Length; i++)
        {
            cmd += "R" + r[i] + "G" + g[i] + "B" + b[i];
        }
        _serial.WriteLine(cmd);
    }

    public void setVT(float velocity, float time)
    {
        throw new System.NotImplementedException();
    }


    public SerialPort MySerial
    {
        get { return _serial; }
    }
}
