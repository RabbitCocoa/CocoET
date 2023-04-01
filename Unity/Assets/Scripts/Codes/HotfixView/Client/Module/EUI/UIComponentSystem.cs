using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
    public class UIComponentAwakeSystem: AwakeSystem<UIComponent>
    {
        protected override void Awake(UIComponent self)
        {
            self.allWindows.Clear();
            self.showWindowStack.Clear();

            int length = Enum.GetNames(typeof (UIWidgeID)).Length;

            self.showWindowStack[UIWindowType.Normal] = new List<UIWidgeID>(length);
            self.showWindowStack[UIWindowType.Pop] = new List<UIWidgeID>(length);
            self.showWindowStack[UIWindowType.Fixed] = new List<UIWidgeID>(length);

            foreach (UIWindowType type in Enum.GetValues(typeof (UIWindowType)))
            {
                self.typeToRoots[type] = GameObject.Find($"/Global/UIRoot/{type.ToString()}").transform;
            }
        }
    }

    public class UIComponentDestroySystem: DestroySystem<UIComponent>
    {
        protected override void Destroy(UIComponent self)
        {
            self.Destroy();
        }
    }

    /// <summary>
    /// 管理Scene上的UI
    /// </summary>
    [FriendOf(typeof (UIComponent))]
    public static class UIComponentSystem
    {
        public static void Destroy(this UIComponent self)
        {
            self.UnloadAllWindow();
            self.Dispose();
        }
        public static void CloseAllWindow(this UIComponent self)
        {
            var windowTypes = Enum.GetNames(typeof (UIWindowType));
            for (int i = 0; i < windowTypes.Length; i++)
            {
                UIWindowType type = UIWindowType.Fixed;

                UIWindowType.TryParse(windowTypes[i], out type);

                var stacks = self.showWindowStack[type];
                for(int j = stacks.Count -1; j>=0;j--)
                {
                    self.ClosePage(stacks[j], false); //关闭同时会从栈中移除
                }

                stacks.Clear();
            }
        }

        public static void UnloadAllWindow(this UIComponent self)
        {
            self.CloseAllWindow();
            var keys = new List<UIWidgeID>(self.allWindows.Keys);
            foreach (var key in keys)
            {
                self.UnLoadWindow(key);
            }
        }

        private static UIWindow GetWindow(this UIComponent self, UIWidgeID widgeID)
        {
            if (!self.allWindows.ContainsKey(widgeID))
            {
                return null;
            }

            return self.allWindows[widgeID];
        }

        public static T GetWindow<T>(this UIComponent self) where T : Entity
        {
            Type t = typeof (T);
            UIWidgeID widgeID = UIWidgeID.None;
            widgeID = UIInfoComponent.Instance.GetWidgeID<T>();

            if (widgeID == UIWidgeID.None)
                return null;

            UIWindow window = self.GetWindow(widgeID);
            if (window == null)
            {
                Log.Error($"{widgeID.ToString()}尚未初始化,获取失败");
                return null;
            }

            return window.GetComponent<T>();
        }

        private static async ETTask<UIWindow> PreloadWindowAsync(this UIComponent self, UIWidgeID widgeID)
        {
            UIWindow window = self.GetWindow(widgeID);
            if (window == null)
            {
                //新建一个UIWindow
                string uipath = UIInfoComponent.Instance.GetUIPath(widgeID);
                //初始化 此处执行OnAwake
                window = self.AddChild<UIWindow>();
                window.UIWidgeID = widgeID;
                window.WindowType = UIInfoComponent.Instance.GetWindowType(widgeID);

                GameObject obj = await AddressableComponent.Instance.InstiateAssetAsync(uipath, null);
                window.GameObject = obj;

                //设置根节点
                window.SetRoot(self.typeToRoots[window.WindowType]);
                //设置次序
                window.SetLabinserting();

                //创建完毕后执行初始化

                UIEventComponent.Instance.GetEventHandler(widgeID).OnStart(window);
                window.GameObject.SetActive(false);
                self.allWindows[widgeID] = window;
            }

            return window;
        }

        private static UIWindow PreloadWindow(this UIComponent self, UIWidgeID widgeID)
        {
            UIWindow window = self.GetWindow(widgeID);
            if (window == null)
            {
                //新建一个UIWindow
                string uipath = UIInfoComponent.Instance.GetUIPath(widgeID);
                //初始化 此处执行OnAwake
                window = self.AddChild<UIWindow>();
                window.UIWidgeID = widgeID;
                window.WindowType = UIInfoComponent.Instance.GetWindowType(widgeID);

                GameObject obj = AddressableComponent.Instance.InstiateAsset(uipath, null);
                window.GameObject = obj;

                //设置根节点
                window.SetRoot(self.typeToRoots[window.WindowType]);
                //设置次序
                window.SetLabinserting();

                //创建完毕后执行初始化

                UIEventComponent.Instance.GetEventHandler(widgeID).OnStart(window);
                window.GameObject.SetActive(false);
                self.allWindows[widgeID] = window;
            }

            return window;
        }

        public static void PreLoadWindow<T>(this UIComponent self)
        {
            UIWidgeID widgeID = UIInfoComponent.Instance.GetWidgeID<T>();
            if (widgeID == UIWidgeID.None)
            {
                Log.Error($"{typeof (T).Name} 没有对应的WidgeID");
                return;
            }

            self.PreloadWindow(widgeID);
        }

        private static async ETTask ShowWindowAsync(this UIComponent self, UIWidgeID widgeID, bool closePrePage = true)
        {
            UIWindow window = self.GetWindow(widgeID);
            if (window == null)
            {
                window = await self.PreloadWindowAsync(widgeID);
            }

            if (window == null)
                return;

            if (window.IsShow)
                return;

            //关闭当前窗口 但不移除栈 只有调用ClosePage才会移除栈
            var curWindowStack = self.showWindowStack[window.WindowType];
            if (closePrePage)
            {
                self.CloseTopPage(window);
            }

            //把当前窗口加进栈 可重复添加
            curWindowStack.Add(widgeID);

            UIEventComponent.Instance.GetEventHandler(widgeID).OnActive(window);
            window.IsShow = true;

            window.GameObject.SetActive(true);
        }

        private static void ShowPreWindow(this UIComponent self, UIWindowType type, int preIndex)
        {
            if (preIndex < 0)
                return;
            var stacks = self.showWindowStack[type];
            var widgeID = stacks[preIndex];
            UIWindow window = self.GetWindow(widgeID);
            if (window == null || window.IsDisposed)
            {
                Log.Error($"前一个界面已经被移除");
                return;
            }

            if (window.IsShow)
                return;

            //只打开 而不加入栈

            UIEventComponent.Instance.GetEventHandler(widgeID).OnActive(window);
            window.IsShow = true;

            window.GameObject.SetActive(true);
        }

        private static void ShowWindow(this UIComponent self, UIWidgeID widgeID, bool closePrePage = true)
        {
            UIWindow window = self.GetWindow(widgeID);
            if (window == null)
            {
                window = self.PreloadWindow(widgeID);
            }

            if (window == null)
                return;
            //重复显示页面的情况会被isShow挡回去
            if (window.IsShow)
                return;

            //关闭当前窗口 但不移除栈 只有调用ClosePage才会移除栈
            if (closePrePage)
                self.CloseTopPage(window);

            var curWindowStack = self.showWindowStack[window.WindowType];

            //把当前窗口加进栈 可重复添加
            curWindowStack.Add(widgeID);

            UIEventComponent.Instance.GetEventHandler(widgeID).OnActive(window);
            window.IsShow = true;

            window.GameObject.SetActive(true);
        }

        //关闭栈顶页面
        private static UIWidgeID CloseTopPage(this UIComponent self, UIWindow curShowwindow)
        {
            var curWindowStack = self.showWindowStack[curShowwindow.WindowType];
            UIWindow firstWindow = null;
            if (curWindowStack.Count > 0)
            {
                firstWindow = self.GetWindow(curWindowStack[^1]); //倒数第一个 
                if (firstWindow.IsShow)
                {
                    UIEventComponent.Instance.GetEventHandler(firstWindow.UIWidgeID).OnHideOrDestroy(firstWindow);
                    UIEventComponent.Instance.GetEventHandler(firstWindow.UIWidgeID).OnHide(firstWindow);
                    firstWindow.IsShow = false;
                    firstWindow.GameObject.SetActive(false);
                }
            }

            return firstWindow?.UIWidgeID ?? UIWidgeID.None;
        }

        public static void ShowWindow<T>(this UIComponent self, bool closePrePage = true) where T : Entity
        {
            UIWidgeID widgeID = UIInfoComponent.Instance.GetWidgeID<T>();
            if (widgeID == UIWidgeID.None)
            {
                Log.Error($"{typeof (T).Name} 没有对应的WidgeID");
                return;
            }

            self.ShowWindow(widgeID, closePrePage);
        }

        public static async ETTask ShowWindowAsync<T>(this UIComponent self, bool closePrePage = true) where T : Entity
        {
            UIWidgeID widgeID = UIInfoComponent.Instance.GetWidgeID<T>();
            if (widgeID == UIWidgeID.None)
            {
                Log.Error($"{typeof (T).Name} 没有对应的WidgeID");
                await ETTask.CompletedTask;
            }

            await self.ShowWindowAsync(widgeID, closePrePage);
        }

        public static void UnLoadWindow(this UIComponent self, UIWidgeID widgeID)
        {
            UIWindow window = self.GetWindow(widgeID);
            if (window == null)
                return;
            if (window.IsShow)
            {
                // 会在close中移除栈
                self.ClosePage(widgeID, false);
            }

            var stack = self.showWindowStack[window.WindowType];
            //由于可能重复多次 所以需要多次移除
            while (stack.Contains(widgeID))
            {
                stack.Remove(widgeID);
            }

            //执行销毁事件
            UIEventComponent.Instance.GetEventHandler(widgeID).OnHideOrDestroy(window);
            UIEventComponent.Instance.GetEventHandler(widgeID).OnDestroy(window);

            AddressableComponent.Instance.ReleaseInstance(window.GameObject);

            self.allWindows.Remove(widgeID);
            window.Dispose();
        }

        public static void UnLoadWindow<T>(this UIComponent self)
        {
            UIWidgeID widgeID = UIInfoComponent.Instance.GetWidgeID<T>();
            if (widgeID == UIWidgeID.None)
            {
                Log.Error($"{typeof (T).Name} 没有对应的WidgeID");
                return;
            }

            self.UnLoadWindow(widgeID);
        }

        /// <summary>
        /// 关闭界面 如果该界面Show了两次 那么该方法只会关闭一次
        /// </summary>
        /// <param name="self"></param>
        /// <param name="widgeID">窗口ID</param>
        /// <param name="showPrePage">是否显示前一个界面</param>
        public static void ClosePage(this UIComponent self, UIWidgeID widgeID, bool showPrePage = true)
        {
            UIWindow window = self.GetWindow(widgeID);
            if (window == null)
            {
                return;
            }

            if (!window.IsShow)
                return;
            window.IsShow = false;
            //执行关闭事件
            UIEventComponent.Instance.GetEventHandler(widgeID).OnHideOrDestroy(window);
            UIEventComponent.Instance.GetEventHandler(widgeID).OnHide(window);
            //关闭显示界面
            window.GameObject.SetActive(false);
            //从栈中移除 从后面找
            var stacks = self.showWindowStack[window.WindowType];
            int index = stacks.LastIndexOf(widgeID);

            var preIndex = index > 0? index - 1 : -1;

            if (preIndex != -1 && showPrePage)
            {
                self.ShowPreWindow(window.WindowType, preIndex);
            }

            stacks.RemoveAt(index);
        }

        public static void ClosePage<T>(this UIComponent self, bool showPrePage = true)
        {
            UIWidgeID widgeID = UIInfoComponent.Instance.GetWidgeID<T>();
            if (widgeID == UIWidgeID.None)
            {
                Log.Error($"{typeof (T).Name} 没有对应的WidgeID");
                return;
            }

            self.ClosePage(widgeID, showPrePage);
        }
    }
}