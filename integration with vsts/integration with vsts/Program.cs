using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace integration_with_vsts
{
    class VSTS
    {
        static void Main(string[] args)
        {
            RunGetBugsQueryUsingClientLib();
            GetProjects();
            GetWorkItem();
        }

        public static async void GetWorkItem()
        {
            try
            {
                var personalaccesstoken = "Its my Personal";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", "", personalaccesstoken))));

                    using (HttpResponseMessage response = client.GetAsync(
                                "https://vso-lis.visualstudio.com/_apis/wit/workitems?ids=11837&api-version=4.1").Result)
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseBody);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public static async void GetProjects()
        {
            try
            {
                var personalaccesstoken = "oqrryl7tr2c73fobfq7b72sxpacfq452fxppfmepg5kcdalp4ata";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", "", personalaccesstoken))));

                    using (HttpResponseMessage response = client.GetAsync(
                                "https://vso-lis.visualstudio.com/DefaultCollection/_apis/projects").Result)
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseBody);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static List<WorkItem> RunGetBugsQueryUsingClientLib()
        {
            Uri uri = new Uri("https://vso-lis.visualstudio.com");
            string personalAccessToken = "oqrryl7tr2c73fobfq7b72sxpacfq452fxppfmepg5kcdalp4ata";
            string project = "SDB20";

            VssBasicCredential credentials = new VssBasicCredential("", personalAccessToken);

            //create a wiql object and build our query
            Wiql wiql = new Wiql()
            {
                Query = "Select [State], [Title] " +
                        "From WorkItems " +
                        "Where [Work Item Type] = 'Bug' " +
                        "And [System.TeamProject] = '" + project + "' " +
                        "And [System.State] <> 'Closed' " +
                        "Order By [State] Asc, [Changed Date] Desc"
            };

            //create instance of work item tracking http client
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(uri, credentials))
            {
                //execute the query to get the list of work items in the results
                WorkItemQueryResult workItemQueryResult = workItemTrackingHttpClient.QueryByWiqlAsync(wiql).Result;

                //some error handling                
                if (workItemQueryResult.WorkItems.Count() != 0)
                {
                    //need to get the list of our work item ids and put them into an array
                    List<int> list = new List<int>();
                    foreach (var item in workItemQueryResult.WorkItems)
                    {
                        list.Add(item.Id);
                    }
                    int[] arr = list.ToArray();

                    //build a list of the fields we want to see
                    string[] fields = new string[3];
                    fields[0] = "System.Id";
                    fields[1] = "System.Title";
                    fields[2] = "System.State";

                    //get work items for the ids found in query
                    var workItems = workItemTrackingHttpClient.GetWorkItemsAsync(arr, fields, workItemQueryResult.AsOf).Result;

                    Console.WriteLine("Query Results: {0} items found", workItems.Count);

                    //loop though work items and write to console
                    foreach (var workItem in workItems)
                    {
                        Console.WriteLine("{0}          {1}                     {2}", workItem.Id, workItem.Fields["System.Title"], workItem.Fields["System.State"]);
                    }

                    return workItems;
                }

                return null;
            }

        }
    }

}
//install-Package Microsoft.TeamFoundationServer.Client -Version 15.112.1
//    Install-Package Microsoft.VisualStudio.Services.Client -Version 15.112.1