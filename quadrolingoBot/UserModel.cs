using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quadrolingoBot
{
	internal class UserModel
	{
		public long ChatId { get; set; }
		public long UserId { get; set; }
		public string BaseLang { get; set; }
		public string StudyLang { get; set; }
		public string UserName { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
	}
}
