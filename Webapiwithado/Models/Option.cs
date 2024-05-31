namespace Webapiwithado.Models
{
    public class Option
    {
        public int OptionID { get; set; }
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; }
        public int QuestionID { get; set; }
    }
}
