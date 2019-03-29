using Unity.UIWidgets.material;
using Unity.UIWidgets.widgets;

public class BookDetailPage : StatelessWidget
{
    private string title;

    public BookDetailPage(string title)
    {
        this.title = title;
    }

    public override Widget build(BuildContext context)
    {
        return new Scaffold(
            body: new Container(
                color: Colors.white,
                child: new Center(
                    child: new Text(title)
                )
            ));
    }
}