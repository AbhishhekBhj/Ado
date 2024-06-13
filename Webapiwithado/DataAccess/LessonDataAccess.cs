using System.Data;
using System.Data.SqlClient;
using Webapiwithado.DTOs;
using Webapiwithado.Interface;
using Webapiwithado.Models;

namespace Webapiwithado.DataAccess
{
    public class LessonDataAccess : IContent
    {

        private readonly string _connectionString;

        public LessonDataAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public async Task<ResponseModel> GetContentofAContentTypeByContentIDAsync(int contentTypeId)
        {
            List<LessonDTO> lessonDTOs = new List<LessonDTO>();
            ContentDTO contentDTO = new ContentDTO();
            Dictionary<int, LessonDTO> lessonDictionary = new Dictionary<int, LessonDTO>();

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    using (SqlCommand sqlCommand = new SqlCommand("getallcontentofacontenttype", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@contentTypeId", contentTypeId);

                        using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                        {
                            while (await sqlDataReader.ReadAsync())
                            {
                                contentDTO.ContentTypeId = Convert.ToInt32(sqlDataReader["ContentTypeID"]);
                                contentDTO.ContentTypeName = sqlDataReader["typename"]?.ToString(); // Use ?. for null-conditional operator

                                int lessonID = Convert.ToInt32(sqlDataReader["lessonid"]);

                                // Check if lesson exists in dictionary before creating a new one
                                if (!lessonDictionary.ContainsKey(lessonID))
                                {
                                    lessonDictionary[lessonID] = new LessonDTO
                                    {
                                        LessonId = lessonID,
                                        LessonName = sqlDataReader["lessonname"]?.ToString(),
                                        SubLessons = new List<SubLessonDTO>()
                                    };
                                }

                                var subLessonName = sqlDataReader["sub_lesson_name"]?.ToString();
                                var subLessonContent = sqlDataReader["lesson_content"]?.ToString();

                                // Check if sub-lesson ID is not null and can be converted to int
                                int? subLessonId = sqlDataReader["sub_lessonid"] as int?;
                                if (!subLessonId.HasValue && int.TryParse(sqlDataReader["sub_lessonid"]?.ToString(), out int id))
                                {
                                    subLessonId = id;
                                }

                                // Add sub-lesson only if it has a valid ID and content
                                if (subLessonId.HasValue && (!string.IsNullOrEmpty(subLessonName) || !string.IsNullOrEmpty(subLessonContent)))
                                {
                                    lessonDictionary[lessonID].SubLessons.Add(new SubLessonDTO
                                    {
                                        SubLessonId = subLessonId.Value,
                                        SubLessonName = subLessonName ?? string.Empty,  // Provide a default empty string if null
                                        SublessonContent = subLessonContent ?? string.Empty // Provide a default empty string if null
                                    });
                                }

                            }

                            // Assign lessons to contentDTO after processing all rows
                            contentDTO.Lessons = lessonDictionary.Values.ToList();
                            Console.WriteLine(lessonDictionary.Keys.ToList());
                        }
                    }
                }

                return new ResponseModel { Status = 200, Data = contentDTO, Message = "Success" };
            }
            catch (SqlException ex)
            {
                return new ResponseModel { Status = 500, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = 500, Message = ex.Message };
            }
        }


        public Task<ResponseModel> GetAllAvailableContentAsync()
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();

                    using (SqlCommand sqlCommand = new SqlCommand("getallcontenttype", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            List<ContentDTO> contentDTOs = new List<ContentDTO>();

                            while (sqlDataReader.Read())
                            {
                                contentDTOs.Add(new ContentDTO
                                {
                                    ContentTypeId = Convert.ToInt32(sqlDataReader["ContentTypeID"]),
                                    ContentTypeName = sqlDataReader["TypeName"]?.ToString()
                                });
                            }

                            return Task.FromResult(new ResponseModel { Status = 200, Data = contentDTOs, Message = "Success" });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return Task.FromResult(new ResponseModel { Status = 500, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ResponseModel { Status = 500, Message = ex.Message });
            }
        }

        public Task<ResponseModel> GetAllLessonsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> GetAllSubLessonsAsync()
        {
            throw new NotImplementedException();
        }

       

        public Task<ResponseModel> GetLessonWithAllSubLessons(int lessonID)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> GetParticularLessonByIDAsync(int lessonID)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> GetParticularSubLessonByIDAsync(int subLessonID)
        {
            throw new NotImplementedException();
        }
    }
}
