using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleToDoList.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleToDoList.ViewModels.Tests
{
    [TestClass()]
    public class ToDoItemViewModelTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        private ToDoItemViewModel testModel;
        private ToDoItemViewModel testModel2;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        private string sTestLog="";

        [TestInitialize()]
        public void TestInitialize()
        {
            testModel = new ToDoItemViewModel();
            testModel2 = new ToDoItemViewModel(new() {Content = "Test2" });
            testModel2.PropertyChanged += (sender, e)
                => DoLog($"PropChg(Sender: {sender?.GetType().Name}, Prop: {e.PropertyName}), Value = ${sender?.GetType().GetProperty(e.PropertyName ?? "")?.GetValue(sender)}");
        }
        private void DoLog(string v)
        {
            sTestLog += $"{v}\r\n"; // !! Fixed NewLine-Sequence
        }

        [TestMethod()]
        public void SetUpTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsNotNull(testModel2);
            Assert.IsInstanceOfType(testModel, typeof(ToDoItemViewModel));
            Assert.IsInstanceOfType(testModel2, typeof(ToDoItemViewModel));
            Assert.AreEqual("Test2", testModel2.Content);
            Assert.IsFalse(testModel2.IsChecked);
            Assert.AreEqual("", sTestLog);
        }

        [DataTestMethod()]
        public void GetToDoItemTest2()
        {
            //Act
            var result = testModel2.GetToDoItem();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test2", result.Content);
            Assert.IsFalse(result.IsChecked);
        }
    }
}