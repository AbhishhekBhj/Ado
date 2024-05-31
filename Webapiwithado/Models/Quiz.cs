using System.Data;

namespace Webapiwithado.Models
{
    public class Quiz
    {
        public string QuizTitle { get; set; }

        public string QuizDescription { get; set; }

        public int QuizID { get; set; }

        public DateTime QuizDate { get; set; }






        //public static object ConvertToDynamicQuizData(DataTable dataTable)
        //{
        //    var quizData = new Dictionary<int, object>();

        //    foreach (DataRow row in dataTable.Rows)
        //    {
        //        int quizId = Convert.ToInt32(row["quizid"]);

        //        if (!quizData.ContainsKey(quizId))
        //        {
        //            quizData[quizId] = new
        //            {
        //                quizid = quizId,
        //                quiztitle = row["quiztitle"].ToString(),
        //                questions = new List<object>()
        //            };
        //        }

        //        var question = new
        //        {
        //            questionid = Convert.ToInt32(row["questionid"]),
        //            questionTitle = row["questionTitle"].ToString(),
        //            questionType = row["questionType"].ToString(),
        //            options = new List<string>(),
        //            correctAnswer = row["correctAnswer"].ToString()
        //        };

        //        if (!IsQuestionExists((List<object>)quizData[quizId].questions, question))
        //        {
        //            ((List<object>)quizData[quizId].questions).Add(question);
        //        }

        //        var options = ((List<string>)question.options);
        //        var optionText = row["optionText"].ToString();
        //        if (!options.Contains(optionText))
        //        {
        //            options.Add(optionText);
        //        }
        //    }

        //    return new
        //    {
        //        status = 200,
        //        message = "Quiz data fetched successfully",
        //        data = quizData.Values.ToList()
        //    };
        //}

        public static bool IsQuestionExists(List<object> questions, object question)
        {
            foreach (var item in questions)
            {
                if (item.GetType().GetProperty("questionid").GetValue(item).ToString() == question.GetType().GetProperty("questionid").GetValue(question).ToString())
                {
                    return true;
                }
            }
            return false;
        }

    }
}
