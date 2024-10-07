using THE_GAME;





void PrintNumbers()
{
    for (int i = 0; i < 10; i++)
    {
        Console.WriteLine(i);
        Task.Delay(1000).Wait();
    }
}
async Task PrintWords()
{
    for (int i = 0; i < 10; i++)
    {
        Console.WriteLine("Слово номер " + i);
        Task.Delay(1000).Wait();
        await Task.Run(PrintNumbers);
    }
    await Task.Run(PrintNumbers);
}

Analyzer analyzer = new Analyzer();
await analyzer.Analyze();
Console.WriteLine(analyzer.GetResults());