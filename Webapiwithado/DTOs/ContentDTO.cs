namespace Webapiwithado.DTOs
{
    public class ContentDTO
    {
        public int ContentTypeId { get; set; }
        public string ContentTypeName { get; set; }

        public List<LessonDTO> Lessons { get; set; }


    }

    public class LessonDTO
    {
        public int LessonId { get; set; }
        public string LessonName { get; set; }
        public List<SubLessonDTO> SubLessons { get; set; }

    }

    public class  SubLessonDTO
    {
        public int SubLessonId { get; set; }
        public string SubLessonName { get; set; }

        public string SublessonContent { get; set; }
        
    }
}
