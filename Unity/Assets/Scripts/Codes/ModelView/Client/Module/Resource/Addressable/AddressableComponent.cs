/*************************
文件:AddressableComponent.cs
作者:cocoa
创建时间:2022-12-25 20-40-17
描述：
************************/

using System;
using System.Collections.Generic;
using System.Linq;
using ET;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Object = UnityEngine.Object;

[FriendOfAttribute(typeof (AddressableComponent))]
public static class AddressableComponentSystem
{
    public class AddressableComponentAwakeSystem: AwakeSystem<AddressableComponent>
    {
        protected override void Awake(AddressableComponent self)
        {
            AddressableComponent.Instance = self;
        }
    }

    public class AddressableComponentDestroySystem: DestroySystem<AddressableComponent>
    {
        protected override void Destroy(AddressableComponent self)
        {
            foreach (var type in self._sublevelDic.Keys)
            {
                self.ReleaseSublevel(type);
            }

            self._sublevelDic.Clear();
            AddressableComponent.Instance = null;
        }
    }

    #region 初始化与更新管理

    #endregion

    #region 资源加载与场景跳转

    #region 同步版本

    public static T LoadAssetByPath<T>(this AddressableComponent self, string assetPath) where T : UnityEngine.Object
    {
        AsyncOperationHandle<T> assetHandle = Addressables.LoadAssetAsync<T>(assetPath);
        return assetHandle.WaitForCompletion();
    }

    public static GameObject InstiateAsset(this AddressableComponent self, string assetPath, Transform transform)
    {
        AsyncOperationHandle<GameObject> assetHandle = Addressables.InstantiateAsync(assetPath, transform);
        return assetHandle.WaitForCompletion();
    }

    /// <summary>
    /// 加载子资源 需要传一个父级类型 父级类型销毁时一并销毁
    /// </summary>
    /// <param name="type">父级类型</param>
    /// <param name="url">url</param>
    /// <typeparam name="T">类型</typeparam>
    /// <returns></returns>
    public static T LoadSubAsset<T>(this AddressableComponent self, string type, string url) where T : UnityEngine.Object
    {
        Dictionary<string, List<UnityEngine.Object>> dic;
        List<UnityEngine.Object> objects;
        UnityEngine.Object obj;
        try
        {
            if (self._sublevelDic.ContainsKey(type))
            {
                dic = self._sublevelDic[type];
                if (dic.ContainsKey(url))
                {
                    objects = dic[url];

                    if (objects.Count > 0)
                    {
                        obj = objects[0];
                    }
                    else
                    {
                        obj = self.LoadAssetByPath<T>(url);
                        objects.Add(obj);
                    }
                }
                else
                {
                    objects = new List<UnityEngine.Object>();
                    obj = self.LoadAssetByPath<T>(url);
                    objects.Add(obj);
                    if (!dic.ContainsKey(url))
                    {
                        dic.Add(url, objects);
                    }
                }
            }
            else
            {
                objects = new List<Object>();
                dic = new Dictionary<string, List<UnityEngine.Object>>();
                obj = self.LoadAssetByPath<T>(url);
                objects.Add(obj);
                dic.Add(url, objects);
                if (!self._sublevelDic.ContainsKey(type))
                {
                    self._sublevelDic.Add(type, dic);
                }
            }
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
            throw;
        }

        return (T)obj;
    }

    public static T LoadSubAsset<T, R>(this AddressableComponent self, string url) where T : UnityEngine.Object where R : Entity
    {
        return self.LoadSubAsset<T>(typeof (R).Name, url);
    }

    /// <summary>
    /// 仅用在实例化子级资源
    /// </summary>
    /// <param name="type">父级类型</param>
    /// <param name="url">url</param>
    /// <param name="transform">父级transform</param>
    /// <returns></returns>
    public static GameObject InstantiateSub(this AddressableComponent self, string type, string url, Transform transform)
    {
        Dictionary<string, List<UnityEngine.Object>> dic;
        List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
        UnityEngine.Object obj;

        try
        {
            if (self._sublevelDic.ContainsKey(type))
            {
                dic = self._sublevelDic[type];

                objects = dic[url];
                obj = self.InstiateAsset(url, transform);
                objects.Add(obj);
                if (!dic.ContainsKey(url))
                {
                    dic.Add(url, objects);
                }
            }
            else
            {
                dic = new Dictionary<string, List<UnityEngine.Object>>();
                obj = self.InstiateAsset(url, transform);
                objects.Add(obj);
                dic.Add(url, objects);
                if (!self._sublevelDic.ContainsKey(type))
                {
                    self._sublevelDic.Add(type, dic);
                }
            }
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
            throw;
        }

        return (GameObject)obj;
    }

    public static GameObject InstantiateSub<T>(this AddressableComponent self, string url, Transform transform) where T : Entity
    {
        return self.InstantiateSub(typeof (T).Name, url, transform);
    }

    #endregion

