using FluentAssertions;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace DrakersDev.Tests
{
    public class RepeatOperationBaseTests
    {
        private static void SetOpMockAbstractMethods(Mock<RepeatOperationBase> mock, Action prepare, Action operation, Action finish)
        {
            mock.Protected()
                .Setup("PrepareTask")
                .Callback(prepare);
            mock.Protected()
                .Setup("Operation")
                .Callback(operation);
            mock.Protected()
                .Setup("FinishTask")
                .Callback(finish);
        }

        [Theory]
        [InlineData(RepeatOperationType.Task)]
        [InlineData(RepeatOperationType.Thread)]
        public void Prepare_And_Finish_Task_Execute_Only_Once(RepeatOperationType opType)
        {
            var exeList = new List<String>();
            var mock = new Mock<RepeatOperationBase>(opType);
            var op = mock.Object;
            var resetEvent = new ManualResetEvent(false);
            #region Mock 설정
            SetOpMockAbstractMethods(mock,
                () =>
                {
                    exeList.Add("PrepareTask");
                },
                () =>
                {
                    exeList.Add("Operation");
                    op.Stop();
                },
                () =>
                {
                    exeList.Add("FinishTask");
                    resetEvent.Set();
                });
            #endregion
            op.Start();
            resetEvent.WaitOne();
            exeList.Should().ContainSingle(v => v.Equals("PrepareTask"));
            exeList.Should().ContainSingle(v => v.Equals("FinishTask"));
            exeList.Should().Contain(v => v.Equals("Operation"));
            exeList.Should().StartWith("PrepareTask");
            exeList.Should().EndWith("FinishTask");
        }

        [Theory]
        [InlineData(RepeatOperationType.Task)]
        [InlineData(RepeatOperationType.Thread)]
        public void Indicates_An_OperationStage_That_Fits_The_Current_Stage(RepeatOperationType opType)
        {
            var stageList = new List<RepeatOperationStage>();
            var mock = new Mock<RepeatOperationBase>(opType);
            var op = mock.Object;
            var resetEvent = new ManualResetEvent(false);
            #region Mock 설정
            SetOpMockAbstractMethods(mock, () => { }, () => { op.Stop(); }, () => { });
            op.RepeatStart += (s, e) =>
            {
                stageList.Add(op.OperationStage);
            };
            op.EachRepeatStart += (s, e) =>
            {
                stageList.Add(op.OperationStage);
            };
            op.EachRepeatEnd += (s, e) =>
            {
                stageList.Add(op.OperationStage);
            };
            op.RepeatEnd += (s, e) =>
            {
                stageList.Add(op.OperationStage);
                resetEvent.Set();
            };
            #endregion
            op.Start();
            resetEvent.WaitOne();
            stageList.Should().ContainSingle(v => v == RepeatOperationStage.Start);
            stageList.Should().ContainSingle(v => v == RepeatOperationStage.Stop);
            stageList.Should().Contain(RepeatOperationStage.Repeat);
            stageList.Should().StartWith(RepeatOperationStage.Start);
            stageList.Should().EndWith(RepeatOperationStage.Stop);
        }

        [Theory]
        [InlineData(RepeatOperationType.Task)]
        [InlineData(RepeatOperationType.Thread)]
        public void Sleep_Interval_Is_Calculated_Only_Set_Interval_If_IntervalCalulateType_Is_AfterOperation(RepeatOperationType opType)
        {
            var stageList = new List<RepeatOperationStage>();
            var mock = new Mock<RepeatOperationBase>(opType);
            var op = mock.Object;
            op.IntervalCalculateType = RepeatIntervalCalculateType.AfterOperation;
            var resetEvent = new ManualResetEvent(false);
            #region Mock 설정
            SetOpMockAbstractMethods(mock,
                () => { },
                () =>
                {
                    op.Stop();
                },
                () =>
                {
                    resetEvent.Set();
                });
            #endregion
            op.Start();
            resetEvent.WaitOne();
            op.LatestSleepTime.Should().Be(100);
        }

        [Theory]
        [InlineData(RepeatOperationType.Task)]
        [InlineData(RepeatOperationType.Thread)]
        public void Sleep_Interval_Is_Calculated_As_The_Set_Interval_Minus_Operation_Time_If_IntervalCalculateType_Is_TotalPeriod(RepeatOperationType opType)
        {
            var stageList = new List<RepeatOperationStage>();
            var mock = new Mock<RepeatOperationBase>(opType);
            var op = mock.Object;
            op.IntervalCalculateType = RepeatIntervalCalculateType.TotalPeriod;
            var resetEvent = new ManualResetEvent(false);
            Int32 testSleepMs = 50;
            #region Mock 설정
            SetOpMockAbstractMethods(mock,
                () => { },
                () =>
                {
                    Thread.Sleep(100);
                    op.Stop();
                },
                () =>
                {
                    resetEvent.Set();
                });
            #endregion
            op.Start();
            resetEvent.WaitOne();
            op.LatestSleepTime.Should().BeLessThanOrEqualTo(op.Interval - testSleepMs);
        }
    }
}
