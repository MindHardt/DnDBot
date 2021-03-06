using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public static class FileWorker
{
	public static string GetLine(string path, int number)
	{
		string line = File.ReadLines(path).Skip(number-1).First();
		return (line[0] == '$' ? line.Substring(10, line.Contains('/') ? line.IndexOf('/') : line.Length) : line.Substring(0, line.Contains('/') ? line.IndexOf('/') : line.Length));
	}
	public static string CreateUnexsistent(string path)
    {
		if (!File.Exists(path)) File.Create(path);
		return path;
    }
	public static string CreateUnexsistentMacrosFile(string path, out string log)
	{
		log = "Проверяю наличие именного файла...\n";
		if (!File.Exists(path))
		{
			log += $":warning: Файл {path} не обнаружен. Создаю файл...";
			File.Create(path);
		}
		else log += $":white_check_mark: Файл {path} найден. Переходим дальше.";
		return path;
	}
	public static async Task<string> FindLine(string file, string name)
    {
		using (StreamReader sr = new StreamReader(Environment.CurrentDirectory + file + ".txt"))
        {
			string line = String.Empty;
			while ((line = await sr.ReadLineAsync()) != null)
            {
				if (line.StartsWith("$" + name)) return (line[0] == '$' ? line.Substring(10, line.Contains('/') ? line.IndexOf('/') : line.Length) : line.Substring(0, line.Contains('/') ? line.IndexOf('/') : line.Length));
			}
			return String.Empty;
        }
    }
}
