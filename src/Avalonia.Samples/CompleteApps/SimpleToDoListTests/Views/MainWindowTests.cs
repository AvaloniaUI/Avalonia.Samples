using Avalonia.Headless.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleToDoList.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleToDoList.ViewModels;
using Avalonia.Headless;
using Avalonia.Input;

namespace SimpleToDoList.Views.Tests;

[TestClass()]
public class MainWindowTests
{
    private MainViewModel _vm;

    [AvaloniaTestMethod()]
    public void MainWindowTest()
    {
        var window = new MainWindow()
        {
            DataContext = _vm = new MainViewModel()
        };

        window.Show();

        // Set values to the input boxes by simulating text input:
        window.ItemInput.Focus();
        window.KeyTextInput("Do something");

        // Raise click event on the button:
        window.ItemAddButton.Focus();
        window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);

        Assert.AreEqual(null,window.ItemInput.Text);
        Assert.AreEqual(1, _vm.ToDoItems.Count);
    }
}