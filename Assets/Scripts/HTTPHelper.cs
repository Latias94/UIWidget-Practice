using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnityEngine;

public class HTTPHelper: MonoBehaviour
{
    public static HTTPHelper instance;

    public HttpClient Client { get; private set; }
    private Task currentLock = Task.CompletedTask;

    public void Awake()
    {
        instance = this;
        Client = new HttpClient();
        // 设置3秒超时
        Client.Timeout = new TimeSpan(0, 0, 5);
    }
    
    private void OnDestroy()
    {
        Client.Dispose();
    }

    public async void GetAsync<T>(string requestUri, Action<object> callback)
    {
        Debug.Log($"GetAsync WaitLock {requestUri}");
        TaskCompletionSource<bool> taskLock = new TaskCompletionSource<bool>();
        await GetLock(taskLock.Task);
        Debug.Log($"GetAsync Execute {requestUri}");

        HttpResponseMessage responseMessage;
        try
        {
            responseMessage = await Client.GetAsync(requestUri);
            Debug.Log($"responseMessage: {responseMessage}");
        }
        catch (Exception e)
        {
            callback(null);
            Debug.Log($"GetAsync Execute Done {requestUri}");
            taskLock.SetResult(true);
            return;
        }

        Debug.Log($"GetAsync Execute Done {requestUri}");
        taskLock.SetResult(true);

        if (responseMessage != null && responseMessage.IsSuccessStatusCode)
        {
            string responseString = await responseMessage.Content.ReadAsStringAsync();
            Debug.Log($"GetAsync Response {responseString}");

            Type type = typeof(T);
            if (type.FullName == "System.String")
            {
                callback(responseString);
                //callback((T)TypeDescriptor.GetConverter(type).ConvertTo(null, null, responseString, type));
            }
            else if (type.IsValueType)
            {
                callback((T) TypeDescriptor.GetConverter(type).ConvertFromString(null, null, responseString));
            }
            else
            {
                T jsonObject;
                try
                {
                    jsonObject = JsonHelper.FromJson<T>(responseString);
                }
                catch
                {
                    Debug.Log("JSON返回异常");
                    callback(null);
                    return;
                }

                callback(jsonObject);
            }
        }
        else
        {
            callback(null);
            //throw new Exception($"HttpClient GetAsync Error {responseMessage.StatusCode}");
        }
    }

    public async void PostAsync<T>(string requestUri, Dictionary<string, string> postParameters,
        Action<object> callback)
    {
        Debug.Log($"PostAsync WaitLock {requestUri}");
        TaskCompletionSource<bool> taskLock = new TaskCompletionSource<bool>();
        await GetLock(taskLock.Task);
        Debug.Log($"PostAsync Execute {requestUri}");

        HttpContent content = new FormUrlEncodedContent(postParameters);
        HttpResponseMessage responseMessage = null;
        try
        {
            responseMessage = await Client.PostAsync(requestUri, content);
        }
        catch (Exception e)
        {
            callback(null);
            Debug.Log($"PostAsync Execute Done {requestUri}");
            taskLock.SetResult(true);
            return;
        }

        Debug.Log($"PostAsync Execute Done {requestUri}");
        taskLock.SetResult(true);

        if (responseMessage != null && responseMessage.IsSuccessStatusCode)
        {
            string responseString = await responseMessage.Content.ReadAsStringAsync();
            Debug.Log($"PostAsync Response {responseString}");
            Type type = typeof(T);
            if (type.FullName == "System.String")
            {
                //callback((T)TypeDescriptor.GetConverter(type).ConvertTo(null, null, responseString, type));
                callback(responseString);
            }
            else if (type.IsValueType)
            {
                callback((T) TypeDescriptor.GetConverter(type).ConvertFromString(null, null, responseString));
            }
            else
            {
                T jsonObject;
                try
                {
                    jsonObject = JsonHelper.FromJson<T>(responseString);
                }
                catch
                {
                    Debug.Log("JSON返回异常");
                    callback(null);
                    return;
                }

                callback(jsonObject);
            }
        }
        else
        {
            callback(null);
            //throw new Exception($"HttpClient PostAsync Error {responseMessage.StatusCode}");
        }
    }

    public async void PostJsonAsync<T>(string requestUri, string postData, Action<object> callback)
    {
        Debug.Log($"PostJsonAsync WaitLock {requestUri}");

        TaskCompletionSource<bool> taskLock = new TaskCompletionSource<bool>();
        await GetLock(taskLock.Task);
        Debug.Log($"PostJsonAsync Execute {requestUri}");


        HttpContent content = new StringContent(postData);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        content.Headers.ContentType.CharSet = "utf-8";

        HttpResponseMessage responseMessage;
        try
        {
            responseMessage = await Client.PostAsync(requestUri, content);
        }
        catch (Exception e)
        {
            callback(null);
            Debug.Log($"PostJsonAsync Execute Done {requestUri}");
            taskLock.SetResult(true);
            return;
        }

        Debug.Log($"PostJsonAsync Execute Done {requestUri}");
        taskLock.SetResult(true);

        if (responseMessage != null && responseMessage.IsSuccessStatusCode)
        {
            string responseString = await responseMessage.Content.ReadAsStringAsync();
            Debug.Log($"PostJsonAsync Response {responseString}");


            Type type = typeof(T);
            if (type.FullName == "System.String")
            {
                callback(responseString);
                //callback((T)TypeDescriptor.GetConverter(type).ConvertTo(null, null, responseString, type));
            }
            else if (type.IsValueType)
            {
                callback((T) TypeDescriptor.GetConverter(type).ConvertFromString(null, null, responseString));
            }
            else
            {
                T jsonObject;
                try
                {
                    jsonObject = JsonHelper.FromJson<T>(responseString);
                }
                catch
                {
                    Debug.Log("JSON返回异常");
                    callback(null);
                    return;
                }

                callback(jsonObject);
            }
        }
        else
        {
            callback(null);
            //throw new Exception($"HttpClient PostAsync Error {responseMessage.StatusCode}");
        }
    }

    private Task GetLock(Task waitLock)
    {
        Task previousLock = currentLock;
        currentLock = waitLock;
        return previousLock;
    }
}