using Webapiwithado.DTOs;
using Webapiwithado.Models;

namespace Webapiwithado.Interface
{
    public interface IAdmin
    {

        Task<ResponseModel> GetAllQuizesAsync();

        Task<ResponseModel> GetQuizByIdAsync(int id);


        Task<ResponseModel> AddQuizAsync(QuizUpdateDto quizUpdateDto);

        Task<ResponseModel> UpdateQuizAsync(int id);


        Task<ResponseModel> DeleteQuizAsync(int id);


        Task<ResponseModel> GetAllQuestionsForAQuizAsync();

        Task<ResponseModel> GetQuestionByQuizIdAsync(int id);

        Task<ResponseModel> AddQuestionAsync(AddQuestionDTO addQuestionDTO);

        Task<ResponseModel> UpdateQuestionAsync(QuizUpdateDto quizUpdateDto);

        Task<ResponseModel> DeleteQuestionAsync(int id);


        

        Task<ResponseModel> GetOptionByQuestionIdAsync(int id);

        Task<ResponseModel> AddOptionAsync();

        Task<ResponseModel> UpdateOptionAsync(int id);

        Task<ResponseModel> DeleteOptionAsync(int id);


        Task<ResponseModel> GetAllUsersAsync();

        Task<ResponseModel> GetUserByIdAsync(int id);

        Task<ResponseModel> AddUserAsync();

        Task<ResponseModel> UpdateUserAsync(int id);


        Task<ResponseModel> DeleteUserAsync(int id);


        Task<ResponseModel> GetAllUserAnswersAsync();

        Task<ResponseModel> GetUserAnswerByIdAsync(int id);


        Task<ResponseModel> GetAllContentAsync();

        Task<ResponseModel> AddContentAsync();

        Task<ResponseModel> UpdateContentAsync(int id);

        Task<ResponseModel> DeleteContentAsync(int id);

        Task<ResponseModel> Get3BestPerformersForQuizbyIdAsync(int id);

        Task<ResponseModel> Get3WorstPerformersForQuizbyIdAsync(int id);

        Task<ResponseModel> GetAllContentTypeAync();




        
    }
}
