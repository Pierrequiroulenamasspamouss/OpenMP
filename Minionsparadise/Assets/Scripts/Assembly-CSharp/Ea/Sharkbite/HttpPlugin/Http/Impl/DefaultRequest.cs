using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEngine;
using strange.extensions.signal.impl;
using Ea.Sharkbite.HttpPlugin.Http.Api;
using Kampai.Util;
using ICSharpCode.SharpZipLib.GZip;
using System.Security.Cryptography.X509Certificates;

namespace Ea.Sharkbite.HttpPlugin.Http.Impl
{
    public class DefaultRequest : IRequest
    {
        #region Constantes et Champs
        public const long UNDEFINED_CONTENT_LENGTH = 0L;
        private const int BLOCK_SIZE = 8192;
        protected const long MIN_NOTIFY_DELTA = 102400L;

        private bool _useGZip;
        protected DownloadProgress _progress;
        protected Action<DownloadProgress, IRequest> _notifyAction;
        protected bool _abort;
        protected bool _isRestarted;
        #endregion

        #region Propriétés
        public string Uri { get; set; }
        public virtual string Method { get; set; }
        public byte[] Body { get; set; }
        public string Accept { get; set; }
        public string ContentType { get; set; }
        public string Username { get; set; }
        public string Password { private get; set; }
        
