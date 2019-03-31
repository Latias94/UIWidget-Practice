using Unity.UIWidgets.material;
using Unity.UIWidgets.widgets;
using UnityEngine;

public class EmptyPage : StatelessWidget
{
    private string title;

    public EmptyPage(string title)
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

public class EmptyPageWithAppBar : StatelessWidget
{
    private string title;

    public EmptyPageWithAppBar(string title)
    {
        this.title = title;
    }

    public override Widget build(BuildContext context)
    {
        return new Scaffold(
            appBar: new AppBar(
                title: new Text(title)),
            body: new Container(
                color: Colors.white,
                child: new Center(
                    child: new Text(title)
                )
            )
        );
    }
}

public class EnterPage : StatelessWidget
{
    public override Widget build(BuildContext context)
    {
        return new Center(child: new RaisedButton(
                child: new Text("进入首页"),
                elevation: 5.0f,
                onPressed: () =>
                {
                    StopAnimating();
                    Navigator.push(context, new MaterialPageRoute(_ => new HomePage()));
                }
            )
        );
    }

    private void StopAnimating()
    {
        GameObject miraikomachi = GameObject.Find("miraikomachi");
        miraikomachi.SetActive(false);
    }
}