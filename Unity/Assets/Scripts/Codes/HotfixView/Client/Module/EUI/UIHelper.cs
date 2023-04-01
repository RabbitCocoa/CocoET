/*************************
文件:UIHelper.cs
作者:cocoa
创建时间:2022-12-30 17-45-01
描述：
************************/

using System;
using UnityEngine.UI;

namespace ET.Client
{
    public static class UIHelper
    {
        public static void AddListenerAsyncWithoutRepeated(this Button button, Func<ETTask> action)
        {
            button.onClick.RemoveAllListeners();

            async ETTask clickActionAsync()
            {
                await action();
            }

            button.onClick.AddListener(() =>
            {
                if (UIEventComponent.Instance == null)
                {
                    return;
                }

                clickActionAsync().Coroutine();
            });
        }

        public static async ETTask ShowWindowAysnc<T>(this Scene scene,bool ClosePrePage = true) where T : Entity
        {
            await scene.GetComponent<UIComponent>()?.ShowWindowAsync<T>(ClosePrePage);
        }

        public static void ClosePage<T>(this Scene scene,bool ShowPrePage) where T : Entity
        {
            scene.GetComponent<UIComponent>()?.ClosePage<T>(ShowPrePage);
        }

        public static void UnLoadWindow<T>(this Scene scene) where T : Entity
        {
            scene.GetComponent<UIComponent>()?.UnLoadWindow<T>();
        }
    }
}