using System.Collections.Generic;


public class NewBooksModel
{
    public List<BookItem> bookItems;

    public Pagination pagination;
//    public List<object> banners ;

    public class BookEditionPrice
    {
        public string name;
        public string key;
    }

    public class BookItem
    {
        public List<Author> authors;
        public List<Translator> translators;
        public string authorNameString;
        public string translatorNameString;
        public int id;
        public string coverKey;
        public string name;
        public string isbn;
        public string @abstract;
        public List<BookEditionPrice> bookEditionPrices;
    }

    public class Pagination
    {
        public int pageCount;
        public int totalItemCount;
        public int pageNumber;
        public bool hasPreviousPage;
        public bool hasNextPage;
        public bool isFirstPage;
        public bool isLastPage;
    }

    public class Author
    {
        public string name;
    }

    public class Translator
    {
        public string name;
    }
}

public class LoginModel
{
    public string email;
    public string password;
}

public class UserTokenModel
{
    public string accessToken;
    public string refreshToken;
    public string userId;
}