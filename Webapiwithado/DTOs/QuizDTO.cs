using System.Collections;
using System.Data.SqlClient;

namespace Webapiwithado.DTOs
{
    public class QuizDTO
    {
        public string QuizTitle { get; set; }
        public string QuizDescription { get; set; }
        public int QuizID { get; set; }
        public List<QuestionDTO> Questions { get; set; }
    }

    public class QuestionDTO
    {
        public string QuestionText { get; set; }
        public int QuestionID { get; set; }
        public List<OptionDTO> Answers { get; set; }
    }

    public class OptionDTO
    {
        public string OptionText { get; set; }
        public int OptionID { get; set; }
        public bool IsCorrect { get; set; }




        
    }   



    
}


