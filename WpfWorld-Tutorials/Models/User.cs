using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfApp.Models
{
    public class User
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string Gender = "Male";
        public string[] Countries { get; } = { "India", "USA", "UK" };
        public string SelectedCountry { get; set; }

        public bool SkillCSharp { get; set; }

        public bool SkillWpf { get; set; }
        public bool SkillDotNet { get; set; }

        public int Experience { get; set; }
        public bool IsTermsAccepted { get; set; }

    }
}

