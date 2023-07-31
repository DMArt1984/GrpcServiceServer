namespace GrpcServicePiter
{
    public class Worker
    {
        public int Id { get; set; } // ID
        public string FirstName { get; set; } // имя
        public string? LastName { get; set; } // фамилия
        public string? MiddleName { get; set; } // отчество
        public string? BirthDay { get; set; } // дата рождения
        public bool Sex { get; set; } // пол (мужской или женский, третьего не дано)
        public bool HaveChildren { get; set; } // наличие детей

        public Worker() // если хотим заполнить данные позже
        {
            
        }

        public Worker(int Id, string FirstName, string BirthDay = "",
            string LastName = "", string MiddleName = "",
            bool Sex = false, bool HaveChildren = false)
        {
            this.Id = Id;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.MiddleName = MiddleName;
            this.BirthDay = BirthDay;
            this.Sex = Sex;
            this.HaveChildren = HaveChildren;
        }

        
    }
}
