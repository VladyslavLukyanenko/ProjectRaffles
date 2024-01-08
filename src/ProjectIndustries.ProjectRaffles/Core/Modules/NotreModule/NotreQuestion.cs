using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.NotreModule
{
    public class NotreQuestion
    {
        public NotreQuestion(string question, List<string> questionOptions, string questionToken, bool isValidAnswer, string answer)
        {
            Question = question;
            QuestionOptions = questionOptions;
            QuestionToken = questionToken;
            IsValidAnswer = isValidAnswer;
            Answer = answer;
        }
        public string Question { get; set; }
        public List<string> QuestionOptions { get; set; }
        public string QuestionToken { get; set; }
        public bool IsValidAnswer { get; set; }
        public string Answer { get; set; }
    }
}