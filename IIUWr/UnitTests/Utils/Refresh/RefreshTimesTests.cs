using IIUWr.Utils.Refresh;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace UnitTests.Utils.Refresh
{
    [TestClass]
    public class RefreshTimesTests
    {
        private RefreshTimes _times;
        private List<NotifyCollectionChangedEventArgs> _updates;

        [TestInitialize]
        public void TestInitialize()
        {
            _times = new RefreshTimes();
            _updates = new List<NotifyCollectionChangedEventArgs>();
            _times.CollectionChanged += CollectionChanged;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _times.CollectionChanged -= CollectionChanged;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _updates.Add(e);
        }

        [DataTestMethod]
        [DataRow(RefreshType.Failed, DisplayName = "Failed")]
        [DataRow(RefreshType.Basic, DisplayName = "Basic")]
        [DataRow(RefreshType.Full, DisplayName = "Full")]
        [DataRow(RefreshType.LoggedInBasic, DisplayName = "LoggedInBasic")]
        [DataRow(RefreshType.LoggedInFull, DisplayName = "LoggedInFull")]
        public void Set_Type(RefreshType type)
        {
            Set(type);

            Assert.AreEqual(1, _times.Count);
            CheckTime(0, type);

            Assert.AreEqual(1, _updates.Count);
            CheckUpdate(0, NotifyCollectionChangedAction.Add, type, 0);
        }

        [DataTestMethod]
        [DataRow(RefreshType.Failed, DisplayName = "Failed")]
        [DataRow(RefreshType.Basic, DisplayName = "Basic")]
        [DataRow(RefreshType.Full, DisplayName = "Full")]
        [DataRow(RefreshType.LoggedInBasic, DisplayName = "LoggedInBasic")]
        [DataRow(RefreshType.LoggedInFull, DisplayName = "LoggedInFull")]
        public void Set_Type_Update(RefreshType type)
        {
            bool updated = false;
            var refreshTime = new RefreshTime(type, DateTimeOffset.Now.AddMinutes(-1));
            refreshTime.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(RefreshTime.Time))
                {
                    updated = true;
                }
            };

            _times.Set(refreshTime);
            Set(type);

            Assert.AreEqual(1, _times.Count);
            CheckTime(0, type);

            Assert.AreEqual(1, _updates.Count);
            CheckUpdate(0, NotifyCollectionChangedAction.Add, type, 0);

            Assert.IsTrue(updated);
        }

        [TestMethod]
        public void Set_Basic_Full()
        {
            Set(RefreshType.Basic);
            Set(RefreshType.Full);

            int i = 0;
            Assert.AreEqual(1, _times.Count);
            CheckTime(i++, RefreshType.Full);

            i = 0;
            Assert.AreEqual(3, _updates.Count);
            CheckUpdate(i++, NotifyCollectionChangedAction.Add, RefreshType.Basic, 0);
            CheckUpdate(i++, NotifyCollectionChangedAction.Remove, RefreshType.Basic, 0);
            CheckUpdate(i++, NotifyCollectionChangedAction.Add, RefreshType.Full, 0);
        }

        [TestMethod]
        public void Set_Full_Basic()
        {
            Set(RefreshType.Full);
            Set(RefreshType.Basic);

            int i = 0;
            Assert.AreEqual(2, _times.Count);
            CheckTime(i++, RefreshType.Basic);
            CheckTime(i++, RefreshType.Full);

            i = 0;
            Assert.AreEqual(2, _updates.Count);
            CheckUpdate(i++, NotifyCollectionChangedAction.Add, RefreshType.Full, 0);
            CheckUpdate(i++, NotifyCollectionChangedAction.Add, RefreshType.Basic, 0);
        }

        [TestMethod]
        public void Set_Full_LogedInFull()
        {
            Set(RefreshType.Full);
            Set(RefreshType.LoggedInFull);

            int i = 0;
            Assert.AreEqual(1, _times.Count);
            CheckTime(i++, RefreshType.LoggedInFull);

            i = 0;
            Assert.AreEqual(3, _updates.Count);
            CheckUpdate(i++, NotifyCollectionChangedAction.Add, RefreshType.Full, 0);
            CheckUpdate(i++, NotifyCollectionChangedAction.Remove, RefreshType.Full, 0);
            CheckUpdate(i++, NotifyCollectionChangedAction.Add, RefreshType.LoggedInFull, 0);
        }

        [TestMethod]
        public void Set_Basic_LogedInFull()
        {
            Set(RefreshType.Basic);
            Set(RefreshType.LoggedInFull);

            int i = 0;
            Assert.AreEqual(1, _times.Count);
            CheckTime(i++, RefreshType.LoggedInFull);

            i = 0;
            Assert.AreEqual(3, _updates.Count);
            CheckUpdate(i++, NotifyCollectionChangedAction.Add, RefreshType.Basic, 0);
            CheckUpdate(i++, NotifyCollectionChangedAction.Remove, RefreshType.Basic, 0);
            CheckUpdate(i++, NotifyCollectionChangedAction.Add, RefreshType.LoggedInFull, 0);
        }

        [TestMethod]
        public void Set_Full_LogedInBasic()
        {
            Set(RefreshType.Full);
            Set(RefreshType.LoggedInBasic);

            int i = 0;
            Assert.AreEqual(2, _times.Count);
            CheckTime(i++, RefreshType.Full);
            CheckTime(i++, RefreshType.LoggedInBasic);

            i = 0;
            Assert.AreEqual(2, _updates.Count);
            CheckUpdate(i++, NotifyCollectionChangedAction.Add, RefreshType.Full, 0);
            CheckUpdate(i++, NotifyCollectionChangedAction.Add, RefreshType.LoggedInBasic, 1);
        }

        [TestMethod]
        public void Set_Full_LogedInBasic_LogedInFull()
        {
            Set(RefreshType.Full);
            Set(RefreshType.LoggedInBasic);
            Set(RefreshType.LoggedInFull);

            int i = 0;
            Assert.AreEqual(1, _times.Count);
            CheckTime(i++, RefreshType.LoggedInFull);

            i = 0;
            Assert.AreEqual(5, _updates.Count);
            CheckUpdate(i++, NotifyCollectionChangedAction.Add, RefreshType.Full, 0);
            CheckUpdate(i++, NotifyCollectionChangedAction.Add, RefreshType.LoggedInBasic, 1);
            CheckUpdate(i++, NotifyCollectionChangedAction.Remove, RefreshType.LoggedInBasic, 1);
            CheckUpdate(i++, NotifyCollectionChangedAction.Remove, RefreshType.Full, 0);
            CheckUpdate(i++, NotifyCollectionChangedAction.Add, RefreshType.LoggedInFull, 0);
        }

        private void Set(RefreshType type)
        {
            _times.Set(new RefreshTime(type));
        }

        private void CheckTime(int index, RefreshType type)
        {
            Assert.AreEqual(type, _times[index].Type);
        }

        private void CheckUpdate(int index, NotifyCollectionChangedAction action, RefreshType type, int startIndex)
        {
            Assert.AreEqual(action, _updates[index].Action);
            switch (action)
            {
                case NotifyCollectionChangedAction.Add:
                    Assert.AreEqual(1, _updates[index].NewItems.Count);
                    Assert.AreEqual(type, ((RefreshTime)_updates[index].NewItems[0]).Type);
                    Assert.AreEqual(startIndex, _updates[index].NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Move:
                    Assert.Fail("Action == Move");
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Assert.AreEqual(1, _updates[index].OldItems.Count);
                    Assert.AreEqual(type, ((RefreshTime)_updates[index].OldItems[0]).Type);
                    Assert.AreEqual(startIndex, _updates[index].OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Assert.Fail("Action == Replace");
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Assert.Fail("Action == Reset");
                    break;
                default:
                    Assert.Fail("Unknown Action");
                    break;
            }
            
        }
    }
}
