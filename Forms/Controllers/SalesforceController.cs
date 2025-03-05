using Forms.DataAccess;
using Forms.DataAccess.Entities;
using Forms.Models.Salesforce;
using Forms.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

namespace Forms.Controllers
{
    public class SalesforceController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly FormsDbContext _context;

        public SalesforceController(IConfiguration config, IUserService userService, FormsDbContext context)
        {
            _config = config;

            _userService = userService;

            _context = context;
        }

        [HttpGet]
        public IActionResult CollectInformation()
        {
            return PartialView("~/Views/Account/CollectInformation.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> Subscribe(string firstName, string lastName, string phone)
        {
            var accessToken = await GetAccessToken();

            if(await CreateContact(firstName,lastName,phone,accessToken))
            {
                return RedirectToAction("Profile", "Account");
            }

            return RedirectToAction("CollectInformation");
        }

        private async Task<string?> CreateAccount(string accessToken)
        {
            using(var client = new HttpClient())
            {
                try
                {
                    var user = await GetUserFromCookies();

                    var account = new SalesforceAccount
                    {
                        Name = user.Username
                    };

                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                    var content = new StringContent(JsonConvert.SerializeObject(account), Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(_config["Salesforce:AccountUrl"], content);

                    var responseContent = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<SalesforceResponse>(responseContent);

                    if(result is not null && result.Errors.Count() == 0)
                    {
                        return result.Id;
                    }

                    return null;
                }
                catch
                {
                    return null;
                }
            }
        }

        private async Task<bool> CreateContact(string firstName, string lastName, string phone, string accessToken)
        {
            using(var client = new HttpClient())
            {
                try
                {
                    var accountId = await CreateAccount(accessToken);

                    if (accountId is not null)
                    {
                        var user = await GetUserFromCookies();

                        var contact = new SalesforceContact
                        {
                            Email = user.Email,
                            Phone = phone,
                            FirstName = firstName,
                            LastName = lastName,
                            AccountId = accountId
                        };

                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                        var content = new StringContent(JsonConvert.SerializeObject(contact), Encoding.UTF8, "application/json");

                        var response = await client.PostAsync(_config["Salesforce:ContactUrl"], content);

                        var responseContent = await response.Content.ReadAsStringAsync();

                        var result = JsonConvert.DeserializeObject<SalesforceResponse>(responseContent);

                        if (result is not null && result.Errors.Count() == 0)
                        {
                            return true;
                        }
                    }

                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        private async Task<string> GetAccessToken()
        {
            using(var client = new HttpClient())
            {
                var requestContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("client_id", "3MVG9k02hQhyUgQD3UADJtoBn.RDM1WUpP5UEh1iKlkTFiINi97ud6KRtDlzXEsO748_NC5M6aLfwRbALgJnw"),
                    new KeyValuePair<string, string>("client_secret", "6D4DB6629CE3C63F536CC850EA61A7CC91D7B357764F430CC16272D57539BBFD"),
                    new KeyValuePair<string, string>("username", "majopa@gmail.com"),
                    new KeyValuePair<string, string>("password", "Amazyaka000hrxwpTWub4o5rxqPCvRKPXTK1")
                });

                var response = await client.PostAsync("https://forms7-dev-ed.develop.my.salesforce.com/services/oauth2/token", requestContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error getting token: {responseContent}");
                }

                dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);
                return jsonResponse.access_token;
            }
        }

        private async Task<UserEntity> GetUserFromCookies()
        {
            var claimValue = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var userId = int.Parse(claimValue);

            var user = await _userService.GetUser(userId, _context);

            return user;
        }
    }
}
