using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class SerialHandler
{
    private SerialPort serial;
    private Thread thread;
    private bool isActive = false;
    private string data;
    private bool update = false;

    public bool ReadStart(string portname,int baudrate)
    {
        if (isActive) ReadEnd();

        try
        {
            serial = new SerialPort(portname, baudrate);
            serial.Open();
            serial.DtrEnable = true;
            serial.RtsEnable = true;
            serial.DiscardInBuffer();
            serial.ReadTimeout = 5;
        }
        catch (Exception)
        {
            serial = null;
            return false;
        }

        isActive = true;
        update = false;
        thread = new Thread(ReadDataLoop);
        thread.Start();
        return true;
    }

    private void ReadDataLoop()
    {
        while (isActive)
        {
            try
            {
                data = serial.ReadLine();
                update = true;
            }
            catch (Exception)
            {
            }
        }
    }

    public bool ReadUpdate(ref string buffer)
    {
        if (update)
        {
            buffer = data;
            update = false;
            return true;
        }
        else
        {
            buffer = "";
            return false;
        }
    }

    public void ReadEnd()
    {
        isActive = false;
        thread.Join();
        serial.Close();
        thread = null;
        serial = null;
    }

    void OnDestroy()
    {
        ReadEnd();
    }
}