        public List<KeyValuePair<string, string>> QueryParams { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public List<KeyValuePair<string, string>> FormParams { get; set; }

        protected long Range { get; set; }
        public bool CanRetry { get; set; }
        public int RetryCount { get; set; }
        public bool TryResume { get; set; }
        public string FilePath { get; set; }
        public string Md5 { get; set; }
        public bool UseUdp { get; set; }
        public int requestCount { get; set; } // Remis en minuscule pour satisfaire l'interface

        public bool UseGZip
        {
            get { return _useGZip; }
            set
            {
                _useGZip = value;
                Headers["Accept-Encoding"] = _useGZip ? "gzip" : "identity";
            }
        }

        public bool AvoidBackup { get; set; }
        public bool RunInBackground { get; set; }

        public Signal<IResponse> ResponseSignal { get; set; }
        public Signal<DownloadProgress, IRequest> ProgressSignal { get; set; }
        #endregion

        #region Constructeur
        public DefaultRequest(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentNullException("uri");

            Uri uriObj = new Uri(uri);
            string scheme = uriObj.Scheme.ToLower();

            if (scheme != "http" && scheme != "https")
                throw new ArgumentException("Only HTTP and HTTPS schemes supported");

            if (!string.IsNullOrEmpty(uriObj.Query))
                throw new ArgumentException("Query parameters should be set using the WithQueryParam method, rather than set directly in the Uri");

            Uri = uri;
            Method = "GET";
            Accept = string.Empty;
            ContentType = string.Empty;
            QueryParams = new List<KeyValuePair<string, string>>();
            Headers = new Dictionary<string, string>();
            FormParams = new List<KeyValuePair<string, string>>();
            Range = 0L;
            UseGZip = false;
            requestCount = 0;
            AvoidBackup = false;
            _progress = new DownloadProgress(uri);
        }
        #endregion

        #region Méthodes HTTP (Verbes)
        public virtual void Get(Action<IResponse> callback) { Method = "GET"; Execute(callback); }
        public virtual void Head(Action<IResponse> callback) { Method = "HEAD"; Execute(callback); }
        public virtual void Options(Action<IResponse> callback) { Method = "OPTIONS"; Execute(callback); }
        public virtual void Post(Action<IResponse> callback) { Method = "POST"; Execute(callback); }
        public virtual void Put(Action<IResponse> callback) { Method = "PUT"; Execute(callback); }
        public virtual void Delete(Action<IResponse> callback) { Method = "DELETE"; Execute(callback); }

        public virtual void Execute(Action<IResponse> callback)
        {
            UnityEngine.Debug.LogFormat("Sending HTTP {0} request to: {1}", Method, Uri);
            ThreadPool.QueueUserWorkItem(state => GetResponse(callback));
        }
        #endregion

        #region API Fluide (Builder Pattern)
        public IRequest WithContentType(string contentType) { ContentType = contentType; return this; }
        public IRequest WithAccept(string accept) { Accept = accept; return this; }
        
        public IRequest WithQueryParam(string key, string value)
        {
            QueryParams.Add(new KeyValuePair<string, string>(key, value));
            return this;
        }

        public IRequest WithHeaderParam(string key, string value)
        {
            Headers[key] = value;
            return this;
        }

        public IRequest WithFormParam(string key, string value)
        {
            FormParams.Add(new KeyValuePair<string, string>(key, value));
            return this;
        }

        public IRequest WithBasicAuth(string username, string password)
        {
            Username = username;
            Password = password;
            return this;
        }

        public IRequest WithBody(byte[] body) { Body = body; return this; }
        
        public IRequest WithPreprocessor(IRequestPreprocessor preprocessor)
        {
            preprocessor.preprocess(this);
            return this;
        }

        public IRequest WithMethod(string method) { Method = method; return this; }
        public IRequest WithOutputFile(string filePath) { FilePath = filePath; return this; }
        public IRequest WithMd5(string md5) { Md5 = md5; return this; }
        public IRequest WithGZip(bool useGZip) { UseGZip = useGZip; return this; }
        public IRequest WithUdp(bool useUdp) { UseUdp = useUdp; return this; }
        public IRequest WithAvoidBackup(bool avoidBackup) { AvoidBackup = avoidBackup; return this; }
        public IRequest WithRunInBackground(bool runInBackground) { RunInBackground = runInBackground; return this; }
        public IRequest WithResponseSignal(Signal<IResponse> responseSignal) { ResponseSignal = responseSignal; return this; }
        public IRequest WithProgressSignal(Signal<DownloadProgress, IRequest> progressSignal) { ProgressSignal = progressSignal; return this; }
        
        public void RegisterNotifiable(Action<DownloadProgress, IRequest> notify) { _notifyAction = notify; }
        
        public IRequest WithRetry(bool retry = true, int times = 3)
        {
            CanRetry = retry;
            RetryCount = times;
            return this;
        }

        public IRequest WithResume(bool tryResume) { TryResume = tryResume; return this; }
        
        public IRequest WithEntity(object entity)
        {
            Body = FastJSONSerializer.SerializeUTF8(entity);
            return this;
        }

	public IRequest WithRequestCount(int requestCount) { this.requestCount = requestCount; return this; }
        #endregion

        #region Contrôle de Flux (Abort/Restart)
        public virtual void Abort()
        {
            _abort = true;
            if (_isRestarted) _isRestarted = false;
        }

        public virtual bool IsAborted() { return _abort; }

        public virtual void Restart()
        {
            Abort();
            _isRestarted = true;
        }

        public virtual bool IsRestarted() { return _isRestarted; }

        protected virtual void TryRestart()
        {
            if (_isRestarted)
            {
                _abort = false;
                _isRestarted = false;
            }
            if (!_abort)
            {
                _progress = new DownloadProgress(Uri);
            }
        }

        public virtual DownloadProgress GetProgress()
        {
            return _progress == null ? null : _progress.Clone();
        }

        public virtual string GetTempFilePath()
        {
            return string.Format("{0}.download", FilePath);
        }
        #endregion

        #region Utilitaires Web
        protected string GetUriWithQueryParams()
        {
            string baseUrl = UseUdp ? MediaClient.ConvertUrl(Uri) : Uri;
            StringBuilder builder = new StringBuilder(baseUrl);
            
            if (QueryParams.Count > 0)
            {
                builder.Append("?");
                List<string> queryParts = new List<string>();
                foreach (var param in QueryParams)
                {
                    queryParts.Add(string.Format("{0}={1}", global::UnityEngine.WWW.EscapeURL(param.Key), global::UnityEngine.WWW.EscapeURL(param.Value)));
                }
                builder.Append(string.Join("&", queryParts.ToArray()));
            }
            return builder.ToString();
        }

        protected string GetBasicAuthHeader()
        {
            string credentials = string.Format("{0}:{1}", Username, Password);
            return string.Format("Basic {0}", Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials)));
        }
        #endregion

