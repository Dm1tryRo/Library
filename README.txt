OnlineLibrary is a library simulator application. It stores information about books, authors and users.
In order to add or remove a book / author, the user must log in. You also need to register to take the book.
Unregistered users can view detailed information about the book / author and see a list of all books and a list of available books.
For registration requires unique name, password and e-mail.

Project structure:
The project is based on the MVC pattern. The main logic is in the class of Librari, in it all the work with the database is registered. 
HomeController uses the librari to build a connection between the model and the view.
Classes user, book and author are supportive.

