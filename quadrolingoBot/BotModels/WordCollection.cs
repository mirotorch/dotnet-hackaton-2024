namespace quadrolingoBot.BotModels;

internal class WordCollection
{
    public int MessageId { get; set; }

    public List<WordModel> list;
    public int Current = 0;

    public WordModel this[int index] { get => list[index]; set => list[index] = value; }

    public int Count => list.Count;

    public int GetCorrectCount() => list.Count(w => w.Correct);

    public WordCollection(List<WordModel> words)
    {
        list = words;
    }
}

internal class WordExerciseCollection : WordCollection
{
    public WordExerciseCollection(List<WordModel> words) : base(words)
    {
        words.Shuffle();
    }
}