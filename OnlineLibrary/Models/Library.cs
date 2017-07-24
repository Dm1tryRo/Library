using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;

namespace OnlineLibrary.Models
{
    public class Library
    {
        private SqlConnection sqlConnection;
        private DataTable getDataTable(string query)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["Library"].ConnectionString;
            sqlConnection = new SqlConnection(ConnectionString);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = new SqlCommand(query, sqlConnection);
            da.Fill(dt);
            return dt;
        }
        private void SetToLibrary(string query)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["Library"].ConnectionString;
            sqlConnection = new SqlConnection(ConnectionString);
            using (sqlConnection)
            {
                sqlConnection.Open();
                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.ExecuteNonQuery();
                sqlConnection.Close();
            }
        }

        #region GetBooks
        public List<Book> GetBooks()
        {
            List<Book> books = new List<Book>();
            String sql = @"SELECT * FROM Books";
            DataTable dt = getDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                var Book = new Book();
                Book.Id = (int)row["Id"];
                Book.Name = row["Name"].ToString();
                Book.Quantity = (int)row["Quantity"];
                books.Add(Book);
            }
            books = GetAuthors(books);
            return books;
        }
        public List<Author> GetBooks(List<Author> authors)
        {
            foreach(Author a in authors)
            {
                string sql = String.Format(@"SELECT * FROM BookAuthors WHERE AuthorId = {0}", a.Id);
                DataTable dt = getDataTable(sql);
                a.Books = new List<Book>();
                foreach (DataRow row in dt.Rows)
                    a.Books.Add(GetBookById(Convert.ToInt32(row["BookId"])));
            }
            return authors;
        }
        public List<Book> GetBooks(List<string> booksName)
        {
            List<Book> result = new List<Book>();
            if (booksName == null) booksName = new List<string>(0);
            foreach (string name in booksName)
            {
                string sql = String.Format(@"SELECT * FROM Books WHERE Name = N'{0}'", name);
                DataTable dt = getDataTable(sql);
                result.Add(new Book { Name = dt.Rows[0]["Name"].ToString(), Id = (int)dt.Rows[0]["Id"] });
            }
            return result;
        }
        public Book GetBookById(int id)
        {
            string sql = String.Format(@"SELECT * FROM Books WHERE Id = {0}", id);
            DataTable dt = getDataTable(sql);
            Book result = new Book
            {
                Id = id,
                Name = dt.Rows[0]["Name"].ToString(),
                Quantity = (int)dt.Rows[0]["Quantity"]
            };
            List<Book> tmp = new List<Book>();
            tmp.Add(result);
            result = GetAuthors(tmp)[0];
            return result;
        }
        #endregion

        #region GetAuthors
        public List<Author> GetAuthors(List<string> authorsName)
        {
            List<Author> result = new List<Author>();
            if (authorsName == null) authorsName = new List<string>(0);
            foreach(string name in authorsName)
            {
                string sql = String.Format(@"SELECT * FROM Authors WHERE Name = N'{0}'", name);
                DataTable dt = getDataTable(sql);
                result.Add(new Author { Name = dt.Rows[0]["Name"].ToString(), Id = (int)dt.Rows[0]["Id"] });
            }
            result = GetBooks(result);
            return result;
        }
        public List<Book> GetAuthors(List<Book> books)
        {
            foreach (Book b in books)
            {
                string sql = String.Format(@"SELECT * FROM BookAuthors WHERE BookId = {0}", b.Id);
                DataTable dt = getDataTable(sql);
                b.Authors = new List<Author>();
                foreach (DataRow row in dt.Rows)
                    b.Authors.Add(GetAuthorById(Convert.ToInt32(row["AuthorId"])));
            }
            return books;
        }
        public List<Author> GetAuthors()
        {
            string sql = @"SELECT * FROM Authors";
            DataTable dt = getDataTable(sql);
            List<Author> Authors = new List<Author>();
            foreach (DataRow row in dt.Rows)
                Authors.Add(new Author { Id = (int)row["Id"], Name = row["Name"].ToString() });
            Authors = GetBooks(Authors);
            return Authors;
        }
        public Author GetAuthorById(int id)
        {
            Author result = new Author();
            string sql = String.Format(@"SELECT * FROM Authors WHERE Id = {0}", id);
            DataTable dt = getDataTable(sql);
            result = new Author { Id = id, Name = dt.Rows[0]["Name"].ToString() };

            result.Books = new List<Book>();
            List<int> tmp = new List<int>();
            string query = String.Format(@"SELECT * FROM BookAuthors WHERE AuthorId = {0}", result.Id);
            DataTable dt1 = getDataTable(query);
            foreach (DataRow row in dt1.Rows)
                tmp.Add((int)row["BookId"]);

            foreach(int num in tmp)
            {
                string sql2 = String.Format(@"SELECT * FROM Books WHERE Id = {0}", num);
                DataTable dt2 = getDataTable(sql2);
                result.Books.Add(new Book
                {
                    Id = id,
                    Name = dt2.Rows[0]["Name"].ToString(),
                    Quantity = (int)dt2.Rows[0]["Quantity"]
                });
            }

            return result;
        }
        #endregion

        #region FindUser
        public User FindUser(Login log)
        {
            string sql = String.Format(@"SELECT * FROM Users Where Email=N'{0}' AND Password=N'{1}'", log.Email, log.Password);
            DataTable dt = getDataTable(sql);
            if (dt.Rows.Count == 0) return null;
            User result = new User
            {
                Id = (int)dt.Rows[0]["Id"],
                Email = dt.Rows[0]["Email"].ToString(),
                Name = dt.Rows[0]["Name"].ToString(),
                Password = dt.Rows[0]["Password"].ToString()               
            };
            return result;
        }
        public User FindUser(Register reg)
        {
            string sql = String.Format(@"SELECT * FROM Users Where Email=N'{0}' OR Name=N'{1}'", reg.Email, reg.Name);
            DataTable dt = getDataTable(sql);
            if (dt.Rows.Count == 0) return null;
            User result = new User
            {
                Id = (int)dt.Rows[0]["Id"],
                Email = dt.Rows[0]["Email"].ToString(),
                Name = dt.Rows[0]["Name"].ToString(),
                Password = dt.Rows[0]["Password"].ToString()
            };
            return result;
        }
        public User FindUserByName(string Name)
        {
            string sql = String.Format(@"SELECT * FROM Users Where Name=N'{0}'", Name);
            DataTable dt = getDataTable(sql);
            if (dt.Rows.Count == 0) return null;
            User result = new User
            {
                Id = (int)dt.Rows[0]["Id"],
                Email = dt.Rows[0]["Email"].ToString(),
                Name = dt.Rows[0]["Name"].ToString(),
                Password = dt.Rows[0]["Password"].ToString()
            };
            result.Takes = new List<Take>();
            result.Takes = GetUsersTakes(result);
            return result;
        }
        public User FindUserById(int id)
        {
            string sql = String.Format(@"SELECT * FROM Users Where Id={0}", id);
            DataTable dt = getDataTable(sql);
            if (dt.Rows.Count == 0) return null;
            User result = FindUserByName(dt.Rows[0]["Name"].ToString());
            //new User { Id = (int)dt.Rows[0]["Id"],Email = dt.Rows[0]["Email"].ToString(),Name = dt.Rows[0]["Name"].ToString(),Password = dt.Rows[0]["Password"].ToString()};
            return result;
        }
        #endregion

        public List<Take> GetUsersTakes(User usr)
        {
            List<Take> result = new List<Take>();
            string sql = String.Format(@"SELECT * FROM Takes WHERE UserId={0}", usr.Id);
            DataTable dt = getDataTable(sql);
            foreach(DataRow row in dt.Rows)
            {
                if (!(bool)row["isReturn"])
                {
                    result.Add(new Take
                    {
                        Id = (int)row["Id"],
                        Book = GetBookById((int)row["BookId"]),
                        Date = Convert.ToDateTime(row["Date"].ToString()),
                        isReturned = (bool)row["isReturn"],
                        User = usr
                    });
                }
            }
            return result;
        }
        public void AddNewUser(Register reg)
        {
            string sql = String.Format(@"INSERT INTO Users(Name, Email, Password) values(N'{0}',N'{1}',N'{2}')", reg.Name, reg.Email, reg.Password);
            SetToLibrary(sql);
        }
        public void AddBook(Book book)
        {
            string sql = @"SELECT Max(Id) FROM Books";
            string setSql = String.Format(@"INSERT INTO Books(Name, Quantity) VALUES(N'{0}', {1})", book.Name, book.Quantity);
            SetToLibrary(setSql);
            DataTable dt = getDataTable(sql);
            book.Id = (int)dt.Rows[0].ItemArray[0];
            SetRelationBookAuthor(book);
        }   
        public void EditBook(Book book)
        {
            string sql = String.Format(@"UPDATE Books SET Name=N'{0}', Quantity={1} WHERE Id={2}", book.Name, book.Quantity, book.Id);
            SetToLibrary(sql);
            DeleteRelationBookAuthor(book);
            SetRelationBookAuthor(book);
        }
        public void SetRelationBookAuthor(Book book)
        {
            foreach(Author a in book.Authors)
            {
                string sql = String.Format("INSERT INTO BookAuthors(BookId, AuthorId) VALUES({0},{1})", book.Id, a.Id);
                SetToLibrary(sql);
            }
        }
        private void DeleteRelationBookAuthor(Book book)
        {
            string sql = String.Format("DELETE FROM BookAuthors WHERE BookId={0}", book.Id);
            SetToLibrary(sql);
        }
        public void DeleteBook(Book book)
        {
            DeleteRelationBookAuthor(book);
            string sql = String.Format(@"DELETE FROM Books WHERE Id={0}", book.Id);
            SetToLibrary(sql);
        }
        public void TakeBook(int id, string UserName)
        {
            Book book = GetBookById(id);
            if (book.Quantity > 0)
            {
                book.Quantity--;
                string sql = String.Format(@"UPDATE Books SET Quantity={0} WHERE Id={1}", book.Quantity, book.Id);
                SetToLibrary(sql);

                User user = FindUserByName(UserName);
                DateTime date = DateTime.Now;
                string thisDate = date.ToString("yyyy-MM-dd HH:mm:ss");
                string sqlInsert = String.Format(@"INSERT INTO Takes(UserId, BookId, isReturn, Date) VALUES({0},{1},0,'{2}')", user.Id, id, thisDate);
                SetToLibrary(sqlInsert);
            }
        }
        public void ReturnBook(Take take)
        {
            take.Book.Quantity++;
            string sql = String.Format(@"UPDATE Takes SET isReturn=1 WHERE Id={0}", take.Id);
            SetToLibrary(sql);
            string query = String.Format(@"UPDATE Books SET Quantity={0} WHERE Id={1}", take.Book.Quantity, take.Book.Id);
            SetToLibrary(query);
        }
        public Take GetTakeById(int id)
        {
            Take result = new Take();
            string sql = String.Format(@"SELECT * FROM Takes WHERE Id={0}", id);
            DataTable dt = getDataTable(sql);
            result = new Take {
                Id = id,
                Book = GetBookById((int)dt.Rows[0]["BookId"]),
                Date = Convert.ToDateTime(dt.Rows[0]["Date"]),
                isReturned = (bool)dt.Rows[0]["isReturn"],
            };
            return result;
        }
        public void EditAuthor(Author author)
        {
            string sql = String.Format(@"UPDATE Authors SET Name=N'{0}' WHERE Id={1}", author.Name, author.Id);
            SetToLibrary(sql);
            DeleteRelationBookAuthor(author);
            SetRelationBookAuthor(author);
        }
        public void SetRelationBookAuthor(Author author)
        {
            foreach (Book a in author.Books)
            {
                string sql = String.Format("INSERT INTO BookAuthors(BookId, AuthorId) VALUES({0},{1})", a.Id, author.Id);
                SetToLibrary(sql);
            }
        }
        private void DeleteRelationBookAuthor(Author author)
        {
            string sql = String.Format("DELETE FROM BookAuthors WHERE AuthorId={0}", author.Id);
            SetToLibrary(sql);
        }
        public void AddAuthor(Author author)
        {
            string sql = @"SELECT Max(Id) FROM Authors";
            string setSql = String.Format(@"INSERT INTO Authors(Name) VALUES(N'{0}')", author.Name);
            SetToLibrary(setSql);
            DataTable dt = getDataTable(sql);
            author.Id = (int)dt.Rows[0].ItemArray[0];
            SetRelationBookAuthor(author);
        }
        public void DeleteAuthor(Author author)
        {
            DeleteRelationBookAuthor(author);
            string sql = String.Format(@"DELETE FROM Authors WHERE Id={0}", author.Id);
            SetToLibrary(sql);
        }
        public List<Take> GetTakesByBookId(int id)
        {
            List<Take> result = new List<Take>();
            string sql = String.Format(@"SELECT * FROM Takes WHERE BookId={0}", id);
            DataTable dt = getDataTable(sql);
            foreach(DataRow row in dt.Rows)
            {
                result.Add(new Take
                {
                    User = FindUserById((int)row["UserId"]),
                    Date = Convert.ToDateTime(row["Date"]),
                    isReturned = (bool)row["isReturn"]
                });
            }
            return result;
        }
    }
}