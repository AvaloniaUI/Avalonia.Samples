using System;
using System.IO;
using System.Threading.Tasks;
using AdvancedToDoList.Helper;
using Dapper;

namespace AdvancedToDoList.Services;

public class DesignDbService : IDbService
{
    public string GetDatabasePath()
    {
        return ":memory:";
    }
    
    public Task SaveAsync()
    {
        return Task.CompletedTask;
    }
}