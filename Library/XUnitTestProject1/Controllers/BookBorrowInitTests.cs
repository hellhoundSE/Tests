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
    public class BookBorrowInitTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;


        public BookBorrowInitTests()
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

                _db.Book.Add(new Book
                {
                    IdBook = 2,
                    Title = "SecondTittle",
                    IdAuthor = 1,
                    PublishYear = "2010",
                });

                _db.Book.Add(new Book
                {
                    IdBook = 1,
                    Title = "myTittle",
                    IdAuthor = 1,
                    PublishYear = "2002",
                });

                _db.BookBorrow.Add(new BookBorrow
                {
                    IdBookBorrow = 2,
                    IdBook = 2,
                    IdUser = 1,
                    Comments = "Before"
                }); 

                _db.SaveChanges();
            }
        }
        [Fact]
        public async Task UpdateBookBorrow_200Ok()
        {
            var payload = JsonConvert.SerializeObject(new
            {
                IdBookBorrow = 2,
                idUser = 1,
                IdBook = 2,
                Comments = "After"
            }) ;

            HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
            var httpResponse = await _client.PutAsync($"{_client.BaseAddress.AbsoluteUri}api/book-borrows/2", c);

            /*
            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            var bookBorrow = JsonConvert.DeserializeObject<BookBorrow>(content);
            */
            var _db = _server.Host.Services.GetRequiredService<LibraryContext>();
            var bb = _db.BookBorrow.FirstOrDefault(b => b.IdBookBorrow == 2);

            //Assert.True(bb.Comments == "Before");

            Assert.True(bb.Comments == "After");

        }

        [Fact]
        public async Task AddBookBorrow_200Ok()
        {

            var payload = JsonConvert.SerializeObject(new
            {
                idUser = "1",
                idBook = "1",
                comment = "MyTestComment",
            });


            HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
            var httpResponse = await _client.PostAsync($"{_client.BaseAddress.AbsoluteUri}api/book-borrows", c);

            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            var bookBorrow = JsonConvert.DeserializeObject<BookBorrow>(content);

            Assert.True(bookBorrow.Comments == "myComment");
        }

    }
}
