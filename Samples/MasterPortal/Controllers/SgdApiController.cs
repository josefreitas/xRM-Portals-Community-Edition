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
using System.Web.Mvc;
using Adxstudio.Xrm.Web.UI.WebForms;
using Microsoft.Xrm.Client.Runtime.Serialization;
using RestSharp;
using Site.Controllers;


namespace Site.Pages
{
    public class SgdApiController : BaseApiController
    {
        
        [System.Web.Http.HttpGet]
        public HttpResponseMessage ReportFichaRegisto(int typeOfReport,  string livroRegistosId) {
            TypeOfReport _typeOfReport = (TypeOfReport)typeOfReport;

            var serviceName = Enum.GetName(typeof(TypeOfReport), typeOfReport);
            
            var result = new HttpResponseMessage();

            switch (_typeOfReport)
            {
                case TypeOfReport.ReportFichaRegistoJornalistica:
                    result = SendRequestToReportToPdf(Method.GET, serviceName, new[]
                    {
                        new Parameter("livroRegistosId", livroRegistosId, ParameterType.QueryString),
                        new Parameter("numerosequencia", 1, ParameterType.QueryString),
                    });
                    break;
                case TypeOfReport.ReportFichaRegistoNoticiosa:
                case TypeOfReport.ReportFichaRegistoDistribuicao:
                case TypeOfReport.ReportFichaRegistoOperadorRadio:
                case TypeOfReport.ReportFichaRegistoTelevisao:
                case TypeOfReport.ReportFichaRegistoPublicacaoPeriodica:
                case TypeOfReport.ReportFichaRegistoPrgDistExclInternet:
                    result = SendRequestToReportToPdf(Method.GET, serviceName, new[]
                    {
                        new Parameter("livroRegistosId", livroRegistosId, ParameterType.QueryString)
                    });
                    break;
                case TypeOfReport.NotSet:
                    result = Request.CreateErrorResponse(HttpStatusCode.NotFound, "Serviço não encontrado");
                    break;
                default:
                    result = Request.CreateErrorResponse(HttpStatusCode.NotFound, "Serviço não encontrado");
                    break;
            }

            return result;
        }
    }
}