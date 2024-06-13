using Webapiwithado.Models;

namespace Webapiwithado.Interface
{
    public interface IContent
    {
        
        Task<ResponseModel> GetAllAvailableContentAsync();

        Task<ResponseModel> GetAllLessonsAsync();
        Task<ResponseModel> GetParticularLessonByIDAsync(int lessonID);
        Task<ResponseModel> GetLessonWithAllSubLessons(int lessonID);
        
        Task<ResponseModel> GetAllSubLessonsAsync();
        Task<ResponseModel> GetParticularSubLessonByIDAsync(int subLessonID);

        Task<ResponseModel> GetContentofAContentTypeByContentIDAsync(int contentTypeId);
    }
}
