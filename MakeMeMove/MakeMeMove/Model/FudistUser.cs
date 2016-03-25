using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace MakeMeMove.Model
{
    [Table("FudistUsers")]
    public class FudistUser
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string UserName { get; set; }
        public bool UserIsPremium { get; set; }
        public DateTime LastChecked { get; set; }
    }
}
