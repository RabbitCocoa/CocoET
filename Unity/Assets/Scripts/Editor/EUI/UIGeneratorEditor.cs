using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ET;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

//UI生成器

public static class UIGeneratorEditor
{
    //模板文件路径
    public const string SCRIPT_Component_TEMPLATE_PATH = "Assets/Scripts/Editor/EUI/UIComponentTemplate.txt";
    public const string SCRIPT_System_TEMPLATE_PATH = "Assets/Scripts/Editor/EUI/UIComponentSystemTemplate.txt";
    public const string SCRIPT_Event_TEMPLATE_PATH = "Assets/Scripts/Editor/EUI/UIEventTemplate.txt";

    public const string SCRIPT_ComComponent_TEMPLATE_PATH = "Assets/Scripts/Editor/EUI/UIComComponentTemplate.txt";
    public const string SCRIPT_ComComponentSystem_TEMPLATE_PATH = "Assets/Scripts/Editor/EUI/UIComComponentSystemTemplate.txt";

    public const string SCRIPT_Component_TEMPLATE_OUT_PATH = "Assets/Scripts/Codes/ModelView/Client/Demo/UI/";
    public const string SCRIPT_SystemEvent_TEMPLATE_OUT_PATH = "Assets/Scripts/Codes/HotfixView/Client/Demo/UI/";

    public class UIFieldInfo
    {
        #region declaration

        public string fieldType;
        public string fieldName;

        #endregion

        #region initialization

        public string fieldPath;

        public bool isCommon; //是否为公共组件
        #endregion
        

        public override string ToString()
        {
            return $"{fieldType} {fieldName} => {fieldPath}";
        }
    }

    static string FullPath(this Transform tr)
    {
        string s = tr.gameObject.name;
        do
        {
            tr = tr.parent;
            if (tr == null)
                break;
            s = $"{tr.gameObject.name}/{s}";
        }
        while (tr != null);

        return s;
    }

    static string InitialLower(this string s)
    {
        StringBuilder sb = new StringBuilder(s);
        sb[0] = char.ToLower(sb[0]);
        return sb.ToString();
    }

    static string InitialHigher(this string s)
    {
        StringBuilder sb = new StringBuilder(s);
        sb[0] = char.ToUpper(sb[0]);
        return sb.ToString();
    }

    private static string GetFieldType(WidgeID w)
    {
        MonoBehaviour mono;
        if ((mono = w.GetComponents<WidgeID>().FirstOrDefault(x => x != w)) != null)
        {
            return mono.GetType().Name;
        }
        else if ((mono = w.GetComponent<Selectable>()) != null)
        {
            return mono.GetType().Name;
        }
        else if ((mono = w.GetComponent<Graphic>()) != null)
        {
            return mono.GetType().Name;
        }
        else
        {
            return "Transform";
        }
    }

    public static void _Gen(Transform tr, string path, List<UIFieldInfo> list)
    {
        if (tr == null)
            return;

        foreach (Transform c in tr)
        {
            string cPath = path == string.Empty? c.name : path + "/" + c.name;
            var w = c.GetComponent<WidgeID>();

            if (w && w.componentype == WidgeID.Componentype.Normal)
            {
                //Debug.Log("+" + cPath);
                string fname = c.name.InitialLower();
                var fieldInfo = list.Find(x => x.fieldName == fname);
                //重名情况
                if (fieldInfo != null)
                    fname = "_" + c.FullPath();
                list.Add(new UIFieldInfo() { fieldName = fname, fieldPath = cPath, fieldType = GetFieldType(w), });
            }
            else if (w && w.componentype == WidgeID.Componentype.CommonComponent)
            {
                //把自己也包含在里面
                string myFieldType = GetFieldType(w);
                string myName = c.name.InitialLower();
                list.Add(new UIFieldInfo() { fieldName = myName, fieldPath = cPath, fieldType = $"Common_{c.name.InitialHigher()}Component", isCommon = true});
                
                //先检查该组件是否存在
                string fileComponentDir = SCRIPT_Component_TEMPLATE_OUT_PATH + "Common";
                string fileSysDir = SCRIPT_SystemEvent_TEMPLATE_OUT_PATH + "Common";

                string fileComponentPath = fileComponentDir + $"/Common_{c.name.InitialHigher()}Component.cs";
                string fileSystemPath = fileSysDir + $"/Common_{c.name.InitialHigher()}ComponentSystem.cs";
              

                if (!Directory.Exists(fileComponentDir))
                    Directory.CreateDirectory(fileComponentDir);
                if (!Directory.Exists(fileSysDir))
                    Directory.CreateDirectory(fileSysDir);

                //删除原本的Component组件
                if (File.Exists(fileComponentPath))
                    File.Delete(fileComponentPath);

                //生成公共组件
                string componentCode = File.ReadAllText(SCRIPT_ComComponent_TEMPLATE_PATH);

                var fieldList = new List<UIFieldInfo>();
                _Gen(c, string.Empty, fieldList);

                StringBuilder sbFieldDef = new StringBuilder();
                StringBuilder sbFieldInit = new StringBuilder();



                
                sbFieldDef.AppendLine($"\t\tpublic {myFieldType} {myName};");
                sbFieldInit.AppendLine($"\t\t\tself.{myName} = self.transform.GetComponent<{myFieldType}>();");

                foreach (var field in fieldList)
                {
                    sbFieldDef.AppendLine($"\t\tpublic {field.fieldType} {field.fieldName};");
                    sbFieldInit.AppendLine(
                        $"\t\t\tself.{field.fieldName} = self.transform.Find(\"{field.fieldPath}\").GetComponent<{field.fieldType}>();");
                }

                string sComponent = componentCode
                        .Replace("{ROOT_UI_NAME}", $"Common_{c.name.InitialHigher()}Component")
                        .Replace("{UI_WIDGET_FIELD_LIST}", sbFieldDef.ToString())
                        .Replace("{FIELD_INITIALIZATION_LIST}", sbFieldInit.ToString())
                        .Replace("{DateTime}", DateTime.Now.ToString("yyyy-M-d HH:HH:mm:ss"));


                File.WriteAllText(fileComponentPath, sComponent, Encoding.UTF8);
                Debug.Log($"{fileComponentPath} 生成成功");
                if (!File.Exists(fileSystemPath))
                {
                    string systemCode = File.ReadAllText(SCRIPT_ComComponentSystem_TEMPLATE_PATH);
                    string sSystem = systemCode.Replace("{ROOT_UI_NAME}", $"Common_{c.name.InitialHigher()}Component")
                            .Replace("{DateTime}", DateTime.Now.ToString("yyyy-M-d HH:HH:mm:ss"));

                    File.WriteAllText(fileSystemPath, sSystem, Encoding.UTF8);
                    Debug.Log($"{fileSystemPath} 生成成功");
                }

                continue;
            }

            _Gen(c, cPath, list);
        }
    }

