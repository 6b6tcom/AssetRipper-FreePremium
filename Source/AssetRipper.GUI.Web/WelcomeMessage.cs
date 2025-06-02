namespace AssetRipper.GUI.Web;

public static class WelcomeMessage
{
    private const string AsciiArt = """
__  __            _             ____  _
| \/  | ___  _ __| | _____ _ __|  _ \(_)_ __ ___
| |\/| |/ _ \| '__| |/ / _ \ '__| |_) | | '__/ _ \
| |  | | (_) | |  |   <  __/ |  |  _ <| | | |  __/
|_|  |_|\___/|_|  |_|\_\___|_|  |_| \_\_|_|  \___|
""";

	public static void Print()
	{
		Console.WriteLine(AsciiArt);
		Console.WriteLine();
	}
}
