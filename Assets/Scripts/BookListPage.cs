using System.Collections.Generic;
using System.Net.Http;
using LitJson;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.ui;
//using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;
using TextStyle = Unity.UIWidgets.painting.TextStyle;

public class BookListPage : StatefulWidget
{
    public override State createState()
    {
        return new BookListState();
    }
}


public class BookListState : State<BookListPage>
{
    private NewBooksModel newBooksModel;
    private bool hasRefresh;

//    public BookListState()
//    {
    // 测试 json 读取
//        var text = Resources.Load<TextAsset>("test");
//        Debug.Log(text.text);
//        var books = JsonHelper.FromJson<NewBooksModel>(text.text);
//        Debug.Log(books.bookItems.Count);
//        newBooksModel = books;
//    }

    private void RequestBooks(int page, BuildContext context)
    {
        Debug.Log("Request book");
        if (page <= 0)
        {
            return;
        }

        string url = $"https://api.test.ituring.com.cn/api/Book?sort=new&page={page}&tab=all";
        HTTPHelper.instance.GetAsync<NewBooksModel>(url, response =>
        {
            if (response != null)
            {
                NewBooksModel result = (NewBooksModel) response;
                SetNewBooks(result, context);
            }
            else
            {
                Debug.Log("error occur");
            }
        });
    }

    private void SetNewBooks(NewBooksModel booksModel, BuildContext context)
    {
        using (WindowProvider.of(context).getScope())
        {
            setState(() => { newBooksModel = booksModel; });
        }
    }


    public override Widget build(BuildContext context)
    {
        if (!hasRefresh)
        {
            RequestBooks(2, context);
            // 不知道为什么会不断调用 build 函数...醉了
            setState(() => hasRefresh = true);
        }

        return BuildList(context);
    }

    private Widget BuildList(BuildContext context)
    {
        if (newBooksModel != null)
        {
            return ListView.builder(
                itemCount: newBooksModel.bookItems.Count,
                itemBuilder: (_context, index) => { return new BookCard(newBooksModel.bookItems[index]); }
            );
        }

        return new Text("无数据");
    }
}


public class BookCard : StatefulWidget
{
    private NewBooksModel.BookItem bookItem;

    public BookCard(NewBooksModel.BookItem item)
    {
        bookItem = item;
    }

    public override State createState()
    {
        return new BookCardState(bookItem);
    }
}

public class BookCardState : State<BookCard>
{
    private NewBooksModel.BookItem bookItem;

    public BookCardState(NewBooksModel.BookItem item)
    {
        bookItem = item;
    }

    public override Widget build(BuildContext context)
    {
        return new InkWell(
            onTap: () => ShowBookDetailPage(context),
            child: new Padding(
                padding: EdgeInsets.symmetric(horizontal: 16.0f, vertical: 5.0f),
                child: new Container(
                    height: 180f,
                    child: new Stack(
                        children: new List<Widget>
                        {
                            BuildCard(context),
                            BuildCover()
                        }
                    )
                )
            )
        );
    }

    private void ShowBookDetailPage(BuildContext context)
    {
        Navigator.push(context, new MaterialPageRoute(_ => new EmptyPageWithAppBar(bookItem.name)));
    }

    private Widget BuildCard(BuildContext context)
    {
        return new Container(
            child: new Card(
                color: Colors.cyan,
                child: new Padding(
                    padding: EdgeInsets.only(
                        top: 8.0f,
                        bottom: 80.0f,
                        left: 200.0f
                    ),
                    child: new ListTile(
                        title: new Text(bookItem.name),
                        subtitle: new Text(bookItem.authorNameString),
                        isThreeLine: true
                    )
                )
            )
        );
    }

    private Widget BuildCover()
    {
        string imgUrl = $"http://file.ituring.com.cn/SmallCover/{bookItem.coverKey}";

        return new Container(
            width: 180.0f,
            height: 180.0f,
            decoration: new BoxDecoration(
                image: new DecorationImage(
                    fit: BoxFit.contain,
                    image: new NetworkImage(imgUrl)
                )
            )
        );
    }
}