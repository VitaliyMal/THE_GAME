using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THE_GAME
{
    public class Result
    {
        public List<int> Winner { get; set; } = new();
        public List<int> TurnCount { get; set; } = new();
        public List<int> Scores { get; set; } = new();
        public List<Strategy> strategies { get; set; } = new();
    }

    public class Analyzer
    {
        private const int MAX_GAMES = 10;
        private List<Strategy> strategies = new List<Strategy>
        {
            new Strategy(){Name = "Attack"},
            new Strategy(){Name = "Defence"},
            new Strategy(){Name = "Random"},
            new Strategy(){Name = "Monte-Carlo"},
        };

        public async Task Analyze()
        {
            //Перебор стратегий
            for (int i = 0; i < strategies.Count - 1; i++)
            {
                for (int j = i + 1; j < strategies.Count; j++)
                {
                    Task[] tasks = new Task[MAX_GAMES];
                    for (int k = 0; k < MAX_GAMES; k++)
                    {
                        var task = Task.Run(() =>
                        {
                            Task.Delay(1000).Wait();
                        });
                        tasks[k] = task;
                    }
                    await Task.WhenAll(tasks);
                    //Извлечь результаты в Results
                }
            }
        }
        public List<Result> Results { get; set; } = new();
        public string GetResults()
        {
            return string.Join("\n\n\n", Results.Select(result =>
            {
                return "Победители:\n"
                + string.Join("|", result.Winner.Select(winner => Convert.ToString(winner))) +
                "\nКоличество ходов:\n" +
                string.Join("|", result.TurnCount.Select(y => Convert.ToString(y))) +
                "\nСчет игроков:\n" +
                string.Join("|", result.Scores.Select(y =>
                Convert.ToString(y)));
            }));
        }
    }
}
