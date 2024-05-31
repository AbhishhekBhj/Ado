namespace Webapiwithado.Models
{
    public class Question
    {
        public int QuestionID { get; set; }
        public string QuestionText { get; set; }

        public int QuizID { get; set; }

        public string QuestionType { get; set; }

        public List<Option> Options { get; set; }


        public DateTime QuestionDate { get; set; }

    }
}
