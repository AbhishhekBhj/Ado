namespace Webapiwithado.Models
{
    public class Dashboard
    {
        public TablesCount TablesCount { get; set; }
        public List<PopularQuiz>? PopularQuizzes { get; set; }
        public int NewUserCount { get; set; }
        public List<UserScore>? UserScores { get; set; }

    }


    public class TablesCount
    {
        public int QuizCount { get; set; }
        public int QuestionCount { get; set; }

        public int UserCount { get; set; }

        public int ContentCount { get; set; }
    }

    public class PopularQuiz
    {
        public int QuizId { get; set; }

        public string QuizName { get; set; }

        public int TotalAttempt { get; set; }
    }

    public class UserScore
    {
        public string Username { get; set; }
        public int UserId { get; set; }
        public int TotalScore { get; set; }

        public int QuizAttempt { get; set; }
    }


    
}
