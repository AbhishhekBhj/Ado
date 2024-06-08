namespace Webapiwithado.DTOs
{
    public class QuizDTO
    {
        public string QuizTitle { get; set; }
        public string QuizDescription { get; set; }
        public int QuizID { get; set; }

        public String? Photo { get; set; }

        public String? ContentType { get; set; }
        public List<QuestionDTO> Questions { get; set; } = new List<QuestionDTO>();
    }

    public class QuestionDTO
    {
        public string QuestionText { get; set; }
        public int QuestionID { get; set; }
        public string QuestionType { get; set; }
        public List<OptionDTO> Options { get; set; } = new List<OptionDTO>();
    }

    public class OptionDTO
    {
        public string OptionText { get; set; }
        public int OptionID { get; set; }
        public bool IsCorrect { get; set; }
    }



    public class QuizUpdateDto
    {
        public string QuizTitle { get; set; }
        public string QuizDescription { get; set; }
        public int QuizID { get; set; }
        public String? Photo { get; set; }
        public String? ContentType { get; set; }

    }
}
