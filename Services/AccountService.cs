using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServicePiter;

namespace GrpcServicePiter.Services
{
    public class AccountService : Accounter.AccounterBase
    {
        Company company;

        private readonly ILogger<AccountService> _logger;

        Status statusNotFound = new Status(StatusCode.NotFound, "Рабочий не найден");

        public AccountService(ILogger<AccountService> logger, ApplicationContext db)
        {
            _logger = logger;
            company = new Company(db);
        }

        // 1.1) Тест связи
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            try
            {
                int count = company.Count(); // количество работников
                return Task.FromResult(new HelloReply
                {
                    Message = "Привет, " + request.Name + " (" + context.Host + $") ! Нас уже {count}",
                    Status = StatusConnect.Ok
                });

            } catch(Exception ex)
            {
                return Task.FromResult(new HelloReply
                {
                    Message = request.Name + "! У нас проблемы: " + ex.Message,
                    Status = StatusConnect.Fault
                });
            }
            
            
        }

        // 1.2) Запрос-Ответ
        public override async Task SayHelloStream(IAsyncStreamReader<HelloRequest> requestStream, 
            IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            await foreach (var request in requestStream.ReadAllAsync())
            {
                await responseStream.WriteAsync(new HelloReply
                {
                    Message = "Привет, " + request.Name + " [" + DateTime.Now + "]",
                    Status = StatusConnect.Unknown
                });
            }
        }

        // 2) Работа с БД
        // 2.1 отправляем список работников
        public override Task<ListReply> ListWorkers(Empty request, ServerCallContext context)
        {
            var listReply = new ListReply();    // определяем список
            try
            {
                // преобразуем каждый объект Worker в объект WorkerReply т.к. эти объекты могут не совпадать
                var workerList = company.List().Select(item => Adapter.ConvertToReply(item)).ToList();
                listReply.Workers.AddRange(workerList);

            }
            catch (Exception ex)
            {
                // нет обработки ошибок, возвращаем пустой список
                _logger.Log(LogLevel.Error, $"{ex.HResult} {ex.Message}");
            }
            return Task.FromResult(listReply);
        }
        // 2.2 поиск одного работника по ID
        public override async Task<WorkerReply> GetWorker(GetWorkerRequest request, ServerCallContext context)
        {
            try
            {
                var worker = await company.FindById(request.Id);
                // если работник не найден, генерируем исключение
                if (worker == null)
                    throw new RpcException(statusNotFound);

                // преобразуем объект Worker в объект WorkerReply
                WorkerReply WorkerReply = Adapter.ConvertToReply(worker);

                return await Task.FromResult(WorkerReply);

            }
            catch (Exception ex)
            {
                // нет обработки ошибок, возвращаем пустой объект
                _logger.Log(LogLevel.Error, $"{ex.HResult} {ex.Message}");
                return await Task.FromResult(new WorkerReply());
            }

        }
        // 2.3 добавление работника (нас стало еще больше)
        public override async Task<Answer> CreateWorker(CreateWorkerRequest request, ServerCallContext context)
        {
            var answer = new Answer() // ответ по умолчянию
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
        // 2.4 обновление работника
        public override async Task<Answer> UpdateWorker(UpdateWorkerRequest request, ServerCallContext context)
        {
            var answer = new Answer() // ответ по умолчянию
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
        // 2.5 удаление работника (ему тесно с нами)
        public override async Task<Answer> DeleteWorker(DeleteWorkerRequest request, ServerCallContext context)
        {
            var answer = new Answer() // ответ по умолчянию
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