    #region 异步版本

    
    /// <summary>
    /// 通过资源路径(AddressableName)异步加载一个资源
    /// </summary>
    public static ETTask<T> LoadAssetByPathAsync<T>(this AddressableComponent self, string assetPath) where T : UnityEngine.Object
    {
        ETTask<T> tcs = ETTask<T>.Create(true);
        AsyncOperationHandle<T> assetHandle = Addressables.LoadAssetAsync<T>(assetPath);
        assetHandle.Completed += (handle) =>
        {
            tcs.SetResult(handle.Result);
            tcs = null;
        };
        return tcs.GetAwaiter();
    }

    // /// <summary>
    // /// 通过资源路径(AddressableName)异步加载多个资源
    // /// </summary>
    // public static async ETTask<List<T>> LoadAssetsByPathsAsync<T>(this AddressableComponent self, List<string> assetPath) where T : UnityEngine.Object
    // {
    //     List<ETTask> tasks = new List<ETTask>();
    //     List<T> results = new List<T>();
    //     for (int i = 0; i < assetPath.Count; i++)
    //     {
    //         ETTask tcs = ETTask.Create(true);
    //         AsyncOperationHandle<T> assetHandle = Addressables.LoadAssetAsync<T>(assetPath);
    //         assetHandle.Completed += (handle) =>
    //         {
    //             tcs.SetResult();
    //             tcs = null;
    //             results.Add(handle.Result);
    //         };
    //         tasks.Add(tcs);
    //     }
    //
    //     await ETTaskHelper.WaitAll(tasks);
    //     return results;
    // }

    public static ETTask<GameObject> InstiateAssetAsync(this AddressableComponent self, string assetPath, Transform transform)
    {
        ETTask<GameObject> tcs = ETTask<GameObject>.Create(true);
        AsyncOperationHandle<GameObject> assetHandle = Addressables.InstantiateAsync(assetPath, transform);
        assetHandle.Completed += (handle) =>
        {
            tcs.SetResult(handle.Result);
            tcs = null;
        };
        return tcs.GetAwaiter();
    }

    /// <summary>
    /// 通过一个Label异步加载多个资源
    /// </summary>
    /// <param name="label">想要加载的物体的Label</param>
    /// <param name="callBack">每加载完成一个执行回调</param>
    /// <returns>返回符合Label条件的所有资源</returns>
    public static ETTask<List<T>> LoadAssetsByLabelAsync<T>(this AddressableComponent self, string label, Action<T> callback = null)
            where T : UnityEngine.Object
    {
        ETTask<List<T>> tcs = ETTask<List<T>>.Create(true);
        AsyncOperationHandle<IList<T>> assetHandle = Addressables.LoadAssetsAsync<T>(label, callback);

        assetHandle.Completed += (handle) =>
        {
            tcs.SetResult(handle.Result.ToList<T>());
            tcs = null;
        };
        return tcs.GetAwaiter();
    }

    /// <summary>
    /// 加载子资源 需要传一个父级类型 父级类型销毁时一并销毁
    /// </summary>
    /// <param name="type">父级类型</param>
    /// <param name="url">url</param>
    /// <typeparam name="T">类型</typeparam>
    /// <returns></returns>
    public static async ETTask<T> LoadSubAssetAsync<T>(this AddressableComponent self, string type, string url) where T : UnityEngine.Object
    {
        Dictionary<string, List<UnityEngine.Object>> dic;
        List<UnityEngine.Object> objects;
        UnityEngine.Object obj;
        try
        {
            if (self._sublevelDic.ContainsKey(type))
            {
                dic = self._sublevelDic[type];
                if (dic.ContainsKey(url))
                {
                    objects = dic[url];

                    if (objects.Count > 0)
                    {
                        obj = objects[0];
                    }
                    else
                    {
                        obj = await self.LoadAssetByPathAsync<T>(url);
                        objects.Add(obj);
                    }
                }
                else
                {
                    objects = new List<UnityEngine.Object>();
                    obj = await self.LoadAssetByPathAsync<T>(url);
                    objects.Add(obj);
                    if (!dic.ContainsKey(url))
                    {
                        dic.Add(url, objects);
                    }
                }
            }
            else
            {
                objects = new List<Object>();
                dic = new Dictionary<string, List<UnityEngine.Object>>();
                obj = await self.LoadAssetByPathAsync<T>(url);
                objects.Add(obj);
                dic.Add(url, objects);
                if (!self._sublevelDic.ContainsKey(type))
                {
                    self._sublevelDic.Add(type, dic);
                }
            }
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
            throw;
        }

        return (T)obj;
    }

    public static async ETTask<T> LoadSubAssetAsync<T, R>(this AddressableComponent self, string url) where T : UnityEngine.Object where R : Entity
    {
        return await self.LoadSubAssetAsync<T>(typeof (R).Name, url);
    }

