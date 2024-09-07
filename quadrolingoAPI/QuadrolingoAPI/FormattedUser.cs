using quadrolingoAPI.Models;

namespace quadrolingoAPI
{
    public class FormattedUser
    {
        public FormattedUser(User u) {
            Id = u.Id;
            CHAT_ID = u.CHAT_ID;
            TELEGRAM_ID = u.TELEGRAM_ID;
            USERNAME = u.USERNAME;
            FIRST_NAME = u.FIRST_NAME;
            LAST_NAME  = u.LAST_NAME;
            BASE_LANG = u.BASE_LANG;
            STUDY_LANG = u.STUDY_LANG;
    }
        public int Id { get; set; }
        public long CHAT_ID { get; set; }
        public long TELEGRAM_ID { get; set; }
        public string USERNAME { get; set; }
        public string FIRST_NAME { get; set; }
        public string? LAST_NAME { get; set; }
        public string BASE_LANG { get; set; }
        public string STUDY_LANG { get; set; }
    }
}
