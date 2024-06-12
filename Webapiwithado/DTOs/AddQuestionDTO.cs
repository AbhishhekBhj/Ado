namespace Webapiwithado.DTOs
{
    public class AddQuestionDTO
    {
        public int? QuizID { get; set; }

        public string QuestionText { get; set; }

        public string QuestionType { get; set; }

        public List<AddOptionDTO> Options { get; set; }


    }



    public class AddOptionDTO
    {

        public string OptionText { get; set; }

        public int IsCorrect { get; set; }

    }




    public class UpdateQuestionDTO
    {
        public int? QuestionID { get; set; }

        public string? QuestionText { get; set; }



        public string QuestionType { get; set; }
    }
}
