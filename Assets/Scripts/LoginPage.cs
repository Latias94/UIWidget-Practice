using System.Collections.Generic;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.widgets;
using UnityEngine;

public class LoginPage : StatefulWidget
{
    public override State createState()
    {
        return new LoginPageState();
    }
}

public class LoginPageState : State<LoginPage>
{
    private readonly TextEditingController _usernameController = new TextEditingController();
    private readonly TextEditingController _passwordController = new TextEditingController();

    public override Widget build(BuildContext context)
    {
        return new Scaffold(
            body: new SafeArea(
                child: new ListView(
                    padding: EdgeInsets.symmetric(horizontal: 24.0f),
                    children: new List<Widget>
                    {
                        new SizedBox(height: 200.0f),
                        new TextField(
                            controller: _usernameController,
                            decoration: new InputDecoration(
                                filled: true,
                                labelText: "用户名"
                            )
                        ),
                        new SizedBox(height: 12.0f),
                        new TextField(
                            controller: _passwordController,
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
                                    child: new Text("关闭"),
                                    onPressed: () => Navigator.pop(context)
                                ),
                                new FlatButton(
                                    child: new Text("清空"),
                                    onPressed: () =>
                                    {
                                        _usernameController.clear();
                                        _passwordController.clear();
                                    }
                                ),
                                new RaisedButton(
                                    child: new Text("登录"),
                                    onPressed: () =>
                                    {
                                        // TODO: 登录逻辑
                                        Navigator.pop(context, "登陆成功");
                                    }
                                )
                            }
                        )
                    }
                )
            )
        );
    }
}