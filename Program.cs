using Depth_Camera;

Logic logic = new Logic();
string chosenFolder = string.Empty;

//Zur Auswahl des Ordners mit den Bilddateien
var thread = new Thread(() =>
{
    FolderBrowser browser = new FolderBrowser();
    Console.WriteLine("Choose location containing images to calculate");
    chosenFolder = browser.ChooseFolder("Choose location containing images to calculate");
});

//Windows Forms wird benötigt für den Folder-Browser, darum muss dieser Teil explizit als Single-Threaded deklariert werden
thread.SetApartmentState(ApartmentState.STA);
thread.Start();
thread.Join();

if (!string.IsNullOrEmpty(chosenFolder))
{
    var startTime = DateTime.Now;
    List<KeyValuePair<string, int>> results = logic.CalculateVolumesForFolder(chosenFolder);
    results = results.OrderByDescending(x => x.Value).ToList();

    Console.WriteLine($"{results.Count} datasets found.");
    for (int i = 0; i < results.Count; i++)
    {
        Console.WriteLine($"{i}: File: {results[i].Key}, Relative-Volume: {(double)results[i].Value / (double)results[0].Value}");
    }
    Console.WriteLine($"Calculated in {(DateTime.Now.Ticks - startTime.Ticks) / 10000} ms");
}
else
{
    Console.WriteLine("Invalid selection!");
}

Console.WriteLine("Console will close on enter");
Console.ReadLine();