    /// <summary>
    /// 仅用在实例化子级资源
    /// </summary>
    /// <param name="type">父级类型</param>
    /// <param name="url">url</param>
    /// <param name="transform">父级transform</param>
    /// <returns></returns>
    public static async ETTask<GameObject> InstantiateSubAsync(this AddressableComponent self, string type, string url, Transform transform)
    {
        Dictionary<string, List<UnityEngine.Object>> dic;
        List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
        UnityEngine.Object obj;

        try
        {
            if (self._sublevelDic.ContainsKey(type))
            {
                dic = self._sublevelDic[type];

                objects = dic[url];
                obj = await self.InstiateAssetAsync(url, transform);
                objects.Add(obj);
                if (!dic.ContainsKey(url))
                {
                    dic.Add(url, objects);
                }
            }
            else
            {
                dic = new Dictionary<string, List<UnityEngine.Object>>();
                obj = await self.InstiateAssetAsync(url, transform);
                objects.Add(obj);
                dic.Add(url, objects);
                if (!self._sublevelDic.ContainsKey(type))
                {
                    self._sublevelDic.Add(type, dic);
                }
            }
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
            throw;
        }

        return (GameObject)obj;
    }

    public static async ETTask<GameObject> InstantiateSubAsync<T>(this AddressableComponent self, string url, Transform transform) where T : Entity
    {
        return await self.InstantiateSubAsync(typeof (T).Name, url, transform);
    }

    #endregion

    public static void ReleaseAsset<T>(this AddressableComponent self,T t)
    {
        Addressables.Release(t);
    }
    public static void ReleaseInstance(this AddressableComponent self,GameObject obj)
    {
        Addressables.ReleaseInstance(obj);
    }
    /// <summary>
    /// 释放资源
    /// </summary>
    /// <param name="type"></param>
    public static void ReleaseSublevel(this AddressableComponent self, string type)
    {
        Dictionary<string, List<UnityEngine.Object>> dic;
        self._sublevelDic.TryGetValue(type, out dic);

        if (dic == null)
        {
            return;
        }

        foreach (List<UnityEngine.Object> objects in dic.Values)
        {
            foreach (UnityEngine.Object obj in objects)
            {
                if (obj is GameObject gameObject)
                {
                    self.ReleaseInstance(gameObject);
                    Log.Debug($" Release GameObject = {gameObject.name}");

                    continue;
                }

                self.ReleaseAsset(obj);

                Log.Debug($" Release GameObject = {obj.name}");
            }
        }

        dic.Clear();

    }

    public static void ReleaseSublevel<T>(this AddressableComponent self) where T : Entity
    {
        self.ReleaseSublevel(typeof (T).Name);
    }

    /// <summary>
    /// 通过场景路径(AddressableName)异步加载场景
    /// </summary>
    /// /// <param name="scenePath">场景资源的路径(AddressableName)</param>
    /// <param name="sceneInstanceHandle">场景加载的句柄，卸载的时候用</param>
    /// <param name="activateOnLoad">加载后是否立刻激活，不等于SceneManager.SetActiveScene()，如果不激活会影响其它资源异步加载完成后的回调</param>
    /// <returns>返回SceneInstance数据，可以通过这个直接获取到</returns>
    public static ETTask<SceneInstance> LoadSceneByPathAsync(this AddressableComponent self, string scenePath,
    out AsyncOperationHandle<SceneInstance> sceneInstanceHandle,
    UnityEngine.SceneManagement.LoadSceneMode loadMode = UnityEngine.SceneManagement.LoadSceneMode.Single, bool activateOnLoad = true,
    int priority = 100)
    {
        ETTask<SceneInstance> tcs = ETTask<SceneInstance>.Create();
        sceneInstanceHandle = Addressables.LoadSceneAsync(scenePath, loadMode, activateOnLoad, priority);
        sceneInstanceHandle.Completed += (handle) =>
        {
            SceneInstance sceneInstance = handle.Result;
            tcs.SetResult(sceneInstance);
        };
        return tcs.GetAwaiter();
    }

    public static ETTask<SceneInstance> LoadSceneByPathAsync(this AddressableComponent self, string scenePath,
    UnityEngine.SceneManagement.LoadSceneMode loadMode = UnityEngine.SceneManagement.LoadSceneMode.Single, bool activateOnLoad = true,
    int priority = 100)
    {
        ETTask<SceneInstance> tcs = ETTask<SceneInstance>.Create();
        var sceneInstanceHandle = Addressables.LoadSceneAsync(scenePath, loadMode, activateOnLoad, priority);
        sceneInstanceHandle.Completed += (handle) =>
        {
            SceneInstance sceneInstance = handle.Result;
            tcs.SetResult(sceneInstance);
        };
        return tcs.GetAwaiter();
    }

    /// <summary>
    /// 激活加载的场景
    /// </summary>
    public static ETTask ActivateLoadScene(this AddressableComponent self, SceneInstance sceneInstance)
    {
        ETTask tcs = ETTask.Create();
        AsyncOperation asyncOperation = sceneInstance.ActivateAsync();
        asyncOperation.completed += (operation) => { tcs.SetResult(); };
        return tcs.GetAwaiter();
    }

    #endregion
}

[ComponentOf(typeof (Scene))]
public class AddressableComponent: Entity, IAwake, IDestroy
{
    public static AddressableComponent Instance { get; set; }

    public bool IsInitialize;

    //缓存子级资源
    public Dictionary<string, Dictionary<string, List<UnityEngine.Object>>>
            _sublevelDic = new Dictionary<string, Dictionary<string, List<UnityEngine.Object>>>();
}