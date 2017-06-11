using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TccUniversal
{
    [Table("Users")]
    public class UserResponse
    {
        [PrimaryKey]
        public decimal id { get; set; }
        public string password { get; set; }
        public string login { get; set; }
        public string email { get; set; }
        public bool active { get; set; }

        /*byte[] image;
        string description;*/
    }
}
