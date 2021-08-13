﻿using System;
using AELTA_test;
using RASDK.Arm.Hiwin;

namespace RASDK.Arm.TMRobot
{
    public class NowPosition
    {
        private SocketClientObject _socketClientObject;
        private CommandSender _commandSender;
        private double[] _position;

        public NowPosition(SocketClientObject socketClientObject)
        {
            _commandSender = new CommandSender(socketClientObject);
            _socketClientObject = socketClientObject;
            _socketClientObject.ReceiveData += ReceiveDataHandler;
        }

        ~NowPosition()
        {
            _socketClientObject.ReceiveData -= ReceiveDataHandler;
        }

        public double[] Value
        {
            get
            {
                _commandSender.Send("1,ListenSend(90,GetString(Robot[0].CoordRobot))");
                return _position;
            }
        }

        private void ReceiveDataHandler(object sender, string recvData)
        {
            recvData = recvData.Trim();
            if (recvData.IndexOf("$TMSTA") == 0 && recvData.Split(',')[2] == "90")
            {

                var head = recvData.IndexOf('{');
                var end = recvData.IndexOf('}');
                var pos = recvData.Substring(head + 1, (end - head) - 1).Split(',');

                if (pos.Length == 6)
                {
                    _position = new[] { 0.0, 0, 0, 0, 0, 0 };
                    for (int i = 0; i < 6; i++)
                    {
                        try
                        {
                            _position[i] = Convert.ToDouble(pos[i]);
                        }
                        catch (Exception)
                        {
                            _position = null;
                            return;
                        }
                    }
                }
                else
                {
                    _position = null;
                }
            }
        }
    }
}