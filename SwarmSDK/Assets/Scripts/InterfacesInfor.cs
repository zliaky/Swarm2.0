using System;
//using System.Collections.Generic;
//using UnityEngine;

public interface ISerialLed
{
    void setLed(int[] r, int[] g, int[] b);
}


public interface ISerialDebug
{
    void onDebug();
    String getDebugInfor();
    void offDebug();
}


public interface ISerialInfor
{
    float power { get; }

    //除以采样率后均值
    double[] posAvg(int sample_rate);
    int angAvg(int sample_rate);
}


public interface ISerialMotor
{
    void setG91(float x, float y, int frq);
    void setVT(float velocity, float time);//等待具体格
}