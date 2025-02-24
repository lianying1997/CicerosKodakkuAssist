using System;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Script;
using KodakkuAssist.Module.Draw;
using Dalamud.Utility.Numerics;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;
using Dalamud.Memory.Exceptions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using static Dalamud.Interface.Utility.Raii.ImRaii;
using KodakkuAssist.Module.GameOperate;
using Newtonsoft.Json.Linq;

namespace CicerosKodakkuAssist.Pandemonium.Normal;

[ScriptType(name:"Abyssos: The Seventh Circle 万魔殿 炼净之狱3",
    territorys:[1085],
    guid:"073ca5d8-9a34-41ff-8757-c3862c465be0",
    version:"0.0.0.9",
    author:"Cicero 灵视",
    note:"A script for Abyssos: The Seventh Circle.\n万魔殿 炼净之狱3的脚本。")]

public class P7N
{
    
    [UserSetting("启用文本提示")]
    public bool Enable_Text_Prompts { get; set; } = true;
    
    [UserSetting("文本提示语言")]
    public Languages_Of_Text_Prompts Language_Of_Text_Prompts { get; set; }
    
    [UserSetting("启用开发者模式")]
    public bool Enable_Developer_Mode { get; set; } = false;
    
    volatile bool bladePromptsHasBeenSent;

    public enum Languages_Of_Text_Prompts {
        
        Simplified_Chinese_简体中文,
        English_英文
        
    }
    
    public void Init(ScriptAccessory accessory) {

        bladePromptsHasBeenSent=false;

    }
    
    [ScriptMethod(name:"Bough of Attis (Front) 阿提斯的巨枝 (前)",
        eventType:EventTypeEnum.StartCasting,
        eventCondition:["ActionId:30714"])]
    
    public void Bough_of_Attis_Front_阿提斯的巨枝_前(Event @event, ScriptAccessory accessory) {

        if(!parseObjectId(@event["SourceId"], out var sourceId)) {
            
            return;
            
        }
        
        var currentProperty=accessory.Data.GetDefaultDrawProperties();
        
        currentProperty.Owner=Convert.ToUInt32(@event["SourceId"],16);
        currentProperty.Scale=new(19);
        currentProperty.DestoryAt=7700;
        currentProperty.Color=accessory.Data.DefaultDangerColor;
        
        accessory.Method.SendDraw(DrawModeEnum.Default,DrawTypeEnum.Circle,currentProperty);

        if(Enable_Text_Prompts) {

            if(Language_Of_Text_Prompts==Languages_Of_Text_Prompts.Simplified_Chinese_简体中文) {

                accessory.Method.TextInfo("远离Boss",2500);

            }

            if(Language_Of_Text_Prompts==Languages_Of_Text_Prompts.English_英文) {
                
                accessory.Method.TextInfo("Stay away from the Boss",2500);
                
            }
            
        }
        
    }
    
    [ScriptMethod(name:"Bough of Attis (Back) 阿提斯的巨枝 (后)",
        eventType:EventTypeEnum.StartCasting,
        eventCondition:["ActionId:30719"])]
    
    public void Bough_of_Attis_Back_阿提斯的巨枝_后(Event @event, ScriptAccessory accessory) {

        if(!parseObjectId(@event["SourceId"], out var sourceId)) {
            
            return;
            
        }
        
        var currentProperty=accessory.Data.GetDefaultDrawProperties();
        
        currentProperty.Owner=Convert.ToUInt32(@event["SourceId"],16);
        currentProperty.Scale=new(25);
        currentProperty.DestoryAt=7700;
        currentProperty.Color=accessory.Data.DefaultDangerColor;
        
        accessory.Method.SendDraw(DrawModeEnum.Default,DrawTypeEnum.Circle,currentProperty);
        
        if(Enable_Text_Prompts) {
            
            if(Language_Of_Text_Prompts==Languages_Of_Text_Prompts.Simplified_Chinese_简体中文) {

                accessory.Method.TextInfo("靠近Boss",2500);

            }

            if(Language_Of_Text_Prompts==Languages_Of_Text_Prompts.English_英文) {
                
                accessory.Method.TextInfo("Approach the Boss",2500);
                
            }
            
        }
        
    }
    
    [ScriptMethod(name:"Bough of Attis (Side) 阿提斯的巨枝 (侧)",
        eventType:EventTypeEnum.StartCasting,
        eventCondition:["ActionId:30717"])]
    
