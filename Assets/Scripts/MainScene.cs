using System.Collections.Generic;
using UIWidgetsSample;
using Unity.UIWidgets.material;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;

public class MainScene : UIWidgetsSamplePanel
{
    protected override Widget createWidget()
    {
        return new MaterialApp(
            showPerformanceOverlay: false,
            initialRoute: "/",
//            textStyle: new TextStyle(fontSize: 24),
//            pageRouteBuilder: pageRouteBuilder,
            routes: new Dictionary<string, WidgetBuilder>
            {
                {"/", context => new HomePage()},
                {"/login", context => new LoginPage()}
            }
        );
    }

    protected override void Awake()
    {
        base.Awake();
        // 加载了图标才会显示
        FontManager.instance.addFont(Resources.Load<Font>(path: "MaterialIcons-Regular"), "Material Icons");
    }

    // 页面切换 缓入缓出
    protected override PageRouteFactory pageRouteBuilder
    {
        get
        {
            return (settings, builder) =>
                new PageRouteBuilder(
                    settings,
                    (context, animation, secondaryAnimation) => builder(context),
                    (context, animation, secondaryAnimation, child) =>
                        new _FadeUpwardsPageTransition(
                            routeAnimation: animation,
                            child: child
                        )
                );
        }
    }
}