using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using RestSharp;

namespace Site.Controllers
{
    public class BaseApiController: ApiController
    {
        public string CapgeminiPortalERCSGDWebApi = ConfigurationManager.AppSettings["CapgeminiPortalERCSGDWebApi"];
        public string CapgeminiPortalERCSGDWebApiToken = ConfigurationManager.AppSettings["CapgeminiPortalERCSGDWebApiToken"];
        public string CapgeminiPortalERCSGDWebApiHeader = ConfigurationManager.AppSettings["CapgeminiPortalERCSGDWebApiHeader"];

        [Flags]
        public enum TypeOfReport
        {
            NotSet=0,
            ReportFichaRegistoJornalistica = 1,
            ReportFichaRegistoNoticiosa = 2,
            ReportFichaRegistoDistribuicao = 3,
            ReportFichaRegistoOperadorRadio = 4,
            ReportFichaRegistoTelevisao = 5,
            ReportFichaRegistoPublicacaoPeriodica = 6,
            ReportFichaRegistoPrgDistExclInternet = 7,
            GetSGDDocument = 8
        }

        [NonAction]
        public HttpResponseMessage GetResponse(byte[] dataFile, string fileName)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StreamContent(new MemoryStream(dataFile));



            response.Content.Headers.ContentLength = dataFile.LongLength;
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue($"attachment")
            {
                Name = fileName,
                FileName = fileName,
                Size = dataFile.Length
            };

            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");


            return response;
        }

        [NonAction]
        public virtual HttpResponseMessage SendRequest(Method method,string serviceName, Parameter[] parameters)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;


            var client = new RestClient(CapgeminiPortalERCSGDWebApi);

            var request = new RestRequest(serviceName, Method.GET);
            request.Parameters.AddRange(parameters);


            request.AddHeader(CapgeminiPortalERCSGDWebApiHeader, CapgeminiPortalERCSGDWebApiToken);
            var response = client.Get(request);

            if (response.RawBytes.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NoContent, "Arquivo não contém informações");
            }

            return GetResponse(response.RawBytes, response.Headers.FirstOrDefault(w => w.Name == "filename").ToString());
        }
      

        public HttpResponseMessage SendRequestToReportToPdf(Method method, string serviceName, Parameter[] parameters)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;


            var client = new RestClient(CapgeminiPortalERCSGDWebApi);

            var request = new RestRequest(serviceName, Method.GET);
            request.Parameters.AddRange(parameters);

          

            request.AddHeader(CapgeminiPortalERCSGDWebApiHeader, CapgeminiPortalERCSGDWebApiToken);
            var response = client.Get(request);

            if (response.RawBytes.Length == 0 || !response.Headers.Any(w => w.Name == "filename"))
            {
                return Request.CreateResponse(HttpStatusCode.NoContent, "Arquivo não contém informações");
            }



            return GetResponse(response.RawBytes, response.Headers.FirstOrDefault(w => w.Name == "filename").ToString());
        }
    }
}