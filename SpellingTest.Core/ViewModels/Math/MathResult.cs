

namespace SpellingTest.Core.ViewModels.Math
{
    public class MathResult
    {
        public string Color { get; set; }
        public string Text { get; set; }
        public static MathResult GetCorrectResult(string question, string yourAnswer)
        {
            return new MathResult()
            {
                Color = "Green",
                Text = $"Question: {question}\nAnswer:{yourAnswer}"
            };
        }
        public static MathResult GetIncorrectResult(string question, string yourAnswer, string correctAnswer)
        {
            return new MathResult()
            {
                Color = "Red",
                Text = $"Question: {question} \n Answer:{correctAnswer} Your Answer:{yourAnswer}"
            };
        }
    }
}