        #region Logique Principale (Requêtes & Réponses)
        protected virtual HttpWebRequest CreateRequest()
        {
            HttpWebRequest request = null;
            try
            {
                request = WebRequest.Create(GetUriWithQueryParams()) as HttpWebRequest;
                request.Timeout = HttpRequestConfig.HttpRequestTimeout;
                request.ReadWriteTimeout = HttpRequestConfig.HttpRequestReadWriteTimeout;
                
                if (ConnectionSettings.ConnectionLimit != 0)
                    request.ServicePoint.ConnectionLimit = ConnectionSettings.ConnectionLimit;

                if (string.IsNullOrEmpty(Method))
                    throw new InvalidOperationException("A request Method (GET, POST, PUT, DELETE) must be provided.");
                
                request.Method = Method;

                if (!string.IsNullOrEmpty(Accept)) request.Accept = Accept;
                if (!string.IsNullOrEmpty(ContentType)) request.ContentType = ContentType;

                foreach (var header in Headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }

                if (Range != 0L) request.AddRange((int)Range);
                if (!string.IsNullOrEmpty(Username)) request.Headers.Add(HttpRequestHeader.Authorization, GetBasicAuthHeader());

                bool hasBody = Body != null;
                bool hasFormParams = FormParams.Count > 0;

                if (hasBody && hasFormParams)
                    throw new InvalidOperationException("Request must contain only form params, or a body, or an entity, without combination.");

                if (hasFormParams)
                {
                    if (string.IsNullOrEmpty(ContentType))
                    {
                        ContentType = "application/x-www-form-urlencoded";
                        request.MediaType = "application/x-www-form-urlencoded";
                    }
                    List<string> formParts = new List<string>();
                    foreach (var param in FormParams)
                    {
                        formParts.Add(string.Format("{0}={1}", global::UnityEngine.WWW.EscapeURL(param.Key), global::UnityEngine.WWW.EscapeURL(param.Value)));
                    }
                    Body = Encoding.UTF8.GetBytes(string.Join("&", formParts.ToArray()));
                }

                // Attention: Le code original attache un callback global ici.
                ServicePointManager.ServerCertificateValidationCallback += CertificateValidationCallback;

                if (Body != null)
                {
                    if (Method == "GET" || Method == "DELETE")
                        throw new ProtocolViolationException();

                    request.ContentLength = Body.Length;
                    // Amélioration propre : utilisation de `using` pour garantir la fermeture du stream
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(Body, 0, Body.Length);
                    }
                }
            }
            catch (WebException ex)
            {
                ServicePointManager.ServerCertificateValidationCallback -= CertificateValidationCallback;
                Native.LogError(ex.Message);
            }
            catch (Exception ex)
            {
                Native.LogError(ex.Message);
            }
            return request;
        }

        protected virtual HttpWebResponse ExecuteRequest()
        {
            HttpWebResponse response = null;
            HttpWebRequest request = CreateRequest();
            
            if (request == null)
            {
                Native.LogError(string.Format("Null request for Uri {0}", Uri));
                return null;
            }
            
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException ex)
            {
                Native.LogWarning(string.Format("WebException Downloading {0}: {1}", Uri, ex.Message));
                if (ex.Response != null)
                {
                    response = ex.Response as HttpWebResponse;
                }
                else
                {
                    ServicePointManager.ServerCertificateValidationCallback -= CertificateValidationCallback;
                    Native.LogError(string.Format("WebException Response is NULL: {0}", ex.Message));
                }
            }
            catch (Exception ex)
            {
                Native.LogError(string.Format("Exception Downloading {0}: {1}", Uri, ex.Message));
            }
            return response;
        }

        protected virtual Dictionary<string, string> ProcessResponse(HttpWebResponse response, string body = "")
        {
            var dict = new Dictionary<string, string>();
            if (response != null && response.Headers != null)
            {
                foreach (string key in response.Headers.AllKeys)
                {
                    dict.Add(key, response.Headers.Get(key));
                }
            }
            ServicePointManager.ServerCertificateValidationCallback -= CertificateValidationCallback;
            return dict;
        }

        protected virtual string ReadResponse(HttpWebResponse response)
        {
            if (!IsFileDownload())
            {
                string result = string.Empty;
                try
                {
                    if (response.GetResponseStream() != null)
                    {
                        Encoding encoding;
                        try { encoding = Encoding.GetEncoding(response.CharacterSet); }
                        catch (ArgumentException) { encoding = Encoding.UTF8; }

                        // Amélioration propre : lecture en blocs sécurisée
                        using (Stream stream = response.GetResponseStream())
                        using (MemoryStream ms = new MemoryStream())
                        {
                            byte[] buffer = new byte[BLOCK_SIZE];
                            int bytesRead;
                            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                ms.Write(buffer, 0, bytesRead);
                            }
                            result = encoding.GetString(ms.ToArray());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Native.LogError(ex.Message);
                }
                return result;
            }

            using (Stream stream = response.GetResponseStream())
            {
                string contentEncoding = response.Headers["Content-Encoding"];
                if (!string.IsNullOrEmpty(contentEncoding) && contentEncoding.ToLower().Contains("gzip"))
                {
                    using (GZipInputStream gzipStream = new GZipInputStream(stream))
                    {
                        return ReadFileResponse(gzipStream);
                    }
                }
                return ReadFileResponse(stream);
            }
        }

        protected virtual void GetResponse(Action<IResponse> callback)
        {
            UnityEngine.Debug.LogFormat("[HTTP] GetResponse started for {0}", Uri);
            DefaultResponse finalResponse = null;
            TryRestart();
            bool isFile = IsFileDownload();

            if (!_abort)
            {
                _progress.StartTimer();
                using (HttpWebResponse httpWebResponse = ExecuteRequest())
                {
                    UnityEngine.Debug.LogFormat("[HTTP] ExecuteRequest returned for {0}. Response is {1}", Uri, (httpWebResponse != null) ? httpWebResponse.StatusCode.ToString() : "NULL");
                    if (httpWebResponse != null)
                    {
                        if (!isFile)
                        {
                            string body = ReadResponse(httpWebResponse);
                            finalResponse = (DefaultResponse)new DefaultResponse()
                                .WithBody(body)
                                .WithCode((int)httpWebResponse.StatusCode)
                                .WithRequest(this)
                                .WithContentLength(httpWebResponse.ContentLength)
                                .WithContentType(httpWebResponse.ContentType)
                                .WithHeaders(ProcessResponse(httpWebResponse, body));
                        }
                        else
                        {
                            PrepareDirectory();
                            try
                            {
                                var headers = ProcessResponse(httpWebResponse, string.Empty);
                                _progress.TotalBytes = GetContentLength(headers);
                                
                                string hashText = (httpWebResponse.StatusCode == HttpStatusCode.RequestedRangeNotSatisfiable) 
                                    ? string.Empty 
                                    : ReadResponse(httpWebResponse);
                                
                                string tempPath = GetTempFilePath();
                                string error = null;
                                int code = (int)httpWebResponse.StatusCode;

#if !UNITY_WEBPLAYER
                                if (!_abort && (string.IsNullOrEmpty(Md5) || Md5 == hashText))
                                {
                                    File.Move(tempPath, FilePath);
                                }
                                else
                                {
                                    if (File.Exists(tempPath)) File.Delete(tempPath);
                                    error = _abort ? "Aborting file download" : string.Format("Invalid MD5SUM {0} != {1}", Md5, hashText);
                                    code = 418; // I am a teapot (souvent utilisé pour des erreurs custom)
                                }
#endif
                                finalResponse = (DefaultResponse)new FileDownloadResponse()
                                    .WithError(error)
                                    .WithCode(code)
                                    .WithRequest(this)
                                    .WithContentLength(httpWebResponse.ContentLength)
                                    .WithContentType(httpWebResponse.ContentType)
                                    .WithHeaders(headers);
                            }
                            catch (Exception ex)
                            {
                                finalResponse = (DefaultResponse)new FileDownloadResponse()
                                    .WithError(ex.Message)
                                    .WithRequest(this)
                                    .WithCode(500);
                                Native.LogError(ex.Message);
                            }
                        }
                    }
                    else
                    {
                        Native.LogError("Null response for " + Uri);
                        finalResponse = isFile 
                            ? (DefaultResponse)new FileDownloadResponse().WithError("The request timed out?").WithCode(408).WithRequest(this).WithConnectionLoss(true) 
                            : (DefaultResponse)new DefaultResponse().WithCode(500).WithRequest(this).WithConnectionLoss(true);
                    }
                }
                _progress.StopTimer();
                finalResponse = (DefaultResponse)finalResponse.WithDownloadTime(_progress.GetDownloadTime());
            }
            else
            {
                finalResponse = isFile 
                    ? (DefaultResponse)new FileDownloadResponse().WithRequest(this).WithCode(500).WithError("Request was aborted before execution.")
                    : (DefaultResponse)new DefaultResponse().WithRequest(this).WithCode(500).WithError("Request was aborted before execution.");
            }

            if (callback != null)
            {
                UnityEngine.Debug.LogFormat("[HTTP] Calling callback for {0}", Uri);
                callback(finalResponse);
            }
        }

        private string ReadFileResponse(Stream input)
        {
#if !UNITY_WEBPLAYER
            try
            {
                using (MD5 md5 = MD5.Create())
                using (FileStream fileStream = File.Create(GetTempFilePath()))
                {
                    try
                    {
                        int bytesRead;
                        byte[] buffer = new byte[BLOCK_SIZE];
                        
                        while (!_abort && (bytesRead = input.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            fileStream.Write(buffer, 0, bytesRead);
                            md5.TransformBlock(buffer, 0, bytesRead, buffer, 0);
                            NotifyProgress(bytesRead, MIN_NOTIFY_DELTA);
                        }
                        
                        if (!_abort)
                        {
                            md5.TransformFinalBlock(buffer, 0, 0);
                        }
                    }
                    finally
                    {
                        NotifyProgress(0L, 0L);
                    }
                    return _abort ? string.Empty : BitConverter.ToString(md5.Hash).Replace("-", string.Empty).ToLower();
                }
            }
            catch (Exception ex)
            {
                Native.LogError(ex.Message);
                return string.Empty;
            }
#else
            return string.Empty;
#endif
        }

        protected virtual void NotifyProgress(long downloadedAmount, long downloadSizeDeltaMin = 0L)
        {
            _progress.CompletedBytes += downloadedAmount;
            _progress.DownloadedBytes += downloadedAmount;
            _progress.Delta += downloadedAmount;
            
            if (_notifyAction != null && _progress.Delta > downloadSizeDeltaMin)
            {
                _notifyAction(GetProgress(), this);
                _progress.Delta = 0L;
            }
        }

        protected bool IsFileDownload()
        {
            return !string.IsNullOrEmpty(FilePath);
        }

        protected void PrepareDirectory()
        {
            if (IsFileDownload())
            {
#if !UNITY_WEBPLAYER
                if (File.Exists(FilePath)) File.Delete(FilePath);
                
                string tempFilePath = GetTempFilePath();
                if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
                
                string directoryName = Path.GetDirectoryName(FilePath);
                if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
#endif
            }
        }

        protected long GetContentLength(IDictionary<string, string> headers)
        {
            long result;
            if (!headers.ContainsKey("Content-Length") || !long.TryParse(headers["Content-Length"], out result))
            {
                result = -1L;
            }
            return result <= 0 ? 0 : result;
        }

        protected string GetHeader(IDictionary<string, string> headers, string key)
        {
            string value;
            headers.TryGetValue(key, out value);
            return value ?? string.Empty;
        }

        protected static bool CertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Dans le jeu original, EA a désactivé la vérification SSL (renvoie toujours true).
            // Pratique en dev, mais risqué en prod !
            return true;
        }
        #endregion
    }
}