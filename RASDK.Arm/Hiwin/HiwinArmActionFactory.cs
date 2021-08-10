﻿using System;
using RASDK.Basic.Message;
using RASDK.Arm.Type;

namespace RASDK.Arm.Hiwin
{
    public class HiwinArmActionFactory : ArmActionFactory
    {
        private readonly string _ip;
        private int _id;
        private static bool _waiting = false;

        public HiwinArmActionFactory(string ip, IMessage message) : base(message)
        {
            _ip = ip;
        }

        public int Id => _id;

        public override IGetConnectionState GetConnectionState(out bool connected)
        {
            return new HiwinGetConnectionState(_id, _message, out connected);
        }

        public override IConnection Connection()
        {
            return new HiwinConnection(_ip, _message, ref _id, ref _waiting);
        }

        public override IAbsoluteMotion AbsoluteMotion(double[] position,
                                                       AdditionalMotionParameters additionalMotionParameters = null)
        {
            if (position.Length == 6)
            {
                return new HiwinAbsoluteMotion(position[(int)Axis.XJ1],
                                               position[(int)Axis.YJ2],
                                               position[(int)Axis.ZJ3],
                                               position[(int)Axis.AJ4],
                                               position[(int)Axis.BJ5],
                                               position[(int)Axis.CJ6],
                                               _id,
                                               _message,
                                               out var returnCode,
                                               ref _waiting,
                                               additionalMotionParameters);
            }
            else
            {
                throw new ArgumentException("Length of position must be 6");
            }
        }

        public override IAbsoluteMotion AbsoluteMotion(double xJ1,
                                                       double yJ2,
                                                       double zJ3,
                                                       double aJ4,
                                                       double bJ5,
                                                       double cJ6,
                                                       AdditionalMotionParameters additionalMotionParameters = null)
        {
            return new HiwinAbsoluteMotion(xJ1,
                                           yJ2,
                                           zJ3,
                                           aJ4,
                                           bJ5,
                                           cJ6,
                                           _id,
                                           _message,
                                           out var returnCode,
                                           ref _waiting,
                                           additionalMotionParameters);
        }

        public override IRelativeMotion RelativeMotion(double[] position,
                                                       AdditionalMotionParameters additionalMotionParameters = null)
        {
            if (position.Length == 6)
            {
                return new HiwinRelativeMotion(position[(int)Axis.XJ1],
                                               position[(int)Axis.YJ2],
                                               position[(int)Axis.ZJ3],
                                               position[(int)Axis.AJ4],
                                               position[(int)Axis.BJ5],
                                               position[(int)Axis.CJ6],
                                               _id,
                                               _message,
                                               out var returnCode,
                                               ref _waiting,
                                               additionalMotionParameters);
            }
            else
            {
                throw new ArgumentException("Length of position must be 6");
            }
        }

        public override IRelativeMotion RelativeMotion(double xJ1,
                                                       double yJ2,
                                                       double zJ3,
                                                       double aJ4,
                                                       double bJ5,
                                                       double cJ6,
                                                       AdditionalMotionParameters additionalMotionParameters = null)
        {
            return new HiwinRelativeMotion(xJ1,
                                           yJ2,
                                           zJ3,
                                           aJ4,
                                           bJ5,
                                           cJ6,
                                           _id,
                                           _message,
                                           out var returnCode,
                                           ref _waiting,
                                           additionalMotionParameters);
        }

        public override IHoming Homing(CoordinateType coordinateType = CoordinateType.Descartes, bool needWait = true)
        {
            return new HiwinHoming(coordinateType, _id, _message, ref _waiting, needWait);
        }

        public override IJog Jog(string axis, bool needWait = true)
        {
            return new HiwinJog(axis, _id, _message, ref _waiting, needWait);
        }

        public override IAbortMotion AbortMotion()
        {
            return new HiwinAbortMotion(_id, _message);
        }
    }
}