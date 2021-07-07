﻿using Basic.Message;

namespace Arm
{
    public abstract class ArmActionFactory
    {
        protected readonly IMessage _message;

        public ArmActionFactory(IMessage message)
        {
            _message = message;
        }

        public abstract Connect GetConnect();
    }
}