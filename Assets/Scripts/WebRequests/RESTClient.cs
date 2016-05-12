using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RESTUtils
{
    /// <summary>
    /// Handles the actual sending of requests to the URL encapsulated in 
    /// a RESTRequest.
    /// Requests occur asynchronously - sendRequest() must be called inside a StartCoroutine
    /// call (requires inheriting MonoBehaviour).
    /// </summary>
    public class RESTClient
    {
        private string auth_token;
        private string endPoint;

        /// <summary>
        /// Creates a client class pointed to the provided endpoint.
        /// Any requests passed into this class will go to the same endpoint.
        /// </summary>
        /// <param name="endPoint">URL string </param>
        public RESTClient(string endPoint)
        {
            this.endPoint = endPoint;
            this.auth_token = null;
        }

        public RESTClient(string endPoint, string auth_token)
        {
            this.endPoint = endPoint;
            this.auth_token = auth_token;
        }

        /// <summary>
        /// Sends a constructed RESTRequest to the client's endpoint. Must be called inside a coroutine -
        /// the returned data will be stored in the RESTRequest.
        /// </summary>
        /// <param name="rr">The request to be sent.</param>
        /// <returns></returns>
        public IEnumerator sendRequest(RESTRequest rr)
        {

            Debug.Log("started data download");

            rr.Start();

            switch (rr.Method())
            {
                case HTTPMethod.GET:
                    GETRequestSetup(rr);
                    break;
                    
                case HTTPMethod.POST:
                    POSTRequestSetup(rr);
                    break;

                case HTTPMethod.PUT:
                    POSTRequestSetup(rr);
                    break;

                case HTTPMethod.DELETE:
                    POSTRequestSetup(rr);
                    break;

                default: break;
            }

            yield return rr.requestData;
            rr.retrieveCallback()(rr.GetData());
            rr.Finish();

            Debug.Log("data download finished");
            
           
        }

        private void GETRequestSetup(RESTRequest rr)
        {
            string fullUrl = this.endPoint + rr.GetParameters();

            rr.requestData = new WWW(fullUrl);
        }

        private void POSTRequestSetup(RESTRequest rr)
        {

            if (rr.IsRawBytes())
            {
                rr.AddHeader("Content-type", "application/json");
            } else
            {
                rr.AddHeader("Content-type", "application/x-www-form-urlencoded");
            }

            if (rr.Method() != HTTPMethod.POST)
            {
                rr.AddHeader("X-HTTP-Method-Override", rr.Method().ToString());
            }

            if (rr.IsRawBytes())
            {
                rr.requestData = new WWW(this.Endpoint(), rr.GetRawBytes(), rr.GetHeaders());
            }
            else
            {
                rr.requestData = new WWW(this.Endpoint(), rr.PostParameters().data, rr.GetHeaders());
            }


        }

        /// <summary>
        /// Returns the client's current endpoint.
        /// </summary>
        /// <returns>The RESTClient's current endpoint.</returns>
        public string Endpoint()
        {
            return endPoint;
        }
    }

}