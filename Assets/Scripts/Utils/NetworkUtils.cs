using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Utils
{
    /// <summary>
    /// Утилитарный класс для работы с сетевыми запросами
    /// </summary>
    public static class NetworkUtils
    {
        /// <summary>
        /// Проверяет доступность интернет-соединения
        /// </summary>
        /// <param name="url">URL для проверки, по умолчанию google.com</param>
        /// <param name="timeout">Время ожидания в секундах</param>
        /// <returns>True если соединение доступно, иначе False</returns>
        public static async Task<bool> CheckInternetConnection(string url = "https://www.google.com", float timeout = 5f)
        {
            try
            {
                using (UnityWebRequest request = new UnityWebRequest(url, "HEAD"))
                {
                    request.timeout = Mathf.RoundToInt(timeout);

                    float startTime = Time.time;
                    UnityWebRequestAsyncOperation operation = request.SendWebRequest();

                    while (!operation.isDone && Time.time - startTime < timeout)
                    {
                        await Task.Yield();
                    }

                    return request.result == UnityWebRequest.Result.Success;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Ошибка при проверке интернет-соединения: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Выполняет асинхронный запрос с таймаутом
        /// </summary>
        /// <param name="request">Объект запроса</param>
        /// <param name="timeout">Время ожидания в секундах</param>
        /// <returns>True если запрос успешен, иначе False</returns>
        public static async Task<bool> ExecuteRequestWithTimeout(UnityWebRequest request, float timeout = 10f)
        {
            if (request == null) return false;

            try
            {
                float startTime = Time.time;
                UnityWebRequestAsyncOperation operation = request.SendWebRequest();

                while (!operation.isDone && Time.time - startTime < timeout)
                {
                    await Task.Yield();
                }

                if (Time.time - startTime >= timeout)
                {
                    Debug.LogWarning($"Запрос на {request.url} превысил таймаут {timeout} секунд");
                    request.Abort();
                    return false;
                }

                return request.result == UnityWebRequest.Result.Success;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка при выполнении запроса: {ex.Message}");
                return false;
            }
        }
    }
}
