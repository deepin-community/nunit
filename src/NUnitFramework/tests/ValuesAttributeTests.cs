// ****************************************************************
// Copyright 2009, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Reflection;

namespace NUnit.Framework.Tests
{
    public class ValuesAttributeTests
    {
        [Test]
        public void ValuesAttributeProvidesSpecifiedValues()
        {
            CheckValues("MethodWithValues", 1, 2, 3);
        }

        private void MethodWithValues( [Values(1, 2, 3)] int x) { }

        [Test]
        public void CanConvertSmallIntsToShort([Values(5)]short x)
        {
        }

        [Test]
        public void CanConvertSmallIntsToByte([Values(5)]byte x)
        {
        }

        [Test]
        public void CanConvertSmallIntsToSByte([Values(5)]sbyte x)
        {
        }

        [Test]
        public void CanConvertIntToDecimal([Values(12)]decimal x)
        {
        }

        [Test]
        public void CanConverDoubleToDecimal([Values(12.5)]decimal x)
        {
        }

        [Test]
        public void CanConvertStringToDecimal([Values("12.5")]decimal x)
        {
        }

        [Test]
        public void RangeAttributeWithIntRange()
        {
            CheckValues("MethodWithIntRange", 11, 12, 13, 14, 15);
        }

        private void MethodWithIntRange([Range(11, 15)] int x) { }

        [Test]
        public void RangeAttributeWithIntRangeAndStep()
        {
            CheckValues("MethodWithIntRangeAndStep", 11, 13, 15);
        }

        private void MethodWithIntRangeAndStep([Range(11, 15, 2)] int x) { }

        [Test]
        public void RangeAttributeWithLongRangeAndStep()
        {
            CheckValues("MethodWithLongRangeAndStep", 11L, 13L, 15L);
        }

        private void MethodWithLongRangeAndStep([Range(11L, 15L, 2)] long x) { }

        [Test]
        public void RangeAttributeWithDoubleRangeAndStep()
        {
            CheckValuesWithinTolerance("MethodWithDoubleRangeAndStep", 0.7, 0.9, 1.1);
        }

        private void MethodWithDoubleRangeAndStep([Range(0.7, 1.2, 0.2)] double x) { }

        [Test]
        public void RangeAttributeWithFloatRangeAndStep()
        {
            CheckValuesWithinTolerance("MethodWithFloatRangeAndStep", 0.7f, 0.9f, 1.1f);
        }

        private void MethodWithFloatRangeAndStep([Range(0.7f, 1.2f, 0.2f)] float x) { }

        [Test]
        public void CanConvertIntRangeToShort([Range(1, 3)] short x) { }

        [Test]
        public void CanConvertIntRangeToByte([Range(1, 3)] byte x) { }

        [Test]
        public void CanConvertIntRangeToSByte([Range(1, 3)] sbyte x) { }

        [Test]
        public void CanConvertIntRangeToDecimal([Range(1, 3)] decimal x) { }

        [Test]
        public void CanConvertDoubleRangeToDecimal([Range(1.0, 1.3, 0.1)] decimal x) { }

        [Test]
        public void CanConvertRandomIntToShort([Random(1, 10, 3)] short x) { }

        [Test]
        public void CanConvertRandomIntToByte([Random(1, 10, 3)] byte x) { }

        [Test]
        public void CanConvertRandomIntToSByte([Random(1, 10, 3)] sbyte x) { }

        [Test]
        public void CanConvertRandomIntToDecimal([Random(1, 10, 3)] decimal x) { }
        
        [Test]
        public void CanConvertRandomDoubleToDecimal([Random(1.0, 10.0, 3)] decimal x) { }

        #region Helper Methods
        private void CheckValues(string methodName, params object[] expected)
        {
            MethodInfo method = GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            ParameterInfo param = method.GetParameters()[0];
            ValuesAttribute attr = param.GetCustomAttributes(typeof(ValuesAttribute), false)[0] as ValuesAttribute;
            Assert.That(attr.GetData(param), Is.EqualTo(expected));
        }

        private void CheckValuesWithinTolerance(string methodName, params object[] expected)
        {
            MethodInfo method = GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            ParameterInfo param = method.GetParameters()[0];
            ValuesAttribute attr = param.GetCustomAttributes(typeof(ValuesAttribute), false)[0] as ValuesAttribute;
            Assert.That(attr.GetData(param), Is.EqualTo(expected).Within(0.000001));
        }
        #endregion
    }
}
