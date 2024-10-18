using PhoneApp.Domain.Attributes;
using PhoneApp.Domain.DTO;
using PhoneApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EmployeesSeederPlugin
{
    [Author(Name = "Author Name")]
    public class EmployeesSeederPlagin : IPluggable
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static string GetJsonString()
        {
            try
            {
                WebRequest request = WebRequest.Create("https://dummyjson.com/users");
                request.Method = "GET";
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                string json = reader.ReadToEnd();
                return json;
            }
            catch (Exception ex) 
            { 
                logger.Error(ex);
                return null;
            }
        }
        public static IEnumerable<EmployeesDTO> Convert(StringBuilder stringBuilder, JToken data, List<EmployeesDTO> resultCollection)
        {
            foreach(JToken elem in data)
            {
                stringBuilder.Append(elem["firstName"]);
                stringBuilder.Append(" ");
                stringBuilder.Append(elem["lastName"]);
                stringBuilder.Append(" ");
                stringBuilder.Append(elem["maidenName"]);

                EmployeesDTO employeesDT = new EmployeesDTO { Name = stringBuilder.ToString() };
                employeesDT.AddPhone(elem["phone"].ToString());
                resultCollection.Add(employeesDT);
                stringBuilder.Clear();
            }
            return resultCollection;
        }
        public IEnumerable<DataTransferObject> Run(IEnumerable<DataTransferObject> args)
        {
            logger.Info("Seeder Plugin Started");
            string json = GetJsonString();
            logger.Info("JSON length:"+json.Length);
            if (!String.IsNullOrEmpty(json))
            {
                StringBuilder sb = new StringBuilder();

                var jsonData = JObject.Parse(json).Children();

                var data = jsonData.First().First();

                List<EmployeesDTO> result = new List<EmployeesDTO>();

                var employeesDtoList = Convert(sb, data, result);

                //for (int i = 0; i < 10000; i++)
                //  args = args.Concat(employeesDtoList.Cast<DataTransferObject>());

                return args.Concat(employeesDtoList.Cast<DataTransferObject>());
            }
            else
            {
                logger.Error("Json is null or empty Class:EmployeeSeederPlugin");
                return args;
            }
        }
    }
}
