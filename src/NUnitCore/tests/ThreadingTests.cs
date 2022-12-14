// ****************************************************************
// Copyright 2009, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Threading;
using NUnit.Framework;
using NUnit.TestUtilities;
using NUnit.TestData;

namespace NUnit.Core.Tests
{
    public class ThreadingTests
    {
        Thread parentThread;
        ApartmentState parentThreadApartment;

        Thread setupThread;

        [TestFixtureSetUp]
        public void GetParentThreadInfo()
        {
            this.parentThread = Thread.CurrentThread;
            this.parentThreadApartment = parentThread.ApartmentState;
        }

        [SetUp]
        public void GetSetUpThreadInfo()
        {
            this.setupThread = Thread.CurrentThread;
        }

        [Test, Timeout(50)]
        public void TestWithTimeoutRunsOnSeparateThread()
        {
            Assert.That(Thread.CurrentThread, Is.Not.EqualTo(parentThread));
        }

        [Test, Timeout(50)]
        public void TestWithTimeoutRunsSetUpAndTestOnSameThread()
        {
            Assert.That(Thread.CurrentThread, Is.EqualTo(setupThread));
        }

        [Test][Platform(Exclude="Mono")]
        public void TestWithInfiniteLoopTimesOut()
        {
            ThreadingFixture fixture = new ThreadingFixture();
            TestSuite suite = TestBuilder.MakeFixture(fixture);
            Test test = TestFinder.Find("InfiniteLoopWith50msTimeout", suite, false);
            TestResult result = test.Run(NullListener.NULL, TestFilter.Empty);
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.Message, Text.Contains("50ms"));
            Assert.That(fixture.TearDownWasRun, "TearDown was not executed");
        }

        [Test]
        public void SetUpWithInfiniteLoopTimesOut()
        {
            ThreadingFixtureWithTimeoutInSetUp fixture = new ThreadingFixtureWithTimeoutInSetUp();
            TestSuite suite = TestBuilder.MakeFixture(fixture);
            Test test = TestFinder.Find("TestWithTimeout", suite, false);
            TestResult result = test.Run(NullListener.NULL, TestFilter.Empty);
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.Message, Text.Contains("50ms"));
            Assert.That(fixture.TearDownWasRun, "TearDown was not run");
        }

        //[Test] Bug: Cannot abort the timeout because it's run in a finally clause
        public void TearDownWithInfiniteLoopTimesOut()
        {
            ThreadingFixtureWithTimeoutInTearDown fixture = new ThreadingFixtureWithTimeoutInTearDown();
            TestSuite suite = TestBuilder.MakeFixture(fixture);
            Test test = TestFinder.Find("TestWithTimeout", suite, false);
            TestResult result = test.Run(NullListener.NULL, TestFilter.Empty);
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.Message, Text.Contains("50ms"));
            Assert.That(fixture.TearDownWasRun, "TearDown was not run");
        }

        [Test, STAThread]
        public void TestWithSTAThreadRunsInSTA()
        {
            Assert.That(Thread.CurrentThread.ApartmentState, Is.EqualTo(ApartmentState.STA));
            if (parentThreadApartment == ApartmentState.STA)
                Assert.That(Thread.CurrentThread, Is.EqualTo(parentThread));
        }

        [Test, MTAThread]
        public void TestWithMTAThreadRunsInMTA()
        {
            Assert.That(Thread.CurrentThread.ApartmentState, Is.EqualTo(ApartmentState.MTA));
            if (parentThreadApartment == ApartmentState.MTA)
                Assert.That(Thread.CurrentThread, Is.EqualTo(parentThread));
        }

        [Test, RequiresSTA]
        public void TestWithRequiresSTARunsInSTA()
        {
            Assert.That(Thread.CurrentThread.ApartmentState, Is.EqualTo(ApartmentState.STA));
            if (parentThreadApartment == ApartmentState.STA)
                Assert.That(Thread.CurrentThread, Is.EqualTo(parentThread));
        }

        [Test, RequiresMTA]
        public void TestWithRequiresMTARunsInMTA()
        {
            Assert.That(Thread.CurrentThread.ApartmentState, Is.EqualTo(ApartmentState.MTA));
            if (parentThreadApartment == ApartmentState.MTA)
                Assert.That(Thread.CurrentThread, Is.EqualTo(parentThread));
        }

        [Test, RequiresThread]
        public void TestWithRequiresThreadRunsInSeparateThread()
        {
            Assert.That(Thread.CurrentThread, Is.Not.EqualTo(parentThread));
        }

        [Test, RequiresThread]
        public void TestWithRequiresThreadRunsSetUpAndTestOnSameThread()
        {
            Assert.That(Thread.CurrentThread, Is.EqualTo(setupThread));
        }

        [Test, RequiresThread(ApartmentState.STA)]
        public void TestWithRequiresThreadWithSTAArgRunsOnSeparateThreadInSTA()
        {
            Assert.That(Thread.CurrentThread.ApartmentState, Is.EqualTo(ApartmentState.STA));
            Assert.That(Thread.CurrentThread, Is.Not.EqualTo(parentThread));
        }

        [Test, RequiresThread(ApartmentState.MTA)]
        public void TestWithRequiresThreadWithMTAArgRunsOnSeparateThreadInMTA()
        {
            Assert.That(Thread.CurrentThread.ApartmentState, Is.EqualTo(ApartmentState.MTA));
            Assert.That(Thread.CurrentThread, Is.Not.EqualTo(parentThread));
        }
        
        [Test]
        public void TestOnSeparateThreadReportsAssertCountCorrectly()
        {
            TestResult result = TestBuilder.RunTestCase(typeof(ThreadingFixture), "MethodWithThreeAsserts");
            Assert.That(result.AssertCount, Is.EqualTo(3));
        }

        [Test][Platform(Exclude="Mono")]
        public void TimeoutCanBeSetOnTestFixture()
        {
            TestResult result = TestBuilder.RunTestFixture(typeof(ThreadingFixtureWithTimeout));
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            result = TestFinder.Find("Test2WithInfiniteLoop", result, false);
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.Message, Text.Contains("50ms"));
        }

        [TestFixture, RequiresSTA]
        class FixtureRequiresSTA
        {
            [Test]
            public void RequiresSTACanBeSetOnTestFixture()
            {
                Assert.That(Thread.CurrentThread.ApartmentState, Is.EqualTo(ApartmentState.STA));
            }
        }

        [TestFixture, RequiresMTA]
        class FixtureRequiresMTA
        {
            [Test]
            public void RequiresMTACanBeSetOnTestFixture()
            {
                Assert.That(Thread.CurrentThread.ApartmentState, Is.EqualTo(ApartmentState.MTA));
            }
        }

        [TestFixture, RequiresThread]
        class FixtureRequiresThread
        {
            [Test]
            public void RequiresThreadCanBeSetOnTestFixture()
            {
                Assert.That(Environment.StackTrace, Text.Contains("TestThread.RunTestProc"));
            }
        }
    }
}
