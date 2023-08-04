namespace GrpcServicePiter
{
    static public class Adapter
    {
        static public Worker ConvertToWorker(int Id, string firstName, string lastName, string middleName, string birthDay, bool sex, bool haveChildren)
        {
            return new Worker
            {
                Id = Id,
                FirstName = firstName,
                LastName = lastName,
                MiddleName = middleName,
                BirthDay = birthDay,
                Sex = sex,
                HaveChildren = haveChildren
            };
        }

        static public WorkerReply ConvertToReply(Worker worker)
        {
            return  new WorkerReply
            {
                Id = worker.Id,
                FirstName = worker.FirstName,
                LastName = worker.LastName,
                MiddleName = worker.MiddleName,
                BirthDay = worker.BirthDay,
                Sex = worker.Sex,
                HaveChildren = worker.HaveChildren
            };
        }

        static public Worker ConvertToWorker(WorkerReply workerReply)
        {
            return new Worker
            {
                Id = workerReply.Id,
                FirstName = workerReply.FirstName,
                LastName = workerReply.LastName,
                MiddleName = workerReply.MiddleName,
                BirthDay = workerReply.BirthDay,
                Sex = workerReply.Sex,
                HaveChildren = workerReply.HaveChildren
            };
        }

        
    }
}
