using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServicePiter;

namespace GrpcServicePiter.Services
{
    public class AccountService : Accounter.AccounterBase
    {
        Company company;

        private readonly ILogger<AccountService> _logger;

        Status statusNotFound = new Status(StatusCode.NotFound, "������� �� ������");

        public AccountService(ILogger<AccountService> logger, ApplicationContext db)
        {
            _logger = logger;
            company = new Company(db);
        }

        // 1.1) ���� �����
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            try
            {
                int count = company.Count(); // ���������� ����������
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
                // ����������� ������ ������ Worker � ������ WorkerReply �.�. ��� ������� ����� �� ���������
                var workerList = company.List().Select(item => Adapter.ConvertToReply(item)).ToList();
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
                var worker = await company.FindById(request.Id);
                // ���� �������� �� ������, ���������� ����������
                if (worker == null)
                    throw new RpcException(statusNotFound);

                // ����������� ������ Worker � ������ WorkerReply
                WorkerReply WorkerReply = Adapter.ConvertToReply(worker);

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
            var answer = new Answer() // ����� �� ���������
            {
                Id = 0,
                Code = 0,
                Text = ""
            };

            try
            {
                var worker = Adapter.ConvertToWorker(request.Worker);
                answer.Id = await company.Create(worker);

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
                int result = await company.Update(Adapter.ConvertToWorker(request.Worker));
                if (result == 0)
                    throw new RpcException(statusNotFound);

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
                int result = await company.DeleteById(request.Id);
                if (result == 0)
                    throw new RpcException(statusNotFound);

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