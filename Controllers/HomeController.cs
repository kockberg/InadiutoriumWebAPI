using InadiutoriumWebAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace InadiutoriumWebAPI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string startDate, string endDate)
        {
            ViewBag.Title = "Home Page";

            IEnumerable<CalendarDateModel> calendar = null;

            // No search has been done. Provide empty table.
            if (startDate == null || endDate == null)
            {
                return View(calendar);
            }

            try
            {
                // Parse dates from user's form feed.
                DateTime _startDate = DateTime.Parse(startDate).Date;
                DateTime _endDate = DateTime.Parse(endDate).Date;

                // Check that the start date is before end date.
                if (_endDate < _startDate)
                    return View(calendar);

                // List for queries.
                List<string> monthQueries = GetMonthQueries(_startDate, _endDate); 

                JArray array = new JArray();

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://calapi.inadiutorium.cz/api/v0/en/calendars/default/");

                    HttpResponseMessage result = new HttpResponseMessage();

                    // Loop through queries for each month.
                    for(int i = 1; i <= monthQueries.Count; i++)
                    {
                        result = GetMonthlyResponseMessage(ref calendar, _startDate, _endDate, monthQueries, array, client, i);
                    }
                }

                // Deserialize the result into the CalendarDateModel.
                calendar = JsonConvert.DeserializeObject<IList<CalendarDateModel>>(array.ToString());

                return View(calendar);
            }
            catch (Exception)
            {
                return View();
            }
        }

        /// <summary>
        /// Gets one month's HttpResponseMessage.
        /// </summary>
        /// <param name="calendar">Referenced calendar data model</param>
        /// <param name="startDate">Search start date from user feed</param>
        /// <param name="endDate">Search end date from user feed</param>
        /// <param name="monthQueries">List of monthQueries</param>
        /// <param name="array">JArray for JSON data</param>
        /// <param name="client">HttpClient</param>
        /// <param name="i">Indexer</param>
        /// <returns></returns>
        private HttpResponseMessage GetMonthlyResponseMessage(ref IEnumerable<CalendarDateModel> calendar, DateTime startDate, DateTime endDate, List<string> monthQueries, JArray array, HttpClient client, int monthNumber)
        {
            HttpResponseMessage result;
            string monthQuery = monthQueries[monthNumber - 1];
            Task<HttpResponseMessage> responseTask = client.GetAsync(monthQuery);
            responseTask.Wait();

            result = responseTask.Result;

            if (result.IsSuccessStatusCode)
            {
                Task<string> MonthString = result.Content.ReadAsStringAsync();
                MonthString.Wait();
                JArray MonthArray = JArray.Parse(MonthString.Result);

                // In case this month is the end month, remove possible dates that are after end date.
                int daysInThisMonth = MonthArray.Count;
                if (monthNumber == monthQueries.Count)
                    for (int j = endDate.Day; j < daysInThisMonth; j++)
                        MonthArray.Last.Remove();

                // In case this month is the start month, remove possible dates that are before start date.
                if (monthNumber == 1)
                    for (int j = 1; j < startDate.Day; j++)
                        MonthArray.First.Remove();

                // Merge the result with the original JArray.
                array.Merge(MonthArray);
            }
            else
            {
                calendar = null;
                ModelState.AddModelError(string.Empty, "Error getting data.");
            }

            return result;
        }

        /// <summary>
        /// Gets monthly queries as a list of strings.
        /// </summary>
        /// <param name="startDate">Start date from user feed</param>
        /// <param name="endDate">End date from user feed</param>
        /// <returns>List of strings of montly queries between start date and end date.</returns>
        private List<string> GetMonthQueries(DateTime startDate, DateTime endDate)
        {
            List<string> monthQueries = new List<string>();

            // Create the needed monthly searches.
            if (startDate.Year == endDate.Year)
            {
                for (int i = startDate.Month; i <= endDate.Month; i++)
                {
                    monthQueries.Add($"{startDate.Year}/{i}");
                }
            }
            else
            {
                int startMonth = startDate.Month;
                int endMonth = endDate.Month;

                for (int i = startDate.Year; i <= endDate.Year; i++)
                {
                    if (i == startDate.Year)
                    {
                        while (startMonth <= 12)
                        {
                            monthQueries.Add($"{startDate.Year}/{startMonth}");
                            startMonth++;
                        }
                    }
                    else if (i == endDate.Year)
                    {
                        for (int j = 1; j <= endMonth; j++)
                        {
                            monthQueries.Add($"{endDate.Year}/{j}");
                        }
                    }
                    else
                    {
                        for (int j = 1; j <= 12; j++)
                        {
                            monthQueries.Add($"{i}/{j}");
                        }
                    }
                }
            }

            return monthQueries;
        }
    }
}
