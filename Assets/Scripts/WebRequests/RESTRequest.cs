using UnityEngine;
using System.Collections.Generic;

namespace RESTUtils
{

    public delegate void OnRequestFinish(string data);

    /// <summary>
    /// Enumeration of HTTP REST methods.
    /// </summary>
    public enum HTTPMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    /// <summary>
    /// Encapsulates the request URL and parameters for a REST API call.
    /// </summary>
    public class RESTRequest
    {
        private Dictionary<string, string> stringParameters;
        private Dictionary<string, int> numericParameters;
        private Dictionary<string, string> headers;

        private OnRequestFinish thisCallback;

        private HTTPMethod method;

        private bool rawBytes;

        private byte[] bytes;

        public WWW requestData;

        private bool loading = false;

        private bool loaded = false;

        private int noParameters;

        /// <summary>
        /// REST request with an empty callback function. Can still gather data if you manually check that the request is finishe
        /// </summary>
        public RESTRequest()
        {
            this.thisCallback = null;
            this.method = HTTPMethod.GET;
            this.stringParameters = new Dictionary<string, string>();
            this.numericParameters = new Dictionary<string, int>();
            this.headers = new Dictionary<string, string>();
            this.noParameters = 0;
            this.rawBytes = false;
        }

        /// <summary>
        /// Constructs an empty RESTRequest object, with the default method of GET.
        /// </summary>
        public RESTRequest(OnRequestFinish callback)
        {
            this.thisCallback = callback;
            this.method = HTTPMethod.GET;
            this.stringParameters = new Dictionary<string, string>();
            this.numericParameters = new Dictionary<string, int>();
            this.headers = new Dictionary<string, string>();
            this.noParameters = 0;
            this.rawBytes = false;
        }
        /// <summary>
        /// Constructs an empty RESTRequest object with the given method set.
        /// </summary>
        /// <param name="method">The method to use for the request.</param>
        public RESTRequest(HTTPMethod method, OnRequestFinish callback)
        {
            this.thisCallback = callback;
            this.method = method;
            this.stringParameters = new Dictionary<string, string>();
            this.numericParameters = new Dictionary<string, int>();
            this.headers = new Dictionary<string, string>();
            this.noParameters = 0;
            this.rawBytes = false;
        }

        /// <summary>
        /// Constructs a request with the given bytes to be sent. Useful for nested JSON data, which can
        /// be constructed beforehand. Don't use for GET.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="bytes"></param>
        public RESTRequest(HTTPMethod method, byte[] bytes, OnRequestFinish callback)
        {
            this.thisCallback = callback;
            this.method = method;
            this.bytes = bytes;
            this.headers = new Dictionary<string, string>();
            rawBytes = true;
        }

        /// <summary>
        /// Adds a key-value parameter to the request. Value is string-type.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddParameter(string key, string value)
        {
            stringParameters.Add(key, value);
            this.noParameters++;
        }

        /// <summary>
        /// Adds a key-value parameter to the request. Value is integer-type.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddParameter(string key, int value)
        {
            numericParameters.Add(key, value);
            this.noParameters++;
        }


        /// <summary>
        /// Returns the headers for the request.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetHeaders()
        {
            return this.headers;
        }

        /// <summary>
        /// Add a header to the request.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddHeader(string key, string value)
        {
            this.headers.Add(key, value);
        }


        /// <summary>
        /// Formats all the entered parameters into a GET request query string.
        /// </summary>
        /// <returns>Query string of all the parameters.</returns>
        public string GetParameters()
        {
            string requestUrl = string.Empty;
            int currentParameter = 0;

            if (this.noParameters > 0)
            {
                requestUrl += "?";
            }
            foreach (KeyValuePair<string, string> parameter in this.stringParameters)
            {
                string key = (parameter.Key);
                string value = WWW.EscapeURL(parameter.Value);
                string requestParameter = key + "=" + value;

                if (this.noParameters > 0 && currentParameter++ < noParameters - 1) requestParameter += "&";

                requestUrl += requestParameter;
            }

            foreach (KeyValuePair<string, int> parameter in this.numericParameters)
            {
                string key = parameter.Key;
                int value = parameter.Value;
                string requestParameter = key + "=" + value;

                if (this.noParameters > 0 && currentParameter++ < noParameters - 1) requestParameter += "&";

                requestUrl += requestParameter;
            }

            return requestUrl;
        }

        /// <summary>
        /// Enters all the parameters into a WWWForm object for POST requests.
        /// </summary>
        /// <returns></returns>
        public WWWForm PostParameters()
        {
            WWWForm returnData = new WWWForm();

            foreach (KeyValuePair<string, string> parameter in this.stringParameters)
            {
                string key = parameter.Key;
                string value = WWW.EscapeURL(parameter.Value);
                returnData.AddField(key, value);
            }

            foreach (KeyValuePair<string, int> parameter in this.numericParameters)
            {
                returnData.AddField(parameter.Key, parameter.Value);
            }



            return returnData;
        }


        public bool IsRawBytes()
        {
            return rawBytes;
        }

        public byte[] GetRawBytes()
        {
            return this.bytes;
        }

        public OnRequestFinish retrieveCallback()
        {
            return this.thisCallback;
        }


        /// <summary>
        /// Sets 'loading' flag to true.
        /// </summary>
        public void Start()
        {
            this.loading = true;
        }

        /// <summary>
        /// Sets 'loading' flag to false, and loaded flag to true.
        /// </summary>
        public void Finish()
        {
            this.loading = false;
            this.loaded = true;
        }

        /// <summary>
        /// Sets 'loaded' flag to false.
        /// </summary>
        public void Reset()
        {
            this.loaded = false;
        }

        public bool IsLoading()
        {
            return this.loading;
        }

        public bool IsLoaded()
        {
            return this.loaded;
        }

        /// <summary>
        /// Returns the text data retrieved by the RESTRequest.
        /// </summary>
        /// <returns></returns>
        public string GetData()
        {
            return requestData.text;
        }

        /// <summary>
        /// The REST method being used by the request.
        /// </summary>
        /// <returns></returns>
        public HTTPMethod Method()
        {
            return method;
        }

        /// <summary>
        /// Prints the response headers from the returned REST request. Call once request has loaded.
        /// </summary>
        public void PrintHeaders()
        {
            foreach (KeyValuePair<string, string> header in requestData.responseHeaders)
            {
                Debug.Log(header.Key + " : " + header.Value);
            }
        }


    }
}