// <copyright file="PayrollTest.cs">Copyright ©  2017</copyright>
using System;
using MSAMISUserInterface;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSAMISUserInterface.Tests
{
    /// <summary>This class contains parameterized unit tests for Payroll</summary>
    [PexClass(typeof(Payroll))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class PayrollTest
    {
        /// <summary>Test stub for ComputeSSS(Int32)</summary>
        [PexMethod]
        public double ComputeSSSTest([PexAssumeUnderTest]Payroll target, int contrib_id)
        {
            double result = target.ComputeSSS(contrib_id);
            return result;
            // TODO: add assertions to method PayrollTest.ComputeSSSTest(Payroll, Int32)
        }
    }
}
