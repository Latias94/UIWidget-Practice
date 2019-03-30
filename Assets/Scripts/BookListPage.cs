using System.Collections.Generic;
using System.Net.Http;
using LitJson;
using Unity.UIWidgets.material;
using Unity.UIWidgets.widgets;
using UnityEngine;

public class BookListPage : StatefulWidget
{
    public override State createState()
    {
        return new BookListState();
    }
}


public class BookListState : State<BookListPage>
{
    private NewBooks newBooks;

    public BookListState()
    {
        // 测试 json 读取
//        var text = Resources.Load<TextAsset>("test");
//        Debug.Log(text.text);
//        var books = JsonHelper.FromJson<NewBooks>(text.text);
//        Debug.Log(books.bookItems.Count);
//        RequestBooks(1);
    }

    private void RequestBooks(int page)
    {
        if (page <= 0)
        {
            return;
        }

        string url = $"https://api.test.ituring.com.cn/api/Book?sort=new&page={page}&tab=all";
        HTTPHelper.instance.GetAsync<NewBooks>(url, response =>
        {
            if (response != null)
            {
                NewBooks result = (NewBooks) response;
                Debug.Log(result.bookItems.Count);
                setState(() => { newBooks = result; });
            }
            else
            {
                Debug.Log("error occur");
            }
        });
    }


    public override Widget build(BuildContext context)
    {
        return BuildList(context);
    }

    private ListView BuildList(BuildContext context)
    {
        if (newBooks != null)
        {
            return ListView.builder(
                // Must have an item count equal to the number of items!
                itemCount: newBooks.bookItems.Count,
                // A callback that will return a widget.
                itemBuilder: (_context, index) =>
                {
                    // In our case, a DogCard for each doggo.
                    return new BookCard(newBooks.bookItems[index]);
                }
            );
        }

        return new ListView(children: new List<Widget>
        {
            new FlatButton(
                child: new Text("刷新"),
                color: Colors.yellow,
                onPressed: () => { RequestBooks(1); }
            )
        });
    }
}

public class BookCard : StatelessWidget
{
    private NewBooks.BookItem bookItem;

    public BookCard(NewBooks.BookItem item)
    {
        bookItem = item;
    }

    public override Widget build(BuildContext context)
    {
        return new Text($"{bookItem.name}");
    }
}