using Avalonia.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleToDoList.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleToDoList.ViewModels.Tests;

[TestClass()]
public class MainViewModelTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    private MainViewModel testModel;
    private MainViewModel testModel2;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    private string sTestLog = "";
    private bool xDesignMode;

    [TestInitialize()]
    public void TestInitialize()
    {
        testModel = new MainViewModel();
        testModel.PropertyChanged += (object? sender, PropertyChangedEventArgs e)
            => DoLog($"PropChg(Sender: {sender?.GetType().Name}, Prop: {e.PropertyName}), Value = ${sender?.GetType().GetProperty(e.PropertyName ?? "")?.GetValue(sender)}");
        testModel.AddItemCommand.CanExecuteChanged += (object? sender, EventArgs e)
            => DoLog($"CmdChg(Sender: {sender?.GetType().Name}) = {sender?.GetType().GetMethod("CanExecute")!.Invoke(sender,[null])}");       
        sTestLog = "";
        xDesignMode = Design.IsDesignMode;
        typeof(Design).GetProperty("IsDesignMode")!.SetValue(null, true);
        testModel2 = new MainViewModel();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        typeof(Design).GetProperty("IsDesignMode")!.SetValue(null, xDesignMode);
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
        Assert.IsInstanceOfType(testModel, typeof(MainViewModel));
        Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        Assert.IsInstanceOfType(testModel2, typeof(MainViewModel));
        Assert.IsInstanceOfType(testModel2, typeof(INotifyPropertyChanged));
        Assert.IsNotNull(testModel.ToDoItems);
        Assert.IsNotNull(testModel2.ToDoItems);
        Assert.AreEqual(0, testModel.ToDoItems.Count);
        Assert.AreEqual(2,testModel2.ToDoItems.Count);
    }

    [DataTestMethod()]
    [DataRow(new string[0], "")]
    [DataRow(new[]{ "Test" }, @"PropChg(Sender: MainViewModel, Prop: NewItemContent), Value = $Test\r\nCmdChg(Sender: RelayCommand) = True\r\n")]
    [DataRow(new[]{ "Test2", null  },@"PropChg(Sender: MainViewModel, Prop: NewItemContent), Value = $Test2\r\nCmdChg(Sender: RelayCommand) = True\r\nPropChg(Sender: MainViewModel, Prop: NewItemContent), Value = $\r\nCmdChg(Sender: RelayCommand) = False\r\n")]
    [DataRow(new[]{ null, "Test3"  }, @"PropChg(Sender: MainViewModel, Prop: NewItemContent), Value = $Test3\r\nCmdChg(Sender: RelayCommand) = True\r\n")]
    public void SetNewItemTest(string?[] asAct,string sExp)
    {
        // Act
        foreach (string? s in asAct)
        {
            testModel.NewItemContent = s;
        }

        // Assert
        Assert.AreEqual(sExp.Replace("\\r\\n","\r\n"), sTestLog);
    }

    [DataTestMethod()]
    [DataRow("Test", 1, @"PropChg(Sender: MainViewModel, Prop: NewItemContent), Value = $Test\r\nCmdChg(Sender: RelayCommand) = True\r\nPropChg(Sender: MainViewModel, Prop: NewItemContent), Value = $\r\nCmdChg(Sender: RelayCommand) = False\r\n")]
    [DataRow("",0, "PropChg(Sender: MainViewModel, Prop: NewItemContent), Value = $\\r\\nCmdChg(Sender: RelayCommand) = False\\r\\n")]
    public void AddItemTest(string sAct,int iExp,string sExp)
    {
        // Arrange
        testModel.NewItemContent = sAct;
        // Act
        if (testModel.AddItemCommand.CanExecute(null))
            testModel.AddItemCommand.Execute(null);

        // Assert
        Assert.AreEqual(iExp, testModel.ToDoItems.Count);
        if (iExp >0)
           Assert.AreEqual(sAct, testModel.ToDoItems[0].Content);
        Assert.AreEqual(iExp > 0?null:sAct, testModel.NewItemContent);
        Assert.AreEqual( sExp.Replace("\\r\\n", "\r\n"), sTestLog);
    }

    [DataTestMethod()]
    [DataRow(new[] { "Test", "Test2" }, "Test" )]
    [DataRow(new[] { "Test", "Test2" }, "Test2" )]
    public void RemoveItemTest(string[] sAct, string sAct2)
    {
        // Arrange
        foreach (string s in sAct)
        {
            testModel.ToDoItems.Add(new() {IsChecked =false,Content = s });
        }
        var _act2 = testModel.ToDoItems.First(s=>s.Content==sAct2);

        // Act
        if (testModel.RemoveItemCommand.CanExecute(_act2))
            testModel.RemoveItemCommand.Execute(_act2);

        // Assert
        Assert.AreEqual(sAct.Length-1, testModel.ToDoItems.Count);
        Assert.AreEqual("", sTestLog);
    }

}