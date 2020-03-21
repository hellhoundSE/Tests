using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace XUnitTestProject1.Controllers
{
    public class UsersInitTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;


        public UsersInitTests()
        {
            _server = ServerFactory.GetServerInstance();
            _client = _server.CreateClient();


            using (var scope = _server.Host.Services.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<LibraryContext>();

                _db.User.Add(new User
                {
                    IdUser = 1,
                    Email = "jd@pja.edu.pl",
                    Name = "Daniel",
                    Surname = "Jabłoński",
                    Login = "jd",
                    Password = "ASNDKWQOJRJOP!JO@JOP"
                });

                _db.SaveChanges();
            }
        }


        [Fact]
        public async Task GestUsers_200Ok()
        {
            //Arrange i Act
            var httpResponse = await _client.GetAsync($"{_client.BaseAddress.AbsoluteUri}api/users");

            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();

            var users = JsonConvert.DeserializeObject<IEnumerable<User>>(content);
            // using (var scope = _server.Host.Services.CreateScope())
            // {
            //     var _db = scope.ServiceProvider.GetRequiredService<LibraryContext>();
            //     Assert.True(_db.User.Any());
            // }

            Assert.True(users.Count() == 1);
            Assert.True(users.ElementAt(0).Login == "jd");
        }
        [Fact]
        public async Task GestUser_200Ok()
        {
            //Arrange i Act
            var httpResponse = await _client.GetAsync($"{_client.BaseAddress.AbsoluteUri}api/users/1");

            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();

            var users = JsonConvert.DeserializeObject<User>(content);
            // using (var scope = _server.Host.Services.CreateScope())
            // {
            //     var _db = scope.ServiceProvider.GetRequiredService<LibraryContext>();
            //     Assert.True(_db.User.Any());
            // }

            Assert.True(users.Name == "Daniel");
        }

        [Fact]
        public async Task AddUser_200Ok()
        {
            // Moze robie cos zle, ale czy nie jest potrzebne wysylanie ID usera, 
            //bo zawsze otrzymywalem blad ze user o takim id istnieje
            //troche zmienilem user repository zeby dzialalo teraz id tworza sie losowo
            //najgorsze rozwjazanie, to wiem
            var payload = JsonConvert.SerializeObject(new
            {
                login = "mylogin",
                password = "mypass",
                name = "rus",
                surname = "val",
                email = "my@mail.com"
            });


            HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
            var httpResponse = await _client.PostAsync($"{_client.BaseAddress.AbsoluteUri}api/users", c);

            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<User>(content);

            Assert.True(users.Name == "rus");
        }

    }
}
