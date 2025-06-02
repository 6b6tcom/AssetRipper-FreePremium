using System.Collections.Generic;
using AssetRipper.Import.Logging;

namespace AssetRipper.Import.Tests;

internal class MemoryLogger : ILogger
{
    public readonly List<(LogType Type, LogCategory Category, string Message)> Messages = new();
    public void Log(LogType type, LogCategory category, string message)
    {
        Messages.Add((type, category, message));
    }

    public void BlankLine(int numLines) { }
}

public class LoggerTests
{
    private MemoryLogger _memoryLogger = null!;

    [SetUp]
    public void SetUp()
    {
        Logger.Clear();
        _memoryLogger = new MemoryLogger();
        Logger.Add(_memoryLogger);
    }

    [TearDown]
    public void TearDown()
    {
        Logger.Clear();
    }

    [Test]
    public void InfoMessageIsLogged()
    {
        const string message = "test info";
        Logger.Info(message);
        Assert.That(_memoryLogger.Messages, Has.Count.EqualTo(1));
        var entry = _memoryLogger.Messages[0];
        Assert.Multiple(() =>
        {
            Assert.That(entry.Type, Is.EqualTo(LogType.Info));
            Assert.That(entry.Category, Is.EqualTo(LogCategory.None));
            Assert.That(entry.Message, Is.EqualTo(message));
        });
    }

    [Test]
    public void WarningMessageWithCategoryIsLogged()
    {
        const string message = "test warning";
        Logger.Warning(LogCategory.Import, message);
        Assert.That(_memoryLogger.Messages, Has.Count.EqualTo(1));
        var entry = _memoryLogger.Messages[0];
        Assert.Multiple(() =>
        {
            Assert.That(entry.Type, Is.EqualTo(LogType.Warning));
            Assert.That(entry.Category, Is.EqualTo(LogCategory.Import));
            Assert.That(entry.Message, Is.EqualTo(message));
        });
    }
}
