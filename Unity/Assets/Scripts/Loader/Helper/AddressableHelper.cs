/*************************
文件:AddressableHelper.cs
作者:cocoa
创建时间:2023-03-18 15-25-04
描述：
************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ET
{
    public  static class AddressableHelper
    {
        private static Dictionary<string, AsyncOperationHandle<IList<UnityEngine.Object>>> labelToOpers 
                = new Dictionary<string, AsyncOperationHandle<IList<UnityEngine.Object>>>();
      
        public static  Dictionary<string, UnityEngine.Object> LoadBundlesByLabel(string label)
        {
            label = label.ToLower();
            
            if(labelToOpers.ContainsKey(label))
                Addressables.Release(labelToOpers[label]);
            
            Dictionary<string, UnityEngine.Object> objects = new Dictionary<string, UnityEngine.Object>();
            var oper = Addressables.LoadAssetsAsync<UnityEngine.Object>(label, null);
            labelToOpers.Add(label,oper);
            var assets =  oper.WaitForCompletion();
            
            foreach (UnityEngine.Object asset in assets)
            {
                Log.Debug(asset.name);
                objects.Add(asset.name, asset);
            }
            
            
            //Addressables.Release(oper);
         
            return objects;
        }

        public static void RelseBundlesByLabel(string label)
        {
            if (labelToOpers.ContainsKey(label))
            {
                Addressables.Release(labelToOpers[label]);
                labelToOpers.Remove(label);
            }
        }
    }
 
}
