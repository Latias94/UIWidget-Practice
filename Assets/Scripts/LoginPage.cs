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
    private readonly TextEditingController emailController = new TextEditingController();
    private readonly TextEditingController passwordController = new TextEditingController();

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
                            controller: emailController,
                            decoration: new InputDecoration(
                                filled: true,
                                labelText: "邮箱"
                            )
                        ),
                        new SizedBox(height: 12.0f),
                        new TextField(
                            controller: passwordController,
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
                                        emailController.clear();
                                        passwordController.clear();
                                    }
                                ),
                                new RaisedButton(
                                    child: new Text("登录"),
                                    onPressed: () =>
                                    {
                                        // TODO: 接口有cors 代码仅供参考了 =-=
//                                        Login();
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

    private void Login()
    {
        string email = emailController.text;
        string password = passwordController.text;
        string url = "https://api.test.ituring.com.cn/api/account/token";
        LoginModel postJson = new LoginModel();
        postJson.email = email;
        postJson.email = password;
        HTTPHelper.instance.PostJsonAsync<UserTokenModel>(url, JsonHelper.ToJson(postJson), res =>
        {
            if (res != null)
            {
                UserTokenModel token = (UserTokenModel) res;
                Debug.Log(token.userId);
            }
            else
            {
                Debug.Log("error occur");
            }
        });
    }
}