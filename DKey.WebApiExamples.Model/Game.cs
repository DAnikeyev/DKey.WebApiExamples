namespace DKey.WebApiExamples.Model;

public class Game
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }

    public Game(string name, int score, string? comment)
    {
        Id = Guid.NewGuid();
        Name = name;
        Score = score;
        Comment = comment;
    }
}