    //[MenuItem("GameObject/RayGame/生成UIPage脚本", priority = 0)]
    //命令规则 
    // UIXXX
    [MenuItem("GameObject/生成UI代码", false, -2)]
    public static void Gen()
    {
        UIPageID pageID = Selection.activeTransform.GetComponent<UIPageID>();
        if (pageID == null)
        {
            Debug.LogError("错误,生成物体必须挂载UIPageID脚本");
            return;
        }

        string componentCode = File.ReadAllText(SCRIPT_Component_TEMPLATE_PATH);
        string eventCode = File.ReadAllText(SCRIPT_Event_TEMPLATE_PATH);
        string systemCode = File.ReadAllText(SCRIPT_System_TEMPLATE_PATH);

        var fieldList = new List<UIFieldInfo>();
        _Gen(Selection.activeTransform, string.Empty, fieldList);

        StringBuilder sbFieldDef = new StringBuilder();
        StringBuilder sbFieldInit = new StringBuilder();
        StringBuilder sbEventInit = new StringBuilder();

        foreach (var field in fieldList)
        {
            sbFieldDef.AppendLine($"\t\tpublic {field.fieldType} {field.fieldName};");
            if (field.isCommon)
            {
                sbFieldInit.AppendLine($"\t\t\tself.{field.fieldName} = self.AddComponent<{field.fieldType},Transform>(self.Transform.Find(\"{field.fieldPath}\"));");
                sbEventInit.AppendLine($"\t\t\t{Selection.activeGameObject.name}.{field.fieldName}.OnStart();");
                //   self.UICloseComponent = self.AddComponent<Common_UICloseComponent>();
                //            //----初始化公共组件-----
                //-----------------------
            }
            else
                sbFieldInit.AppendLine($"\t\t\tself.{field.fieldName} = self.Transform.Find(\"{field.fieldPath}\").GetComponent<{field.fieldType}>();");
        }

        string sComponent = componentCode
                        .Replace("{ROOT_UI_NAME}", Selection.activeGameObject.name)
                        .Replace("{UI_WIDGET_FIELD_LIST}", sbFieldDef.ToString())
                        .Replace("{FIELD_INITIALIZATION_LIST}", sbFieldInit.ToString())
                        .Replace("{UI_TYPE}", pageID.windowtType.ToString())
                        .Replace("{DateTime}", DateTime.Now.ToString("yyyy-M-d HH:HH:mm:ss"))
                ;
        string sEvent = eventCode.Replace("{ROOT_UI_NAME}", Selection.activeGameObject.name)
                .Replace("{ComComponentStart}",sbEventInit.ToString())
                .Replace("{DateTime}", DateTime.Now.ToString("yyyy-M-d HH:HH:mm:ss"));
                
        string sSystemCode = systemCode.Replace("{ROOT_UI_NAME}", Selection.activeGameObject.name)
                .Replace("{DateTime}", DateTime.Now.ToString("yyyy-M-d HH:HH:mm:ss"));

        string dir = SCRIPT_Component_TEMPLATE_OUT_PATH + Selection.activeGameObject.name;
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string scriptPath = SCRIPT_Component_TEMPLATE_OUT_PATH + Selection.activeGameObject.name + "/" + Selection.activeGameObject.name +
                "Component.cs";
        if (File.Exists(scriptPath))
            File.Delete(scriptPath);
        File.WriteAllText(scriptPath, sComponent, Encoding.UTF8);
        Debug.Log($"自动生成{scriptPath}成功");

        dir = SCRIPT_SystemEvent_TEMPLATE_OUT_PATH + Selection.activeGameObject.name;
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string systemPath = SCRIPT_SystemEvent_TEMPLATE_OUT_PATH + Selection.activeGameObject.name + "/" + Selection.activeGameObject.name +
                "ComponentSystem.cs";
        // NB: 视图文件不能自动删除并重建，因为可能已经写了很多代码了
        if (!File.Exists(systemPath))
        {
            Debug.Log($"自动生成{systemPath}成功");
            File.WriteAllText(systemPath, sSystemCode, Encoding.UTF8);
        }

        string eventPath = SCRIPT_SystemEvent_TEMPLATE_OUT_PATH + Selection.activeGameObject.name + "/" + Selection.activeGameObject.name +
                "Event.cs";
        // NB: 视图文件不能自动删除并重建，因为可能已经写了很多代码了
        if (!File.Exists(eventPath))
        {
            Debug.Log($"自动生成{eventPath}成功");
            File.WriteAllText(eventPath, sEvent, Encoding.UTF8);
        }

        AssetDatabase.Refresh();
    }
}