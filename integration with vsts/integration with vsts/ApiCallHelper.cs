using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace integration_with_vsts
{
    public class ApiCallHelper
    {
        public string CallToAPI(string address, string output, string methodType, bool IsTokenRequired = true, string token = null, bool ContentTypeRequired = false)
        {
            // New code:
            HttpClient client = new HttpClient();
            string responseData = string.Empty;
            client.BaseAddress = new Uri("");
            client.DefaultRequestHeaders.Accept.Clear();
            if (ContentTypeRequired)
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (IsTokenRequired)
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            if (methodType == "GET")
            {
                HttpResponseMessage response = client.GetAsync(address).Result;  // Blocking call!
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body. Blocking!
                    responseData = response.Content.ReadAsStringAsync().Result;

                }
            }
            else if (methodType == "PUT")
            {
                HttpResponseMessage response = client.PutAsync(address, new StringContent(output, Encoding.UTF8, "text/json")).Result;  // Blocking call!
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body. Blocking!
                    responseData = response.Content.ReadAsStringAsync().Result;
                    if (string.IsNullOrEmpty(responseData))
                    {
                        responseData = "success";
                    }
                }
            }
            else
            {
                HttpResponseMessage response = client.PostAsync(address, new StringContent(output, Encoding.UTF8, "text/json")).Result;  // Blocking call!
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body. Blocking!
                    responseData = response.Content.ReadAsStringAsync().Result;
                    if (string.IsNullOrEmpty(responseData))
                    {
                        responseData = "success";
                    }

                }
            }
            return responseData;
        }

    }
}
