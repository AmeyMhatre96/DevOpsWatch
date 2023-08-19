using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System;
using Newtonsoft.Json;
using System.Text;
using System.Linq;
using DevOpsWatch.BL.AzureDevOpsSource.Models;
using DevOpsWatch.BL.Core;

namespace DevOpsWatch.BL.AzureDevOpsSource
{
    public sealed class AzureDevOpsClient
    {
        private readonly HttpClient httpClient;
        private readonly HttpClient vsspsHttpClient;

        public AzureDevOpsClient(Settings settings)
        {
            this.organization = settings.Organization;
            this.project = settings.Project;
            shouldCloseTasksIfEffortsCompleted = settings.CloseTasksWhenEffortsAreComplete;
            shouldShowNewTasks = settings.ShouldShowNewTasks;

            httpClient = new HttpClient();
            // Configure the base URL and any required headers for your API
            httpClient.BaseAddress = new Uri("https://dev.azure.com/");
            string encodedToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{settings.PAT}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedToken);

            vsspsHttpClient = new HttpClient();
            vsspsHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedToken);
            var response = vsspsHttpClient.GetAsync($"https://vssps.dev.azure.com/{organization}/_apis/profile/profiles/me").Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var queryResult = JsonConvert.DeserializeObject<UserModel>(responseContent);
            this.username = queryResult.EmailAddress;

        }

        private readonly string organization;

        private readonly string project;

        private readonly string username;

        private readonly bool shouldCloseTasksIfEffortsCompleted;

        private readonly bool shouldShowNewTasks;

        public string GetCurrentIteration()
        {
            HttpResponseMessage response = httpClient.GetAsync($"/{organization}/{project}/_apis/work/teamsettings/iterations").Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<IterationModel>(responseContent);
                if (item != null && item.value?.Count > 0)
                {
                    var current = item.value.FirstOrDefault(v => v.attributes.timeFrame == "current");
                    if (current != null)
                    {
                        return current.path;
                    }
                }
            }
            return "";
        }

        public IEnumerable<WorkItemReference> GetAllTasksAssignedToUserInCurrentIteration()
        {
            var iter = GetCurrentIteration();
            string iterQuery = !string.IsNullOrEmpty(iter)? $"AND [System.IterationPath] = '{iter}'": "";
            string query = $"Select [System.Id], [System.Title], [System.State] From WorkItems Where [System.WorkItemType] = 'Task' AND [System.AssignedTo] = '{username}' AND [System.State] = 'ACTIVE' {iterQuery}";

            if (shouldShowNewTasks)
            {
                query = $"Select [System.Id], [System.Title], [System.State] From WorkItems Where [System.WorkItemType] = 'Task' AND [System.AssignedTo] = '{username}' AND ([System.State] = 'ACTIVE' OR  [System.State] = 'New') {iterQuery}";
            }

            string requestUrl = $"/{organization}/{project}/_apis/wit/wiql?api-version=6.1-preview.2";
            var requestData = new { query };
            var content = new StringContent(JsonConvert.SerializeObject(requestData), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = httpClient.PostAsync(requestUrl, content).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var queryResult = JsonConvert.DeserializeObject<WorkItemQueryResult>(responseContent);
                return queryResult.WorkItems;
            }
            else
            {
                // Handle the API error response
                string errorResponse = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine($"API request failed with status code: {response.StatusCode}");
                Console.WriteLine($"Error response: {errorResponse}");
                return new List<WorkItemReference>();
            }
        }

        public WorkItemModel GetWorkItemById(int id)
        {
            HttpResponseMessage response = httpClient.GetAsync($"/{organization}/{project}/_apis/wit/workItems/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<WorkItemModel>(responseContent);
                return item;
            }
            else
            {
                // Handle the error response accordingly
                throw new Exception($"Failed to retrieve work item. Status code: {response.StatusCode}");
            }
        }

        public void UpdateCompletedAndRemainingWork(int id, double completedWork, double remainingWork, string state)
        {
            List<PatchOperation> operations = new List<PatchOperation>();
            // Construct the JSON payload for the patch request
            operations.Add(new PatchOperation
            {
                op = "Replace",
                path = "/fields/Microsoft.VSTS.Scheduling.CompletedWork",
                value = completedWork.ToString(),
            });
            operations.Add(new PatchOperation
            {
                op = "Replace",
                path = "/fields/Microsoft.VSTS.Scheduling.RemainingWork",
                value = remainingWork.ToString(),
            });
            if (remainingWork == 0 && shouldCloseTasksIfEffortsCompleted)
            {
                operations.Add(new PatchOperation
                {
                    op = "Replace",
                    path = "/fields/System.State",
                    value = "Closed",
                });
            }
            if (state == "New" && completedWork != 0)
            {
                operations.Add(new PatchOperation
                {
                    op = "Replace",
                    path = "/fields/System.State",
                    value = "Active",
                });
            }
            string jsonPayload = JsonConvert.SerializeObject(operations, Formatting.Indented);

            // Create the HTTP client and request
            using (HttpClient client = new HttpClient())
            {
                string requestUrl = $"{organization}/{project}/_apis/wit/workitems/{id}?api-version=6.1-preview.3";
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUrl)
                {
                    Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json-patch+json")
                };
                HttpResponseMessage response = httpClient.SendAsync(request).Result;

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Work item updated successfully
                    // Handle the response if necessary
                }
                else
                {
                    // Request failed, handle the error
                    string errorMessage = response.Content.ReadAsStringAsync().Result;
                    // Handle the error message
                }
            }
        }

        internal List<WorkItemUpdate> GetWorkItemUpdates(int id)
        {
            {
                HttpResponseMessage response = httpClient.GetAsync($"/{organization}/{project}/_apis/wit/workItems/{id}/updates").Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    var item = JsonConvert.DeserializeObject<WorkItemUpdateRoot>(responseContent);
                    return item.Value.ToList();
                }
                else
                {
                    // Handle the error response accordingly
                    throw new Exception($"Failed to retrieve work item. Status code: {response.StatusCode}");
                }
            }
        }
    }
}