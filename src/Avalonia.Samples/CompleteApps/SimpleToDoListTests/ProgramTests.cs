using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleToDoList.Tests;

[TestClass()]
public class ProgramTests
{
    [TestMethod()]
    public void BuildAvaloniaAppTest()
    {
        var app = Program.BuildAvaloniaApp();

        Assert.IsNotNull(app);
    }
}
