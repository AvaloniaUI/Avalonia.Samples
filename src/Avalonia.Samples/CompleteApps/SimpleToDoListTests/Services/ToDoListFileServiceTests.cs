using SimpleToDoList.Models;

namespace SimpleToDoList.Services.Tests;

[TestClass()]
public class ToDoListFileServiceTests
{
    // The path to the file we are going to test
    private string destPath;

    [TestInitialize]
    public void TestInitialize()
    {
        destPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Avalonia.SimpleToDoList", "MyToDoList.txt");

        // Move a file if it exists to a backup file
        if (File.Exists(destPath))
        {
            File.Move(destPath, Path.ChangeExtension(destPath, ".txorg"));
        }
    }

    [TestCleanup]
    public void TestCleanup()
    {
        // Delete the file we created
        if (File.Exists(destPath))
        {
            File.Delete(destPath);
        }

        // Move the backup file back
        if (File.Exists(Path.ChangeExtension(destPath, ".txorg")))
        {
            File.Move(Path.ChangeExtension(destPath, ".txorg"),destPath);
        }
    }


    [DataTestMethod()]
    [DataRow(new[] { "Hello World" }, new[] { true }, "[{\"IsChecked\":true,\"Content\":\"Hello World\"}]")]
    [DataRow(new[] { "Get up", "Do chores" }, new[] { true, false }, "[{\"IsChecked\":true,\"Content\":\"Get up\"},{\"IsChecked\":false,\"Content\":\"Do chores\"}]")]
    public void SaveToFileAsyncTest(string[] sAct, bool[] xAct, string sExp)
    {
        // Arrange
        var items = sAct.Zip(xAct, (s, x) => new ToDoItem { Content = s, IsChecked = x });

        // Act
        Task t = ToDoListFileService.SaveToFileAsync(items);
        t.Wait();

        // Assert
        Assert.IsTrue(File.Exists(destPath));
        Assert.AreEqual(sExp, File.ReadAllText(destPath));
    }

    [DataTestMethod()]
    [DataRow(new[] { "Hello World" }, new[] { true }, "[{\"IsChecked\":true,\"Content\":\"Hello World\"}]")]
    [DataRow(new[] { "Get up", "Do chores" }, new[] { true, false }, "[{\"IsChecked\":true,\"Content\":\"Get up\"},{\"IsChecked\":false,\"Content\":\"Do chores\"}]")]
    public void LoadFromFileAsyncTest(string[] sExp, bool[] xExp, string sAct)
    {
        // Arrange
        File.WriteAllText(destPath, sAct);

        // Act
        Task<IEnumerable<ToDoItem>?> t = ToDoListFileService.LoadFromFileAsync();
        t.Wait();

        // Assert
        Assert.IsNotNull(t.Result);
        var items = t.Result;
        Assert.AreEqual(sExp.Length, items.Count());
        for (int i = 0; i < sExp.Length; i++)
        {
            Assert.AreEqual(sExp[i], items.ElementAt(i).Content);
            Assert.AreEqual(xExp[i], items.ElementAt(i).IsChecked);
        }
    }

    [TestMethod()]
    public void LoadFromFileAsyncTest2()
    {
        // Act
        Task<IEnumerable<ToDoItem>?> t = ToDoListFileService.LoadFromFileAsync();
        t.Wait();

        // Assert
        Assert.IsNull(t.Result);
    }


}