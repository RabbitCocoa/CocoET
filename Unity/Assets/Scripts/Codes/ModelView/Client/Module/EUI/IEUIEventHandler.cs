/*************************
文件:IEUIEventHandler.cs
作者:cocoa
创建时间:2022-12-25 11-04-31
描述：
************************/

using ET.Client;

public interface IEUIEventHandler
{
    // Awake后调用
    public void OnStart(UIWindow uiWindow);
    
    //每次激活窗口时调用
    public void OnActive(UIWindow uiWindow);

    //隐藏窗口时调用
    public void OnHide(UIWindow uiWindow);

    //销毁时调用
    public void OnDestroy(UIWindow uiWindow);

    //隐藏和销毁时都会调用 在 OnHide 和 OnDestroy 前执行 
    public void OnHideOrDestroy(UIWindow uiWindow);
}
