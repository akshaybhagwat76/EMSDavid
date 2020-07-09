using System.ComponentModel.DataAnnotations;

namespace Ems.Authorization.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}