    public void Bough_of_Attis_Side_阿提斯的巨枝_侧(Event @event, ScriptAccessory accessory) {

        if(!parseObjectId(@event["SourceId"], out var sourceId)) {
            
            return;
            
        }
        
        var currentProperty=accessory.Data.GetDefaultDrawProperties();
        var effectPositionInJson=JObject.Parse(@event["EffectPosition"]);
        float currentX=effectPositionInJson["X"]?.Value<float>()??0;
        
        currentProperty.Owner=Convert.ToUInt32(@event["SourceId"],16);
        currentProperty.Offset=new Vector3(0,0,10);
        currentProperty.Scale=new(25,50);
        currentProperty.DestoryAt=4700;
        currentProperty.Color=accessory.Data.DefaultDangerColor;
        
        accessory.Method.SendDraw(DrawModeEnum.Default,DrawTypeEnum.Rect,currentProperty);
        
        if(Enable_Text_Prompts) {

            if(currentX<100) {
                // The EffectPosition when the Boss hits the left side is {"X":80.70,"Y":0.08,"Z":83.09}
                
                if(Language_Of_Text_Prompts==Languages_Of_Text_Prompts.Simplified_Chinese_简体中文) {

                    accessory.Method.TextInfo("去右边躲避",1500);

                }

                if(Language_Of_Text_Prompts==Languages_Of_Text_Prompts.English_英文) {
                
                    accessory.Method.TextInfo("Dodge on the right",1500);
                
                }

            }
                
            if(currentX>100) {
                // The EffectPosition when the Boss hits the right side is {"X":119.28,"Y":0.08,"Z":83.09}
                
                if(Language_Of_Text_Prompts==Languages_Of_Text_Prompts.Simplified_Chinese_简体中文) {

                    accessory.Method.TextInfo("去左边躲避",1500);

                }

                if(Language_Of_Text_Prompts==Languages_Of_Text_Prompts.English_英文) {
                
                    accessory.Method.TextInfo("Dodge on the left",1500);
                
                }
                
            }

        }
        
        if(Enable_Developer_Mode) {
            
            accessory.Method.SendChat($"""
                                       /e 
                                       @event["EffectPosition"]={@event["EffectPosition"]}
                                       currentX={currentX}
                                       
                                       """);
            
        }
        
    }
    
    [ScriptMethod(name:"Static Moon 静电之月",
        eventType:EventTypeEnum.StartCasting,
        eventCondition:["ActionId:30722"])]
    
    public void Static_Moon_静电之月(Event @event, ScriptAccessory accessory) {

        if(!parseObjectId(@event["SourceId"], out var sourceId)) {
            
            return;
            
        }
        
        var currentProperty=accessory.Data.GetDefaultDrawProperties();
        
        currentProperty.Owner=Convert.ToUInt32(@event["SourceId"],16);
        currentProperty.Scale=new(10);
        currentProperty.DestoryAt=4700;
        currentProperty.Color=accessory.Data.DefaultDangerColor;
        
        accessory.Method.SendDraw(DrawModeEnum.Default,DrawTypeEnum.Circle,currentProperty);
        
        if(Enable_Text_Prompts) {
            
            if(Language_Of_Text_Prompts==Languages_Of_Text_Prompts.Simplified_Chinese_简体中文) {

                accessory.Method.TextInfo("远离贝希摩斯",1500);

            }

            if(Language_Of_Text_Prompts==Languages_Of_Text_Prompts.English_英文) {
                
                accessory.Method.TextInfo("Stay away from behemoths",1500);
                
            }
            
        }
        
    }
    
    [ScriptMethod(name:"Stymphalian Strike 怪鸟强袭",
        eventType:EventTypeEnum.StartCasting,
        eventCondition:["ActionId:30723"])]
    
    public void Stymphalian_Strike_怪鸟强袭(Event @event, ScriptAccessory accessory) {

        if(!parseObjectId(@event["SourceId"], out var sourceId)) {
            
            return;
            
        }
        
        var currentProperty=accessory.Data.GetDefaultDrawProperties();
        
        currentProperty.Owner=Convert.ToUInt32(@event["SourceId"],16);
        currentProperty.Scale=new(8,39);
        currentProperty.DestoryAt=4700;
        currentProperty.Color=accessory.Data.DefaultDangerColor;
        
        accessory.Method.SendDraw(DrawModeEnum.Default,DrawTypeEnum.Rect,currentProperty);
        
        if(Enable_Text_Prompts) {
            
            if(Language_Of_Text_Prompts==Languages_Of_Text_Prompts.Simplified_Chinese_简体中文) {

                accessory.Method.TextInfo("远离怪鸟正面",1500);

            }

            if(Language_Of_Text_Prompts==Languages_Of_Text_Prompts.English_英文) {
                
                accessory.Method.TextInfo("Stay away from the front of birds",1500);
                
            }
            
        }
        
    }
    
