﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ET
{
	public class CodeLoader: Singleton<CodeLoader>
	{
		private Assembly model;

		private const string modelDll = "Model.dll";
		private const string modelPDB = "Model.pdb";
		private const string hotfixDll = "Hotfix.dll";
		private const string hotfixPDB = "Hotfix.pdb";
		
		private const string hotfixLabel = "hotfix";
	
		public void Start()
		{
			Log.Debug("开始Loader");
			if (Define.EnableCodes)
			{
				GlobalConfig globalConfig = Resources.Load<GlobalConfig>("GlobalConfig");
				if (globalConfig.CodeMode != CodeMode.ClientServer)
				{
					throw new Exception("ENABLE_CODES mode must use ClientServer code mode!");
				}
				
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				Dictionary<string, Type> types = AssemblyHelper.GetAssemblyTypes(assemblies);
				EventSystem.Instance.Add(types);
				foreach (Assembly ass in assemblies)
				{
					string name = ass.GetName().Name;
					if (name == "Unity.Model.Codes")
					{
						this.model = ass;
					}
				}
			}
			else
			{
				byte[] assBytes;
				byte[] pdbBytes;
				if (!Define.IsEditor)
				{
					Dictionary<string, UnityEngine.Object> dictionary = AddressableHelper.LoadBundlesByLabel(hotfixLabel); //AssetsBundleHelper.LoadBundle("code.unity3d");

			
					assBytes = ((TextAsset)dictionary[modelDll]).bytes;
					pdbBytes = ((TextAsset)dictionary[modelPDB]).bytes;
				
					AddressableHelper.RelseBundlesByLabel(hotfixLabel);

					
					if (Define.EnableIL2CPP)
					{
						HybridCLRHelper.Load();
					}
				}
				else
				{
					assBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Model.dll"));
					pdbBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Model.pdb"));
				}
			
				this.model = Assembly.Load(assBytes, pdbBytes);
				this.LoadHotfix();
				
				
			}
			
			IStaticMethod start = new StaticMethod(this.model, "ET.Entry", "Start");
			Log.Debug(start.ToString());
			start.Run();
		}

		// 热重载调用该方法
		public void LoadHotfix()
		{
			byte[] assBytes;
			byte[] pdbBytes;
			if (!Define.IsEditor)
			{
				Dictionary<string, UnityEngine.Object> dictionary = AddressableHelper.LoadBundlesByLabel(hotfixLabel);
				assBytes = ((TextAsset)dictionary[hotfixDll]).bytes;
				pdbBytes = ((TextAsset)dictionary[hotfixPDB]).bytes;
				AddressableHelper.RelseBundlesByLabel(hotfixLabel);

			}
			else
			{
				// 傻屌Unity在这里搞了个傻逼优化，认为同一个路径的dll，返回的程序集就一样。所以这里每次编译都要随机名字
				string[] logicFiles = Directory.GetFiles(Define.BuildOutputDir, "Hotfix_*.dll");
				if (logicFiles.Length != 1)
				{
					throw new Exception("Logic dll count != 1");
				}
				string logicName = Path.GetFileNameWithoutExtension(logicFiles[0]);
				assBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, $"{logicName}.dll"));
				pdbBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, $"{logicName}.pdb"));
			}

			Assembly hotfixAssembly = Assembly.Load(assBytes, pdbBytes);
			
			Dictionary<string, Type> types = AssemblyHelper.GetAssemblyTypes(typeof (Game).Assembly, typeof(Init).Assembly, this.model, hotfixAssembly);
			
			EventSystem.Instance.Add(types);
		}
	}
}