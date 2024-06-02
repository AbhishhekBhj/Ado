namespace Webapiwithado.Models
{
    public class SubmitQuizScore
    {

        //this model will be used to post the quiz score
        public int QuizID { get; set; }
        public int UserID { get; set; }
        public int Score { get; set; }


    }

    public class GetQuizScore
    {

        //this model will be used to get the quiz score
        public int UserQuizID { get; set; }
        public int QuizID { get; set; }
        public int UserID { get; set; }

        public int Score { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
