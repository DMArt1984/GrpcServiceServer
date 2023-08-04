namespace GrpcServicePiter
{
    public interface ISource
    {
        public int Count(); // количество записей
        public List<Worker> List(); // список записей
        public Task<Worker> FindById(int Id); // поиск записи по ID
        public Task<int> Create(Worker newWorker); // добавить (создать) запись
        public Task<int> Update(Worker newWorker); // обновить запись
        public Task<int> DeleteById(int Id); // удалить запись
    }
}
