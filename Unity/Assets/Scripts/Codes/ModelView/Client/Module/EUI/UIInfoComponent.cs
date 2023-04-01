/*************************
文件:UIInfoComponent.cs
作者:cocoa
创建时间:2022-12-25 11-27-00
描述：
************************/

using System;
using System.Collections.Generic;
using ET;

[ComponentOf(typeof(Scene))]
public class UIInfoComponent : Entity,IAwake,IDestroy
{
    public static UIInfoComponent Instance { get; set; }
    public Dictionary<UIWidgeID, UIWindowType> idToWindowTypes = new Dictionary<UIWidgeID, UIWindowType>();
    public Dictionary<UIWidgeID, string> idToPaths = new Dictionary<UIWidgeID, string>();
    public Dictionary<Type, UIWidgeID> typeToIds = new Dictionary<Type, UIWidgeID>();
    
    
    
}
