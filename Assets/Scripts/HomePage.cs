using System;
using System.Collections.Generic;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.widgets;
using UnityEngine;
using Color = Unity.UIWidgets.ui.Color;

public class HomePage : StatefulWidget
{
    public HomePage(Key key = null) : base(key)
    {
    }

    public override State createState()
    {
        return new HomePageState();
    }
}

public class HomePageState : State<HomePage>
{
    private const string Title = "UIWidgets-Practice";

    private GlobalKey<ScaffoldState> _scaffoldKey = GlobalKey<ScaffoldState>.key();

//    List<NavigationIconView> navigationViews;
    private int _selectedIndex;
    PageController _pageController;

    public HomePageState()
    {
        _selectedIndex = 0;
    }

    public override void initState()
    {
        base.initState();
        _pageController = new PageController();
    }

    public override Widget build(BuildContext context)
    {
//        new Scaffold(
//            Key key,
//            this.appBar,//标题栏
//            this.body,//内容
//            this.floatingActionButton,//悬浮按钮
//            this.persistentFooterButtons,底部持久化现实按钮
//            this.drawer,//侧滑菜单左
//            this.endDrawer,//侧滑菜单右
//            this.bottomNavigationBar,//底部导航
//            this.backgroundColor,//背景颜色
//            this.resizeToAvoidBottomPadding: true,//自动适应底部padding
//            this.primary: true,使用primary主色
//        )
        return new Scaffold(
            key: _scaffoldKey,
            appBar: new AppBar(
                title: new Center(
                    child: new Text(Title)
                ),
                leading: new IconButton(
                    tooltip: "菜单",
                    icon: new Icon(Icons.menu, color: Color.white),
                    onPressed: DisplayDrawer
                ),
                actions: new List<Widget>
                {
                    new IconButton(
                        icon: new Icon(Icons.search),
                        tooltip: "搜索",
                        onPressed: () => { Debug.Log("搜索"); }
                    ),
                    new IconButton(
                        icon: new Icon(Icons.filter),
                        tooltip: "过滤",
                        onPressed: () => { Debug.Log("过滤"); }
                    )
                }
            ),
//            body: new Padding(
//                padding: EdgeInsets.all(16.0f),
//                child: new Center(
//                    child: new Text("You did it!")
//                )
//            ),
//            body: buildBody(),
            body: new PageView(
                children: new List<Widget>
                {
                    new BookDetailPage("首页"),
                    new BookDetailPage("书籍"),
                    new BookDetailPage("观点")
                },
                onPageChanged: OnPageChanged,
                controller: _pageController
            ),
            drawer: new Drawer(
                child: new ListView(
                    padding: EdgeInsets.zero,
                    children: new List<Widget>
                    {
                        // UIWidget 还没实现 UserAccountsDrawerHeader
//                        new UserAccountsDrawerHeader(),
                        new DrawerHeader(
                            decoration: new BoxDecoration(
                                image: new DecorationImage(
                                    image: new AssetImage("unity-black"),
                                    fit: BoxFit.contain)
                            )),
                        new ListTile(
                            leading: new Icon(Icons.account_circle),
                            title: new Text("登录"),
                            onTap: () =>
                            {
                                // 关闭菜单
                                // When a user opens the Drawer, Flutter adds the drawer to the navigation stack under
                                // the hood. Therefore, to close the drawer, we can call Navigator.pop(context).
                                Navigator.of(context).pop();
                                // 相比flutter用的async await，UIWidgets 用的是promise
                                Navigator.push(context, new MaterialPageRoute(_ => new LoginPage()))
                                    .Then(newValue => DisplaySnackbar("登录成功"));
                            }
                        ),
                        new Divider(height: 2.0f),
                        new ListTile(
                            leading: new Icon(Icons.account_balance_wallet),
                            title: new Text("注册"),
                            onTap: () => { }
                        ),
                        new Divider(height: 2.0f),
                        new ListTile(
                            leading: new Icon(Icons.accessibility),
                            title: new Text("balabala"),
                            onTap: () => { }
                        )
                    }
                )
            ),
            bottomNavigationBar: new BottomNavigationBar(
                items: new List<BottomNavigationBarItem>
                {
                    new BottomNavigationBarItem(
                        icon: new Icon(Icons.home)
//                        title: new Text("首页")
                    ),
                    new BottomNavigationBarItem(
                        icon: new Icon(Icons.book)
//                        title: new Text("图书")
                    ),
                    new BottomNavigationBarItem(
                        icon: new Icon(Icons.play_circle_filled)
//                        title: new Text("观点")
                    )
                },
                fixedColor: Colors.blue,
                currentIndex: _selectedIndex,
                onTap: NavigationTapped
            )
//            floatingActionButton: new FloatingActionButton(
//                backgroundColor: Colors.redAccent,
//                child: new Icon(Icons.add_alert),
//                onPressed: () => { Debug.Log("点击悬浮按钮"); }
//            )
        );
    }

    private void OnPageChanged(int index)
    {
        setState(() => { _selectedIndex = index; });
    }

    // https://stackoverflow.com/questions/51304568/scaffold-of-called-with-a-context-that-does-not-contain-a-scaffold/51304732
    private void DisplayDrawer()
    {
        _scaffoldKey.currentState.openDrawer();
    }

    private void DisplaySnackbar(string text)
    {
        SnackBar snackBar = new SnackBar(null, new Text(text));
        _scaffoldKey.currentState.showSnackBar(snackBar);
    }

    private void NavigationTapped(int page)
    {
        // Animating to the page.
        // You can use whatever duration and curve you like
        _pageController.animateToPage(page,
            duration: new TimeSpan(300), curve: Curves.ease);
    }
}