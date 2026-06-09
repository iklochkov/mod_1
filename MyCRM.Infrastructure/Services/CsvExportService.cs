using MyCRM.Infrastructure.Config;
using MyCRM.Application.DTOs;
using MyCRM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCRM.Domain.Entities;
using MyCRM.Application.Interfaces;
using System.Threading;
using Task = System.Threading.Tasks.Task;


namespace MyCRM.Infrastructure.Services
{
    public class CsvExportService : IDisposable
    {
        private readonly IClientRepository _clientRepository;
        private readonly SemaphoreSlim _exportSemaphore;
        private readonly AppConfig _config;

        private bool _disposed = false;

        public CsvExportService(IClientRepository clientRepository)
            : this(clientRepository, AppConfig.Load())
        {
        }


        public CsvExportService(IClientRepository clientRepository, AppConfig config)
        {
            _clientRepository = clientRepository;
            _config = config;
            _exportSemaphore = new SemaphoreSlim(config.Export.MaxConcurrentExports, config.Export.MaxConcurrentExports);
        }

        public async Task ExportClientsToCsvAsync(CancellationToken token = default)
        {
            await _exportSemaphore.WaitAsync(token);
            try
            {
                token.ThrowIfCancellationRequested();

                List<Client> clients = await _clientRepository.GetAllClientAsync(token);

                if (clients.Count == 0)
                {
                    Console.WriteLine("Нет клиентов для экспорта");
                    return;
                }

                string fileName = $"clients_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                string delimiter = _config.Export.CsvDelimiter;

                await using (StreamWriter writer = new StreamWriter(fileName))
                {
                    await writer.WriteLineAsync($"\"Id\"{delimiter}\"Name\"{delimiter}\"Surname\"{delimiter}\"Age\"{delimiter}\"CreatedAt\"");

                    foreach (var client in clients)
                    {
                        token.ThrowIfCancellationRequested();
                        string createdAt = DateTime.Now.ToString("o");
                        await writer.WriteLineAsync($"\"{client.Id}\"{delimiter}\"{client.Name}\"{delimiter}\"{client.Surname}\"{delimiter}\"{client.Age}\"{delimiter}\"{createdAt}\"");
                    }

                    Console.WriteLine("Экспорт завершен!");
                    Console.WriteLine($"Файл: {fileName}");
                    Console.WriteLine($"Экспортировано клиентов: {clients.Count}");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Экспорт отменён");
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при экспорте: {e.Message}");
            }
            finally
            {
                _exportSemaphore.Release();
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _exportSemaphore?.Dispose();
                _disposed = true;
            }
        }
    }
}