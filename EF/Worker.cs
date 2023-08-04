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

        public Worker()
        {
            
        }

        
    }
}
