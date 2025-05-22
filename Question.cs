public class Question
{
    public string Text { get; set; }
    public string[] Options { get; set; }
    public string CorrectAnswer { get; set; }
    public bool IsOpenEnded { get; set; }
    public string Explanation { get; set; } // Новое поле для объяснения
}