    [ScriptMethod(name:"Blades of Attis 阿提斯的叶刃",
        eventType:EventTypeEnum.ActionEffect,
        eventCondition:["ActionId:regex:^(30725|30726)$"])]
    
    public void Blades_of_Attis_阿提斯的叶刃(Event @event, ScriptAccessory accessory) {

        if(!parseObjectId(@event["SourceId"], out var sourceId)) {
            
            return;
            
        }
        
        var currentProperty=accessory.Data.GetDefaultDrawProperties();
        
        currentProperty.Owner=Convert.ToUInt32(@event["SourceId"],16);
        currentProperty.Offset=new Vector3(0,0,-8);
        currentProperty.Scale=new(7);
        currentProperty.DestoryAt=1250;
        currentProperty.Color=accessory.Data.DefaultDangerColor;
        
        accessory.Method.SendDraw(DrawModeEnum.Default,DrawTypeEnum.Circle,currentProperty);

        if(Enable_Text_Prompts&&!bladePromptsHasBeenSent) {
            
            bladePromptsHasBeenSent=true;
            
            if(Language_Of_Text_Prompts==Languages_Of_Text_Prompts.Simplified_Chinese_简体中文) {

                accessory.Method.TextInfo("躲避步进式AOE",7700);

            }

            if(Language_Of_Text_Prompts==Languages_Of_Text_Prompts.English_英文) {
                
                accessory.Method.TextInfo("Dodge stepping AOEs",7700);
                
            }
            
        }
        
    }
    
    [ScriptMethod(name:"Hemitheos's Aero IV 半神飙风",
        eventType:EventTypeEnum.StartCasting,
        eventCondition:["ActionId:30785"])]
    
    public void Hemitheoss_Aero_IV_半神飙风(Event @event, ScriptAccessory accessory) {

        if(!parseObjectId(@event["SourceId"], out var sourceId)) {
            
            return;
            
        }
        
        var currentProperty=accessory.Data.GetDefaultDrawProperties();

        currentProperty.Owner=accessory.Data.Me;
        currentProperty.TargetObject=sourceId;
        currentProperty.Rotation=float.Pi;
        currentProperty.Scale=new(1.5f,25);
        currentProperty.DestoryAt=6700;
        currentProperty.Color=accessory.Data.DefaultDangerColor;
        
        accessory.Method.SendDraw(DrawModeEnum.Default,DrawTypeEnum.Displacement,currentProperty);
        
        currentProperty=accessory.Data.GetDefaultDrawProperties();

        currentProperty.Owner=sourceId;
        currentProperty.TargetObject=accessory.Data.Me;
        currentProperty.ScaleMode|=ScaleMode.YByDistance;
        currentProperty.Scale=new(1.5f);
        currentProperty.DestoryAt=6700;
        currentProperty.Color=accessory.Data.DefaultDangerColor;
        
        accessory.Method.SendDraw(DrawModeEnum.Default,DrawTypeEnum.Displacement,currentProperty);
        
        if(Enable_Text_Prompts) {
            
            if(Language_Of_Text_Prompts==Languages_Of_Text_Prompts.Simplified_Chinese_简体中文) {

                accessory.Method.TextInfo("击退",2000);

            }

            if(Language_Of_Text_Prompts==Languages_Of_Text_Prompts.English_英文) {
                
                accessory.Method.TextInfo("Knock back",2000);
                
            }

        }
        
    }
    
    private static bool parseObjectId(string? idStr, out uint id) {
        // I just simply copied and pasted this function from Karlin-Z's code. Appreciate!
        
        id = 0;
        if (string.IsNullOrEmpty(idStr)) return false;
        try
        {
            var idStr2 = idStr.Replace("0x", "");
            id = uint.Parse(idStr2, System.Globalization.NumberStyles.HexNumber);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
        
    }
    
}