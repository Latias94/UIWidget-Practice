using System.Collections.Generic;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.widgets;
using UnityEngine;

public class MainPage : UIWidgetsSample.UIWidgetsSamplePanel
{
    protected override Widget createWidget()
    {
        return new WidgetsApp(
            home: new LoginPage(),
            pageRouteBuilder: pageRouteBuilder);
    }

    public class LoginPage : StatefulWidget
    {
        public override State createState()
        {
            return new LoginPageState();
        }
    }

    public class LoginPageState : State<LoginPage>
    {
        public override Widget build(BuildContext context)
        {
            return new Scaffold(
                body: new SafeArea(
                    child: new ListView(
                        padding: EdgeInsets.symmetric(horizontal: 24.0f),
                        children: new List<Widget>
                        {
                            new SizedBox(height: 80.0f),
                            new Column(
                                children: new List<Widget>
                                {
                                    new Text("图灵")
                                }),
                            new SizedBox(height: 120.0f),
                            new TextField(
                                decoration: new InputDecoration(
                                    filled: true,
                                    labelText: "用户名"
                                )
                            ),
                            new SizedBox(height: 12.0f),
                            new TextField(
                                decoration: new InputDecoration(
                                    filled: true,
                                    labelText: "密码"
                                ),
                                obscureText: true
                            ),
                            new ButtonBar(
                                children: new List<Widget>
                                {
                                    new FlatButton(
                                        child: new Text("取消"),
                                        onPressed: () => { Debug.Log("点击取消按钮"); }
                                    ),
                                    new RaisedButton(
                                        child: new Text("登录"),
                                        onPressed: () => { Debug.Log("点击登录按钮"); }
                                    )
                                }
                            )
                        }
                    )
                )
            );
        }
    }
}