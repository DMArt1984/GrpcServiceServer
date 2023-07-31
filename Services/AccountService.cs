using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServicePiter;

namespace GrpcServicePiter.Services
{
    public class AccountService : Accounter.AccounterBase
    {
        ApplicationContext db;

        private readonly ILogger<AccountService> _logger;

        Status statusNotFound = new Status(StatusCode.NotFound, "������� �� ������");

        public AccountService(ILogger<AccountService> logger, ApplicationContext db)
        {
            _logger = logger;
            this.db = db;
        }

        // 1.1) ���� �����
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            try
            {
                int count = db.Workers.Count(); // ���������� ����������
                return Task.FromResult(new HelloReply
                {
                    Message = "������, " + request.Name + " (" + context.Host + $") ! ��� ��� {count}",
                    Status = StatusConnect.Ok
                });

            } catch(Exception ex)
            {
                return Task.FromResult(new HelloReply
                {
                    Message = request.Name + "! � ��� ��������: " + ex.Message,
                    Status = StatusConnect.Fault
                });
            }
            
            
        }

        // 1.2) ������-�����
        public override async Task SayHelloStream(IAsyncStreamReader<HelloRequest> requestStream, 
            IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            await foreach (var request in requestStream.ReadAllAsync())
            {
                await responseStream.WriteAsync(new HelloReply
                {
                    Message = "������, " + request.Name + " [" + DateTime.Now + "]",
                    Status = StatusConnect.Unknown
                });
            }
        }

        // 2) ������ � ��
        // 2.1 ���������� ������ ����������
        public override Task<ListReply> ListWorkers(Empty request, ServerCallContext context)
        {
            var listReply = new ListReply();    // ���������� ������
            try
            {
                // ����������� ������ ������ Worker � ������ WorkerReply
                var workerList = db.Workers.Select(item => new WorkerReply
                {
                    Id = item.Id,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    MiddleName = item.MiddleName,
                    BirthDay = item.BirthDay,
                    Sex = item.Sex,
                    HaveChildren = item.HaveChildren
                }).ToList();
                listReply.Workers.AddRange(workerList);

            }
            catch (Exception ex)
            {
                // ��� ��������� ������, ���������� ������ ������
                _logger.Log(LogLevel.Error, $"{ex.HResult} {ex.Message}");
            }
            return Task.FromResult(listReply);
        }
        // 2.2 ����� ������ ��������� �� ID
        public override async Task<WorkerReply> GetWorker(GetWorkerRequest request, ServerCallContext context)
        {
            try
            {
                var worker = await db.Workers.FindAsync(request.Id);
                // ���� �������� �� ������, ���������� ����������
                if (worker == null)
                    throw new RpcException(statusNotFound);

                // ����������� ������ Worker � ������ WorkerReply
                WorkerReply WorkerReply = new WorkerReply()
                {
                    Id = worker.Id,
                    FirstName = worker.FirstName,
                    LastName = worker.LastName,
                    MiddleName = worker.MiddleName,
                    BirthDay = worker.BirthDay,
                    Sex = worker.Sex,
                    HaveChildren = worker.HaveChildren
                };
                return await Task.FromResult(WorkerReply);

            }
            catch (Exception ex)
            {
                // ��� ��������� ������, ���������� ������ ������
                _logger.Log(LogLevel.Error, $"{ex.HResult} {ex.Message}");
                return await Task.FromResult(new WorkerReply());
            }

        }
        // 2.3 ���������� ��������� (��� ����� ��� ������)
        public override async Task<Answer> CreateWorker(CreateWorkerRequest request, ServerCallContext context)
        {
            // ��������� �� ������ ������ Worker � ��������� ��� � ������ Workers
            var worker = new Worker
            {
                FirstName = request.Worker.FirstName,
                LastName = request.Worker.LastName,
                MiddleName = request.Worker.MiddleName,
                BirthDay = request.Worker.BirthDay,
                Sex = request.Worker.Sex,
                HaveChildren = request.Worker.HaveChildren
            };

            var answer = new Answer() // ����� �� ���������
            {
                Id = 0,
                Code = 0,
                Text = ""
            };

            try
            {
                // ���������� ������
                await db.Workers.AddAsync(worker);
                await db.SaveChangesAsync();

                // �������� ID
                if (db.Workers.Any())
                    answer.Id = db.Workers.OrderBy(x => x.Id).Last().Id;

            }
            catch (Exception ex)
            {
                answer.Code = ex.HResult;
                answer.Text = ex.Message;
                _logger.Log(LogLevel.Error, $"{ex.HResult} {ex.Message}");
            }
            return await Task.FromResult(answer);
        }
        // 2.4 ���������� ���������
        public override async Task<Answer> UpdateWorker(UpdateWorkerRequest request, ServerCallContext context)
        {
            var answer = new Answer() // ����� �� ���������
            {
                Id = request.Worker.Id,
                Code = 0,
                Text = ""
            };

            try
            {
                var worker = await db.Workers.FindAsync(request.Worker.Id); // ���� ���������
                if (worker == null) // �������, � ��� ��� ���?
                    throw new RpcException(statusNotFound);

                worker.FirstName = request.Worker.FirstName;
                worker.LastName = request.Worker.LastName;
                worker.MiddleName = request.Worker.MiddleName;
                worker.BirthDay = request.Worker.BirthDay;
                worker.Sex = request.Worker.Sex;
                worker.HaveChildren = request.Worker.HaveChildren;

                // ���������� ������
                await db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                answer.Code = ex.HResult;
                answer.Text = ex.Message;
                _logger.Log(LogLevel.Error, $"{ex.HResult} {ex.Message}");
            }
            return await Task.FromResult(answer);
        }
        // 2.5 �������� ��������� (��� ����� � ����)
        public override async Task<Answer> DeleteWorker(DeleteWorkerRequest request, ServerCallContext context)
        {
            var answer = new Answer() // ����� �� ���������
            {
                Id = request.Id,
                Code = 0,
                Text = ""
            };

            try
            {
                var worker = await db.Workers.FindAsync(request.Id); // ���� ���������
                if (worker == null) // �� �����, ������ ��� ������
                    throw new RpcException(statusNotFound);

                // �������� ������
                db.Workers.Remove(worker); 
                await db.SaveChangesAsync();

            } catch(Exception ex)
            {
                answer.Code = ex.HResult;
                answer.Text = ex.Message;
                _logger.Log(LogLevel.Error, $"{ex.HResult} {ex.Message}");
            }
            return await Task.FromResult(answer);
        }




    }
}