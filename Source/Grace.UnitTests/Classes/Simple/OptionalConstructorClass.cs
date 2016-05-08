﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.UnitTests.Classes.Simple
{
    public interface IOptionalServiceConstructor
    {
        ISimpleObject SimpleObject { get; }
    }

    public class OptionalServiceConstructor : IOptionalServiceConstructor
    {
        public OptionalServiceConstructor(ISimpleObject simpleObject = null)
        {
            SimpleObject = simpleObject;
        }

        public ISimpleObject SimpleObject { get; private set; }
    }

    public interface IOptionalIntServiceConstructor
    {
        ISimpleObject SimpleObject { get; }

        int Value { get; }
    }

    public class OptionalIntServiceConstructor : IOptionalIntServiceConstructor
    {
        public OptionalIntServiceConstructor(ISimpleObject simpleObject, int value = 0)
        {
            Value = value;
            SimpleObject = simpleObject;
        }

        public ISimpleObject SimpleObject { get; private set; }

        public int Value { get; private set; }
    }
}
