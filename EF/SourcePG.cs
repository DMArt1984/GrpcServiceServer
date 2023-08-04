using Grpc.Core;

namespace GrpcServicePiter
{
    public class SourcePG: ISource
    {
        // обработка ошибок ложится на использующих данный класс!

        ApplicationContext db;
        public SourcePG(ApplicationContext db)
        {
            this.db = db;
        }


        // количество записей
        public int Count() => db.Workers.Count(); 


        // список записей
        public List<Worker> List() => db.Workers.ToList();
        

        // поиск записи по ID
        public async Task<Worker> FindById(int Id) => await db.Workers.FindAsync(Id);
        

        // добавить (создать) запись
        public async Task<int> Create(Worker newWorker)
        {
            // добавление работника
            await db.Workers.AddAsync(newWorker);
            await db.SaveChangesAsync();

            // получаем ID
            if (db.Workers.Any())
                return db.Workers.OrderBy(x => x.Id).Last().Id;

            // не успешно
            return 0;
        }

        // обновить запись
        public async Task<int> Update(Worker newWorker)
        {
            var worker = await db.Workers.FindAsync(newWorker.Id); // ищем работника
            if (worker == null) // странно, а кто это был?
                return 0; // не успешно

            worker.FirstName = newWorker.FirstName;
            worker.LastName = newWorker.LastName;
            worker.MiddleName = newWorker.MiddleName;
            worker.BirthDay = newWorker.BirthDay;
            worker.Sex = newWorker.Sex;
            worker.HaveChildren = newWorker.HaveChildren;

            // обновление работника
            await db.SaveChangesAsync();

            // успешно
            return newWorker.Id;
        }

        // удалить запись
        public async Task<int> DeleteById(int Id)
        {
            var worker = await db.Workers.FindAsync(Id); // ищем работника
            if (worker == null) // не нашли, похоже уже удален
                return 0; // не успешно

            // удаление записи
            db.Workers.Remove(worker);
            await db.SaveChangesAsync();

            // успешно
            return Id;
        }

    }
}
