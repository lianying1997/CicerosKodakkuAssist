using System;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent.Struct;
using KodakkuAssist.Module.Draw;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommons;
using System.Numerics;
using Newtonsoft.Json;
using System.Linq;
using System.ComponentModel;
using System.Xml.Linq;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Utility.Numerics;
using ECommons.MathHelpers;
using Newtonsoft.Json.Linq;

namespace MyScriptNamespace
{
    
    [ScriptType(name:"Karlin-Z's FRU script (Customized by Cicero) K佬的绝伊甸脚本(天生有灵视修改版)",
        territorys:[1238],
        guid:"148718fd-575d-493a-8ac7-1cc7092aff85",
        version:"0.0.0.30",
        note:noteStr,
        author:"Karlin-Z (customized by Cicero)")]
    
    public class EdenUltimate
    {
        const string noteStr =
        """
        Karlin-Z's script of Futures Rewritten (Ultimate). Customized by Cicero, branched out from Version 0.0.0.10.
        Add guidance for the second half of Phase 3,
        guidance related to Drachen Wanderers and refinements for the second half of Phase 4,
        guidance for Wings Dark And Light in Phase 5,
        guidance for Polarizing Strikes in Phase 5.
        Please re-configure the user settings of this customized script according to your settings in the original script!
        
        Karlin-Z的另一个未来(绝伊甸)脚本。天生有灵视基于0.0.0.10版本做了修改。
        添加了P3二运指路，
        P4二运指路的精修和圣龙气息(白圈)相关指路，
        P5光与暗之翼(踩塔)指路，
        P5失调打击(挡枪)指路。
        请记得按照原版脚本重新配置一下这个脚本的用户设置！
        """;

        [UserSetting("P1_转轮召分组依据")]
        public P1BrightFireEnum P1BrightFireGroup { get; set; }
        [UserSetting("P1_四连线头顶标记")]
        public P1TetherEnum p1Thther4Type { get; set; }
        [UserSetting("P1_四连线头顶标记")]
        public bool p1Thther4Marker { get; set; } = false;

        [UserSetting("P2_光爆拉线方式")]
        public P2LightRampantTetherEmum P2LightRampantTetherDeal { get; set; }
        [UserSetting("P2_光爆八方站位方式")]
        public P2LightRampant8DirEmum P2LightRampant8DirSet { get; set; }

        [UserSetting("P3_分灯方式")]
        public P3LampEmum P3LampDeal { get; set; }
        [UserSetting("Phase3_Strats_Of_The_Second_Half_二运策略")]
        public Phase3_Strats_Of_The_Second_Half Phase3_Strats_Of_The_Second_Half_二运策略 { get; set; }
        [UserSetting("Phase3_Who_Baits_Darkest_Dance_谁引导暗夜舞蹈")]
        public Phase3_Who_Baits_Darkest_Dance Phase3_Who_Baits_Darkest_Dance_谁引导暗夜舞蹈 { get; set; }
        [UserSetting("Phase3_Colour_Of_Darkest_Dance_暗夜舞蹈的颜色")]
        public ScriptColor Phase3_Colour_Of_Darkest_Dance_暗夜舞蹈的颜色 { get; set; } = new() { V4=new(1f,0f,0f,1f) };

        [UserSetting("P4_二运常/慢灯AOE显示时间(ms)")]
        public uint P4LampDisplayDur { get; set; } =3000;
        [UserSetting("Phase4_Length_Of_Drachen_Hitboxes_龙头碰撞箱长度")]
        public float Phase4_Length_Of_Drachen_Hitboxes_龙头碰撞箱长度 { get; set; } = 3;
        [UserSetting("Phase4_Residue_For_Dark_Eruption_暗炎喷发的白圈")]
        public Phase4_Positions_Of_Drachen_Wanderer_Residues Phase4_Residue_For_Dark_Eruption_暗炎喷发的白圈 { get; set; } = Phase4_Positions_Of_Drachen_Wanderer_Residues.EASTMOST_最东侧;
        [UserSetting("Phase4_Residue_For_Unholy_Darkness_黑暗神圣的白圈")]
        public Phase4_Positions_Of_Drachen_Wanderer_Residues Phase4_Residue_For_Unholy_Darkness_黑暗神圣的白圈 { get; set; } = Phase4_Positions_Of_Drachen_Wanderer_Residues.ABOUT_EAST_次东侧;
        [UserSetting("Phase4_Residue_For_Dark_Blizzard_III_黑暗冰封的白圈")]
        public Phase4_Positions_Of_Drachen_Wanderer_Residues Phase4_Residue_For_Dark_Blizzard_III_黑暗冰封的白圈 { get; set; } = Phase4_Positions_Of_Drachen_Wanderer_Residues.ABOUT_WEST_次西侧;
        [UserSetting("Phase4_Residue_For_Dark_Water_III_黑暗狂水的白圈")]
        public Phase4_Positions_Of_Drachen_Wanderer_Residues Phase4_Residue_For_Dark_Water_III_黑暗狂水的白圈 { get; set; } = Phase4_Positions_Of_Drachen_Wanderer_Residues.WESTMOST_最西侧;
        [UserSetting("Phase4_Colour_Of_Hitboxes_And_Guidance_碰撞箱及指路的颜色")]
        public ScriptColor Phase4_Colour_Of_Hitboxes_And_Guidance_碰撞箱及指路的颜色 { get; set; } = new() { V4=new(1f,1f,0f,1f) };

        [UserSetting("P5_地火颜色")]
        public ScriptColor P5PathColor { get; set; } = new() { V4=new(0,1,1,1)};
        [UserSetting("Phase5_Strats_Of_Wings_Dark_And_Light_光与暗之翼策略")]
        public Phase5_Strats_Of_Wings_Dark_And_Light Phase5_Strats_Of_Wings_Dark_And_Light_光与暗之翼策略 { get; set; }
        [UserSetting("Phase5_Hints_Of_Provoking_挑衅提示")]
        public bool Phase5_Hints_Of_Provoking_挑衅提示 { get; set; } = true;
        [UserSetting("Phase5_Orders_During_Polarizing_Strikes_挡枪顺序")]
        public Phase5_Orders_During_Polarizing_Strikes Phase5_Orders_During_Polarizing_Strikes_挡枪顺序 { get; set; }
        
        [UserSetting("Enable Developer Mode 启用开发者模式")]
        public bool Enable_Developer_Mode_启用开发者模式 { get; set; } = false;

        int? firstTargetIcon = null;
        double parse = 0;

        int P1雾龙计数 =0;
        int[] P1雾龙记录 = [0, 0, 0, 0];
        bool P1雾龙雷=false;

        bool P1转轮召雷 = false;
        List<int> P1转轮召抓人 = [0, 0, 0, 0, 0, 0, 0, 0];
        List<int> P1四连线 = [];
        bool P1四连线开始 = false;
        List<int> P1塔 = [0, 0, 0, 0];

        bool P2DDDircle = false;
        List<int> P2DDIceDir = [];
        List<int> P2RedMirror = [];
        uint P2BossId = 0;
        List<int> P2LightRampantCircle = [];
        List<int> P2LightRampantBuff = [0, 0, 0, 0, 0, 0, 0, 0];
        bool P2LightRampantTetherDone = new();

        List<int> P3FireBuff = [0, 0, 0, 0, 0, 0, 0, 0];
        List<int> P3WaterBuff = [0, 0, 0, 0, 0, 0, 0, 0];
        List<int> P3ReturnBuff = [0, 0, 0, 0, 0, 0, 0, 0];
        List<int> P3Lamp = [0, 0, 0, 0, 0, 0, 0, 0];
        List<int> P3LampWise = [0, 0, 0, 0, 0, 0, 0, 0];
        List<int> P3Stack = [0, 0, 0, 0, 0, 0, 0, 0];
        bool P3FloorFireDone = false;
        int P3FloorFire = 0;
        List<Phase3_Types_Of_Dark_Water_III> typeOfDarkWaterIii=[
            Phase3_Types_Of_Dark_Water_III.NONE,
            Phase3_Types_Of_Dark_Water_III.NONE,
            Phase3_Types_Of_Dark_Water_III.NONE,
            Phase3_Types_Of_Dark_Water_III.NONE,
            Phase3_Types_Of_Dark_Water_III.NONE,
            Phase3_Types_Of_Dark_Water_III.NONE,
            Phase3_Types_Of_Dark_Water_III.NONE,
            Phase3_Types_Of_Dark_Water_III.NONE
        ];
        List<int> indexWhileDoubleGroup=[2,3,0,1,4,5,6,7];
        // The temporary priority would be H1 H2 MT OT M1 M2 R1 R2 or H1 H2 MT ST D1 D2 D3 D4 while adopting the MMW Double Group strat.
        uint timesStatus2458Disappeared=0;
        object lockOfTimesStatus2458Disappeared=new object();
        
        uint P4FragmentId;
        List<int> P4Tether = [-1, -1, -1, -1, -1, -1, -1, -1];
        List<int> P4Stack = [0, 0, 0, 0, 0, 0, 0, 0];
        bool P4TetherDone = false;
        List<int> P4ClawBuff = [0, 0, 0, 0, 0, 0, 0, 0];
        List<int> P4OtherBuff = [0, 0, 0, 0, 0, 0, 0, 0];
        int P4BlueTether = 0;
        List<Vector3> P4WhiteCirclePos = [];
        List<Vector3> P4WaterPos = [];
        List<uint> residueIdFromEastToWest=[0,0,0,0];
        // The leftmost (0), the about left (1), the about right (2), the rightmost (3) while facing south.
        bool residueGuidanceHasBeenGenerated=false;

        volatile bool hasAcquiredTheFirstTower=false;
        string indexOfTheFirstTower="";
        bool hasDrawnTheInitialPositionOfMT=false;
        Vector3 leftSideOfSouth=new Vector3(98,0,107);
        Vector3 rightSideOfSouth=new Vector3(102,0,107);
        Vector3 leftSideOfNortheast=new Vector3(107.06f,0,98.23f);
        Vector3 rightSideOfNortheast=new Vector3(105.06f,0,94.77f);
        Vector3 leftSideOfNorthwest=new Vector3(94.94f,0,94.77f);
        Vector3 rightSideOfNorthwest=new Vector3(92.94f,0,98.23f);
        // The left and right here refer to the left and right while facing the center of the zone (100,0,100).
        Vector3 standbyPositionBetweenNortheastAndNorthwest=new Vector3(100,0,93);
        Vector3 standbyPositionBetweenNorthwestAndSouth=new Vector3(93.94f,0,103.50f);
        Vector3 standbyPositionBetweenNortheastAndSouth=new Vector3(106.06f,0,103.50f);

        Vector3 positionToTakeHitsOnTheLeft=new Vector3(95.93f,0,104.07f);
        Vector3 positionToBeCoveredOnTheLeft=new Vector3(93.81f,0,106.19f);
        Vector3 positionToStandbyOnTheLeft=new Vector3(98.78f,0,106.89f);
        Vector3 positionToTakeHitsOnTheRight=new Vector3(104.07f,0,104.07f);
        Vector3 positionToBeCoveredOnTheRight=new Vector3(106.19f,0,106.19f);
        Vector3 positionToStandbyOnTheRight=new Vector3(101.22f,0,106.89f);
        // The left and right here refer to the left and right while facing the center of the zone (100,0,100).

        public enum P1TetherEnum
        {
            OneLine,
            Mgl_TwoLine
        }
        public enum P1BrightFireEnum
        {
            TH_Up,
            MtGroup_Up,
            MtStD3D4_Up
        }
        public enum P2LightRampant8DirEmum
        {
            Normal,
            TN_Up
        }
        public enum P2LightRampantTetherEmum
        {
            CircleNum,
            LTeam,
            AC_Cross,
            NewGrey9
        }

        public enum P3LampEmum
        {
            MGL
        }

        public enum Phase3_Strats_Of_The_Second_Half {
            
            MMW_Double_Group_双分组法,
            Other_Strats_Work_In_Progress_其他策略正在施工中
            
        }

        public enum Phase3_Who_Baits_Darkest_Dance {
            
            MT,
            OT_ST
            
        }
        
        public enum Phase3_Types_Of_Dark_Water_III {
            
            LONG,
            MEDIUM,
            SHORT,
            NONE
            
        }

        public enum P4WhiteCirleEmum
        {
            IceB,
            Ice3
        }

        public enum Phase4_Positions_Of_Drachen_Wanderer_Residues {

            EASTMOST_最东侧,
            ABOUT_EAST_次东侧,
            ABOUT_WEST_次西侧,
            WESTMOST_最西侧,
            UNKNOWN_未知

        }
        
        public enum Phase5_Strats_Of_Wings_Dark_And_Light {
            
            GREY9_灰九式,
            Other_Strats_Work_In_Progress_其他策略正在施工中
            
        }

        public enum Phase5_Orders_During_Polarizing_Strikes {
            
            TANKS_MELEES_RANGES_HEALERS_坦克近战远程奶妈,
            TANKS_HEALERS_MELEES_RANGES_坦克奶妈近战远程,
            Other_Orders_Work_In_Progress_其他顺序正在施工中
            
        }

        public void Init(ScriptAccessory accessory)
        {
            accessory.Method.RemoveDraw(".*");
            if (p1Thther4Marker)
                accessory.Method.MarkClear();
            parse = 1d;
            P1雾龙记录 = [0, 0, 0, 0];
            P1雾龙计数 = 0;
            P1转轮召抓人 = [0, 0, 0, 0, 0, 0, 0, 0];
            P1四连线 = [];
            P1四连线开始 = false;
            P1塔 = [0, 0, 0, 0];

            P2DDIceDir.Clear();

            P3FloorFireDone = false;
            P3Stack = [0, 0, 0, 0, 0, 0, 0, 0];
            typeOfDarkWaterIii=[
                Phase3_Types_Of_Dark_Water_III.NONE,
                Phase3_Types_Of_Dark_Water_III.NONE,
                Phase3_Types_Of_Dark_Water_III.NONE,
                Phase3_Types_Of_Dark_Water_III.NONE,
                Phase3_Types_Of_Dark_Water_III.NONE,
                Phase3_Types_Of_Dark_Water_III.NONE,
                Phase3_Types_Of_Dark_Water_III.NONE,
                Phase3_Types_Of_Dark_Water_III.NONE
            ];
            timesStatus2458Disappeared=0;

            residueIdFromEastToWest=[0,0,0,0];
            residueGuidanceHasBeenGenerated=false;

            hasAcquiredTheFirstTower=false;
            indexOfTheFirstTower="";
            hasDrawnTheInitialPositionOfMT=false;
        }

        #region P1

        [ScriptMethod(name: "P1_八方雷火_引导扇形",eventType: EventTypeEnum.StartCasting,eventCondition: ["ActionId:regex:^((4014[48])|40329|40330)$"])]
        public void P1_八方雷火_引导扇形(Event @event, ScriptAccessory accessory)
        {
            if (parse != 1d) return;
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            foreach (var pm in accessory.Data.PartyList)
            {
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P1_八方雷火_引导扇形";
                dp.Scale = new(60);
                dp.Radian = float.Pi / 8;
                dp.Owner = sid;
                dp.TargetObject=pm;
                dp.Color = accessory.Data.DefaultDangerColor;
                dp.DestoryAt = 7000;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
            }

        }
        [ScriptMethod(name: "P1_八方雷火_后续扇形", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:regex:^(40145)$", "TargetIndex:1"])]
        public void P1_八方雷火_后续扇形(Event @event, ScriptAccessory accessory)
        {
            var dur = 2000;
            if (parse != 1d) return;
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (!float.TryParse(@event["SourceRotation"], out var rot)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P1_八方雷火_后续扇形1";
            dp.Scale = new(60);
            dp.FixRotation = true;
            dp.Rotation = rot;
            dp.Radian = float.Pi / 8;
            dp.Owner = sid;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.DestoryAt = dur;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P1_八方雷火_后续扇形2";
            dp.Scale = new(60);
            dp.Radian = float.Pi / 8;
            dp.FixRotation = true;
            dp.Rotation = rot + float.Pi / -8;
            dp.Owner = sid;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 2000;
            dp.DestoryAt = dur;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P1_八方雷火_后续扇形3";
            dp.Scale = new(60);
            dp.FixRotation = true;
            dp.Rotation = rot + float.Pi / -4;
            dp.Radian = float.Pi / 8;
            dp.Owner = sid;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 4000;
            dp.DestoryAt = dur;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);

        }
        [ScriptMethod(name: "P1_八方雷火_分散分摊", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^((4014[48])|40329|40330)$"])]
        public void P1_八方雷火_分散分摊(Event @event, ScriptAccessory accessory)
        {
            if (parse != 1d) return;
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            if (@event["ActionId"]== "40148" || @event["ActionId"] == "40330")
            {
                foreach (var pm in accessory.Data.PartyList)
                {
                    var dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P1_八方雷火_分散";
                    dp.Scale = new(6);
                    dp.Owner = pm;
                    dp.Color = accessory.Data.DefaultDangerColor;
                    dp.Delay = 5000;
                    dp.DestoryAt = 4000;
                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
                }
            }
            else
            {
                int[] group = [6, 7, 4, 5, 2, 3, 0, 1];
                var myindex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
                for (int i = 0; i < 4; i++)
                {
                    var ismygroup = myindex == i || group[i] == myindex;

                    var dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P1_八方雷火_分摊";
                    dp.Scale = new(6);
                    dp.Owner = accessory.Data.PartyList[i];
                    dp.Color = ismygroup ? accessory.Data.DefaultSafeColor : accessory.Data.DefaultDangerColor;
                    dp.Delay = 5000;
                    dp.DestoryAt = 4000;
                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
                }
            }
            

        }
        [ScriptMethod(name: "P1_八方雷火_引导位置", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^((4014[48])|40329|40330)$"])]
        public void P1_八方雷火_引导位置(Event @event, ScriptAccessory accessory)
        {
            if (parse != 1d) return;
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var myindex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
            var spread = @event["ActionId"] == "40148"|| @event["ActionId"] == "40330";
            var rot8 = myindex switch
            {
                0 => 0,
                1 => 2,
                2 => 6,
                3 => 4,
                4 => 5,
                5 => 3,
                6 => 7,
                7 => 1,
                _ => 0,
            };
            var outPoint = spread && (myindex == 2 || myindex == 3 || myindex == 6 || myindex == 7);
            var mPosEnd = RotatePoint(outPoint? new(100, 0, 90) : new(100, 0, 95), new(100, 0, 100), float.Pi / 4 * rot8);

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P1_八方雷火_引导位置";
            dp.Scale = new(2);
            dp.Owner = accessory.Data.Me;
            dp.TargetPosition = mPosEnd;
            dp.ScaleMode |= ScaleMode.YByDistance;
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.DestoryAt = 7000;
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

        }
        [ScriptMethod(name: "P1_T死刑Buff爆炸", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:4166"])]
        public void P1_T死刑Buff爆炸(Event @event, ScriptAccessory accessory)
        {
            if (parse != 1d) return;
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            if(!int.TryParse(@event["DurationMilliseconds"], out var dur)) return;
            var displayTime = 4000;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P1_T死刑Buff爆炸1";
            dp.Scale = new(10);
            dp.Owner = tid;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = dur- displayTime;
            dp.DestoryAt = displayTime;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P1_T死刑Buff爆炸2";
            dp.Scale = new(10);
            dp.Owner = tid;
            dp.CentreResolvePattern=PositionResolvePatternEnum.PlayerNearestOrder;
            dp.CentreOrderIndex = 1;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = dur - displayTime;
            dp.DestoryAt = displayTime;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

        }
        [ScriptMethod(name: "P1_雾龙_位置记录", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40158)$"], userControl: false)]
        public void P1_雾龙_位置记录(Event @event, ScriptAccessory accessory)
        {
            if (parse != 1d) return;
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var obj= accessory.Data.Objects.SearchByEntityId(sid+1);
            if(obj == null) return;
            var dir8= PositionTo8Dir(obj.Position, new(100, 0, 100));
            P1雾龙记录[dir8 % 4] = 1;
        }
        [ScriptMethod(name: "P1_雾龙_雷火记录", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:regex:^(4015[45])$"], userControl: false)]
        public void P1_雾龙_雷火记录(Event @event, ScriptAccessory accessory)
        {
            if (parse != 1d) return;
            P1雾龙雷 = (@event["ActionId"] == "40155");
        }
        [ScriptMethod(name: "P1_雾龙_范围", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40158)$"])]
        public void P1_雾龙_范围(Event @event, ScriptAccessory accessory)
        {
            if (parse != 1d) return;
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P1_雾龙范围";
            dp.Scale = new(16,50);
            dp.Owner = sid+1;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.DestoryAt = 9000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);

        }
        [ScriptMethod(name: "P1_雾龙_分散分摊", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:regex:^(4015[45])$"])]
        public void P1_雾龙_分散分摊(Event @event, ScriptAccessory accessory)
        {
            if (parse != 1d) return;

            if (@event["ActionId"] == "40155")
            {
                foreach (var pm in accessory.Data.PartyList)
                {
                    var dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P1_雾龙_分散";
                    dp.Scale = new(5);
                    dp.Owner = pm;
                    dp.Color = accessory.Data.DefaultDangerColor;
                    dp.Delay = 10000;
                    dp.DestoryAt = 5000;
                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
                }
            }
            else
            {
                List<int> h1group = [0, 2, 4, 6];
                var myindex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);

                var isH1group = h1group.Contains(myindex);

                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P1_雾龙_分摊1";
                dp.Scale = new(6);
                dp.Owner = accessory.Data.PartyList[2];
                dp.Color = isH1group ? accessory.Data.DefaultSafeColor : accessory.Data.DefaultDangerColor;
                dp.Delay = 10000;
                dp.DestoryAt = 5000;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P1_雾龙_分摊2";
                dp.Scale = new(6);
                dp.Owner = accessory.Data.PartyList[3];
                dp.Color = !isH1group ? accessory.Data.DefaultSafeColor : accessory.Data.DefaultDangerColor;
                dp.Delay = 10000;
                dp.DestoryAt = 5000;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            }

        }
        [ScriptMethod(name: "P1_雾龙_预站位位置", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4015[45])$"])]
        public void P1_雾龙_预站位位置(Event @event, ScriptAccessory accessory)
        {
            if (parse != 1d) return;
            var myindex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
            var rot8 = myindex switch
            {
                0 => 0,
                1 => 1,
                2 => 6,
                3 => 4,
                4 => 5,
                5 => 3,
                6 => 7,
                7 => 2,
                _ => 0,
            };
            var mPosEnd = RotatePoint(new(100, 0, 82), new(100, 0, 100), float.Pi / 4 * rot8);
            if (myindex==0)
            {
                mPosEnd = RotatePoint(mPosEnd, new(100, 0, 100), float.Pi / 36);
            }
            if (myindex == 6)
            {
                mPosEnd = RotatePoint(mPosEnd, new(100, 0, 100), float.Pi / -36);
            }

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P1_雾龙_预站位位置";
            dp.Scale = new(2);
            dp.Owner = accessory.Data.Me;
            dp.TargetPosition = mPosEnd;
            dp.ScaleMode |= ScaleMode.YByDistance;
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.DestoryAt = 9000;
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

        }
        [ScriptMethod(name: "P1_雾龙_处理位置", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40158)$"])]
        public void P1_雾龙_处理位置(Event @event, ScriptAccessory accessory)
        {
            if (parse != 1d) return;

            lock (this)
            {
                P1雾龙计数 ++; 
                if(P1雾龙计数 != 3) return;
                Task.Delay(100).ContinueWith(t =>
                {
                    if (!P1雾龙雷)
                    {
                        var safeDir = P1雾龙记录.IndexOf(0);
                        List<int> h1group = [0, 2, 4, 6];
                        var myindex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
                        var isH1group = h1group.Contains(myindex);
                        var rot8 = safeDir switch
                        {
                            0 => isH1group ? 0 : 4,
                            1 => isH1group ? 5 : 1,
                            2 => isH1group ? 6 : 2,
                            3 => isH1group ? 7 : 3,
                            _ => 0
                        };
                        var mPosEnd = RotatePoint(new(100,0,84), new(100, 0, 100), float.Pi / 4 * rot8);

                        var dp = accessory.Data.GetDefaultDrawProperties();
                        dp.Name = "P1_雾龙_分摊处理位置";
                        dp.Scale = new(2);
                        dp.Owner = accessory.Data.Me;
                        dp.TargetPosition = mPosEnd;
                        dp.ScaleMode |= ScaleMode.YByDistance;
                        dp.Color = accessory.Data.DefaultSafeColor;
                        dp.DestoryAt = 9000;
                        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                    }
                    else
                    {
                        var safeDir = P1雾龙记录.IndexOf(0);
                        List<int> h1group = [0, 2, 4, 6];
                        var myindex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
                        var isH1group = h1group.Contains(myindex);
                        Vector3 p1 = new(100.0f, 0, 88.0f);
                        Vector3 p2 = new(100.0f, 0, 80.5f);
                        Vector3 p3 = new(106.5f, 0, 81.5f);
                        Vector3 p4 = new(093.5f, 0, 81.5f);
                        var rot8 = safeDir switch
                        {
                            0 => isH1group ? 0 : 4,
                            1 => isH1group ? 5 : 1,
                            2 => isH1group ? 6 : 2,
                            3 => isH1group ? 7 : 3,
                            _ => 0
                        };
                        var myPosA = myindex switch
                        {
                            0 => p2,
                            1 => p2,
                            2 => p1,
                            3 => p1,
                            4 => p3,
                            5 => p3,
                            6 => p4,
                            7 => p4,
                            _ => p1,
                        };
                        var mPosEnd = RotatePoint(myPosA, new(100, 0, 100), float.Pi / 4 * rot8);

                        var dp = accessory.Data.GetDefaultDrawProperties();
                        dp.Name = "P1_雾龙_分散处理位置";
                        dp.Scale = new(2);
                        dp.Owner = accessory.Data.Me;
                        dp.TargetPosition = mPosEnd;
                        dp.ScaleMode |= ScaleMode.YByDistance;
                        dp.Color = accessory.Data.DefaultSafeColor;
                        dp.DestoryAt = 9000;
                        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                    }
                });
                
            }

        }


        [ScriptMethod(name: "P1_转轮召_雷火记录", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4015[01])$"], userControl: false)]
        public void P1_转轮召_雷火记录(Event @event, ScriptAccessory accessory)
        {
            if (parse != 1d) return;
            P1转轮召雷 = (@event["ActionId"] == "40151");
        }
        [ScriptMethod(name: "P1_转轮召_雷直线", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40164)$"])]
        public void P1_转轮召_雷直线(Event @event, ScriptAccessory accessory)
        {
            if (parse != 1d) return;
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var delay = 4000;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P1_转轮召_雷直线";
            dp.Scale = new(20, 40);
            dp.Owner = sid;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay=delay;
            dp.DestoryAt = 9700-delay;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);

        }
        [ScriptMethod(name: "P1_转轮召_火直线", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40161)$"])]
        public void P1_转轮召_火直线(Event @event, ScriptAccessory accessory)
        {
            if (parse != 1d) return;
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var delay = 4000;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P1_转轮召_火直线";
            dp.Scale = new(10, 40);
            dp.Owner = sid;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = delay;
            dp.DestoryAt = 7700 - delay;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);

        }

        [ScriptMethod(name: "P1_转轮召_抓人记录", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:4165"],userControl:false)]
        public void P1_转轮召_抓人记录(Event @event, ScriptAccessory accessory)
        {
            if (parse != 1d) return;
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            lock (this)
            {
                P1转轮召抓人[accessory.Data.PartyList.IndexOf(tid)] = 1;
            }
        }
        [ScriptMethod(name: "P1_转轮召_击退处理位置", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40152)$"])]
        public void P1_转轮召_击退处理位置(Event @event, ScriptAccessory accessory)
        {
            //dy 7
            if (parse != 1d) return;
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var pos= JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
            if (MathF.Abs(pos.Z - 100) > 1) return;
            
            var atEast = pos.X - 100 > 1;
            var o1= P1转轮召抓人.IndexOf(1);
            var o2 = P1转轮召抓人.LastIndexOf(1);
            List<int> upGroup = [];
            if (P1BrightFireGroup==P1BrightFireEnum.TH_Up)
            {
                upGroup.Add(o1);
                if (o1 != 1 && o2 != 1) upGroup.Add(1);
                if (o1 != 2 && o2 != 2) upGroup.Add(2);
                if (o1 != 3 && o2 != 3) upGroup.Add(3);
                if (upGroup.Count < 4 && o1 != 0 && o2 != 0) upGroup.Add(0);
                if (upGroup.Count < 4 && o1 != 4 && o2 != 4) upGroup.Add(4);
            }
            if (P1BrightFireGroup == P1BrightFireEnum.MtGroup_Up)
            {
                upGroup.Add(o1);
                if (o1 != 2 && o2 != 2) upGroup.Add(2);
                if (o1 != 4 && o2 != 4) upGroup.Add(4);
                if (o1 != 6 && o2 != 6) upGroup.Add(6);
                if (upGroup.Count < 4 && o1 != 0 && o2 != 0) upGroup.Add(0);
                if (upGroup.Count < 4 && o1 != 1 && o2 != 1) upGroup.Add(1);
            }
            if (P1BrightFireGroup == P1BrightFireEnum.MtStD3D4_Up)
            {
                List<int> upIndex = [0, 1, 6, 7];
                if (upIndex.Contains(o1) && !upIndex.Contains(o2)) upGroup.Add(o1);
                if (upIndex.Contains(o2) && !upIndex.Contains(o1)) upGroup.Add(o2);
                if (upIndex.Contains(o1) && !upIndex.Contains(o2))
                {
                    if (upIndex.IndexOf(o1)<upIndex.IndexOf(o2))
                    {
                        upGroup.Add(o1);
                    }
                    else
                    {
                        upGroup.Add(o2);
                    }
                }
                var up0 = upGroup[0];
                var down0 = up0 == o1 ? o2 : o1;
                if (up0 != 1 && down0 != 1) upGroup.Add(1);
                if (up0 != 6 && down0 != 6) upGroup.Add(6);
                if (up0 != 7 && down0 != 7) upGroup.Add(7);
                if (upGroup.Count < 4 && up0 != 0 && down0 != 0) upGroup.Add(0);
                if (upGroup.Count < 4 && up0 != 4 && down0 != 4) upGroup.Add(4);
            }

            var myindex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
            var dealpos1 = new Vector3(atEast ? 105.5f : 94.5f, 0, upGroup.Contains(myindex) ? 93 : 107);
            var dealpos2 = new Vector3(atEast ? 102 : 98, 0, upGroup.Contains(myindex) ? 93 : 107);
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P1_转轮召_击退处理位置1";
            dp.Scale = new(2);
            dp.Owner = accessory.Data.Me;
            dp.TargetPosition = dealpos1;
            dp.ScaleMode |= ScaleMode.YByDistance;
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.DestoryAt = 4000;
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P1_转轮召_击退处理位置2";
            dp.Scale = new(2);
            dp.Position = dealpos1;
            dp.TargetPosition = dealpos2;
            dp.ScaleMode |= ScaleMode.YByDistance;
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.DestoryAt = 4000;
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P1_转轮召_击退处理位置3";
            dp.Scale = new(2);
            dp.Owner = accessory.Data.Me;
            dp.TargetPosition = dealpos2;
            dp.ScaleMode |= ScaleMode.YByDistance;
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.Delay = 4000;
            dp.DestoryAt = 2000;
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);


        }

        [ScriptMethod(name: "P1_四连线_清除连线记录器", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40170)$"])]
        public void P1_四连线_清除连线记录器(Event @event, ScriptAccessory accessory)
        {
            
            if (parse != 1d) return;
            P1四连线.Clear();
            P1四连线开始 =true;
            if (p1Thther4Marker)
                accessory.Method.MarkClear();
        }
        [ScriptMethod(name: "P1_四连线_连线记录器", eventType: EventTypeEnum.Tether, eventCondition: ["Id:regex:^(00F9|011F)$"],userControl:false)]
        public void P1_四连线_连线记录器(Event @event, ScriptAccessory accessory)
        {
            if (parse != 1d) return;
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            var index= accessory.Data.PartyList.IndexOf(tid);
            var id = @event["Id"] == "00F9" ? 10 : 20;
            P1四连线.Add(id + index);
        }
        [ScriptMethod(name: "P1_四连线_头顶标记", eventType: EventTypeEnum.Tether, eventCondition: ["Id:regex:^(00F9|011F)$"],userControl:false)]
        public void P1_四连线_头顶标记(Event @event, ScriptAccessory accessory)
        {
            if (parse != 1d) return;
            if (!p1Thther4Marker) return;
            if (!P1四连线开始) return;
            Task.Delay(50).ContinueWith(t =>
            {
                var index = P1四连线.Last() % 10;
                accessory.Method.Mark(accessory.Data.PartyList[index], (KodakkuAssist.Module.GameOperate.MarkType)P1四连线.Count);
                //accessory.Log.Debug($"{index} {(KodakkuAssist.Module.GameOperate.MarkType)P1四连线.Count}");
            });
        }
        [ScriptMethod(name: "P1_四连线_处理位置", eventType: EventTypeEnum.Tether, eventCondition: ["Id:regex:^(00F9|011F)$"])]
        public void P1_四连线_处理位置(Event @event, ScriptAccessory accessory)
        {
            if (!P1四连线开始) return;
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            var dis = 3f;//距离点名人
            var far = 4.5f;//距离boss
            Task.Delay(50).ContinueWith(t =>
            {
                var myindex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
                Vector3 t1p1 = new(100, 0, 100 - far);
                Vector3 t1p2 = new(100, 0, 100-far-dis);
                Vector3 t2p1 = new(100, 0, 100 + far);
                Vector3 t2p2 = new(100, 0, 100 + far + dis);
                Vector3 t3p1 = new(100, 0, 100 - far - dis);
                Vector3 t3p2 = new(100, 0, 100 - far);
                Vector3 t4p1 = new(100, 0, 100 + far + dis);
                Vector3 t4p2 = new(100, 0, 100 + far);
                
                if (P1四连线.Count ==1 && tid==accessory.Data.Me)
                {
                    var dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P1_四连线_线1处理位置1";
                    dp.Scale = new(2);
                    dp.Owner = accessory.Data.Me;
                    dp.TargetPosition = t1p1;
                    dp.ScaleMode |= ScaleMode.YByDistance;
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.DestoryAt = 13000;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                    dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P1_四连线_线1处理位置2";
                    dp.Scale = new(2);
                    dp.Owner = accessory.Data.Me;
                    dp.TargetPosition = t1p2;
                    dp.ScaleMode |= ScaleMode.YByDistance;
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.Delay =13000;
                    dp.DestoryAt = 6000;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                }
                if (P1四连线.Count == 2 && tid == accessory.Data.Me)
                {
                    var dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P1_四连线_线2处理位置1";
                    dp.Scale = new(2);
                    dp.Owner = accessory.Data.Me;
                    dp.TargetPosition = t2p1;
                    dp.ScaleMode |= ScaleMode.YByDistance;
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.DestoryAt = 13500;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                    dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P1_四连线_线2处理位置2";
                    dp.Scale = new(2);
                    dp.Owner = accessory.Data.Me;
                    dp.TargetPosition = t2p2;
                    dp.ScaleMode |= ScaleMode.YByDistance;
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.Delay = 13500;
                    dp.DestoryAt = 5000;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                }
                if (P1四连线.Count == 3 && tid == accessory.Data.Me)
                {
                    var dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P1_四连线_线3处理位置1";
                    dp.Scale = new(2);
                    dp.Owner = accessory.Data.Me;
                    dp.TargetPosition = t3p1;
                    dp.ScaleMode |= ScaleMode.YByDistance;
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.DestoryAt = 7500;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                    dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P1_四连线_线3处理位置2";
                    dp.Scale = new(3);
                    dp.Owner = accessory.Data.Me;
                    dp.TargetPosition = t3p2;
                    dp.ScaleMode |= ScaleMode.YByDistance;
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.Delay = 7500;
                    dp.DestoryAt = 6000;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                }
                if (P1四连线.Count == 4 && tid == accessory.Data.Me)
                {
                    var dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P1_四连线_线4处理位置1";
                    dp.Scale = new(2);
                    dp.Owner = accessory.Data.Me;
                    dp.TargetPosition = t4p1;
                    dp.ScaleMode |= ScaleMode.YByDistance;
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.DestoryAt = 8500;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                    dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P1_四连线_线4处理位置2";
                    dp.Scale = new(3);
                    dp.Owner = accessory.Data.Me;
                    dp.TargetPosition = t4p2;
                    dp.ScaleMode |= ScaleMode.YByDistance;
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.Delay = 8500;
                    dp.DestoryAt = 5000;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                }
                if (P1四连线.Count == 4)
                {
                    var tehterObjIndex = P1四连线.Select(o => o % 10).ToList();
                    var tehterIsFire = P1四连线.Select(o => o < 20).ToList();
                    List<int> idleObjIndex = [];
                    if (p1Thther4Type==P1TetherEnum.OneLine)
                    {
                        for (int i = 0; i < accessory.Data.PartyList.Count; i++)
                        {
                            if (!tehterObjIndex.Contains(i))
                            { idleObjIndex.Add(i); }
                        }
                    }
                    if(p1Thther4Type==P1TetherEnum.Mgl_TwoLine)
                    {
                        List<int> group1 = [0, 1, 2, 3];
                        List<int> group2 = [4, 5, 6, 7];
                        group1.RemoveAll(x => tehterObjIndex.Contains(x));
                        while (group1.Count>2)
                        {
                            var m = group1.First();
                            group1.Remove(m);
                            group2.Add(m);
                        }
                        idleObjIndex.AddRange(group1);
                        idleObjIndex.AddRange(group2);
                    }
                    
                    if (!idleObjIndex.Contains(myindex)) return;

                    Vector3 i1p1 = tehterIsFire[0] ? new(100, 0, 100 - far - dis) : new(100 - dis, 0, 100 - far);
                    Vector3 i1p2 = tehterIsFire[2] ? new(100, 0, 100 - far - dis) : new(100 - dis, 0, 100 - far);
                    Vector3 i2p1 = tehterIsFire[0] ? new(100, 0, 100 - far - dis) : new(100 + dis, 0, 100 - far);
                    Vector3 i2p2 = tehterIsFire[2] ? new(100, 0, 100 - far - dis) : new(100 + dis, 0, 100 - far);
                    Vector3 i3p1 = tehterIsFire[1] ? new(100, 0, 100 + far + dis) : new(100 - dis, 0, 100 + far);
                    Vector3 i3p2 = tehterIsFire[3] ? new(100, 0, 100 + far + dis) : new(100 - dis, 0, 100 + far);
                    Vector3 i4p1 = tehterIsFire[1] ? new(100, 0, 100 + far + dis) : new(100 + dis, 0, 100 + far);
                    Vector3 i4p2 = tehterIsFire[3] ? new(100, 0, 100 + far + dis) : new(100 + dis, 0, 100 + far);
                    Vector3 dealpos1 = default;
                    Vector3 dealpos2 = default;

                    dealpos1 = idleObjIndex.IndexOf(myindex) switch
                    {
                        0 => i1p1,
                        1 => i2p1,
                        2 => i3p1,
                        3 => i4p1,
                    };
                    dealpos2 = idleObjIndex.IndexOf(myindex) switch
                    {
                        0 => i1p2,
                        1 => i2p2,
                        2 => i3p2,
                        3 => i4p2,
                    };
                    var upgroup = (idleObjIndex.IndexOf(myindex) == 0 || idleObjIndex.IndexOf(myindex) == 1);

                    var dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P1_四连线_处理位置1";
                    dp.Scale = new(2);
                    dp.Owner = accessory.Data.Me;
                    dp.TargetPosition = dealpos1;
                    dp.ScaleMode |= ScaleMode.YByDistance;
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.DestoryAt = upgroup ? 5000 : 8500;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                    dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P1_四连线_处理位置2";
                    dp.Scale = new(2);
                    dp.Owner = accessory.Data.Me;
                    dp.TargetPosition = dealpos2;
                    dp.ScaleMode |= ScaleMode.YByDistance;
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.Delay = upgroup ? 5000 : 8500;
                    dp.DestoryAt = upgroup ? 6000 : 5000;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                }
            });
        }

        [ScriptMethod(name: "P1_塔_塔记录器", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4012[234567]|4013[15])$"], userControl: false)]
        public void P1_塔_塔记录器(Event @event, ScriptAccessory accessory)
        {
            if (parse != 1d) return;
            lock (this)
            {
                var pos = JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
                var count = @event["ActionId"] switch
                {
                    "40135" => 1,
                    "40131" => 1,
                    "40122" => 2,
                    "40123" => 3,
                    "40124" => 4,
                    "40125" => 2,
                    "40126" => 3,
                    "40127" => 4,
                };
                if (MathF.Abs(pos.Z - 100) < 1)
                {
                    P1塔[1] = count;
                }
                else
                {
                    if (pos.Z - 100 > 1) P1塔[2] = count;
                    else P1塔[0] = count;
                }
                if (pos.X - 100 > 1)
                {
                    P1塔[3] = 1;
                }
            }
        }
        [ScriptMethod(name: "P1_塔_雷火直线", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40134|40129)$"])]
        public void P1_塔_雷火直线(Event @event, ScriptAccessory accessory)
        {
            if (parse != 1d) return;
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var myindex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
            if (@event["ActionId"] == "40134")
            {
                //雷
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P1_塔_雷直线";
                dp.Scale = new(20, 40);
                dp.Owner = sid;
                dp.Color = accessory.Data.DefaultDangerColor;
                dp.DestoryAt = 8200;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Straight, dp);

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P1_塔_雷直线内";
                dp.Scale = new(10, 40);
                dp.Owner = sid;
                dp.Color = accessory.Data.DefaultDangerColor;
                dp.DestoryAt = 6500;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Straight, dp);

            }
            else
            {
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P1_塔_火直线";
                dp.Scale = new(10, 40);
                dp.Owner = sid;
                dp.Color = accessory.Data.DefaultDangerColor;
                dp.DestoryAt = 6500;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Straight, dp);
            }


        }
        [ScriptMethod(name: "P1_塔_塔处理位置", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40134|40129)$"])]
        public void P1_塔_塔处理位置(Event @event, ScriptAccessory accessory)
        {
            if (parse != 1d) return;
            Task.Delay(100).ContinueWith(t =>
            {
                var myindex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
                if (@event["ActionId"] == "40134")
                {
                    var eastTower = P1塔[3] == 1;
                    //雷
                    if (myindex==0|| myindex==1)
                    {
                        var dx = eastTower ? -10.5f : 10.5f;
                        var dy = myindex == 1 ? -5.5f : 5.5f;
                        var dp = accessory.Data.GetDefaultDrawProperties();
                        dp.Name = "P1_雷塔_塔处理位置_T";
                        dp.Scale = new(2);
                        dp.Owner = accessory.Data.Me;
                        dp.TargetPosition = new(100+dx,0,100+dy);
                        dp.ScaleMode |= ScaleMode.YByDistance;
                        dp.Color = accessory.Data.DefaultSafeColor;
                        dp.DestoryAt = 10500;
                        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                    }
                    else
                    {
                        var myIndex2 = myindex - 1;
                        Vector3 dealpos = default;
                        if (myIndex2 > 0 && myIndex2 <= P1塔[0]) dealpos = new(eastTower ? 113.08f : 86.92f, 0, 90.81f);
                        if (myIndex2 > P1塔[0] && myIndex2 <= P1塔[0]+ P1塔[1]) dealpos = new(eastTower ? 115.98f : 84.02f, 0, 100f);
                        if (myIndex2 > P1塔[0] + P1塔[1] && myIndex2 <= P1塔[0] + P1塔[1]+ P1塔[2]) dealpos = new(eastTower ? 113.08f : 86.92f, 0, 109.18f);

                        Vector3 towerpos = default;
                        if (myIndex2 > 0 && myIndex2 <= P1塔[0]) towerpos = new(eastTower ? 113.08f : 86.92f, 0, 90.81f);
                        if (myIndex2 > P1塔[0] && myIndex2 <= P1塔[0] + P1塔[1]) towerpos = new(eastTower ? 115.98f : 84.02f, 0, 100f);
                        if (myIndex2 > P1塔[0] + P1塔[1] && myIndex2 <= P1塔[0] + P1塔[1] + P1塔[2]) towerpos = new(eastTower ? 113.08f : 86.92f, 0, 109.18f);

                        var dp = accessory.Data.GetDefaultDrawProperties();
                        dp.Name = "P1_雷塔_塔处理位置_ND";
                        dp.Scale = new(2);
                        dp.Owner = accessory.Data.Me;
                        dp.TargetPosition = dealpos;
                        dp.ScaleMode |= ScaleMode.YByDistance;
                        dp.Color = accessory.Data.DefaultSafeColor;
                        dp.DestoryAt = 10500;
                        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                        dp = accessory.Data.GetDefaultDrawProperties();
                        dp.Name = "P1_雷塔_塔_ND";
                        dp.Scale = new(4);
                        dp.Position = towerpos;
                        dp.Color = accessory.Data.DefaultSafeColor;
                        dp.DestoryAt = 10500;
                        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Circle, dp);

                    }
                }
                else
                {
                    var eastTower = P1塔[3] == 1;
                    //火
                    if (myindex == 0 || myindex == 1)
                    {
                        var dx2 = eastTower ? -2f : 2f;
                        var dx1 = eastTower ? -5.5f : 5.5f;
                        var dy = myindex == 1 ? -5.5f : 5.5f;

                        var dp = accessory.Data.GetDefaultDrawProperties();
                        dp.Name = "P1_雷塔_塔处理位置_T1";
                        dp.Scale = new(2);
                        dp.Owner = accessory.Data.Me;
                        dp.TargetPosition = new(100 + dx1, 0, 100 + dy);
                        dp.ScaleMode |= ScaleMode.YByDistance;
                        dp.Color = accessory.Data.DefaultSafeColor;
                        dp.DestoryAt = 6500;
                        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                        dp = accessory.Data.GetDefaultDrawProperties();
                        dp.Name = "P1_雷塔_塔处理位置_T2";
                        dp.Scale = new(2);
                        dp.Position = new(100 + dx1, 0, 100 + dy);
                        dp.TargetPosition = new(100 + dx2, 0, 100 + dy);
                        dp.ScaleMode |= ScaleMode.YByDistance;
                        dp.Color = accessory.Data.DefaultSafeColor;
                        dp.DestoryAt = 6500;
                        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                        dp = accessory.Data.GetDefaultDrawProperties();
                        dp.Name = "P1_雷塔_塔处理位置_T3";
                        dp.Scale = new(2);
                        dp.Owner = accessory.Data.Me;
                        dp.TargetPosition = new(100 + dx2, 0, 100 + dy);
                        dp.ScaleMode |= ScaleMode.YByDistance;
                        dp.Color = accessory.Data.DefaultSafeColor;
                        dp.Delay = 6500;
                        dp.DestoryAt = 1700;
                        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                    }
                    else
                    {
                        var myIndex2 = myindex - 1;
                        Vector3 dealpos = default;
                        if (myIndex2 > 0 && myIndex2 <= P1塔[0]) dealpos = new(eastTower ? 102f : 98f, 0, 90.81f);
                        if (myIndex2 > P1塔[0] && myIndex2 <= P1塔[0] + P1塔[1]) dealpos = new(eastTower ? 102f : 98f, 0, 100f);
                        if (myIndex2 > P1塔[0] + P1塔[1] && myIndex2 <= P1塔[0] + P1塔[1] + P1塔[2]) dealpos = new(eastTower ? 102f : 98f, 0, 109.18f);

                        Vector3 towerpos = default;
                        if (myIndex2 > 0 && myIndex2 <= P1塔[0]) towerpos = new(eastTower ? 113.08f : 86.92f, 0, 90.81f);
                        if (myIndex2 > P1塔[0] && myIndex2 <= P1塔[0] + P1塔[1]) towerpos = new(eastTower ? 115.98f : 84.02f, 0, 100f);
                        if (myIndex2 > P1塔[0] + P1塔[1] && myIndex2 <= P1塔[0] + P1塔[1] + P1塔[2]) towerpos = new(eastTower ? 113.08f : 86.92f, 0, 109.18f);

                        var dp = accessory.Data.GetDefaultDrawProperties();
                        dp.Name = "P1_雷塔_塔处理位置_ND";
                        dp.Scale = new(2);
                        dp.Owner = accessory.Data.Me;
                        dp.TargetPosition = dealpos;
                        dp.ScaleMode |= ScaleMode.YByDistance;
                        dp.Color = accessory.Data.DefaultSafeColor;
                        dp.DestoryAt = 9000;
                        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                        dp = accessory.Data.GetDefaultDrawProperties();
                        dp.Name = "P1_雷塔_塔_ND";
                        dp.Scale = new(4);
                        dp.Position = towerpos;
                        dp.Color = accessory.Data.DefaultSafeColor;
                        dp.DestoryAt = 10500;
                        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Circle, dp);

                    }
                }
            });
            
        }

        #endregion

        #region P2
        [ScriptMethod(name: "P2_换P", eventType: EventTypeEnum.Director, eventCondition: ["Instance:800375BF", "Command:8000001E"],userControl:false)]
        public void P2_换P(Event @event, ScriptAccessory accessory)
        {
            parse = 2d;
        }

        [ScriptMethod(name: "P2_钻石星尘_BossId记录", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40180)$"], userControl: false)]
        public void P2_钻石星尘_BossId记录(Event @event, ScriptAccessory accessory)
        {
            parse = 2.1d;
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            P2BossId = sid;
            P2DDIceDir.Clear();
        }
        [ScriptMethod(name: "P2_钻石星尘_钢铁月环记录", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^((4020[23]))$"],userControl: false)]
        public void P2_钻石星尘_钢铁月环记录(Event @event, ScriptAccessory accessory)
        {
            P2DDDircle = (@event["ActionId"] == "40202");//钢铁
        }
        [ScriptMethod(name: "P2_钻石星尘_钢铁月环", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^((4020[23]))$"])]
        public void P2_钻石星尘_钢铁月环(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (@event["ActionId"]=="40202")//钢铁
            {
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P2_钻石星尘_钢铁";
                dp.Scale = new(16);
                dp.Owner = sid;
                dp.Color = accessory.Data.DefaultDangerColor;
                dp.DestoryAt = 6000;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            }
            else
            {
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P2_钻石星尘_月环";
                dp.Scale = new(20);
                dp.InnerScale = new(4);
                dp.Radian = float.Pi * 2;
                dp.Owner = sid;
                dp.Color = accessory.Data.DefaultDangerColor;
                dp.DestoryAt = 6000;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Donut, dp);
            }
        }
        [ScriptMethod(name: "P2_钻石星尘_扇形引导", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^((4020[23]))$"])]
        public void P2_钻石星尘_扇形引导(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dur = 3000;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2_钻石星尘_扇形引导1";
            dp.Scale = new(60);
            dp.Radian = float.Pi / 6;
            dp.Owner = sid;
            dp.TargetResolvePattern=PositionResolvePatternEnum.PlayerNearestOrder;
            dp.TargetOrderIndex = 1;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 7000 - dur;
            dp.DestoryAt = dur;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2_钻石星尘_扇形引导2";
            dp.Scale = new(60);
            dp.Radian = float.Pi / 6;
            dp.Owner = sid;
            dp.TargetResolvePattern = PositionResolvePatternEnum.PlayerNearestOrder;
            dp.TargetOrderIndex = 2;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 7000 - dur;
            dp.DestoryAt = dur;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2_钻石星尘_扇形引导3";
            dp.Scale = new(60);
            dp.Radian = float.Pi / 6;
            dp.Owner = sid;
            dp.TargetResolvePattern = PositionResolvePatternEnum.PlayerNearestOrder;
            dp.TargetOrderIndex = 3;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 7000 - dur;
            dp.DestoryAt = dur;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2_钻石星尘_扇形引导4";
            dp.Scale = new(60);
            dp.Radian = float.Pi / 6;
            dp.Owner = sid;
            dp.TargetResolvePattern = PositionResolvePatternEnum.PlayerNearestOrder;
            dp.TargetOrderIndex = 4;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 7000 - dur;
            dp.DestoryAt = dur;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);


        }
        [ScriptMethod(name: "P2_钻石星尘_冰花放置位置", eventType: EventTypeEnum.TargetIcon)]
        public void P2_钻石星尘_冰花放置位置(Event @event, ScriptAccessory accessory)
        {
            //accessory.Log.Debug($"{ParsTargetIcon(@event["Id"])}");
            if (ParsTargetIcon(@event["Id"]) != 127) return;
            if (parse != 2.1) return;
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            if (tid != accessory.Data.Me) return;
            var myIndex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
            var rot = myIndex switch
            {
                0 => 6,
                1 => 0,
                2 => 4,
                3 => 2,
                4 => 4,
                5 => 2,
                6 => 6,
                7 => 0,
                _ => 0,
            };
            Vector3 epos1 = P2DDDircle ? new(119.5f, 0, 100.0f) : new(103.5f, 0, 100.0f);
            Vector3 epos2 = P2DDDircle ? new(119.5f, 0, 100.0f) : new(108.0f, 0, 100.0f);
            var dir8 = P2DDIceDir.FirstOrDefault() % 4;
            var dr = dir8 == 0 || dir8 == 2 ? -1 : 0;
            var dealpos1 = RotatePoint(epos1, new(100, 0, 100), float.Pi / 4 * (rot + dr));
            var dealpos2 = RotatePoint(epos2, new(100, 0, 100), float.Pi / 4 * (rot + dr));
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2_钻石星尘_冰花放置位置1";
            dp.Scale = new(2);
            dp.ScaleMode |= ScaleMode.YByDistance;
            dp.Owner = accessory.Data.Me;
            dp.TargetPosition= dealpos1;
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.DestoryAt = 5500;
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2_钻石星尘_冰花放置位置3";
            dp.Scale = new(2);
            dp.ScaleMode |= ScaleMode.YByDistance;
            dp.Position = dealpos1;
            dp.TargetPosition = dealpos2;
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.DestoryAt = 5500;
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2_钻石星尘_冰花放置位置3";
            dp.Scale = new(2);
            dp.ScaleMode |= ScaleMode.YByDistance;
            dp.Owner = accessory.Data.Me;
            dp.TargetPosition = dealpos2;
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.Delay = 5500;
            dp.DestoryAt = 2500;
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
        }
        [ScriptMethod(name: "P2_钻石星尘_扇形引导位置", eventType: EventTypeEnum.TargetIcon)]
        public void P2_钻石星尘_扇形引导位置(Event @event, ScriptAccessory accessory)
        {
            //accessory.Log.Debug($"{ParsTargetIcon(@event["Id"])}");
            if (ParsTargetIcon(@event["Id"]) != 127) return;
            if (parse != 2.1) return;
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            var myIndex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
            int[] group = [6, 7, 4, 5, 2, 3, 0, 1];
            if (accessory.Data.PartyList.IndexOf(tid) != group[myIndex]) return;
            var rot = myIndex switch
            {
                0 => 6,
                1 => 0,
                2 => 4,
                3 => 2,
                4 => 4,
                5 => 2,
                6 => 6,
                7 => 0,
                _ => 0,
            };
            var dir8 = P2DDIceDir.FirstOrDefault() % 4;
            var dr = dir8 == 0 || dir8 == 2 ? 0 : -1;
            Vector3 epos = P2DDDircle ? new(116.5f, 0, 100f): new(101f, 0, 100f);
            var dealpos = RotatePoint(epos, new(100, 0, 100), float.Pi / 4 * (rot+dr));
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2_钻石星尘_扇形引导位置";
            dp.Scale = new(2);
            dp.ScaleMode |= ScaleMode.YByDistance;
            dp.Owner = accessory.Data.Me;
            dp.TargetPosition = dealpos;
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.DestoryAt = 6500;
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
        }
        [ScriptMethod(name: "P2_钻石星尘_九连环记录", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40198)$"], userControl: false)]
        public void P2_钻石星尘_九连环记录(Event @event, ScriptAccessory accessory)
        {
            if (parse != 2.1) return;
            var pos= JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
            lock (P2DDIceDir)
            {
                P2DDIceDir.Add(PositionTo8Dir(pos, new(100, 0, 100)));
            }
        }
        [ScriptMethod(name: "P2_钻石星尘_击退位置", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:regex:^((4020[23]))$", "TargetIndex:1"])]
        public void P2_钻石星尘_击退位置(Event @event, ScriptAccessory accessory)
        {
            if (parse != 2.1) return;
            Task.Delay(2500).ContinueWith(t =>
            {
                var nPos = new Vector3(100, 0, 96);
                var dir8 = P2DDIceDir.FirstOrDefault() % 4;
                int[] h1Group = [0, 2, 4, 6];
                var myIndex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
                var isH1Group = h1Group.Contains(myIndex);
                
                var rot = dir8 switch
                {
                    0 => 4,
                    1 => 1,
                    2 => 2,
                    3 => 3,
                };
                
                rot += isH1Group ? 4 : 0;
                var dealpos = RotatePoint(nPos, new(100, 0, 100), float.Pi / 4 * rot);
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P2_钻石星尘_击退位置";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = dealpos;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = 6000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
            });
            
        }
        [ScriptMethod(name: "P2_钻石星尘_连续剑分身位置", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:regex:^40208$", "TargetIndex:1"])]
        public void P2_钻石星尘_连续剑分身位置(Event @event, ScriptAccessory accessory)
        {
            if (parse != 2.1) return;
            var pos = JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);

            Vector3 dealpos = new(100 + (pos.X - 100) * 1.4f, 0, 100 + (pos.Z - 100) * 1.4f);
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2_钻石星尘_连续剑分身位置";
            dp.Scale = new(2);
            dp.ScaleMode |= ScaleMode.YByDistance;
            dp.Owner = accessory.Data.Me;
            dp.TargetPosition = dealpos;
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.DestoryAt = 9000;
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);


        }
        [ScriptMethod(name: "P2_钻石星尘_连续剑范围", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^4019[34]$"])]
        public void P2_钻石星尘_连续剑范围(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var time = 300;
            //93 先正面
            if (@event["ActionId"]=="40193")
            {
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P2_钻石星尘_连续剑范围正1";
                dp.Scale = new(30);
                dp.Radian = float.Pi / 2 * 3;
                dp.Owner = sid;
                dp.Color = accessory.Data.DefaultDangerColor;
                dp.DestoryAt = 3500-time;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P2_钻石星尘_连续剑范围反2";
                dp.Scale = new(30);
                dp.Radian = float.Pi / 2;
                dp.Rotation = float.Pi;
                dp.Owner = sid;
                dp.Color = accessory.Data.DefaultDangerColor;
                dp.Delay = 3500-time;
                dp.DestoryAt = 2000;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
            }
            else
            {
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P2_钻石星尘_连续剑范围反1";
                dp.Scale = new(30);
                dp.Radian = float.Pi / 2;
                dp.Rotation = float.Pi;
                dp.Owner = sid;
                dp.Color = accessory.Data.DefaultDangerColor;
                dp.DestoryAt = 3500-time;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P2_钻石星尘_连续剑范围正2";
                dp.Scale = new(30);
                dp.Radian = float.Pi / 2 * 3;
                dp.Owner = sid;
                dp.Color = accessory.Data.DefaultDangerColor;
                dp.Delay = 3500-time;
                dp.DestoryAt = 2000;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
            }
        }
        [ScriptMethod(name: "P2_钻石星尘_Boss背对", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:regex:^40208$", "TargetIndex:1"])]
        public void P2_钻石星尘_Boss背对(Event @event, ScriptAccessory accessory)
        {
            if (parse != 2.1) return;
            
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2_钻石星尘_Boss背对";
            dp.Scale = new(5);
            dp.Owner = accessory.Data.Me;
            dp.TargetObject=P2BossId;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 3000;
            dp.DestoryAt = 6000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.SightAvoid, dp);


        }

        [ScriptMethod(name: "P2_双镜_分P", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40179)$"], userControl: false)]
        public void P2_双镜_分P(Event @event, ScriptAccessory accessory)
        {
            parse = 2.2d;
            P2RedMirror.Clear();
        }
        [ScriptMethod(name: "P2_双镜_分散分摊", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4022[01])$"])]
        public void P2_双镜_分散分摊(Event @event, ScriptAccessory accessory)
        {
            if(parse != 2.2) return;
            if (@event["ActionId"]=="40221")
            {
                foreach (var pm in accessory.Data.PartyList)
                {
                    var dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P2_双镜_分散";
                    dp.Scale = new(5);
                    dp.Owner = pm;
                    dp.Color = accessory.Data.DefaultDangerColor;
                    dp.DestoryAt = 5000;
                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
                }
            }
            else
            {
                int[] group = [6, 7, 4, 5, 2, 3, 0, 1];
                var myindex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
                for (int i = 0; i < 4; i++)
                {
                    var dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P2_双镜_分摊";
                    dp.Scale = new(5);
                    dp.Owner = accessory.Data.PartyList[i];
                    dp.Color = group[myindex]==i||i==myindex?accessory.Data.DefaultSafeColor: accessory.Data.DefaultDangerColor;
                    dp.DestoryAt = 5000;
                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
                }
            }
            
        }
        [ScriptMethod(name: "P2_双镜_蓝镜月环加引导", eventType: EventTypeEnum.EnvControl, eventCondition: ["DirectorId:800375BF", "State:00020001"])]
        public void P2_双镜_蓝镜月环加引导(Event @event, ScriptAccessory accessory)
        {
            if (parse != 2.2) return;
            if (!int.TryParse(@event["Index"], out var dir8)) return;
            Vector3 npos = new(100, 0, 80);
            dir8--;
            Vector3 dealpos = RotatePoint(npos, new(100, 0, 100), float.Pi / 4 * dir8);
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2_双镜_蓝镜月环";
            dp.Scale = new(20);
            dp.InnerScale = new(4);
            dp.Radian = float.Pi * 2;
            dp.Position = dealpos;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 6000;
            dp.DestoryAt = 6000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Donut, dp);

             dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2_双镜_蓝镜扇形引导1";
            dp.Scale = new(60);
            dp.Radian = float.Pi / 6;
            dp.Position = dealpos;
            dp.TargetResolvePattern = PositionResolvePatternEnum.PlayerNearestOrder;
            dp.TargetOrderIndex = 1;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 6000;
            dp.DestoryAt = 7000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2_双镜_蓝镜扇形引导2";
            dp.Scale = new(60);
            dp.Radian = float.Pi / 6;
            dp.Position = dealpos;
            dp.TargetResolvePattern = PositionResolvePatternEnum.PlayerNearestOrder;
            dp.TargetOrderIndex = 2;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 6000;
            dp.DestoryAt = 7000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2_双镜_蓝镜扇形引导3";
            dp.Scale = new(60);
            dp.Radian = float.Pi / 6;
            dp.Position = dealpos;
            dp.TargetResolvePattern = PositionResolvePatternEnum.PlayerNearestOrder;
            dp.TargetOrderIndex = 3;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 6000;
            dp.DestoryAt = 7000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2_双镜_蓝镜扇形引导4";
            dp.Scale = new(60);
            dp.Radian = float.Pi / 6;
            dp.Position = dealpos;
            dp.TargetResolvePattern = PositionResolvePatternEnum.PlayerNearestOrder;
            dp.TargetOrderIndex = 4;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 6000;
            dp.DestoryAt = 7000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);

        }
        [ScriptMethod(name: "P2_双镜_红镜月环加引导", eventType: EventTypeEnum.EnvControl, eventCondition: ["DirectorId:800375BF", "State:02000100"])]
        public void P2_双镜_红月环加引导(Event @event, ScriptAccessory accessory)
        {
            if (parse != 2.2) return;
            if (!int.TryParse(@event["Index"], out var dir8)) return;
            Vector3 npos = new(100, 0, 80);
            dir8--;
            Vector3 dealpos = RotatePoint(npos, new(100, 0, 100), float.Pi / 4 * dir8);
            var dur = 3000;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2_双镜_红镜月环";
            dp.Scale = new(20);
            dp.InnerScale = new(4);
            dp.Radian = float.Pi * 2;
            dp.Position = dealpos;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 17500;
            dp.DestoryAt = 5000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Donut, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2_双镜_红镜扇形引导1";
            dp.Scale = new(60);
            dp.Radian = float.Pi / 6;
            dp.Position = dealpos;
            dp.TargetResolvePattern = PositionResolvePatternEnum.PlayerNearestOrder;
            dp.TargetOrderIndex = 1;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 23000-dur;
            dp.DestoryAt = dur;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2_双镜_红镜扇形引导2";
            dp.Scale = new(60);
            dp.Radian = float.Pi / 6;
            dp.Position = dealpos;
            dp.TargetResolvePattern = PositionResolvePatternEnum.PlayerNearestOrder;
            dp.TargetOrderIndex = 2;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 23000 - dur;
            dp.DestoryAt = dur;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2_双镜_红镜扇形引导3";
            dp.Scale = new(60);
            dp.Radian = float.Pi / 6;
            dp.Position = dealpos;
            dp.TargetResolvePattern = PositionResolvePatternEnum.PlayerNearestOrder;
            dp.TargetOrderIndex = 3;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 23000 - dur;
            dp.DestoryAt = dur;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2_双镜_红镜扇形引导4";
            dp.Scale = new(60);
            dp.Radian = float.Pi / 6;
            dp.Position = dealpos;
            dp.TargetResolvePattern = PositionResolvePatternEnum.PlayerNearestOrder;
            dp.TargetOrderIndex = 4;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 23000 - dur;
            dp.DestoryAt = dur;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);

        }
        [ScriptMethod(name: "P2_双镜_蓝镜月环加引导位置", eventType: EventTypeEnum.EnvControl, eventCondition: ["DirectorId:800375BF", "State:00020001"])]
        public void P2_双镜_蓝镜月环加引导位置(Event @event, ScriptAccessory accessory)
        {
            if (parse != 2.2) return;
            if (!int.TryParse(@event["Index"], out var dir8)) return;
            Vector3 npos = new(100, 0, 80);
            dir8--;
            var myindex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
            if (myindex == 0 || myindex == 1 || myindex == 4 || myindex == 5)
            {
                dir8 += 4;
                npos = new(100, 0, 85);
            }
           
            Vector3 dealpos = RotatePoint(npos, new(100, 0, 100), float.Pi / 4 * dir8);
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2_双镜_蓝镜月环加引导位置";
            dp.Scale = new(2);
            dp.ScaleMode |= ScaleMode.YByDistance;
            dp.Owner = accessory.Data.Me;
            dp.TargetPosition = dealpos;
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.DestoryAt = 12000;
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
        }
        [ScriptMethod(name: "P2_双镜_红镜引导位置", eventType: EventTypeEnum.EnvControl, eventCondition: ["DirectorId:800375BF", "State:02000100"])]
        public void P2_双镜_红镜引导位置(Event @event, ScriptAccessory accessory)
        {
            if (parse != 2.2) return;
            if (!int.TryParse(@event["Index"], out var dir8)) return;
            dir8--;
            lock (P2RedMirror)
            {
                P2RedMirror.Add(dir8);
                if (P2RedMirror.Count != 2) return;
            }
            var leftRot8 = (P2RedMirror[0] - P2RedMirror[1] == -2 || P2RedMirror[0] - P2RedMirror[1] - 8 == -2) ? P2RedMirror[0] : P2RedMirror[1];
            var rightRot8 = (P2RedMirror[0] - P2RedMirror[1] == 2 || P2RedMirror[0] + 8 - P2RedMirror[1] == 2) ? P2RedMirror[0] : P2RedMirror[1];

            var myrot = leftRot8;
            var myindex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
            if (myindex == 0 || myindex == 1 || myindex == 4 || myindex == 5)
            {
                myrot = rightRot8;
            }
            Vector3 npos = myindex switch
            {
                0 => new(102f, 0, 80.5f),
                1 => new(98f, 0, 80.5f),
                2 => new(102f, 0, 80.5f),
                3 => new(98f, 0, 80.5f),
                4 => new(101.3f, 0, 83f),
                5 => new(98.7f, 0, 83f),
                6 => new(101.3f, 0, 83f),
                7 => new(98.7f, 0, 83f),
                _ => new(100, 0, 80)
            };
            var dealpos = RotatePoint(npos, new(100, 0, 100), myrot * float.Pi / 4);
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2_双镜_红镜引导位置";
            dp.Scale = new(2);
            dp.ScaleMode |= ScaleMode.YByDistance;
            dp.Owner = accessory.Data.Me;
            dp.TargetPosition = dealpos;
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.Delay = 13500;
            dp.DestoryAt = 9000;
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
        }

        [ScriptMethod(name: "P2_光之暴走_分P", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40212)$"], userControl: false)]
        public void P2_光之暴走_分P(Event @event, ScriptAccessory accessory)
        {
            parse = 2.3d;
            P2LightRampantCircle.Clear();
            P2LightRampantTetherDone = false;
            P2LightRampantBuff = [0, 0, 0, 0, 0, 0, 0, 0];
        }
        [ScriptMethod(name: "P2_光之暴走_大圈收集", eventType: EventTypeEnum.TargetIcon,userControl:false)]
        public void P2_光之暴走_大圈收集(Event @event, ScriptAccessory accessory)
        {
            if (ParsTargetIcon(@event["Id"]) != 157) return;
            if (parse != 2.3) return;
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            var index=accessory.Data.PartyList.IndexOf(tid);
            lock (P2LightRampantCircle)
            {
                P2LightRampantCircle.Add(index);
            }
        }
        [ScriptMethod(name: "P2_光之暴走_Buff收集", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:2257"], userControl: false)]
        public void P2_光之暴走_Buff收集(Event @event, ScriptAccessory accessory)
        {
            
            if (parse != 2.3) return;
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            if (!int.TryParse(@event["StackCount"], out var count)) return;
            var index = accessory.Data.PartyList.IndexOf(tid);
            lock (P2LightRampantBuff)
            {
                P2LightRampantBuff[index] = count;
            }
        }
        [ScriptMethod(name: "P2_光之暴走_分散分摊", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4022[01])$"])]
        public void P2_光之暴走_分散分摊(Event @event, ScriptAccessory accessory)
        {
            if (parse != 2.3) return;
            if (@event["ActionId"] == "40221")
            {
                foreach (var pm in accessory.Data.PartyList)
                {
                    var dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P2_光之暴走_分散";
                    dp.Scale = new(5);
                    dp.Owner = pm;
                    dp.Color = accessory.Data.DefaultDangerColor;
                    dp.DestoryAt = 5000;
                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
                }
            }
            else
            {
                int[] group = [6, 7, 4, 5, 2, 3, 0, 1];
                var myindex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
                for (int i = 0; i < 4; i++)
                {
                    var dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P2_光之暴走_分摊";
                    dp.Scale = new(5);
                    dp.Owner = accessory.Data.PartyList[i];
                    dp.Color = group[myindex] == i || i == myindex ? accessory.Data.DefaultSafeColor : accessory.Data.DefaultDangerColor;
                    dp.DestoryAt = 5000;
                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
                }
            }

        }
        [ScriptMethod(name: "P2_光之暴走_分摊buff", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:4159"])]
        public void P2_光之暴走_分摊buff(Event @event, ScriptAccessory accessory)
        {
            if (parse != 2.3) return;
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2_光之暴走_分摊buff";
            dp.Scale = new(5);
            dp.Owner = tid;
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.Delay = 12000;
            dp.DestoryAt = 5000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
        [ScriptMethod(name: "P2_光之暴走_塔处理位置", eventType: EventTypeEnum.TargetIcon)]
        public void P2_光之暴走_塔处理位置(Event @event, ScriptAccessory accessory)
        {

            if (ParsTargetIcon(@event["Id"]) != 157) return;
            if (parse != 2.3) return;
            lock (this)
            {
                if (P2LightRampantTetherDone) return;
                P2LightRampantTetherDone = true;
            }
            Task.Delay(50).ContinueWith(t =>
            {
                var myindex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
                if (P2LightRampantCircle.Contains(myindex)) return;
                
                List<int> tetherGroup = [];
                if (P2LightRampant8DirSet == P2LightRampant8DirEmum.Normal)
                {
                    if (!P2LightRampantCircle.Contains(2)) tetherGroup.Add(2);
                    if (!P2LightRampantCircle.Contains(6)) tetherGroup.Add(6);
                    if (!P2LightRampantCircle.Contains(0)) tetherGroup.Add(0);
                    if (!P2LightRampantCircle.Contains(7)) tetherGroup.Add(7);
                    if (!P2LightRampantCircle.Contains(3)) tetherGroup.Add(3);
                    if (!P2LightRampantCircle.Contains(5)) tetherGroup.Add(5);
                    if (!P2LightRampantCircle.Contains(1)) tetherGroup.Add(1);
                    if (!P2LightRampantCircle.Contains(4)) tetherGroup.Add(4);
                }
                if (P2LightRampant8DirSet == P2LightRampant8DirEmum.TN_Up)
                {
                    if (!P2LightRampantCircle.Contains(0)) tetherGroup.Add(0);
                    if (!P2LightRampantCircle.Contains(1)) tetherGroup.Add(1);
                    if (!P2LightRampantCircle.Contains(2)) tetherGroup.Add(2);
                    if (!P2LightRampantCircle.Contains(3)) tetherGroup.Add(3);
                    if (!P2LightRampantCircle.Contains(7)) tetherGroup.Add(7);
                    if (!P2LightRampantCircle.Contains(6)) tetherGroup.Add(6);
                    if (!P2LightRampantCircle.Contains(5)) tetherGroup.Add(5);
                    if (!P2LightRampantCircle.Contains(4)) tetherGroup.Add(4);
                }


                var myGroupIndex = tetherGroup.IndexOf(myindex);
                Vector3 t1 = new(100.00f, 0, 084.00f);
                Vector3 t2 = new(113.85f, 0, 092.00f);
                Vector3 t3 = new(113.85f, 0, 108.00f);
                Vector3 t4 = new(100.00f, 0, 116.00f);
                Vector3 t5 = new(086.14f, 0, 108.00f);
                Vector3 t6 = new(086.14f, 0, 092.00f);

                Vector3 pa = new(100.00f, 0, 82.00f);
                Vector3 pb = new(118.00f, 0, 100.00f);
                Vector3 pc = new(100.00f, 0, 118.00f);
                Vector3 pd = new(82.00f, 0, 100.00f);


                Vector3 dealpos = default;
                Vector3 dealpos2 = default;
                if (P2LightRampantTetherDeal == P2LightRampantTetherEmum.CircleNum)
                {
                    var count = 0;
                    if (myindex == 0)
                    {
                        dealpos = t4;
                    }
                    count += P2LightRampantCircle.Contains(0) ? 1 : 0;
                    if (myindex == 7)
                    {
                        dealpos = P2LightRampantCircle.Contains(0) ? t4 : t2;
                    }
                    count += P2LightRampantCircle.Contains(7) ? 1 : 0;
                    if (myindex == 1)
                    {
                        if (count == 0) dealpos = t6;
                        if (count == 1) dealpos = t2;
                        if (count == 2) dealpos = t4;
                    }
                    count += P2LightRampantCircle.Contains(1) ? 1 : 0;
                    if (myindex == 5)
                    {
                        if (count == 0) dealpos = t3;
                        if (count == 1) dealpos = t6;
                        if (count == 2) dealpos = t2;
                    }
                    count += P2LightRampantCircle.Contains(5) ? 1 : 0;
                    if (myindex == 3)
                    {
                        if (count == 0) dealpos = t5;
                        if (count == 1) dealpos = t3;
                        if (count == 2) dealpos = t6;
                    }
                    count += P2LightRampantCircle.Contains(3) ? 1 : 0;
                    if (myindex == 4)
                    {
                        if (count == 0) dealpos = t1;
                        if (count == 1) dealpos = t5;
                        if (count == 2) dealpos = t3;
                    }
                    count += P2LightRampantCircle.Contains(4) ? 1 : 0;
                    if (myindex == 2)
                    {
                        dealpos = P2LightRampantCircle.Contains(6) ? t1 : t5;
                    }
                    if (myindex == 6)
                    {
                        dealpos = t1;
                    }


                    if ((dealpos - t1).Length() < 1 || (dealpos - t2).Length() < 1 || (dealpos - t3).Length() < 1)
                    {
                        dealpos2 = pb;
                    }
                    else
                    {
                        dealpos2 = pd;
                    }
                }
                if (P2LightRampantTetherDeal == P2LightRampantTetherEmum.NewGrey9)
                {
                    var count = 0;
                    if (myindex == 0)
                    {
                        dealpos = t4;
                    }
                    count += P2LightRampantCircle.Contains(0) ? 1 : 0;
                    if (myindex == 7)
                    {
                        dealpos = P2LightRampantCircle.Contains(0) ? t4 : t6;
                    }
                    count += P2LightRampantCircle.Contains(7) ? 1 : 0;
                    if (myindex == 1)
                    {
                        if (count == 0) dealpos = t2;
                        if (count == 1) dealpos = t6;
                        if (count == 2) dealpos = t4;
                    }
                    count += P2LightRampantCircle.Contains(1) ? 1 : 0;
                    if (myindex == 5)
                    {
                        if (count == 0) dealpos = t5;
                        if (count == 1) dealpos = t2;
                        if (count == 2) dealpos = t6;
                    }
                    count += P2LightRampantCircle.Contains(5) ? 1 : 0;
                    if (myindex == 3)
                    {
                        if (count == 0) dealpos = t3;
                        if (count == 1) dealpos = t5;
                        if (count == 2) dealpos = t2;
                    }
                    count += P2LightRampantCircle.Contains(3) ? 1 : 0;
                    if (myindex == 4)
                    {
                        if (count == 0) dealpos = t1;
                        if (count == 1) dealpos = t3;
                        if (count == 2) dealpos = t5;
                    }
                    count += P2LightRampantCircle.Contains(4) ? 1 : 0;
                    if (myindex == 2)
                    {
                        dealpos = P2LightRampantCircle.Contains(6) ? t1 : t3;
                    }
                    if (myindex == 6)
                    {
                        dealpos = t1;
                    }

                    if ((dealpos - t2).Length() < 1 || (dealpos - t3).Length() < 1 || (dealpos - t4).Length() < 1)
                    {
                        dealpos2 = pb;
                    }
                    else
                    {
                        dealpos2 = pd;
                    }
                }
                if (P2LightRampantTetherDeal == P2LightRampantTetherEmum.LTeam)
                {
                    dealpos = myGroupIndex switch
                    {
                        1 => t1,
                        4 => t4,
                        0 => t5,
                        2 => t3,
                        3 => t6,
                        5 => t2,
                    };
                    if ((dealpos - t1).Length() < 1 || (dealpos - t2).Length() < 1 || (dealpos - t6).Length() < 1)
                    {
                        dealpos2 = pa;
                    }
                    else
                    {
                        dealpos2 = pc;
                    }
                }
                if (P2LightRampantTetherDeal == P2LightRampantTetherEmum.AC_Cross)
                {
                    dealpos = myGroupIndex switch
                    {
                        1 => t4,
                        4 => t1,
                        0 => t6,
                        2 => t2,
                        3 => t5,
                        5 => t3,
                    };
                    if ((dealpos - t1).Length() < 1 || (dealpos - t2).Length() < 1 || (dealpos - t6).Length() < 1)
                    {
                        dealpos2 = pa;
                    }
                    else
                    {
                        dealpos2 = pc;
                    }
                }

                

                var dur = 10000;
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P2_光之暴走_塔处理位置";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = dealpos;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = dur;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P2_光之暴走_塔处理位置";
                dp.Scale = new(4);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Position = dealpos;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = dur;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Circle, dp);

                

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P2_光之暴走_集合位置";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Position = dealpos;
                dp.TargetPosition = dealpos2;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = dur;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P2_光之暴走_集合位置";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = dealpos2;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.Delay = dur;
                dp.DestoryAt = 6000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
            });

        }
        [ScriptMethod(name: "P2_双镜_中央踩塔位置", eventType: EventTypeEnum.EnvControl, eventCondition: ["DirectorId:800375BF", "State:00020001", "Index:00000015"])]
        public void P2_光之暴走_中央踩塔位置(Event @event, ScriptAccessory accessory)
        {
            if (parse != 2.3) return;
            var myindex= accessory.Data.PartyList.IndexOf(accessory.Data.Me);
            if (P2LightRampantBuff[myindex]<=2)
            {
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P2_光之暴走_中央踩塔位置";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = new(100,0,100);
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = 8000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P2_光之暴走_塔处理位置";
                dp.Scale = new(4);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Position = new(100, 0, 100);
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = 8000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Circle, dp);
            }
        }
        
        [ScriptMethod(name: "P2_光之暴走_八方分散位置", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:regex:^(4022[01])$"])]
        public void P2_光之暴走_八方分散位置(Event @event, ScriptAccessory accessory)
        {
            if (parse != 2.3) return;
            var myindex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
            var rot8 = myindex switch
            {
                0 => 0,
                1 => 2,
                2 => 6,
                3 => 4,
                4 => 5,
                5 => 3,
                6 => 7,
                7 => 1,
                _ => 0,
            };
            var mPosEnd = RotatePoint(new(100, 0, 95), new(100, 0, 100), float.Pi / 4 * rot8);

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2_光之暴走_八方分散位置";
            dp.Scale = new(2);
            dp.Owner = accessory.Data.Me;
            dp.TargetPosition = mPosEnd;
            dp.ScaleMode |= ScaleMode.YByDistance;
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.DestoryAt = 9000;
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

        }

        [ScriptMethod(name: "P2.5_暗水晶AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40262"])]
        public void P2_暗水晶AOE(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P2.5_暗水晶AOE";
            dp.Scale = new(50);
            dp.Radian = float.Pi / 9;
            dp.Owner = sid;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.DestoryAt = 3000;
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Fan, dp);
        }
        #endregion

        #region P3
        [ScriptMethod(name: "P3_时间压缩_分P", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40266)$"], userControl: false)]
        public void P3_时间压缩_分P(Event @event, ScriptAccessory accessory)
        {
            parse = 3.1d;
            P3FireBuff = [0, 0, 0, 0, 0, 0, 0, 0];
            P3WaterBuff= [0, 0, 0, 0, 0, 0, 0, 0];
            P3ReturnBuff = [0, 0, 0, 0, 0, 0, 0, 0];
            P3Lamp = [0, 0, 0, 0, 0, 0, 0, 0];
            P3LampWise = [0, 0, 0, 0, 0, 0, 0, 0];
        }
        [ScriptMethod(name: "P3_时间压缩_Buff记录", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:regex:^(2455|2456|2464|2462|2461|2460)$"], userControl: false)]
        public void P3_时间压缩_Buff记录(Event @event, ScriptAccessory accessory)
        {
            if (parse != 3.1) return;
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            if(!float.TryParse(@event["Duration"], out var dur)) return;
            var index = accessory.Data.PartyList.IndexOf(tid);
            if (index == -1) return;
            //冰
            if (@event["StatusID"] == "2462")
            {
                lock (P3FireBuff)
                {
                    P3FireBuff[index] = 4;
                }
            }
            //火
            if (@event["StatusID"] == "2455")
            {
                
                var count = 1;
                if (dur > 20) count = 2;
                if (dur > 30) count = 3;
                lock (P3FireBuff)
                {
                    P3FireBuff[index] = count;
                }
            }
            //回返
            if (@event["StatusID"] == "2464")
            {
                var count = 1;
                if (dur > 20) count = 3;
                lock (P3ReturnBuff)
                {
                    P3ReturnBuff[index] = count;
                }
            }
            //水
            if (@event["StatusID"] == "2461")
            {
                lock (P3WaterBuff)
                {
                    P3WaterBuff[index] = 1;
                }
            }
            //圈
            if (@event["StatusID"] == "2460")
            {
                lock (P3WaterBuff)
                {
                    P3WaterBuff[index] = 2;
                }
            }
            //背对
            if (@event["StatusID"] == "2456")
            {
                lock (P3WaterBuff)
                {
                    P3WaterBuff[index] = 3;
                }
            }



        }
        [ScriptMethod(name: "P3_时间压缩_灯记录", eventType: EventTypeEnum.Tether, eventCondition: ["Id:regex:^(0085|0086)$"], userControl: false)]
        public void P3_时间压缩_灯记录(Event @event, ScriptAccessory accessory)
        {
            //0085紫
            //0086黄
            if (parse != 3.1) return;
            var pos = JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
            var dir8= PositionTo8Dir(pos, new(100, 0, 100));
            lock (P3Lamp)
            {
                P3Lamp[dir8] = @event["Id"] == "0086" ? 1 : 2;
            }
        }
        [ScriptMethod(name: "P3_时间压缩_灯顺逆记录", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:2970"],userControl:false)]
        public void P3_时间压缩_灯顺逆记录(Event @event, ScriptAccessory accessory)
        {
            //buff2970, 13 269顺时针 92 348逆时针
            if (parse != 3.1) return;
            var pos = JsonConvert.DeserializeObject<Vector3>(@event["TargetPosition"]);
            Vector3 centre = new(100, 0, 100);
            var dir8 = PositionTo8Dir(pos, centre);
            P3LampWise[dir8] = @event["StackCount"] == "92" ? 1 : 0;
        }
        [ScriptMethod(name: "P3_时间压缩_灯AOE", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:40235", "TargetIndex:1"])]
        public void P3_时间压缩_灯AOE(Event @event, ScriptAccessory accessory)
        {
            if (parse != 3.1) return;
            var pos = JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
            var rot= JsonConvert.DeserializeObject<float>(@event["SourceRotation"]);
            Vector3 centre = new(100, 0, 100);
            var dir8 = PositionTo8Dir(pos, centre);
            var isWise = P3LampWise[dir8] == 1;
            for (int i = 0; i < 9; i++)
            {
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P3_时间压缩_灯AOE";
                dp.Scale = new(5,50);
                dp.Position = pos;
                dp.Rotation = rot + (i + 1) * float.Pi / 12 * (isWise ? -1 : 1);
                dp.Color = accessory.Data.DefaultDangerColor;
                dp.DestoryAt = 2000+(i*1000);
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
            }
        }
        [ScriptMethod(name: "P3_时间压缩_Buff处理位置", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40293"])]
        public void P3_时间压缩_Buff处理位置(Event @event, ScriptAccessory accessory)
        {
            if (parse != 3.1) return;
            var myIndex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
            if (myIndex == -1) return;
            var myDir8 = MyLampIndex(myIndex);
            //accessory.Log.Debug($"myDir8 {myDir8}");
            if (myDir8 == -1) return;
            var myRot = myDir8 * float.Pi / 4;

            Vector3 centre = new(100, 0, 100);
            Vector3 fireN = new(100, 0, 84.5f);
            Vector3 returnPosN = P3WaterBuff[myIndex] == 2 ? new(100, 0, 91.5f) : new(100, 0, 98);
            Vector3 stopPos = new(100, 0, 101);
            //火
            var myFire = P3FireBuff[myIndex];
            //短火
            if (myFire == 1)
            {
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P3_时间压缩_短火_放火";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = RotatePoint(fireN, centre, myRot);
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = 7500;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P3_时间压缩_短火_放回溯";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = RotatePoint(returnPosN, centre, myRot);
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.Delay = 7500;
                dp.DestoryAt = 5000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P3_时间压缩_短火_场中分摊";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = centre;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.Delay = 12500;
                dp.DestoryAt = 5000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);


                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P3_时间压缩_短火_输出位置";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = RotatePoint(stopPos, centre, myRot);
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.Delay = 22500;
                dp.DestoryAt = 15000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
            }

            //中火
            if (myFire == 2)
            {
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P3_时间压缩_中火_中场分摊";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = centre;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = 7500;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P3_时间压缩_中火_放回溯";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = RotatePoint(returnPosN, centre, myRot);
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.Delay = 7500;
                dp.DestoryAt = 5000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P3_时间压缩_中火_放火";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = RotatePoint(fireN, centre, myRot);
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.Delay = 12500;
                dp.DestoryAt = 5000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P3_时间压缩_中火_中场";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition =centre;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.Delay = 17500;
                dp.DestoryAt = 10000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P3_时间压缩_中火_输出位置";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = RotatePoint(stopPos, centre, myRot);
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.Delay = 32500;
                dp.DestoryAt = 5000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

            }

            //长火
            if (myFire == 3)
            {
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P3_时间压缩_长火_中场分摊";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = centre;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = 7500;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P3_时间压缩_长火_中场分摊";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = centre;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.Delay = 12500;
                dp.DestoryAt = 5000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P3_时间压缩_长火_回溯";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = RotatePoint(returnPosN, centre, myRot);
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.Delay = 17500;
                dp.DestoryAt = 5000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P3_时间压缩_长火_放火";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = RotatePoint(fireN, centre, myRot);
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.Delay = 22500;
                dp.DestoryAt = 5000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P3_时间压缩_长火_输出";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = RotatePoint(stopPos, centre, myRot);
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.Delay = 27500;
                dp.DestoryAt = 10000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
            }

            if (myFire == 4)
            {
                if (myIndex <4)
                {
                    var dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P3_时间压缩_冰TH_放冰";
                    dp.Scale = new(2);
                    dp.ScaleMode |= ScaleMode.YByDistance;
                    dp.Owner = accessory.Data.Me;
                    dp.TargetPosition = centre;
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.DestoryAt = 7500;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                    dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P3_时间压缩_冰TH_放回溯";
                    dp.Scale = new(2);
                    dp.ScaleMode |= ScaleMode.YByDistance;
                    dp.Owner = accessory.Data.Me;
                    dp.TargetPosition = RotatePoint(returnPosN, centre, myRot);
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.Delay = 7500;
                    dp.DestoryAt = 5000;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                    dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P3_时间压缩_冰TH_场中分摊";
                    dp.Scale = new(2);
                    dp.ScaleMode |= ScaleMode.YByDistance;
                    dp.Owner = accessory.Data.Me;
                    dp.TargetPosition = centre;
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.Delay = 12500;
                    dp.DestoryAt = 5000;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);


                    dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P3_时间压缩_冰TH_输出位置";
                    dp.Scale = new(2);
                    dp.ScaleMode |= ScaleMode.YByDistance;
                    dp.Owner = accessory.Data.Me;
                    dp.TargetPosition = RotatePoint(stopPos, centre, myRot);
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.Delay = 22500;
                    dp.DestoryAt = 15000;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                }
                else
                {
                    var dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P3_时间压缩_冰D_中场分摊";
                    dp.Scale = new(2);
                    dp.ScaleMode |= ScaleMode.YByDistance;
                    dp.Owner = accessory.Data.Me;
                    dp.TargetPosition = centre;
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.DestoryAt = 7500;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                    dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P3_时间压缩_冰D_中场分摊";
                    dp.Scale = new(2);
                    dp.ScaleMode |= ScaleMode.YByDistance;
                    dp.Owner = accessory.Data.Me;
                    dp.TargetPosition = centre;
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.Delay = 12500;
                    dp.DestoryAt = 5000;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                    dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P3_时间压缩_冰D_回溯";
                    dp.Scale = new(2);
                    dp.ScaleMode |= ScaleMode.YByDistance;
                    dp.Owner = accessory.Data.Me;
                    dp.TargetPosition = RotatePoint(returnPosN, centre, myRot);
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.Delay = 17500;
                    dp.DestoryAt = 5000;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                    dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P3_时间压缩_冰D_放冰";
                    dp.Scale = new(2);
                    dp.ScaleMode |= ScaleMode.YByDistance;
                    dp.Owner = accessory.Data.Me;
                    dp.TargetPosition = centre;
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.Delay = 22500;
                    dp.DestoryAt = 5000;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                    dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P3_时间压缩_长火_输出";
                    dp.Scale = new(2);
                    dp.ScaleMode |= ScaleMode.YByDistance;
                    dp.Owner = accessory.Data.Me;
                    dp.TargetPosition = RotatePoint(stopPos, centre, myRot);
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.Delay = 27500;
                    dp.DestoryAt = 10000;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                }
            }
        }
        [ScriptMethod(name: "P3_时间压缩_灯处理位置", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:2970"])]
        public void P3_时间压缩_灯处理位置(Event @event, ScriptAccessory accessory)
        {
            if (parse != 3.1) return;
            //buff2970, 13 269顺时针 92 348逆时针
            var pos = JsonConvert.DeserializeObject<Vector3>(@event["TargetPosition"]);
            Vector3 centre = new(100, 0, 100);
            var myIndex=accessory.Data.PartyList.IndexOf(accessory.Data.Me);
            var dir8 = PositionTo8Dir(pos, centre);
            Vector3 nPos = @event["StackCount"] == "92" ? new(98, 0, 90) : new(102, 0, 90);
            if (dir8 == MyLampIndex(myIndex))
            {
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P3_时间压缩_灯处理位置";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = RotatePoint(nPos,centre,dir8*float.Pi/4);
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = 4000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
            }
        }

        [ScriptMethod(name: "P3_时间压缩_破盾一击集合提示", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40286"])]
        public void P3_时间压缩_破盾一击集合提示(Event @event, ScriptAccessory accessory)
        {
            if (parse != 3.1) return;
            accessory.Method.TextInfo("场中集合",3000);
            accessory.Method.TTS("场中集合");
        }
        [ScriptMethod(name: "P3_时间压缩_黑暗光环", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40290"])]
        public void P3_时间压缩_黑暗光环(Event @event, ScriptAccessory accessory)
        {
            if (parse != 3.1) return;
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            var myindex=accessory.Data.PartyList.IndexOf(accessory.Data.Me);
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P3_时间压缩_黑暗光环";
            dp.Scale = new(20);
            dp.Owner = sid;
            dp.TargetObject = tid;
            dp.Color = myindex == 0 || myindex == 1 ? accessory.Data.DefaultSafeColor : accessory.Data.DefaultDangerColor;
            dp.DestoryAt = 5000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
        }


        [ScriptMethod(name: "P3_延迟咏唱回响_分P", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40269)$"], userControl: false)]
        public void P3_延迟咏唱回响_分P(Event @event, ScriptAccessory accessory)
        {
            parse = 3.2d;
            P3FloorFire = -1;
            typeOfDarkWaterIii=[
                Phase3_Types_Of_Dark_Water_III.NONE,
                Phase3_Types_Of_Dark_Water_III.NONE,
                Phase3_Types_Of_Dark_Water_III.NONE,
                Phase3_Types_Of_Dark_Water_III.NONE,
                Phase3_Types_Of_Dark_Water_III.NONE,
                Phase3_Types_Of_Dark_Water_III.NONE,
                Phase3_Types_Of_Dark_Water_III.NONE,
                Phase3_Types_Of_Dark_Water_III.NONE
            ];
            timesStatus2458Disappeared=0;
        }
        [ScriptMethod(name: "P3_延迟咏唱回响_地火", eventType: EventTypeEnum.ObjectEffect, eventCondition: ["Id1:4", "Id2:regex:^(16|64)$"])]
        public void P3_延迟咏唱回响_地火(Event @event, ScriptAccessory accessory)
        {
            if (parse != 3.2) return;
            lock (this)
            {
                if (P3FloorFireDone) return;
                P3FloorFireDone = true;
            }
            Vector3 centre = new(100, 0, 100);
            var pos = JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
            var clockwise = @event["Id2"] == "64" ? -1 : 1;
            var preTime = 100;
            //间隔11 2 2 2 2 2

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P3_二运_地火_中心";
            dp.Scale = new(9);
            dp.Position = centre;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 3000;
            dp.DestoryAt = 9700;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P3_二运_地火_起始点_11";
            dp.Scale = new(9);
            dp.Position = pos;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 3000;
            dp.DestoryAt = 12000 - preTime;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P3_二运_地火_起始点_12";
            dp.Scale = new(9);
            dp.Position = pos;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 17000 - preTime;
            dp.DestoryAt = 4000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P3_二运_地火_起始点_21";
            dp.Scale = new(9);
            dp.Position = RotatePoint(pos, centre, float.Pi);
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 3000;
            dp.DestoryAt = 12000 - preTime;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P3_二运_地火_起始点_22";
            dp.Scale = new(9);
            dp.Position = RotatePoint(pos, centre, float.Pi);
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 17000 - preTime;
            dp.DestoryAt = 4000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P3_二运_地火_第二点_11";
            dp.Scale = new(9);
            dp.Position = RotatePoint(pos, centre, float.Pi / 4 * clockwise);
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 3000;
            dp.DestoryAt = 14000 - preTime;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P3_二运_地火_第二点_12";
            dp.Scale = new(9);
            dp.Position = RotatePoint(pos, centre, float.Pi / 4 * clockwise);
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 19000 - preTime;
            dp.DestoryAt = 2000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P3_二运_地火_第二点_21";
            dp.Scale = new(9);
            dp.Position = RotatePoint(pos, centre, float.Pi / 4 * clockwise + float.Pi);
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 3000;
            dp.DestoryAt = 14000 - preTime;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P3_二运_地火_第二点_22";
            dp.Scale = new(9);
            dp.Position = RotatePoint(pos, centre, float.Pi / 4 * clockwise + float.Pi);
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 19000 - preTime;
            dp.DestoryAt = 2000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P3_二运_地火_第三点_11";
            dp.Scale = new(9);
            dp.Position = RotatePoint(pos, centre, float.Pi / 2 * clockwise);
            dp.Color = (accessory.Data.DefaultDangerColor + accessory.Data.DefaultSafeColor) / 2;
            dp.Delay = 3000;
            dp.DestoryAt = 8000 - preTime;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P3_二运_地火_第三点_12";
            dp.Scale = new(9);
            dp.Position = RotatePoint(pos, centre, float.Pi / 2 * clockwise);
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 11000 - preTime;
            dp.DestoryAt = 8000 - preTime;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P3_二运_地火_第三点_21";
            dp.Scale = new(9);
            dp.Position = RotatePoint(pos, centre, float.Pi / 2 * clockwise + float.Pi);
            dp.Color = (accessory.Data.DefaultDangerColor + accessory.Data.DefaultSafeColor) / 2;
            dp.Delay = 3000;
            dp.DestoryAt = 8000 - preTime;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P3_二运_地火_第三点_22";
            dp.Scale = new(9);
            dp.Position = RotatePoint(pos, centre, float.Pi / 2 * clockwise + float.Pi);
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 11000 - preTime;
            dp.DestoryAt = 8000 - preTime;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P3_二运_地火_第四点_11";
            dp.Scale = new(9);
            dp.Position = RotatePoint(pos, centre, float.Pi / 4 * 3 * clockwise);
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 15000 - preTime;
            dp.DestoryAt = 6000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P3_二运_地火_第四点_21";
            dp.Scale = new(9);
            dp.Position = RotatePoint(pos, centre, float.Pi / 4 * 3 * clockwise + float.Pi);
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 15000 - preTime;
            dp.DestoryAt = 6000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

        }
        
        [ScriptMethod(name:"Phase3_Determine_Types_Of_Dark_Water_III_确定黑暗狂水类型",
            eventType:EventTypeEnum.StatusAdd,
            eventCondition:["StatusID:2461"],
            userControl:false)]
        
        public void Phase3_Determine_Types_Of_Dark_Water_III_确定黑暗狂水类型(Event @event, ScriptAccessory accessory) {
            
            if(parse!=3.2) {

                return;

            }

            if(!ParseObjectId(@event["TargetId"], out var targetId)) {

                return;

            }

            int currentIndex=accessory.Data.PartyList.IndexOf(targetId);
            uint timeOfDarkWaterIii=Convert.ToUInt32(@event["DurationMilliseconds"],10);

            if(timeOfDarkWaterIii>36000) {
                // Actually it's 38000ms (38s), but just in case.

                lock(typeOfDarkWaterIii) {

                    typeOfDarkWaterIii[currentIndex]=Phase3_Types_Of_Dark_Water_III.LONG;

                }

            }

            else {

                if(timeOfDarkWaterIii>27000) {
                    // Actually it's 29000ms (29s), but just in case.

                    lock(typeOfDarkWaterIii) {

                        typeOfDarkWaterIii[currentIndex]=Phase3_Types_Of_Dark_Water_III.MEDIUM;

                    }

                }

                else {

                    if(timeOfDarkWaterIii>8000) {
                        // Actually it's 10000ms (10s), but just in case.

                        lock(typeOfDarkWaterIii) {

                            typeOfDarkWaterIii[currentIndex]=Phase3_Types_Of_Dark_Water_III.SHORT;

                        }

                    }

                }

            }

            if(Enable_Developer_Mode_启用开发者模式) {

                accessory.Method.SendChat($"Checking the party member {currentIndex}... The DurationMilliseconds of StatusID 2461 is: {timeOfDarkWaterIii}. The value of typeOfDarkWaterIii is: {typeOfDarkWaterIii[currentIndex].ToString()}.");
                
            }
            
        }

        [ScriptMethod(name:"Phase3_Guidance_Of_Dark_Water_III_黑暗狂水指路",
            eventType:EventTypeEnum.StatusRemove,
            eventCondition:["StatusID:2458"])]
        
        public void Phase3_Guidance_Of_Dark_Water_III_黑暗狂水指路(Event @event,ScriptAccessory accessory) {

            if(parse!=3.2) {

                return;

            }

            bool targetPositionConfirmed=false;
            var currentProperty=accessory.Data.GetDefaultDrawProperties();
            
            currentProperty.Name="Phase3_Guidance_Of_Dark_Water_III_黑暗狂水指路";
            currentProperty.Scale=new(2);
            currentProperty.ScaleMode|=ScaleMode.YByDistance;
            currentProperty.Owner=accessory.Data.Me;
            currentProperty.Color=accessory.Data.DefaultSafeColor;
            currentProperty.DestoryAt=4750;

            if(Phase3_Strats_Of_The_Second_Half_二运策略==Phase3_Strats_Of_The_Second_Half.MMW_Double_Group_双分组法) {
                
                bool goToLeft=shouldThePartyMemberGoLeftWhileDoubleGroup(accessory.Data.PartyList.IndexOf(accessory.Data.Me));

                if(Enable_Developer_Mode_启用开发者模式) {
                    
                    accessory.Method.SendChat($"The value of goToLeft is: {goToLeft}. The value of timesStatus2458Disappeared is: {timesStatus2458Disappeared}. This debug message may be sent twice.");
                    
                }

                if(goToLeft) {

                    if(timesStatus2458Disappeared<2) {
                        // First time of Dark Water III.

                        currentProperty.TargetPosition=new Vector3(93,0,100);
                        targetPositionConfirmed=true;
                        
                    }

                    else {
                        
                        if(timesStatus2458Disappeared<4) {
                            // Second time of Dark Water III.

                            currentProperty.TargetPosition=new Vector3(96,0,100);
                            targetPositionConfirmed=true;
                        
                        }

                        else {
                            
                            if(timesStatus2458Disappeared<6) {
                                // Third time of Dark Water III.
                                // Suggested by @lunarflower223 on Discord. Thank you!

                                for(int i=0;i<8;++i) {

                                    if(typeOfDarkWaterIii[i]==Phase3_Types_Of_Dark_Water_III.LONG) {
                                        
                                        var temporaryProperty=accessory.Data.GetDefaultDrawProperties();
                                        
                                        temporaryProperty.Name="Phase3_Range_Guidance_Of_Dark_Water_III_黑暗狂水范围指路";
                                        temporaryProperty.Scale=new(6);
                                        temporaryProperty.Owner=accessory.Data.PartyList[i];
                                        temporaryProperty.DestoryAt=5000;

                                        if(shouldThePartyMemberGoLeftWhileDoubleGroup(i)) {

                                            temporaryProperty.Color=accessory.Data.DefaultSafeColor;

                                        }

                                        else {

                                            temporaryProperty.Color=accessory.Data.DefaultDangerColor;

                                        }
                                        
                                        accessory.Method.SendDraw(DrawModeEnum.Default,DrawTypeEnum.Circle,temporaryProperty);
                                        
                                    }
                                    
                                }
                                
                            }
                            
                        }
                        
                        // No guidance for the third time since I'm unable to acquire the boss position here.
                        
                    }

                    accessory.Method.TextInfo("Stack on the left 去左侧分摊",2500);
                    accessory.Method.TTS("Stack on the left 去左侧分摊");

                }

                else {
                    
                    if(timesStatus2458Disappeared<2) {

                        currentProperty.TargetPosition=new Vector3(107,0,100);
                        targetPositionConfirmed=true;
                        
                    }

                    else {
                        
                        if(timesStatus2458Disappeared<4) {

                            currentProperty.TargetPosition=new Vector3(104,0,100);
                            targetPositionConfirmed=true;
                        
                        }

                        else {

                            if(timesStatus2458Disappeared<6) {

                                for(int i=0;i<8;++i) {

                                    if(typeOfDarkWaterIii[i]==Phase3_Types_Of_Dark_Water_III.LONG) {
                                        
                                        var temporaryProperty=accessory.Data.GetDefaultDrawProperties();
                                        
                                        temporaryProperty.Name="Phase3_Range_Guidance_Of_Dark_Water_III_黑暗狂水范围指路";
                                        temporaryProperty.Scale=new(6);
                                        temporaryProperty.Owner=accessory.Data.PartyList[i];
                                        temporaryProperty.DestoryAt=5000;

                                        if(shouldThePartyMemberGoLeftWhileDoubleGroup(i)) {

                                            temporaryProperty.Color=accessory.Data.DefaultDangerColor;

                                        }

                                        else {

                                            temporaryProperty.Color=accessory.Data.DefaultSafeColor;

                                        }
                                        
                                        accessory.Method.SendDraw(DrawModeEnum.Default,DrawTypeEnum.Circle,temporaryProperty);
                                        
                                    }
                                    
                                }
                                
                            }
                            
                        }
                        
                    }
                    
                    accessory.Method.TextInfo("Stack on the right 去右侧分摊",2500);
                    accessory.Method.TTS("Stack on the right 去右侧分摊");

                }

            }

            if(targetPositionConfirmed) {

                accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);
                
            }
            
            lock(lockOfTimesStatus2458Disappeared) {

                ++timesStatus2458Disappeared;

            }

        }

        private bool shouldThePartyMemberGoLeftWhileDoubleGroup(int originalIndex) {
            
            int currentIndexWhileDoubleGroup=getIndexWhileDoubleGroup(originalIndex);
            Phase3_Types_Of_Dark_Water_III currentType=typeOfDarkWaterIii[originalIndex];
            bool result=false;

            for(int i=0;i<8;++i) {

                if(typeOfDarkWaterIii[indexWhileDoubleGroup[i]]==currentType&&i!=currentIndexWhileDoubleGroup) {

                    if(i>currentIndexWhileDoubleGroup) {

                        result=true;
                        // Should go left.

                        break;

                    }

                    if(i<currentIndexWhileDoubleGroup) {

                        result=false;
                        // Should go right.

                        break;

                    }
                        
                }
                    
            }

            return result;

        }

        private int getIndexWhileDoubleGroup(int originalIndex) {

            for(int i=0;i<8;++i) {

                if(originalIndex==indexWhileDoubleGroup[i]) {

                    return i;

                }
                
            }

            return originalIndex;
            // Just a placeholder and should never be reached.

        }
        
        [ScriptMethod(name:"Phase3_Range_Of_Spirit_Taker_碎灵一击范围",
            eventType:EventTypeEnum.StartCasting,
            eventCondition:["ActionId:40288"])]
        
        public void Phase3_Range_Of_Spirit_Taker_碎灵一击范围(Event @event,ScriptAccessory accessory) {

            if(parse!=3.2) {

                return;

            }
            
            for(int i=0;i<8;++i) {
                
                var currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                currentProperty.Name="Phase3_Range_Of_Spirit_Taker_碎灵一击范围";
                currentProperty.Scale=new(5);
                currentProperty.Owner=accessory.Data.PartyList[i];
                currentProperty.Color=accessory.Data.DefaultDangerColor;
                currentProperty.Delay=1000;
                currentProperty.DestoryAt=2750;
                
                accessory.Method.SendDraw(DrawModeEnum.Default,DrawTypeEnum.Circle,currentProperty);
                
            }
            
            System.Threading.Thread.Sleep(1000);
            
            accessory.Method.TextInfo("Spread 分散",2000);
            accessory.Method.TTS("Spread 分散");

        }
        
        [ScriptMethod(name:"Phase3_Guidance_Of_Spirit_Taker_碎灵一击指路",
            eventType:EventTypeEnum.StartCasting,
            eventCondition:["ActionId:40288"])]
        
        public void Phase3_Guidance_Of_Spirit_Taker_碎灵一击指路(Event @event,ScriptAccessory accessory) {

            if(parse!=3.2) {

                return;

            }

            bool targetPositionConfirmed=false;
            var currentProperty=accessory.Data.GetDefaultDrawProperties();
            
            currentProperty.Name="Phase3_Guidance_Of_Spirit_Taker_碎灵一击指路";
            currentProperty.Scale=new(2);
            currentProperty.ScaleMode|=ScaleMode.YByDistance;
            currentProperty.Owner=accessory.Data.Me;
            currentProperty.Color=accessory.Data.DefaultSafeColor;
            currentProperty.Delay=1000;
            currentProperty.DestoryAt=2750;

            if(Phase3_Strats_Of_The_Second_Half_二运策略==Phase3_Strats_Of_The_Second_Half.MMW_Double_Group_双分组法) {

                int myIndexWhileDoubleGroup=getIndexWhileDoubleGroup(accessory.Data.PartyList.IndexOf(accessory.Data.Me));

                switch(myIndexWhileDoubleGroup) {

                    case 0: {
                        // H1
                        
                        currentProperty.TargetPosition=new Vector3(85,0,100);
                        targetPositionConfirmed=true;

                        break;
                        
                    }

                    case 1: {
                        // H2
                        
                        bool goToLeft=shouldThePartyMemberGoLeftWhileDoubleGroup(accessory.Data.PartyList.IndexOf(accessory.Data.Me));

                        if(Enable_Developer_Mode_启用开发者模式) {
                    
                            accessory.Method.SendChat($"The value of goToLeft is: {goToLeft}. This debug message may be sent six times.");
                    
                        }

                        if(goToLeft) {
                            
                            currentProperty.TargetPosition=new Vector3(93,0,92);
                            targetPositionConfirmed=true;
                            
                        }

                        else {
                            
                            currentProperty.TargetPosition=new Vector3(107,0,92);
                            targetPositionConfirmed=true;
                            
                        }

                        break;

                    }

                    case 2: {
                        // MT
                        
                        currentProperty.TargetPosition=new Vector3(100,0,92);
                        targetPositionConfirmed=true;

                        break;
                        
                    }
                    
                    case 3: {
                        // OT or ST
                        
                        currentProperty.TargetPosition=new Vector3(100,0,100);
                        targetPositionConfirmed=true;

                        break;
                        
                    }
                    
                    case 4: {
                        // M1 or D1
                        
                        bool goToLeft=shouldThePartyMemberGoLeftWhileDoubleGroup(accessory.Data.PartyList.IndexOf(accessory.Data.Me));

                        if(Enable_Developer_Mode_启用开发者模式) {
                    
                            accessory.Method.SendChat($"The value of goToLeft is: {goToLeft}. This debug message may be sent six times.");
                    
                        }

                        if(goToLeft) {
                            
                            currentProperty.TargetPosition=new Vector3(93,0,100);
                            targetPositionConfirmed=true;
                            
                        }

                        else {
                            
                            currentProperty.TargetPosition=new Vector3(107,0,100);
                            targetPositionConfirmed=true;
                            
                        }

                        break;
                        
                    }
                    
                    case 5: {
                        // M2 or D2
                        
                        currentProperty.TargetPosition=new Vector3(100,0,108);
                        targetPositionConfirmed=true;

                        break;
                        
                    }
                    
                    case 6: {
                        // R1 or D3
                        
                        bool goToLeft=shouldThePartyMemberGoLeftWhileDoubleGroup(accessory.Data.PartyList.IndexOf(accessory.Data.Me));

                        if(Enable_Developer_Mode_启用开发者模式) {
                    
                            accessory.Method.SendChat($"The value of goToLeft is: {goToLeft}. This debug message may be sent six times.");
                    
                        }

                        if(goToLeft) {
                            
                            currentProperty.TargetPosition=new Vector3(93,0,108);
                            targetPositionConfirmed=true;
                            
                        }

                        else {
                            
                            currentProperty.TargetPosition=new Vector3(107,0,108);
                            targetPositionConfirmed=true;
                            
                        }

                        break;
                        
                    }
                    
                    case 7: {
                        // R2 or D4
                        
                        currentProperty.TargetPosition=new Vector3(115,0,100);
                        targetPositionConfirmed=true;

                        break;
                        
                    }
                    
                    default: {
                        // Just a placeholder and should never be reached.

                        break;
                        
                    }
                    
                }

            }

            if(targetPositionConfirmed) {

                accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);
                
            }

        }
        
        [ScriptMethod(name: "P3_延迟咏唱回响_击退提示", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:40182", "TargetIndex:1"])]
        public void P3_延迟咏唱回响_击退提示(Event @event, ScriptAccessory accessory)
        {
            if (parse != 3.2) return;
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P3_延迟咏唱回响_击退提示1";
            dp.Scale = new(2,21);
            dp.Owner = accessory.Data.Me;
            dp.TargetObject = sid;
            dp.Rotation = float.Pi;
            dp.Color = accessory.Data.DefaultDangerColor.WithW(3);
            dp.DestoryAt = 3000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Displacement, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P3_延迟咏唱回响_击退提示2";
            dp.Scale = new(2);
            dp.Owner = sid;
            dp.TargetObject = accessory.Data.Me;
            dp.ScaleMode |= ScaleMode.YByDistance;
            dp.Color = accessory.Data.DefaultDangerColor.WithW(3);
            dp.DestoryAt = 3000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Displacement, dp);
        }
        [ScriptMethod(name: "P3_延迟咏唱回响_地火记录", eventType: EventTypeEnum.ObjectEffect, eventCondition: ["Id1:4", "Id2:regex:^(16|64)$"],userControl:false)]
        public void P3_延迟咏唱回响_地火记录(Event @event, ScriptAccessory accessory)
        {
            if (parse != 3.2) return;
            lock (this)
            {
                if (P3FloorFire != -1) return;
                Vector3 centre = new(100, 0, 100);
                var pos = JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
                P3FloorFire = PositionTo8Dir(pos,new(100,0,100));
                P3FloorFire+= @event["Id2"] == "64" ? 10 : 20;
            }
            
        }
        
        [ScriptMethod(name:"Phase3_Range_Of_Darkest_Dance_暗夜舞蹈范围",
            eventType:EventTypeEnum.StartCasting,
            eventCondition:["ActionId:40181"])]
        
        public void Phase3_Range_Of_Darkest_Dance_暗夜舞蹈范围(Event @event, ScriptAccessory accessory) {
            
            if(parse!=3.2) {
                
                return;
                
            }

            if(!ParseObjectId(@event["SourceId"], out var sourceId)) {

                return;
                
            }

            bool goBait=false;

            if(Phase3_Who_Baits_Darkest_Dance_谁引导暗夜舞蹈==Phase3_Who_Baits_Darkest_Dance.MT
               &&
               accessory.Data.PartyList.IndexOf(accessory.Data.Me)==0) { 
                
                goBait=true;

            }

            if(Phase3_Who_Baits_Darkest_Dance_谁引导暗夜舞蹈==Phase3_Who_Baits_Darkest_Dance.OT_ST
               &&
               accessory.Data.PartyList.IndexOf(accessory.Data.Me)==1) {
                
                goBait=true;

            }
            
            var currentProperty=accessory.Data.GetDefaultDrawProperties();

            currentProperty.Name="Phase3_Range_Of_Darkest_Dance_暗夜舞蹈范围";
            currentProperty.Scale=new(8);
            currentProperty.Owner=sourceId;
            currentProperty.CentreResolvePattern=PositionResolvePatternEnum.PlayerFarestOrder;
            currentProperty.Color=Phase3_Colour_Of_Darkest_Dance_暗夜舞蹈的颜色.V4.WithW(1.5f);
            currentProperty.Delay=2200;
            currentProperty.DestoryAt=4000;

            accessory.Method.SendDraw(DrawModeEnum.Default,DrawTypeEnum.Circle,currentProperty);
            
            System.Threading.Thread.Sleep(2200);
            
            if(goBait) {

                accessory.Method.TextInfo("Go bait 引导死刑",1500);
                accessory.Method.TTS("Go bait 引导死刑");

            }

            else {

                if(Phase3_Who_Baits_Darkest_Dance_谁引导暗夜舞蹈==Phase3_Who_Baits_Darkest_Dance.MT) {
                    
                    accessory.Method.TextInfo("Stay away from MT 远离MT",1500);
                    accessory.Method.TTS("Stay away from MT 远离MT");
                    
                }

                else {
                    
                    accessory.Method.TextInfo("Stay away from OT 远离ST",1500);
                    accessory.Method.TTS("Stay away from OT 远离ST");
                    
                }

            }

        }
        
        [ScriptMethod(name:"Phase3_Guidance_Of_Darkest_Dance_暗夜舞蹈指路",
            eventType:EventTypeEnum.StartCasting,
            eventCondition:["ActionId:40181"])]
        
        public void Phase3_Guidance_Of_Darkest_Dance_暗夜舞蹈指路(Event @event, ScriptAccessory accessory) {
            
            if(parse!=3.2) {
                
                return;
                
            }

            var tankWhoBaitsDarkestDance=accessory.Data.Objects.SearchById(accessory.Data.Me);
            bool goBait=false;

            if(Phase3_Who_Baits_Darkest_Dance_谁引导暗夜舞蹈==Phase3_Who_Baits_Darkest_Dance.MT) {

                tankWhoBaitsDarkestDance=accessory.Data.Objects.SearchById(accessory.Data.PartyList[0]);

                if(accessory.Data.PartyList.IndexOf(accessory.Data.Me)==0) {

                    goBait=true;

                }

            }

            else {

                tankWhoBaitsDarkestDance=accessory.Data.Objects.SearchById(accessory.Data.PartyList[1]);
                
                if(accessory.Data.PartyList.IndexOf(accessory.Data.Me)==1) {

                    goBait=true;

                }

            }

            if(tankWhoBaitsDarkestDance==null) {
                
                return;
                
            }
            
            // ----- Calculations of the position where the tank should bait -----
            // This part was directly inherited from Karlin-Z's original script.
            // The algorithm seems to be too mysterious to me, and it definitely works nice.
            // So as a result, I kept this part as is.

            var dir8=P3FloorFire%10%4;
            Vector3 posN=new(100,0,86);
            var rot=dir8 switch {
                0=>6,
                1=>7,
                2=>0,
                3=>5
            };
            var pos1=RotatePoint(posN,new(100,0,100),float.Pi/4*rot);
            var pos2=RotatePoint(posN,new(100,0,100),float.Pi/4*rot+float.Pi);
            var dealpos=((pos1-tankWhoBaitsDarkestDance.Position).Length()<(pos2-tankWhoBaitsDarkestDance.Position).Length())?(pos1):(pos2);
            
            // ----- -----
            
            var currentProperty=accessory.Data.GetDefaultDrawProperties();
            
            if(goBait) {
                
                currentProperty.Owner=accessory.Data.Me;
                currentProperty.Color=accessory.Data.DefaultSafeColor;
                
            }

            else {

                if(Phase3_Who_Baits_Darkest_Dance_谁引导暗夜舞蹈==Phase3_Who_Baits_Darkest_Dance.MT) {

                    currentProperty.Owner=accessory.Data.PartyList[0];
                    currentProperty.Color=Phase3_Colour_Of_Darkest_Dance_暗夜舞蹈的颜色.V4.WithW(1.5f);

                }

                else {

                    currentProperty.Owner=accessory.Data.PartyList[1];
                    currentProperty.Color=Phase3_Colour_Of_Darkest_Dance_暗夜舞蹈的颜色.V4.WithW(1.5f);

                }

            }
            
            currentProperty.Name="Phase3_Guidance_Of_Darkest_Dance_暗夜舞蹈指路";
            currentProperty.Scale=new(2);
            currentProperty.ScaleMode|=ScaleMode.YByDistance;
            currentProperty.TargetPosition=dealpos;
            currentProperty.Delay=2200;
            currentProperty.DestoryAt=4000;
            
            accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);

        }

        private int MyLampIndex(int myPartyIndex)
        {
            var nLampIndex = 0;
            for (int i = 0; i < 8; i++)
            {
                if (P3Lamp[i] == 1 && P3Lamp[(i+3)%8]==1 && P3Lamp[(i + 5) % 8] == 1)
                {
                    nLampIndex=i;
                    break;
                }
            }
            if (P3LampDeal==P3LampEmum.MGL)
            {
                //短火
                if (P3FireBuff[myPartyIndex] == 1)
                {    
                    if (myPartyIndex<4)
                    {
                        return (nLampIndex + 4) % 8;
                    }
                    else
                    {
                        var lowIndex = P3FireBuff.LastIndexOf(1);
                        if (lowIndex != myPartyIndex)
                        {
                            return (nLampIndex + 7) % 8;
                        }
                        else
                        {
                            return (nLampIndex + 1) % 8;
                        }
                    }
                    
                }
                //中火
                if (P3FireBuff[myPartyIndex] == 2)
                {
                    if (myPartyIndex < 4) return (nLampIndex + 6) % 8;
                    else return (nLampIndex + 2) % 8;
                }
                //长火
                if (P3FireBuff[myPartyIndex] == 3)
                {
                    if (myPartyIndex<4)
                    {
                        var highIndex = P3FireBuff.IndexOf(3);
                        if (highIndex == myPartyIndex)
                        {
                            return (nLampIndex + 5) % 8;
                        }
                        else
                        {
                            return (nLampIndex + 3) % 8;
                        }
                    }
                    else
                    {
                        return (nLampIndex + 0) % 8;
                    }
                    
                }
                //冰
                if (P3FireBuff[myPartyIndex] == 4)
                {
                    if (myPartyIndex < 4) return (nLampIndex + 4) % 8;
                    else return (nLampIndex + 0) % 8;
                }
            }

            return -1;
        }
        #endregion

        #region P4

       
        [ScriptMethod(name: "P4_具象化_分P", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40246"], userControl: false)]
        public void P4_具象化_分P(Event @event, ScriptAccessory accessory)
        {
            parse = 4.1d;
        }
        [ScriptMethod(name: "P4_时间结晶_记忆水晶收集", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40174"], userControl: false)]
        public void P4_时间结晶_记忆水晶收集(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            P4FragmentId = sid;
        }
        [ScriptMethod(name: "P4_具象化_天光轮回", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40237"])]
        public void P4_具象化_天光轮回(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P4_具象化_天光轮回";
            dp.Scale = new(4);
            dp.Owner = sid;
            dp.TargetObject = sid;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.DestoryAt = 8000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
        [ScriptMethod(name: "P4_具象化_天光轮回集合提醒", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:40246"])]
        public void P4_具象化_天光轮回集合提醒(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("集合", 9500);
            accessory.Method.TTS("集合");
        }
        [ScriptMethod(name: "P4_具象化_天光轮回躲避提醒", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:40186"])]
        public void P4_具象化_天光轮回躲避提醒(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("快跑", 3000);
            accessory.Method.TTS("快跑");
        }

        [ScriptMethod(name: "P4_暗光龙诗_分P", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40239"], userControl: false)]
        public void P4_暗光龙诗_分P(Event @event, ScriptAccessory accessory)
        {
            parse = 4.2d;
            P4Tether = [-1, -1, -1, -1, -1, -1, -1, -1];
            P4Stack = [0, 0, 0, 0, 0, 0, 0, 0];
            P4TetherDone = false;
        }
        [ScriptMethod(name: "P4_暗光龙诗_Buff记录", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:2461"], userControl: false)]
        public void P4_暗光龙诗_Buff记录(Event @event, ScriptAccessory accessory)
        {
            if (parse != 4.2) return;
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            var tIndex = accessory.Data.PartyList.IndexOf(tid);
            P4Stack[tIndex] = 1;
        }
        [ScriptMethod(name: "P4_暗光龙诗_连线收集", eventType: EventTypeEnum.Tether, eventCondition: ["Id:006E"], userControl: false)]
        public void P4_暗光龙诗_连线收集(Event @event, ScriptAccessory accessory)
        {
            if (parse != 4.2) return;
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            var sIndex = accessory.Data.PartyList.IndexOf(sid);
            var tIndex = accessory.Data.PartyList.IndexOf(tid);
            P4Tether[sIndex] = tIndex;
        }
        [ScriptMethod(name: "P4_暗光龙诗_引导扇形", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40187"])]
        public void P4_暗光龙诗_引导扇形(Event @event, ScriptAccessory accessory)
        {
            if (parse != 4.2) return;
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            for (uint i = 1; i < 5; i++)
            {
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P4_暗光龙诗_引导扇形";
                dp.Scale = new(20);
                dp.Radian = float.Pi / 3;
                dp.Owner = sid;
                dp.TargetResolvePattern=PositionResolvePatternEnum.PlayerNearestOrder;
                dp.TargetOrderIndex = i;
                dp.Color = accessory.Data.DefaultDangerColor;
                dp.Delay = 4000;
                dp.DestoryAt = 5000;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
            }
        }
        [ScriptMethod(name: "P4_暗光龙诗_碎灵一击", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:40187"])]
        public void P4_暗光龙诗_碎灵一击(Event @event, ScriptAccessory accessory)
        {
            if (parse != 4.2) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P4_暗光龙诗_碎灵一击_水晶";
            dp.Scale = new(8.5f);
            dp.Owner = P4FragmentId;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.DestoryAt = 3500;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

            for (int i = 0; i < 8; i++)
            {
                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P4_暗光龙诗_碎灵一击";
                dp.Scale = new(5);
                dp.Owner = accessory.Data.PartyList[i];
                dp.Color = accessory.Data.DefaultDangerColor;
                dp.DestoryAt = 3500;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            }
        }
        [ScriptMethod(name: "P4_暗光龙诗_神圣之翼", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4022[78])$"])]
        public void P4_暗光龙诗_神圣之翼(Event @event, ScriptAccessory accessory)
        {
            if (parse != 4.2) return;
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P4_暗光龙诗_神圣之翼";
            dp.Scale = new(40,20);
            dp.Owner = sid;
            dp.Rotation = @event["ActionId"] == "40227" ? float.Pi / 2 : float.Pi / -2;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.DestoryAt = 5000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);

        }
        [ScriptMethod(name: "P4_暗光龙诗_水分摊", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4022[78])$"])]
        public void P4_暗光龙诗_水分摊(Event @event, ScriptAccessory accessory)
        {
            var tIndex = P4Tether[0] == -1 ? 1 : 0;
            var nIndex = P4Tether[2] == -1 ? 3 : 2;
            var d1Index = -1;
            var d2Index = -1;
            List<int> upGroup = [];
            List<int> downGroup = [];
            for (int i = 4; i < 7; i++)
            {
                for (int j = i + 1; j < 8; j++)
                {
                    if (P4Tether[i] != -1 && P4Tether[j] != -1)
                    {
                        d1Index = i;
                        d2Index = j;
                    }
                }
            }
            // t连线 高d 低d 蝴蝶结
            if ((P4Tether[tIndex] == d1Index && P4Tether[d2Index] == tIndex) || (P4Tether[tIndex] == d2Index && P4Tether[d1Index] == tIndex))
            {
                upGroup.Add(tIndex);
                upGroup.Add(nIndex);
                downGroup.Add(d1Index);
                downGroup.Add(d2Index);
            }
            // t连线 高d n 方块
            if ((P4Tether[tIndex] == d1Index && P4Tether[nIndex] == tIndex) || (P4Tether[d1Index] == tIndex && P4Tether[tIndex] == nIndex))
            {
                upGroup.Add(d1Index);
                upGroup.Add(nIndex);
                downGroup.Add(tIndex);
                downGroup.Add(d2Index);
            }
            // t连线 低d n 沙漏
            if ((P4Tether[tIndex] == d2Index && P4Tether[nIndex] == tIndex) || (P4Tether[d2Index] == tIndex && P4Tether[tIndex] == nIndex))
            {
                upGroup.Add(tIndex);
                upGroup.Add(d1Index);
                downGroup.Add(nIndex);
                downGroup.Add(d2Index);
            }

            var stack1 = P4Stack.IndexOf(1);
            var stack2 = P4Stack.LastIndexOf(1);
            var tetherStack = P4Tether[stack1] == -1 ? stack2 : stack1;
            var idleStack= P4Tether[stack1] == -1 ? stack1 : stack2;

            List<int> idles = [];
            for (int i = 0; i < 8; i++)
            {
                if (P4Tether[i] == -1)
                {
                    idles.Add(i);
                }
            }
            var ii = idles.IndexOf(idleStack);

            if (upGroup.Contains(tetherStack))
            {
                //线分摊在上
                if (ii==0||ii==2)
                {
                    downGroup.Add(idles[0]);
                    downGroup.Add(idles[2]);
                    upGroup.Add(idles[1]);
                    upGroup.Add(idles[3]);
                }
                if (ii == 1 || ii == 3)
                {
                    downGroup.Add(idles[1]);
                    downGroup.Add(idles[3]);
                    upGroup.Add(idles[0]);
                    upGroup.Add(idles[2]);
                }
            }
            if (downGroup.Contains(tetherStack))
            {
                //线分摊在下
                if (ii == 0 || ii == 2)
                {
                    upGroup.Add(idles[0]);
                    upGroup.Add(idles[2]);
                    downGroup.Add(idles[1]);
                    downGroup.Add(idles[3]);
                }
                if (ii == 1 || ii == 3)
                {
                    upGroup.Add(idles[1]);
                    upGroup.Add(idles[3]);
                    downGroup.Add(idles[0]);
                    downGroup.Add(idles[2]);
                }
            }

            var myindex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P4_暗光龙诗_分摊";
            dp.Scale = new(6);
            dp.Owner = accessory.Data.PartyList[tetherStack];
            dp.Color = upGroup.Contains(tetherStack)==upGroup.Contains(myindex)?accessory.Data.DefaultSafeColor: accessory.Data.DefaultDangerColor;
            dp.DestoryAt = 5000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P4_暗光龙诗_分摊";
            dp.Scale = new(6);
            dp.Owner = accessory.Data.PartyList[idleStack];
            dp.Color = upGroup.Contains(idleStack) == upGroup.Contains(myindex) ? accessory.Data.DefaultSafeColor : accessory.Data.DefaultDangerColor;
            dp.DestoryAt = 5000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P4_暗光龙诗_分摊_水晶";
            dp.Scale = new(9.5f);
            dp.Owner = P4FragmentId;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.DestoryAt = 5000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
        [ScriptMethod(name: "P4_暗光龙诗_无尽顿悟", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40249"])]
        public void P4_暗光龙诗_无尽顿悟(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P4_暗光龙诗_无尽顿悟";
            dp.Scale = new(4);
            dp.Owner =sid;
            dp.CentreResolvePattern=PositionResolvePatternEnum.OwnerTarget;
            dp.Color =accessory.Data.DefaultDangerColor;
            dp.DestoryAt = 6000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
        #region 远近跳
        //[ScriptMethod(name: "P4_暗光龙诗_远近跳", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40283"])]
        //public void P4_暗光龙诗_远近跳(Event @event, ScriptAccessory accessory)
        //{
        //    if (parse != 4.2) return;
        //    if (!ParseObjectId(@event["SourceId"], out var sid)) return;

        //    var dp = accessory.Data.GetDefaultDrawProperties();
        //    dp.Name = "P4_暗光龙诗_远跳";
        //    dp.Scale = new(8);
        //    dp.Owner = sid;
        //    dp.CentreResolvePattern = PositionResolvePatternEnum.PlayerFarestOrder;
        //    dp.Color = accessory.Data.DefaultDangerColor;
        //    dp.DestoryAt = 5000;
        //    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

        //    dp = accessory.Data.GetDefaultDrawProperties();
        //    dp.Name = "P4_暗光龙诗_近跳";
        //    dp.Scale = new(8);
        //    dp.Owner = sid;
        //    dp.CentreResolvePattern = PositionResolvePatternEnum.PlayerNearestOrder;
        //    dp.Color = accessory.Data.DefaultDangerColor;
        //    dp.Delay = 5000;
        //    dp.Delay = 3500;
        //    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

        //}
        #endregion
        [ScriptMethod(name: "P4_暗光龙诗_塔处理位置", eventType: EventTypeEnum.Tether, eventCondition: ["Id:006E"])]
        public void P4_暗光龙诗_塔处理位置(Event @event, ScriptAccessory accessory)
        {
            if (parse != 4.2) return;
            
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (sid != accessory.Data.Me) return;
            //accessory.Log.Debug("线");
            Task.Delay(50).ContinueWith(t =>
            {
                var tIndex = P4Tether[0] == -1 ? 1 : 0;
                var nIndex = P4Tether[2] == -1 ? 3 : 2;
                var d1Index = -1;
                var d2Index = -1;
                List<int> upGroup = [];
                List<int> downGroup = [];
                for (int i = 4; i < 7; i++)
                {
                    for (int j = i + 1; j < 8; j++)
                    {
                        if (P4Tether[i] != -1 && P4Tether[j] != -1)
                        {
                            d1Index = i;
                            d2Index = j;
                        }
                    }
                }
                // t连线 高d 低d 蝴蝶结
                if ((P4Tether[tIndex] == d1Index && P4Tether[d2Index] == tIndex) || (P4Tether[tIndex] == d2Index && P4Tether[d1Index] == tIndex))
                {
                    upGroup.Add(tIndex);
                    upGroup.Add(nIndex);
                    downGroup.Add(d1Index);
                    downGroup.Add(d2Index);
                }
                // t连线 高d n 方块
                if ((P4Tether[tIndex] == d1Index && P4Tether[nIndex] == tIndex) || (P4Tether[d1Index] == tIndex && P4Tether[tIndex] == nIndex))
                {
                    upGroup.Add(d1Index);
                    upGroup.Add(nIndex);
                    downGroup.Add(tIndex);
                    downGroup.Add(d2Index);
                }
                // t连线 低d n 沙漏
                if ((P4Tether[tIndex] == d2Index && P4Tether[nIndex] == tIndex) || (P4Tether[d2Index] == tIndex && P4Tether[tIndex] == nIndex))
                {
                    upGroup.Add(tIndex);
                    upGroup.Add(d1Index);
                    downGroup.Add(nIndex);
                    downGroup.Add(d2Index);
                }

                var myIndex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
                Vector3 dealpos = upGroup.Contains(myIndex) ? new(100, 0, 92) : new(100, 0, 108);

                var dur = 10000;
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P4_暗光龙诗_塔处理位置";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = dealpos;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = dur;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P4_暗光龙诗_塔处理位置";
                dp.Scale = new(4);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Position = dealpos;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = dur;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Circle, dp);
            });
            
        }
        [ScriptMethod(name: "P4_暗光龙诗_引导处理位置", eventType: EventTypeEnum.Tether, eventCondition: ["Id:006E"])]
        public void P4_暗光龙诗_引导处理位置(Event @event, ScriptAccessory accessory)
        {
            if (parse != 4.2) return;
            lock (this)
            {
                if (P4TetherDone) return;
                P4TetherDone = true;
            }
            Task.Delay(50).ContinueWith(t =>
            {
                List<int> idles = [];
                for (int i = 0; i < 8; i++)
                {
                    if (P4Tether[i] == -1)
                    {
                        idles.Add(i);
                    }
                }
                var myIndex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
                if (!idles.Contains(myIndex)) return;
                Vector3 dealpos = idles.IndexOf(myIndex) switch
                {
                    0 => new(095.8f, 0, 098.0f),
                    1 => new(104.2f, 0, 098.0f),
                    2 => new(095.8f, 0, 102.0f),
                    3 => new(104.2f, 0, 102.0f),
                };

                var dur = 10000;
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P4_暗光龙诗_引导处理位置";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = dealpos;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = dur;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
            });

        }
        [ScriptMethod(name: "P4_暗光龙诗_分摊处理位置", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4022[78])$"])]
        public void P4_暗光龙诗_分摊处理位置(Event @event, ScriptAccessory accessory)
        {
            var tIndex = P4Tether[0] == -1 ? 1 : 0;
            var nIndex = P4Tether[2] == -1 ? 3 : 2;
            var d1Index = -1;
            var d2Index = -1;
            List<int> upGroup = [];
            List<int> downGroup = [];
            for (int i = 4; i < 7; i++)
            {
                for (int j = i + 1; j < 8; j++)
                {
                    if (P4Tether[i] != -1 && P4Tether[j] != -1)
                    {
                        d1Index = i;
                        d2Index = j;
                    }
                }
            }
            // t连线 高d 低d 蝴蝶结
            if ((P4Tether[tIndex] == d1Index && P4Tether[d2Index] == tIndex) || (P4Tether[tIndex] == d2Index && P4Tether[d1Index] == tIndex))
            {
                upGroup.Add(tIndex);
                upGroup.Add(nIndex);
                downGroup.Add(d1Index);
                downGroup.Add(d2Index);
            }
            // t连线 高d n 方块
            if ((P4Tether[tIndex] == d1Index && P4Tether[nIndex] == tIndex) || (P4Tether[d1Index] == tIndex && P4Tether[tIndex] == nIndex))
            {
                upGroup.Add(d1Index);
                upGroup.Add(nIndex);
                downGroup.Add(tIndex);
                downGroup.Add(d2Index);
            }
            // t连线 低d n 沙漏
            if ((P4Tether[tIndex] == d2Index && P4Tether[nIndex] == tIndex) || (P4Tether[d2Index] == tIndex && P4Tether[tIndex] == nIndex))
            {
                upGroup.Add(tIndex);
                upGroup.Add(d1Index);
                downGroup.Add(nIndex);
                downGroup.Add(d2Index);
            }

            var stack1 = P4Stack.IndexOf(1);
            var stack2 = P4Stack.LastIndexOf(1);
            var tetherStack = P4Tether[stack1] == -1 ? stack2 : stack1;
            var idleStack = P4Tether[stack1] == -1 ? stack1 : stack2;

            List<int> idles = [];
            for (int i = 0; i < 8; i++)
            {
                if (P4Tether[i] == -1)
                {
                    idles.Add(i);
                }
            }
            var ii = idles.IndexOf(idleStack);

            if (upGroup.Contains(tetherStack))
            {
                //线分摊在上
                if (ii == 0 || ii == 2)
                {
                    downGroup.Add(idles[0]);
                    downGroup.Add(idles[2]);
                    upGroup.Add(idles[1]);
                    upGroup.Add(idles[3]);
                }
                if (ii == 1 || ii == 3)
                {
                    downGroup.Add(idles[1]);
                    downGroup.Add(idles[3]);
                    upGroup.Add(idles[0]);
                    upGroup.Add(idles[2]);
                }
            }
            if (downGroup.Contains(tetherStack))
            {
                //线分摊在下
                if (ii == 0 || ii == 2)
                {
                    upGroup.Add(idles[0]);
                    upGroup.Add(idles[2]);
                    downGroup.Add(idles[1]);
                    downGroup.Add(idles[3]);
                }
                if (ii == 1 || ii == 3)
                {
                    upGroup.Add(idles[1]);
                    upGroup.Add(idles[3]);
                    downGroup.Add(idles[0]);
                    downGroup.Add(idles[2]);
                }
            }

            var myindex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);

            Vector3 dealpos = new(@event["ActionId"] == "40227" ? 105 : 95, 0, upGroup.Contains(myindex) ? 92.5f : 107.5f);

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P4_暗光龙诗_分摊处理位置";
            dp.Scale = new(2);
            dp.ScaleMode |= ScaleMode.YByDistance;
            dp.Owner = accessory.Data.Me;
            dp.TargetPosition = dealpos;
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.DestoryAt = 5000;
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);


        }

        [ScriptMethod(name: "P4_时间结晶_分P", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40240"], userControl: false)]
        public void P4_时间结晶_分P(Event @event, ScriptAccessory accessory)
        {
            parse = 4.3d;
            P4ClawBuff = [0, 0, 0, 0, 0, 0, 0, 0];
            P4OtherBuff = [0, 0, 0, 0, 0, 0, 0, 0];
            P4WhiteCirclePos = [];
            P4WaterPos = [];
            residueIdFromEastToWest=[0,0,0,0];
            residueGuidanceHasBeenGenerated=false;
        }
        [ScriptMethod(name: "P4_时间结晶_Buff收集", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:regex:^(326[34]|2454|246[0123])$"], userControl: false)]
        public void P4_时间结晶_Buff收集(Event @event, ScriptAccessory accessory)
        {
            if (parse != 4.3) return;
            var id = @event["StatusID"];
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            var index = accessory.Data.PartyList.IndexOf(tid);
            //3623红爪 1短2长
            if (id == "3263")
            {
                if (!float.TryParse(@event["Duration"], out float dur)) return;
                P4ClawBuff[index] = dur > 20 ? 2 : 1;
            }

            if (id == "3264")
            {
                P4ClawBuff[index] = 3;
            }
            //暗 4
            if (id == "2460")
            {
                P4OtherBuff[index] = 4;
            }
            //水 3
            if (id == "2461")
            {
                P4OtherBuff[index] = 3;
            }
            //冰 1
            if (id == "2462")
            {
                P4OtherBuff[index] = 1;
            }
            //风 2
            if (id == "2463")
            {
                P4OtherBuff[index] = 2;
            }
            //土 5
            if (id == "2454")
            {
                P4OtherBuff[index] = 5;
            }
        }
        [ScriptMethod(name: "P4_时间结晶_蓝线收集", eventType: EventTypeEnum.Tether, eventCondition: ["Id:0085"], userControl: false)]
        public void P4_时间结晶_蓝线收集(Event @event, ScriptAccessory accessory)
        {
            if (parse != 4.3) return;
            var pos = JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
            P4BlueTether = PositionTo6Dir(pos, new(100, 0, 100)) % 3;
        }
        
        [ScriptMethod(name:"Phase4_Determine_Residues_Of_Drachen_Wanderers_确定圣龙气息白圈",
            eventType:EventTypeEnum.ObjectChanged,
            eventCondition:["DataId:2014529"],
            userControl:false)]
        
        public void Phase4_Determine_Residues_Of_Drachen_Wanderers_确定圣龙气息白圈(Event @event, ScriptAccessory accessory) {
            
            if(parse!=4.3) {

                return;

            }
            
            if(@event["Operate"].Equals("Remove")) {

                return;

            }

            if(!ParseObjectId(@event["SourceId"], out var sourceId)) {
            
                return;
            
            }
            
            var sourcePositionInJson=JObject.Parse(@event["SourcePosition"]);
            float currentX=sourcePositionInJson["X"]?.Value<float>()??0;

            if(residueIdFromEastToWest[0]==0
               ||
               residueIdFromEastToWest[3]==0) {

                if(currentX<100) {

                    lock(residueIdFromEastToWest) {

                        residueIdFromEastToWest[3]=sourceId;
                        // The rightmost while facing south.
                        
                    }

                }

                if(currentX>100) {

                    lock(residueIdFromEastToWest) {

                        residueIdFromEastToWest[0]=sourceId;
                        // The leftmost while facing south.
                        
                    }

                }
                
            }

            else {

                if(residueIdFromEastToWest[1]==0
                   ||
                   residueIdFromEastToWest[2]==0) { 
                    
                    if(currentX<100) {

                        lock(residueIdFromEastToWest) {

                            residueIdFromEastToWest[2]=sourceId;
                            // The about right while facing south.

                        }

                    }

                    if(currentX>100) {

                        lock(residueIdFromEastToWest) {

                            residueIdFromEastToWest[1]=sourceId;
                            // The about left while facing south.

                        }

                    }

                }
                
            }

            if(Enable_Developer_Mode_启用开发者模式) {
                
                accessory.Method.SendChat($"The SourceId of this ObjectChanged event is: {@event["SourceId"]}, the decimal sourceId acquired by parsing is: {sourceId}. The SourcePosition of this ObjectChanged event is: {@event["SourcePosition"]}, the X value acquired by parsing is: {currentX}.");
                
            }
        
        }

        [ScriptMethod(name:"Phase4_Guidance_Of_Drachen_Wanderer_Residues_圣龙气息白圈指路",
            eventType:EventTypeEnum.ActionEffect,
            eventCondition:["ActionId:regex:^(40252|40253)$"])]

        public void Phase4_Guidance_Of_Drachen_Wanderer_Residues_圣龙气息白圈指路(Event @event, ScriptAccessory accessory) {

            if(parse!=4.3) {

                return;

            }

            if(residueGuidanceHasBeenGenerated) {

                return;

            }

            if(!ParseObjectId(@event["SourceId"], out var sourceId)) {

                return;

            }

            int myIndex=accessory.Data.PartyList.IndexOf(accessory.Data.Me);
            Phase4_Positions_Of_Drachen_Wanderer_Residues positionOfMyResidue=getResiduePositionByIndex(myIndex);
            uint idOfMyResidue=getResidueIdByPosition(positionOfMyResidue);

            if(Enable_Developer_Mode_启用开发者模式) {
                
                accessory.Method.SendChat($"The object IDs acquired which are stored in residueIdFromEastToWest are: {residueIdFromEastToWest[0]}, {residueIdFromEastToWest[1]}, {residueIdFromEastToWest[2]}, {residueIdFromEastToWest[3]}.");
                accessory.Method.SendChat($"The value of myIndex is: {myIndex}, the value of P4ClawBuff[{myIndex}] is: {P4ClawBuff[myIndex]}, the value of P4OtherBuff[{myIndex}] is: {P4OtherBuff[myIndex]}, the value of positionOfMyResidue is: {positionOfMyResidue}, the value of idOfMyResidue is: {idOfMyResidue}.");
                
            }

            if(positionOfMyResidue!=Phase4_Positions_Of_Drachen_Wanderer_Residues.UNKNOWN_未知
               &&
               idOfMyResidue!=0) { 

                var currentProperty=accessory.Data.GetDefaultDrawProperties();

                currentProperty.Name="Phase4_Guidance_Of_Drachen_Wanderer_Residues_圣龙气息白圈指路";
                currentProperty.Scale=new(2);
                currentProperty.ScaleMode|=ScaleMode.YByDistance;
                currentProperty.Owner=accessory.Data.Me;
                currentProperty.Color=Phase4_Colour_Of_Hitboxes_And_Guidance_碰撞箱及指路的颜色.V4.WithW(1f);
                currentProperty.DestoryAt=23000;

                var residueObject=accessory.Data.Objects.SearchById(idOfMyResidue);

                if(residueObject!=null) {
                        
                    residueGuidanceHasBeenGenerated=true;
                    
                    currentProperty.TargetPosition=residueObject.Position;

                    accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);
                    accessory.Method.TextInfo(getResidueTextByPosition(positionOfMyResidue),2500);
                    accessory.Method.TTS(getResidueTextByPosition(positionOfMyResidue));
                    
                    if(Enable_Developer_Mode_启用开发者模式) {
                            
                        accessory.Method.SendChat($"The position retrieved from objects is: {residueObject.Position}.");
                            
                    }
                    
                }

            }

        }
        
        [ScriptMethod(name:"Phase4_Remove_Guidance_Of_Drachen_Wanderer_Residues_移除圣龙气息白圈指路",
            eventType:EventTypeEnum.StatusRemove,
            eventCondition:["StatusID:3264"],
            userControl:false)]

        public void Phase4_Remove_Guidance_Of_Drachen_Wanderer_Residues_移除圣龙气息白圈指路(Event @event, ScriptAccessory accessory) {

            if(parse!=4.3) {

                return;

            }

            if(!residueGuidanceHasBeenGenerated) {

                return;

            }

            if(!ParseObjectId(@event["TargetId"], out var targetId)) {

                return;

            }

            if(targetId!=accessory.Data.Me) {

                return;

            }
            
            accessory.Method.RemoveDraw("Phase4_Guidance_Of_Drachen_Wanderer_Residues_圣龙气息白圈指路");

        }

        [ScriptMethod(name:"Phase4_Highlights_Of_Drachen_Wanderer_Residues_圣龙气息白圈高亮",
            eventType:EventTypeEnum.ObjectChanged,
            eventCondition:["DataId:2014529"])]

        public void Phase4_Highlights_Of_Drachen_Wanderer_Residues_圣龙气息白圈高亮(Event @event, ScriptAccessory accessory) {

            if(parse!=4.3) {

                return;

            }

            if(!@event["Operate"].Equals("Add")) {

                return;

            }

            if(!ParseObjectId(@event["SourceId"], out var sourceId)) {

                return;

            }

            var currentProperty=accessory.Data.GetDefaultDrawProperties();

            currentProperty.Name=$"Phase4_Highlights_Of_Drachen_Wanderer_Residues_圣龙气息白圈高亮_{sourceId}";
            currentProperty.Scale=new(1f);
            currentProperty.InnerScale=new(0.8f);
            currentProperty.Color=accessory.Data.DefaultDangerColor.WithW(25f);
            currentProperty.Radian=float.Pi*2;
            currentProperty.Owner=sourceId;
            currentProperty.DestoryAt=17000;
            
            accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Donut,currentProperty);

        }

        [ScriptMethod(name:"Phase4_Remove_Highlights_Of_Drachen_Wanderer_Residues_移除圣龙气息白圈高亮",
            eventType:EventTypeEnum.ObjectChanged,
            eventCondition:["DataId:2014529"],
            userControl:false)]

        public void Phase4_Remove_Highlights_Of_Drachen_Wanderer_Residues_移除圣龙气息白圈高亮(Event @event, ScriptAccessory accessory) {

            if(parse!=4.3) {

                return;
    
            }

            if(!@event["Operate"].Equals("Remove")) {

                return;

            }

            if(!ParseObjectId(@event["SourceId"], out var sourceId)) {
                
                return;
                
            }
            
            accessory.Method.RemoveDraw($"Phase4_Highlights_Of_Drachen_Wanderer_Residues_圣龙气息白圈高亮_{sourceId}");

            uint idOfMyResidue=getResidueIdByPosition(getResiduePositionByIndex(accessory.Data.PartyList.IndexOf(accessory.Data.Me)));

            if(idOfMyResidue!=0
               &&
               idOfMyResidue==sourceId) {
                
                accessory.Method.RemoveDraw("Phase4_Guidance_Of_Drachen_Wanderer_Residues_圣龙气息白圈指路");
                
            }

        }
        
        [ScriptMethod(name:"Phase4_Remove_Highlights_Of_Drachen_Wanderer_Residues_In_Advance_提前移除圣龙气息白圈高亮",
            eventType:EventTypeEnum.StatusRemove,
            eventCondition:["StatusID:3264"],
            userControl:false)]
        
        // The ObjectChanged event with the property "Operate" as "Remove" would only be triggered a full three seconds after the player takes a residue.
        // If the draw removal relies on the event, it would be too late and may cause confusion.
        // Here is an optimized method for players with the Wyrmfang debuff (the blue debuff), which is to monitor the StatusRemove events of the Wyrmfang debuff and acquire the closest residue.
        // Obviously, the method will not be able to help if a player with the Wyrmclaw debuff (the red debuff) takes a residue. However that's already a wipe, so whatever.
        // Thanks to Cyf5119 for providing a Dalamud way to detect if the entity is dead, so that the method would skip the StatusRemove events caused by death.

        public void Phase4_Remove_Highlights_Of_Drachen_Wanderer_Residues_In_Advance_提前移除圣龙气息白圈高亮(Event @event, ScriptAccessory accessory) {
            
            if(parse!=4.3) {

                return;

            }

            if(!ParseObjectId(@event["TargetId"], out var targetId)) {

                return;

            }
            
            var targetObject=accessory.Data.Objects.SearchById(targetId);

            if(targetObject==null) {

                return;

            }
            
            Vector3 targetPosition=targetObject.Position;

            if((IBattleChara?)targetObject==null) {

                return;

            }

            if(((IBattleChara?)targetObject).IsDead) {
                // Ignore the situations that the debuff was removed due to a death.

                return;

            }

            int closestResidue=-1;
            float closestDistance=float.PositiveInfinity;

            for(int i=0;i<4;++i) {

                var residueObject=accessory.Data.Objects.SearchByEntityId(residueIdFromEastToWest[i]);

                if(residueObject!=null) {

                    if(Vector3.Distance(targetPosition,residueObject.Position)<closestDistance) {

                        closestResidue=i;
                        closestDistance=Vector3.Distance(targetPosition,residueObject.Position);

                    }
                    
                }

            }

            if(0<=closestResidue&&closestResidue<4) {
                
                accessory.Method.RemoveDraw($"Phase4_Highlights_Of_Drachen_Wanderer_Residues_圣龙气息白圈高亮_{residueIdFromEastToWest[closestResidue]}");
                
            }

        }
        
        private Phase4_Positions_Of_Drachen_Wanderer_Residues getResiduePositionByIndex(int currentIndex) {

            if(currentIndex<0||currentIndex>7) {
                
                return Phase4_Positions_Of_Drachen_Wanderer_Residues.UNKNOWN_未知;
                
            }
            
            if(P4ClawBuff[currentIndex]==1||P4ClawBuff[currentIndex]==2) {
                // 1 stands for short Wyrmclaw (the red debuff), 2 stands for long Wyrmclaw (also the red debuff).

                return Phase4_Positions_Of_Drachen_Wanderer_Residues.UNKNOWN_未知;

            }

            if(P4ClawBuff[currentIndex]==3) {
                // 3 stands for Wyrmfang (the blue debuff).

                if(P4OtherBuff[currentIndex]==4) {
                    // 4 stands for Dark Eruption.

                    return Phase4_Residue_For_Dark_Eruption_暗炎喷发的白圈;

                }

                if(P4OtherBuff[currentIndex]==5) {
                    // 5 stands for Unholy Darkness.

                    return Phase4_Residue_For_Unholy_Darkness_黑暗神圣的白圈;

                }

                if(P4OtherBuff[currentIndex]==1) {
                    // 1 stands for Dark Blizzard III.

                    return Phase4_Residue_For_Dark_Blizzard_III_黑暗冰封的白圈;

                }

                if(P4OtherBuff[currentIndex]==3) {
                    // 3 stands for Dark Water III.

                    return Phase4_Residue_For_Dark_Water_III_黑暗狂水的白圈;

                }

            }

            return Phase4_Positions_Of_Drachen_Wanderer_Residues.UNKNOWN_未知;
            // Just a placeholder and should never be reached.

        }

        private uint getResidueIdByPosition(Phase4_Positions_Of_Drachen_Wanderer_Residues residuePosition) {
            
            switch(residuePosition) {

                case(Phase4_Positions_Of_Drachen_Wanderer_Residues.EASTMOST_最东侧): {

                    return residueIdFromEastToWest[0];

                }
                
                case(Phase4_Positions_Of_Drachen_Wanderer_Residues.ABOUT_EAST_次东侧): {

                    return residueIdFromEastToWest[1];

                }
                
                case(Phase4_Positions_Of_Drachen_Wanderer_Residues.ABOUT_WEST_次西侧): {

                    return residueIdFromEastToWest[2];

                }

                case(Phase4_Positions_Of_Drachen_Wanderer_Residues.WESTMOST_最西侧): {

                    return residueIdFromEastToWest[3];

                }

                case(Phase4_Positions_Of_Drachen_Wanderer_Residues.UNKNOWN_未知): {

                    return 0;

                }

                default: {

                    return 0;
                    // Just a placeholder and should never be reached.

                }
                
            }
            
        }

        private String getResidueTextByPosition(Phase4_Positions_Of_Drachen_Wanderer_Residues residuePosition) {

            switch (residuePosition) {

                case(Phase4_Positions_Of_Drachen_Wanderer_Residues.EASTMOST_最东侧): {

                    return "Leftmost/Eastmost 最左/最东";

                }
                
                case(Phase4_Positions_Of_Drachen_Wanderer_Residues.ABOUT_EAST_次东侧): {

                    return "About left/About east 次左/次东";

                }
                
                case(Phase4_Positions_Of_Drachen_Wanderer_Residues.ABOUT_WEST_次西侧): {

                    return "About right/About west 次右/次西";

                }

                case(Phase4_Positions_Of_Drachen_Wanderer_Residues.WESTMOST_最西侧): {

                    return "Rightmost/Westmost 最右/最西";

                }

                case(Phase4_Positions_Of_Drachen_Wanderer_Residues.UNKNOWN_未知): {
                    
                    return "Unknown residue 白圈未知";
                    
                }

                default: {

                    return "Unknown residue 白圈未知";
                    // Just a placeholder and should never be reached.

                }
                
            }
            
        }
        
        [ScriptMethod(name:"Phase4_Hitboxes_Of_Drachen_Wanderers_圣龙气息碰撞箱",
            eventType:EventTypeEnum.AddCombatant,
            eventCondition:["DataId:17836"])]

        public void Phase4_Hitboxes_Of_Drachen_Wanderers_圣龙气息碰撞箱(Event @event, ScriptAccessory accessory) {

            if(parse!=4.3) {

                return;

            }

            if(!ParseObjectId(@event["SourceId"], out var sourceId)) {

                return;

            }

            var currentProperty=accessory.Data.GetDefaultDrawProperties();

            currentProperty.Name=$"Phase4_Hitboxes_Of_Drachen_Wanderers_圣龙气息碰撞箱_{sourceId}";
            currentProperty.Scale=new(2f,Phase4_Length_Of_Drachen_Hitboxes_龙头碰撞箱长度);
            currentProperty.Color=Phase4_Colour_Of_Hitboxes_And_Guidance_碰撞箱及指路的颜色.V4.WithW(25f);
            currentProperty.Offset=new(0f,0f,1f);
            currentProperty.Owner=sourceId;
            currentProperty.DestoryAt=34000;
            
            accessory.Method.SendDraw(DrawModeEnum.Default,DrawTypeEnum.Displacement,currentProperty);

        }
        
        [ScriptMethod(name:"Phase4_Explosion_Ranges_Of_Drachen_Wanderers_圣龙气息爆炸范围",
            eventType:EventTypeEnum.AddCombatant,
            eventCondition:["DataId:17836"])]

        public void Phase4_Explosion_Ranges_Of_Drachen_Wanderers_圣龙气息爆炸范围(Event @event, ScriptAccessory accessory) {

            if(parse!=4.3) {

                return;

            }

            if(!ParseObjectId(@event["SourceId"], out var sourceId)) {

                return;

            }

            var currentProperty=accessory.Data.GetDefaultDrawProperties();
                
            currentProperty.Name=$"Phase4_Explosion_Ranges_Of_Drachen_Wanderers_圣龙气息爆炸范围_{sourceId}";
            currentProperty.Scale=new(12);
            currentProperty.Owner=sourceId;
            currentProperty.Color=accessory.Data.DefaultDangerColor;
            currentProperty.DestoryAt=34000;
                
            accessory.Method.SendDraw(DrawModeEnum.Default,DrawTypeEnum.Circle,currentProperty);

        }
        
        [ScriptMethod(name:"Phase4_Remove_Hitboxes_And_Explosion_Ranges_Of_Drachen_Wanderers_移除圣龙气息碰撞箱与爆炸范围",
            eventType:EventTypeEnum.RemoveCombatant,
            eventCondition:["DataId:17836"],
            userControl:false)]

        public void Phase4_Remove_Hitboxes_And_Explosion_Ranges_Of_Drachen_Wanderers_移除圣龙气息碰撞箱与爆炸范围(Event @event, ScriptAccessory accessory) {

            if(parse!=4.3) {

                return;

            }

            if(!ParseObjectId(@event["SourceId"], out var sourceId)) {

                return;

            }

            accessory.Method.RemoveDraw($"Phase4_Hitboxes_Of_Drachen_Wanderers_圣龙气息碰撞箱_{sourceId}");
            accessory.Method.RemoveDraw($"Phase4_Explosion_Ranges_Of_Drachen_Wanderers_圣龙气息爆炸范围_{sourceId}");

        }

        [ScriptMethod(name: "P4_时间结晶_灯AOE", eventType: EventTypeEnum.Tether, eventCondition: ["Id:0085"])]
        public void P4_时间结晶_灯AOE(Event @event, ScriptAccessory accessory)
        {
            if (parse != 4.3) return;
            var pos = JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
            Vector3 normalPos = new(pos.X, 0, 200 - pos.Z);
            Vector3 fastPos = new(100, 0, pos.Z > 100 ? 111 : 89);

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P4_时间结晶_灯AOE_快";
            dp.Scale = new(12);
            dp.Position = fastPos;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.DestoryAt = 7500;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            
            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P4_时间结晶_灯AOE_中";
            dp.Scale = new(12);
            dp.Position = normalPos;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 13000 - P4LampDisplayDur;
            dp.DestoryAt = P4LampDisplayDur;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P4_时间结晶_灯AOE_慢";
            dp.Scale = new(12);
            dp.Position = pos;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 18000 - P4LampDisplayDur;
            dp.DestoryAt = P4LampDisplayDur;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
        [ScriptMethod(name: "P4_时间结晶_土分摊范围", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:2454"])]
        public void P4_时间结晶_土分摊范围(Event @event, ScriptAccessory accessory)
        {
            if (parse != 4.3) return;
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P4_时间结晶_土分摊范围";
            dp.Scale = new(6);
            dp.Owner = tid;
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.Delay = 14000;
            dp.DestoryAt = 3000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P4_时间结晶_土分摊范围_水晶";
            dp.Scale = new(9.5f);
            dp.Owner = P4FragmentId;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 14000;
            dp.DestoryAt = 3000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
        [ScriptMethod(name: "P4_时间结晶_碎灵一击", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:2452"])]
        public void P4_时间结晶_碎灵一击(Event @event, ScriptAccessory accessory)
        {
            if (parse != 4.3) return;
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            if (tid != accessory.Data.Me) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P4_时间结晶_碎灵一击_水晶";
            dp.Scale = new(8.5f);
            dp.Owner = P4FragmentId;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.DestoryAt = 3500;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            for (int i = 0; i < 8; i++)
            {
                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P4_时间结晶_碎灵一击";
                dp.Scale = new(5);
                dp.Owner = accessory.Data.PartyList[i];
                dp.Color = accessory.Data.DefaultDangerColor;
                dp.DestoryAt = 3500;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            }
            
            
        }
        [ScriptMethod(name: "P4_时间结晶_Buff处理位置", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40293"])]
        public void P4_时间结晶_Buff处理位置(Event @event, ScriptAccessory accessory)
        {
            
            //buff后3.5s
            if (parse != 4.3) return;
            var myIndex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
            //短红
            if (P4ClawBuff[myIndex] == 1)
            {
                var isHigh = P4ClawBuff.IndexOf(1) == myIndex;
                Vector3 dealpos= isHigh ? new(87, 0, 100) : new(113, 0, 100);

                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P4_时间结晶_Buff处理位置_撞龙头";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = dealpos;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = 10500;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                //高分摊 088 085 -> 093 082
                //高闲   081 103 -> 081 097
                //低分摊 112 085 -> 107 082
                //低闲   119 103 -> 119 97
                Vector3 dealpos2 = isHigh ? (P4BlueTether == 1 ? new(081, 0, 103) : new(088, 0, 085)) : (P4BlueTether == 1 ? new(112, 0, 085) : new(119, 0, 103));
                Vector3 dealpos3 = isHigh ? (P4BlueTether == 1 ? new(081, 0, 097) : new(093, 0, 082)) : (P4BlueTether == 1 ? new(107, 0, 082) : new(119, 0, 097));

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P4_时间结晶_Buff处理位置_pos2预连线";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Position = dealpos;
                dp.TargetPosition = dealpos2;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = 10500;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P4_时间结晶_Buff处理位置_pos2位置";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = dealpos2;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.Delay = 10500;
                dp.DestoryAt = 3000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P4_时间结晶_Buff处理位置_pos3预连线";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Position = dealpos2;
                dp.TargetPosition = dealpos3;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = 13500;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P4_时间结晶_Buff处理位置_pos3位置";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = dealpos3;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.Delay = 13500;
                dp.DestoryAt = 3000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
            }
            //长红
            if (P4ClawBuff[myIndex] == 2)
            {
                var isHigh = P4ClawBuff.IndexOf(2) == myIndex;
                Vector3 dealpos1 = isHigh ? new(088.5f, 0, 115.5f) : new(111.5f, 0, 115.5f);
                Vector3 dealpos2 = isHigh ? new(090.2f, 0, 117.0f) : new(109.8f, 0, 117.0f);
                Vector3 dealpos3 = isHigh ? new(092.5f, 0, 118.0f) : new(107.5f, 0, 118.0f);
                Vector3 dealpos4 = isHigh ? new(092.0f, 0, 110.0f) : new(108.0f, 0, 110.0f);

                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P4_时间结晶_Buff处理位置_躲ac";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = dealpos1;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = 7500;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P4_时间结晶_Buff处理位置_躲ac->击退";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Position = dealpos1;
                dp.TargetPosition = dealpos2;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = 7500;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P4_时间结晶_Buff处理位置_击退";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = dealpos2;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.Delay = 7500;
                dp.DestoryAt = 3000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P4_时间结晶_Buff处理位置_击退->躲斜点";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Position = dealpos2;
                dp.TargetPosition = dealpos3;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = 10500;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P4_时间结晶_Buff处理位置_躲斜点";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = dealpos3;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.Delay = 10500;
                dp.DestoryAt = 2500;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P4_时间结晶_Buff处理位置_躲斜点->撞头";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Position = dealpos3;
                dp.TargetPosition = dealpos4;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = 16000;
                // The value has been adjusted by Cicero. It was 13000 before.
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P4_时间结晶_Buff处理位置_撞头";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = dealpos2;
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.Delay = 16000;
                // The value has been adjusted by Cicero. It was 13000 before.
                dp.DestoryAt = 2000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
            }
            //蓝
            if (P4ClawBuff[myIndex] == 3)
            {
                if (P4OtherBuff[myIndex] == 4)
                {
                    Vector3 dealpos1 = P4BlueTether == 1 ? new(112, 0, 85) : new(88, 0, 85);
                    var dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P4_时间结晶_Buff处理位置_躲灯1";
                    dp.Scale = new(2);
                    dp.ScaleMode |= ScaleMode.YByDistance;
                    dp.Owner = accessory.Data.Me;
                    dp.TargetPosition = dealpos1;
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.DestoryAt = 14500;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                }
                else
                {
                    Vector3 dealpos1 = P4BlueTether == 1 ? new(88, 0, 115) : new(112, 0, 115);
                    Vector3 dealpos2 = P4BlueTether == 1 ? new(090.8f, 0, 116.0f) : new(109.2f, 0, 116.0f);
                    var dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P4_时间结晶_Buff处理位置_躲灯ac";
                    dp.Scale = new(2);
                    dp.ScaleMode |= ScaleMode.YByDistance;
                    dp.Owner = accessory.Data.Me;
                    dp.TargetPosition = dealpos1;
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.DestoryAt = 7500;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

                    dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P4_时间结晶_Buff处理位置_躲ac->击退";
                    dp.Scale = new(2);
                    dp.ScaleMode |= ScaleMode.YByDistance;
                    dp.Position = dealpos1;
                    dp.TargetPosition = dealpos2;
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.DestoryAt = 7500;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                    dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "P4_时间结晶_Buff处理位置_击退";
                    dp.Scale = new(2);
                    dp.ScaleMode |= ScaleMode.YByDistance;
                    dp.Owner = accessory.Data.Me;
                    dp.TargetPosition = dealpos2;
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.Delay = 7500;
                    dp.DestoryAt = 3000;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                }
            }
        }
        [ScriptMethod(name: "P4_时间结晶_白圈位置指示", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:40241", "TargetIndex:1"])]
        public void P4_时间结晶_白圈位置指示(Event @event, ScriptAccessory accessory)
        {
            if (parse != 4.3) return;
            var pos = JsonConvert.DeserializeObject<Vector3>(@event["EffectPosition"]);
            lock (P4WhiteCirclePos)
            {
                P4WhiteCirclePos.Add(pos);
                if (P4WhiteCirclePos.Count == 1 || P4WhiteCirclePos.Count == 3) return;

            }
        }
        [ScriptMethod(name: "P4_时间结晶_放回返位置", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40251"])]
        public void P4_时间结晶_放回返位置(Event @event, ScriptAccessory accessory)
        {
            if (parse != 4.3) return;
            var pos = JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
            P4WaterPos.Add(pos);
            if (P4WaterPos.Count == 1) return;
            var myindex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
            Vector3 centre = new(100, 0, 100);
            var dir8 = PositionTo8Dir((P4WaterPos[0] + P4WaterPos[1]) / 2, centre)-1;
            Vector3 mtPos = new(107, 0, 88);
            Vector3 stPos = new(112, 0, 93);
            Vector3 mtgPos = new(106, 0, 92);
            Vector3 stgPos = new(108, 0, 94);
            if (myindex==0)
            {
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P4_时间结晶_放回返位置_MT";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = RotatePoint(mtPos, centre, float.Pi / 4 * dir8);
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = 9000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
            }
            if (myindex == 1)
            {
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P4_时间结晶_放回返位置_ST";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = RotatePoint(stPos, centre, float.Pi / 4 * dir8);
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = 9000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
            }
            if (myindex == 2 || myindex == 4 || myindex == 6)
            {
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P4_时间结晶_放回返位置_MTG";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = RotatePoint(mtgPos, centre, float.Pi / 4 * dir8);
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = 9000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
            }
            if (myindex == 3 || myindex == 5 || myindex == 7)
            {
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "P4_时间结晶_放回返位置_STG";
                dp.Scale = new(2);
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = RotatePoint(stgPos, centre, float.Pi / 4 * dir8);
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = 9000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
            }
        }

        #endregion

        #region P5
        [ScriptMethod(name: "P5_地火", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40118|40307)$"])]
        public void P5_地火(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P5_地火";
            dp.Scale = new(80, 5);
            dp.Owner = sid;
            dp.Color = P5PathColor.V4.WithW(3);
            dp.DestoryAt = 7000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"P5_地火_前进_{@event["SourceId"]}";
            dp.Scale = new(80, 5);
            dp.Offset = new(0,0,-5);
            dp.Owner = sid;
            dp.Color = P5PathColor.V4.WithW(3);
            dp.Delay = 7000;
            dp.DestoryAt = 20000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);

        }
        [ScriptMethod(name: "P5_地火消除", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:regex:^(40118|4030[789])$"],userControl:false)]
        public void P5_地火消除(Event @event, ScriptAccessory accessory)
        {
            if (!float.TryParse(@event["SourceRotation"],out var rot)) return;
            var pos = JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
            Vector3 centre = new(100, 0, 100);
            Vector3 posNext = new(pos.X + 5 * MathF.Sin(rot), 0, pos.Z + 5 * MathF.Cos(rot));
            if ((posNext- centre).Length()>20)
            {
                accessory.Method.RemoveDraw($"P5_地火_前进_{@event["SourceId"]}");
            }
        }

        [ScriptMethod(name: "P5_光与暗之翼", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40313|40233)$"])]
        public void P5_光与暗之翼(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var r = 225f;
            var rot = (180 - r / 2) / 180f * float.Pi;
            
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P5_光与暗之翼";
            dp.Scale = new(20);
            dp.Owner = sid;
            dp.Radian = r / 180 * float.Pi;
            dp.TargetObject = accessory.Data.EnmityList[sid][0];
            dp.Rotation = @event["ActionId"] == "40313" ? rot : -rot;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.DestoryAt = 7300;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P5_光与暗之翼_远离靠近";
            dp.Scale = new(4);
            dp.Owner = sid;
            dp.CentreResolvePattern = @event["ActionId"] == "40313"? PositionResolvePatternEnum.PlayerFarestOrder: PositionResolvePatternEnum.PlayerNearestOrder;
            dp.Rotation = @event["ActionId"] == "40313" ? rot : -rot;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.DestoryAt = 7300;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P5_光与暗之翼";
            dp.Scale = new(20);
            dp.Owner = sid;
            dp.Radian = r / 180 * float.Pi;
            dp.TargetResolvePattern = PositionResolvePatternEnum.OwnerTarget;
            dp.Rotation = @event["ActionId"] == "40313" ? -rot : rot;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 7300;
            dp.DestoryAt = 4000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);

            dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "P5_光与暗之翼_远离靠近";
            dp.Scale = new(4);
            dp.Owner = sid;
            dp.CentreResolvePattern = @event["ActionId"] == "40313" ? PositionResolvePatternEnum.PlayerNearestOrder : PositionResolvePatternEnum.PlayerFarestOrder;
            dp.Rotation = @event["ActionId"] == "40313" ? rot : -rot;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Delay = 7300;
            dp.DestoryAt = 4000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
        
        [ScriptMethod(name:"Phase5_Reset_Data_Of_Wings_Dark_And_Light_重置光与暗之翼的数据",
            eventType:EventTypeEnum.StartCasting,
            eventCondition:["ActionId:40319"],
            userControl:false)]
        
        public void Phase5_Reset_Data_Of_Wings_Dark_And_Light_重置光与暗之翼的数据(Event @event, ScriptAccessory accessory) {
            
            hasAcquiredTheFirstTower=false;
            indexOfTheFirstTower="";
            hasDrawnTheInitialPositionOfMT=false;
            
        }
        
        [ScriptMethod(name:"Phase5_Acquire_The_First_Tower_Of_Wings_Dark_And_Light_获取光与暗之翼的第一座塔",
            eventType:EventTypeEnum.EnvControl,
            eventCondition:["DirectorId:800375BF","State:00010004","Index:regex:^(0000003[012])"],
            userControl:false)]
        
        public void Phase5_Acquire_The_First_Tower_Of_Wings_Dark_And_Light_获取光与暗之翼的第一座塔(Event @event, ScriptAccessory accessory) {

            if(!hasAcquiredTheFirstTower) {
                
                indexOfTheFirstTower=@event["Index"];
                
                hasAcquiredTheFirstTower=true;
                
            }
            
        }
        
        [ScriptMethod(name:"Phase5_Initial_Position_Of_MT_During_Wings_Dark_And_Light_光与暗之翼MT初始位置",
            eventType:EventTypeEnum.EnvControl,
            eventCondition:["DirectorId:800375BF","State:00010004","Index:regex:^(0000003[012])"])]
        
        public void Phase5_Initial_Position_Of_MT_During_Wings_Dark_And_Light_光与暗之翼MT初始位置(Event @event, ScriptAccessory accessory) {

            if(accessory.Data.PartyList.IndexOf(accessory.Data.Me)!=0) {

                return;

            }
            
            if(hasDrawnTheInitialPositionOfMT) {

                return;
                
            }

            while(!hasAcquiredTheFirstTower);

            Vector3 positionOfTheFirstTower=new Vector3(0,0,0);

            if(indexOfTheFirstTower.Equals("00000030")) {
                
                hasDrawnTheInitialPositionOfMT=true;

                positionOfTheFirstTower=new Vector3(93.94f,0,96.50f);

            }
            
            if(indexOfTheFirstTower.Equals("00000031")) {
                
                hasDrawnTheInitialPositionOfMT=true;
                
                positionOfTheFirstTower=new Vector3(106.06f,0,96.50f);
                
            }
            
            if(indexOfTheFirstTower.Equals("00000032")) {
                
                hasDrawnTheInitialPositionOfMT=true;
                
                positionOfTheFirstTower=new Vector3(100,0,107);
                
            }

            if(!hasDrawnTheInitialPositionOfMT) {
                
                hasDrawnTheInitialPositionOfMT=true;

                return;

            }

            if(Phase5_Strats_Of_Wings_Dark_And_Light_光与暗之翼策略==Phase5_Strats_Of_Wings_Dark_And_Light.GREY9_灰九式) {

                Vector3 initialPositionOfMt=RotatePoint(positionOfTheFirstTower,new Vector3(100, 0, 100),float.Pi);

                var currentProperty=accessory.Data.GetDefaultDrawProperties();

                currentProperty.Name="Phase5_Initial_Position_Of_MT_During_Wings_Dark_And_Light_光与暗之翼MT初始位置";
                currentProperty.Scale=new(2);
                currentProperty.Owner=accessory.Data.Me;
                currentProperty.TargetPosition=initialPositionOfMt;
                currentProperty.ScaleMode|=ScaleMode.YByDistance;
                currentProperty.Color=accessory.Data.DefaultSafeColor;
                currentProperty.DestoryAt=2300;

                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, currentProperty);

            }

        }
        
        [ScriptMethod(name:"Phase5_Guidance_For_Tanks_During_Wings_Dark_And_Light_光与暗之翼坦克指路",
            eventType:EventTypeEnum.StartCasting,
            eventCondition:["ActionId:regex:^(40313|40233)$"])]
        
        public void Phase5_Guidance_For_Tanks_During_Wings_Dark_And_Light_光与暗之翼坦克指路(Event @event, ScriptAccessory accessory) {
            
            if(accessory.Data.PartyList.IndexOf(accessory.Data.Me)!=0
               &&
               accessory.Data.PartyList.IndexOf(accessory.Data.Me)!=1) {

                return;

            }
            
            if(!hasAcquiredTheFirstTower) {

                return;
                
            }

            bool isLeftFirstAndFarFirst=true;

            if(@event["ActionId"].Equals("40313")) {
                // 40313 stands for left first then right, far first then near.

                isLeftFirstAndFarFirst=true;

            }

            if(@event["ActionId"].Equals("40233")) {
                // 40233 stands for right first then left, near first then far.

                isLeftFirstAndFarFirst=false;

            }

            Vector3 positionOfTheFirstTower=new Vector3(0,0,0);
            bool hasConfirmedThePositionOfTheFirstTower=false;

            if(indexOfTheFirstTower.Equals("00000030")) {

                positionOfTheFirstTower=new Vector3(93.94f,0,96.50f);
                
                hasConfirmedThePositionOfTheFirstTower=true;

            }
            
            if(indexOfTheFirstTower.Equals("00000031")) {
                
                positionOfTheFirstTower=new Vector3(106.06f,0,96.50f);
                
                hasConfirmedThePositionOfTheFirstTower=true;
                
            }
            
            if(indexOfTheFirstTower.Equals("00000032")) {
                
                positionOfTheFirstTower=new Vector3(100,0,107);
                
                hasConfirmedThePositionOfTheFirstTower=true;
                
            }

            if(!hasConfirmedThePositionOfTheFirstTower) {

                return;

            }

            if(Phase5_Strats_Of_Wings_Dark_And_Light_光与暗之翼策略==Phase5_Strats_Of_Wings_Dark_And_Light.GREY9_灰九式) {

                Vector3 mtPosition1=RotatePoint(positionOfTheFirstTower,new Vector3(100,0,100),float.Pi);
                // Just opposite the first tower.
                Vector3 mtPosition2=isLeftFirstAndFarFirst?
                    new((mtPosition1.X-100)/7+100,0,(mtPosition1.Z-100)/7+100):
                    new((mtPosition1.X-100)/7*18+100,0,(mtPosition1.Z-100)/7*18+100);
                // The calculations of Position 2 were directly inherited from Karlin-Z's original script.
                // I don't know the mathematical ideas behind the algorithm, but it works and it definitely works great.
                // So as a result, except the multiplier was adjusted from 15 to 18, I just keep the part as is.
                
                Vector3 otPosition2=RotatePoint(mtPosition1,new(100,0,100),isLeftFirstAndFarFirst?
                    120f.DegToRad():
                    // Rotate right, since the boss will hit left.
                    -120f.DegToRad());
                    // Rotate left.
                Vector3 otPosition1=isLeftFirstAndFarFirst?
                    new((otPosition2.X-100)/7*18+100,0,(otPosition2.Z-100)/7*18+100):
                    new((otPosition2.X-100)/7+100,0,(otPosition2.Z-100)/7+100);
                
                if(accessory.Data.PartyList.IndexOf(accessory.Data.Me)==0) {

                    var currentProperty=accessory.Data.GetDefaultDrawProperties();
                    currentProperty.Name="Phase5_MT_Position_1_During_Wings_Dark_And_Light_光与暗之翼MT位置1";
                    currentProperty.Scale=new(2);
                    currentProperty.Owner=accessory.Data.Me;
                    currentProperty.TargetPosition=mtPosition1;
                    currentProperty.ScaleMode|=ScaleMode.YByDistance;
                    currentProperty.Color=accessory.Data.DefaultSafeColor;
                    currentProperty.DestoryAt=6900;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);

                    currentProperty=accessory.Data.GetDefaultDrawProperties();
                    currentProperty.Name="Phase5_MT_Position_2_Preview_During_Wings_Dark_And_Light_光与暗之翼MT位置2预览";
                    currentProperty.Scale=new(2);
                    currentProperty.Position=mtPosition1;
                    currentProperty.TargetPosition=mtPosition2;
                    currentProperty.ScaleMode|=ScaleMode.YByDistance;
                    currentProperty.Color=accessory.Data.DefaultSafeColor;
                    currentProperty.DestoryAt=6900;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);

                    currentProperty=accessory.Data.GetDefaultDrawProperties();
                    currentProperty.Name="Phase5_MT_Position_2_During_Wings_Dark_And_Light_光与暗之翼MT位置2";
                    currentProperty.Scale=new(2);
                    currentProperty.Owner=accessory.Data.Me;
                    currentProperty.TargetPosition=mtPosition2;
                    currentProperty.ScaleMode|=ScaleMode.YByDistance;
                    currentProperty.Color=accessory.Data.DefaultSafeColor;
                    currentProperty.Delay=6900;
                    currentProperty.DestoryAt=4500;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);
                    
                    if(Phase5_Hints_Of_Provoking_挑衅提示) {
                        
                        System.Threading.Thread.Sleep(2000);
                        
                        accessory.Method.TextInfo("OT provoke not you! ST挑衅不是你！",2500);
                        accessory.Method.TTS("OT provoke not you! ST挑衅不是你！");
                        
                    }

                }

                if(accessory.Data.PartyList.IndexOf(accessory.Data.Me)==1) {

                    var currentProperty=accessory.Data.GetDefaultDrawProperties();
                    currentProperty.Name="Phase5_OT_Position_1_During_Wings_Dark_And_Light_光与暗之翼ST位置1";
                    currentProperty.Scale=new(2);
                    currentProperty.Owner=accessory.Data.Me;
                    currentProperty.TargetPosition=otPosition1;
                    currentProperty.ScaleMode|=ScaleMode.YByDistance;
                    currentProperty.Color=accessory.Data.DefaultSafeColor;
                    currentProperty.DestoryAt=7900;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);

                    currentProperty=accessory.Data.GetDefaultDrawProperties();
                    currentProperty.Name="Phase5_OT_Position_2_Preview_During_Wings_Dark_And_Light_光与暗之翼ST位置2预览";
                    currentProperty.Scale=new(2);
                    currentProperty.Position=otPosition1;
                    currentProperty.TargetPosition=otPosition2;
                    currentProperty.ScaleMode|=ScaleMode.YByDistance;
                    currentProperty.Color=accessory.Data.DefaultSafeColor;
                    currentProperty.DestoryAt=7900;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);
                    
                    currentProperty=accessory.Data.GetDefaultDrawProperties();
                    currentProperty.Name="Phase5_OT_Position_3_Preview_During_Wings_Dark_And_Light_光与暗之翼ST位置3预览";
                    currentProperty.Scale=new(2);
                    currentProperty.Position=otPosition2;
                    currentProperty.TargetPosition=new Vector3(100,0,93);
                    currentProperty.ScaleMode|=ScaleMode.YByDistance;
                    currentProperty.Color=accessory.Data.DefaultSafeColor;
                    currentProperty.DestoryAt=7900;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);

                    currentProperty=accessory.Data.GetDefaultDrawProperties();
                    currentProperty.Name="Phase5_OT_Position_2_During_Wings_Dark_And_Light_光与暗之翼ST位置2";
                    currentProperty.Scale=new(2);
                    currentProperty.Owner=accessory.Data.Me;
                    currentProperty.TargetPosition=otPosition2;
                    currentProperty.ScaleMode|=ScaleMode.YByDistance;
                    currentProperty.Color=accessory.Data.DefaultSafeColor;
                    currentProperty.Delay=7900;
                    currentProperty.DestoryAt=3500;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);
                    
                    currentProperty=accessory.Data.GetDefaultDrawProperties();
                    currentProperty.Name="Phase5_OT_Position_3_Preview_During_Wings_Dark_And_Light_光与暗之翼ST位置3预览";
                    currentProperty.Scale=new(2);
                    currentProperty.Position=otPosition2;
                    currentProperty.TargetPosition=new Vector3(100,0,93);
                    currentProperty.ScaleMode|=ScaleMode.YByDistance;
                    currentProperty.Color=accessory.Data.DefaultSafeColor;
                    currentProperty.Delay=7900;
                    currentProperty.DestoryAt=3500;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);
                    
                    currentProperty=accessory.Data.GetDefaultDrawProperties();
                    currentProperty.Name="Phase5_OT_Position_3_During_Wings_Dark_And_Light_光与暗之翼ST位置3";
                    currentProperty.Scale=new(2);
                    currentProperty.Owner=accessory.Data.Me;
                    currentProperty.TargetPosition=new Vector3(100,0,93);
                    currentProperty.ScaleMode|=ScaleMode.YByDistance;
                    currentProperty.Color=accessory.Data.DefaultSafeColor;
                    currentProperty.Delay=11400;
                    currentProperty.DestoryAt=6000;
                    accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);

                    if(Phase5_Hints_Of_Provoking_挑衅提示) {
                        
                        System.Threading.Thread.Sleep(2000);
                        
                        accessory.Method.TextInfo("Provoke! 挑衅！",2500);
                        accessory.Method.TTS("Provoke! 挑衅！");
                        
                    }

                }
                
            }

        }
        
        [ScriptMethod(name:"Phase5_Guidance_For_Others_During_Wings_Dark_And_Light_光与暗之翼人群指路",
            eventType:EventTypeEnum.StartCasting,
            eventCondition:["ActionId:regex:^(40313|40233)$"])]
        
        public void Phase5_Guidance_For_Others_During_Wings_Dark_And_Light_光与暗之翼人群指路(Event @event, ScriptAccessory accessory) {
            
            if(accessory.Data.PartyList.IndexOf(accessory.Data.Me)==0
               ||
               accessory.Data.PartyList.IndexOf(accessory.Data.Me)==1) {

                return;

            }
            
            if(!hasAcquiredTheFirstTower) {

                return;
                
            }

            bool isLeftFirstAndFarFirst=true;

            if(@event["ActionId"].Equals("40313")) {

                isLeftFirstAndFarFirst=true;

            }

            if(@event["ActionId"].Equals("40233")) {

                isLeftFirstAndFarFirst=false;

            }
            
            if(Phase5_Strats_Of_Wings_Dark_And_Light_光与暗之翼策略==Phase5_Strats_Of_Wings_Dark_And_Light.GREY9_灰九式) {
                
                var currentProperty=accessory.Data.GetDefaultDrawProperties();

                if(indexOfTheFirstTower.Equals("00000030")) {
                    // The first tower is in the northwest.
                    
                    if(accessory.Data.PartyList.IndexOf(accessory.Data.Me)==4
                       ||
                       accessory.Data.PartyList.IndexOf(accessory.Data.Me)==5) {
                        // The melee group, that is M1 & M2 or D1 & D2.
                        
                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_1_For_Melees_During_Wings_Dark_And_Light_光与暗之翼近战指路1";
                        currentProperty.Scale=new(2);
                        currentProperty.Owner=accessory.Data.Me;
                        currentProperty.TargetPosition=isLeftFirstAndFarFirst?rightSideOfNorthwest:leftSideOfNorthwest;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.DestoryAt=7300;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);
                        
                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_2_Preview_For_Melees_During_Wings_Dark_And_Light_光与暗之翼近战指路2预览";
                        currentProperty.Scale=new(2);
                        currentProperty.Position=isLeftFirstAndFarFirst?rightSideOfNorthwest:leftSideOfNorthwest;
                        currentProperty.TargetPosition=standbyPositionBetweenNortheastAndSouth;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.DestoryAt=7300;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);

                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_2_For_Melees_During_Wings_Dark_And_Light_光与暗之翼近战指路2";
                        currentProperty.Scale=new(2);
                        currentProperty.Owner=accessory.Data.Me;
                        currentProperty.TargetPosition=standbyPositionBetweenNortheastAndSouth;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.Delay=7300;
                        currentProperty.DestoryAt=7100;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);

                    }
                    
                    if(accessory.Data.PartyList.IndexOf(accessory.Data.Me)==6
                       ||
                       accessory.Data.PartyList.IndexOf(accessory.Data.Me)==7) {
                        // The range group, that is R1 & R2 or D3 & D4.
                        
                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_1_For_Ranges_During_Wings_Dark_And_Light_光与暗之翼远程指路1";
                        currentProperty.Scale=new(2);
                        currentProperty.Owner=accessory.Data.Me;
                        currentProperty.TargetPosition=isLeftFirstAndFarFirst?standbyPositionBetweenNorthwestAndSouth:standbyPositionBetweenNortheastAndNorthwest;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.DestoryAt=6900;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);
                        
                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_2_Preview_For_Ranges_During_Wings_Dark_And_Light_光与暗之翼远程指路2预览";
                        currentProperty.Scale=new(2);
                        currentProperty.Position=isLeftFirstAndFarFirst?standbyPositionBetweenNorthwestAndSouth:standbyPositionBetweenNortheastAndNorthwest;
                        currentProperty.TargetPosition=rightSideOfSouth;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.DestoryAt=6900;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);

                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_2_For_Ranges_During_Wings_Dark_And_Light_光与暗之翼远程指路2";
                        currentProperty.Scale=new(2);
                        currentProperty.Owner=accessory.Data.Me;
                        currentProperty.TargetPosition=rightSideOfSouth;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.Delay=6900;
                        currentProperty.DestoryAt=7500;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);

                    }
                    
                    if(accessory.Data.PartyList.IndexOf(accessory.Data.Me)==2
                       ||
                       accessory.Data.PartyList.IndexOf(accessory.Data.Me)==3) {
                        // The healer group, that is H1 & H2.
                        
                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_1_For_Healers_During_Wings_Dark_And_Light_光与暗之翼奶妈指路1";
                        currentProperty.Scale=new(2);
                        currentProperty.Owner=accessory.Data.Me;
                        currentProperty.TargetPosition=isLeftFirstAndFarFirst?standbyPositionBetweenNorthwestAndSouth:standbyPositionBetweenNortheastAndNorthwest;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.DestoryAt=6900;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);
                        
                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_2_Preview_For_Healers_During_Wings_Dark_And_Light_光与暗之翼奶妈指路2预览";
                        currentProperty.Scale=new(2);
                        currentProperty.Position=isLeftFirstAndFarFirst?standbyPositionBetweenNorthwestAndSouth:standbyPositionBetweenNortheastAndNorthwest;
                        currentProperty.TargetPosition=leftSideOfNortheast;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.DestoryAt=6900;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);

                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_2_For_Healers_During_Wings_Dark_And_Light_光与暗之翼奶妈指路2";
                        currentProperty.Scale=new(2);
                        currentProperty.Owner=accessory.Data.Me;
                        currentProperty.TargetPosition=leftSideOfNortheast;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.Delay=6900;
                        currentProperty.DestoryAt=7500;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);

                    }

                }
            
                if(indexOfTheFirstTower.Equals("00000031")) {
                    // The first tower is in the northeast.
                    
                    if(accessory.Data.PartyList.IndexOf(accessory.Data.Me)==4
                       ||
                       accessory.Data.PartyList.IndexOf(accessory.Data.Me)==5) {
                        
                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_1_For_Melees_During_Wings_Dark_And_Light_光与暗之翼近战指路1";
                        currentProperty.Scale=new(2);
                        currentProperty.Owner=accessory.Data.Me;
                        currentProperty.TargetPosition=isLeftFirstAndFarFirst?rightSideOfNortheast:leftSideOfNortheast;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.DestoryAt=7300;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);
                        
                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_2_Preview_For_Melees_During_Wings_Dark_And_Light_光与暗之翼近战指路2预览";
                        currentProperty.Scale=new(2);
                        currentProperty.Position=isLeftFirstAndFarFirst?rightSideOfNortheast:leftSideOfNortheast;
                        currentProperty.TargetPosition=standbyPositionBetweenNorthwestAndSouth;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.DestoryAt=7300;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);

                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_2_For_Melees_During_Wings_Dark_And_Light_光与暗之翼近战指路2";
                        currentProperty.Scale=new(2);
                        currentProperty.Owner=accessory.Data.Me;
                        currentProperty.TargetPosition=standbyPositionBetweenNorthwestAndSouth;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.Delay=7300;
                        currentProperty.DestoryAt=7100;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);

                    }
                    
                    if(accessory.Data.PartyList.IndexOf(accessory.Data.Me)==6
                       ||
                       accessory.Data.PartyList.IndexOf(accessory.Data.Me)==7) {
                        
                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_1_For_Ranges_During_Wings_Dark_And_Light_光与暗之翼远程指路1";
                        currentProperty.Scale=new(2);
                        currentProperty.Owner=accessory.Data.Me;
                        currentProperty.TargetPosition=isLeftFirstAndFarFirst?standbyPositionBetweenNortheastAndNorthwest:standbyPositionBetweenNortheastAndSouth;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.DestoryAt=6900;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);
                        
                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_2_Preview_For_Ranges_During_Wings_Dark_And_Light_光与暗之翼远程指路2预览";
                        currentProperty.Scale=new(2);
                        currentProperty.Position=isLeftFirstAndFarFirst?standbyPositionBetweenNortheastAndNorthwest:standbyPositionBetweenNortheastAndSouth;
                        currentProperty.TargetPosition=rightSideOfNorthwest;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.DestoryAt=6900;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);

                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_2_For_Ranges_During_Wings_Dark_And_Light_光与暗之翼远程指路2";
                        currentProperty.Scale=new(2);
                        currentProperty.Owner=accessory.Data.Me;
                        currentProperty.TargetPosition=rightSideOfNorthwest;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.Delay=6900;
                        currentProperty.DestoryAt=7500;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);

                    }
                    
                    if(accessory.Data.PartyList.IndexOf(accessory.Data.Me)==2
                       ||
                       accessory.Data.PartyList.IndexOf(accessory.Data.Me)==3) {
                        
                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_1_For_Healers_During_Wings_Dark_And_Light_光与暗之翼奶妈指路1";
                        currentProperty.Scale=new(2);
                        currentProperty.Owner=accessory.Data.Me;
                        currentProperty.TargetPosition=isLeftFirstAndFarFirst?standbyPositionBetweenNortheastAndNorthwest:standbyPositionBetweenNortheastAndSouth;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.DestoryAt=6900;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);
                        
                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_2_Preview_For_Healers_During_Wings_Dark_And_Light_光与暗之翼奶妈指路2预览";
                        currentProperty.Scale=new(2);
                        currentProperty.Position=isLeftFirstAndFarFirst?standbyPositionBetweenNortheastAndNorthwest:standbyPositionBetweenNortheastAndSouth;
                        currentProperty.TargetPosition=leftSideOfSouth;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.DestoryAt=6900;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);

                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_2_For_Healers_During_Wings_Dark_And_Light_光与暗之翼奶妈指路2";
                        currentProperty.Scale=new(2);
                        currentProperty.Owner=accessory.Data.Me;
                        currentProperty.TargetPosition=leftSideOfSouth;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.Delay=6900;
                        currentProperty.DestoryAt=7500;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);

                    }
                
                }
            
                if(indexOfTheFirstTower.Equals("00000032")) {
                    // The first tower is in the south.
                    
                    if(accessory.Data.PartyList.IndexOf(accessory.Data.Me)==4
                       ||
                       accessory.Data.PartyList.IndexOf(accessory.Data.Me)==5) {
                        
                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_1_For_Melees_During_Wings_Dark_And_Light_光与暗之翼近战指路1";
                        currentProperty.Scale=new(2);
                        currentProperty.Owner=accessory.Data.Me;
                        currentProperty.TargetPosition=isLeftFirstAndFarFirst?rightSideOfSouth:leftSideOfSouth;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.DestoryAt=7300;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);
                        
                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_2_Preview_For_Melees_During_Wings_Dark_And_Light_光与暗之翼近战指路2预览";
                        currentProperty.Scale=new(2);
                        currentProperty.Position=isLeftFirstAndFarFirst?rightSideOfSouth:leftSideOfSouth;
                        currentProperty.TargetPosition=standbyPositionBetweenNortheastAndNorthwest;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.DestoryAt=7300;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);

                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_2_For_Melees_During_Wings_Dark_And_Light_光与暗之翼近战指路2";
                        currentProperty.Scale=new(2);
                        currentProperty.Owner=accessory.Data.Me;
                        currentProperty.TargetPosition=standbyPositionBetweenNortheastAndNorthwest;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.Delay=7300;
                        currentProperty.DestoryAt=7100;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);

                    }
                    
                    if(accessory.Data.PartyList.IndexOf(accessory.Data.Me)==6
                       ||
                       accessory.Data.PartyList.IndexOf(accessory.Data.Me)==7) {
                        
                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_1_For_Ranges_During_Wings_Dark_And_Light_光与暗之翼远程指路1";
                        currentProperty.Scale=new(2);
                        currentProperty.Owner=accessory.Data.Me;
                        currentProperty.TargetPosition=isLeftFirstAndFarFirst?standbyPositionBetweenNortheastAndSouth:standbyPositionBetweenNorthwestAndSouth;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.DestoryAt=6900;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);
                        
                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_2_Preview_For_Ranges_During_Wings_Dark_And_Light_光与暗之翼远程指路2预览";
                        currentProperty.Scale=new(2);
                        currentProperty.Position=isLeftFirstAndFarFirst?standbyPositionBetweenNortheastAndSouth:standbyPositionBetweenNorthwestAndSouth;
                        currentProperty.TargetPosition=rightSideOfNortheast;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.DestoryAt=6900;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);

                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_2_For_Ranges_During_Wings_Dark_And_Light_光与暗之翼远程指路2";
                        currentProperty.Scale=new(2);
                        currentProperty.Owner=accessory.Data.Me;
                        currentProperty.TargetPosition=rightSideOfNortheast;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.Delay=6900;
                        currentProperty.DestoryAt=7500;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);

                    }
                    
                    if(accessory.Data.PartyList.IndexOf(accessory.Data.Me)==2
                       ||
                       accessory.Data.PartyList.IndexOf(accessory.Data.Me)==3) {
                        
                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_1_For_Healers_During_Wings_Dark_And_Light_光与暗之翼奶妈指路1";
                        currentProperty.Scale=new(2);
                        currentProperty.Owner=accessory.Data.Me;
                        currentProperty.TargetPosition=isLeftFirstAndFarFirst?standbyPositionBetweenNortheastAndSouth:standbyPositionBetweenNorthwestAndSouth;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.DestoryAt=6900;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);
                        
                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_2_Preview_For_Healers_During_Wings_Dark_And_Light_光与暗之翼奶妈指路2预览";
                        currentProperty.Scale=new(2);
                        currentProperty.Position=isLeftFirstAndFarFirst?standbyPositionBetweenNortheastAndSouth:standbyPositionBetweenNorthwestAndSouth;
                        currentProperty.TargetPosition=leftSideOfNorthwest;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.DestoryAt=6900;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);

                        currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                        currentProperty.Name="Phase5_Guidance_2_For_Healers_During_Wings_Dark_And_Light_光与暗之翼奶妈指路2";
                        currentProperty.Scale=new(2);
                        currentProperty.Owner=accessory.Data.Me;
                        currentProperty.TargetPosition=leftSideOfNorthwest;
                        currentProperty.ScaleMode|=ScaleMode.YByDistance;
                        currentProperty.Color=accessory.Data.DefaultSafeColor;
                        currentProperty.Delay=6900;
                        currentProperty.DestoryAt=7500;
                        
                        accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);

                    }
                
                }

                
            }

        }
        
        [ScriptMethod(name:"Phase5_Guidance_Of_Polarizing_Strikes_失调打击指路",
            eventType:EventTypeEnum.StartCasting,
            eventCondition:["ActionId:40316"])]
        
        public void Phase5_Guidance_Of_Polarizing_Strikes_失调打击指路(Event @event, ScriptAccessory accessory) {

            int myIndex=accessory.Data.PartyList.IndexOf(accessory.Data.Me);
            bool inTheLeftGroup=true;
            int myRoundToTakeHits=getRoundByIndex(myIndex);
            var currentProperty=accessory.Data.GetDefaultDrawProperties();
            int timeline=0;

            if(myRoundToTakeHits<1||myRoundToTakeHits>4) {

                return;

            } 

            if(myIndex==0
               ||
               myIndex==2
               ||
               myIndex==4
               ||
               myIndex==6) {

                inTheLeftGroup=true;

            }
            
            if(myIndex==1
               ||
               myIndex==3
               ||
               myIndex==5
               ||
               myIndex==7) {

                inTheLeftGroup=false;

            }
            
            // ----- Initial guidance -----

            if(myRoundToTakeHits==1) {
                
                currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                currentProperty.Name="Phase5_Initial_Guidance_Of_Polarizing_Strikes_失调打击初始指路";
                currentProperty.Scale=new(2);
                currentProperty.Owner=accessory.Data.Me;
                currentProperty.TargetPosition=inTheLeftGroup?positionToTakeHitsOnTheLeft:positionToTakeHitsOnTheRight;
                currentProperty.ScaleMode|=ScaleMode.YByDistance;
                currentProperty.Color=accessory.Data.DefaultSafeColor;
                currentProperty.DestoryAt=4550;
                timeline+=4550;
                        
                accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);
                
            }

            else {
                
                currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                currentProperty.Name="Phase5_Initial_Guidance_Of_Polarizing_Strikes_失调打击初始指路";
                currentProperty.Scale=new(2);
                currentProperty.Owner=accessory.Data.Me;
                currentProperty.TargetPosition=inTheLeftGroup?positionToBeCoveredOnTheLeft:positionToBeCoveredOnTheRight;
                currentProperty.ScaleMode|=ScaleMode.YByDistance;
                currentProperty.Color=accessory.Data.DefaultSafeColor;
                currentProperty.DestoryAt=4550;
                timeline+=4550;
                        
                accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);
                
            }
            
            // ----- Be covered in the current group -----

            for(int i=1;i<myRoundToTakeHits;++i) {
                
                currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                currentProperty.Name="Phase5_Guidance_Of_Polarizing_Strikes_In_The_Current_Group_失调打击当前分组指路";
                currentProperty.Scale=new(2);
                currentProperty.Owner=accessory.Data.Me;
                currentProperty.TargetPosition=inTheLeftGroup?positionToBeCoveredOnTheLeft:positionToBeCoveredOnTheRight;
                currentProperty.ScaleMode|=ScaleMode.YByDistance;
                currentProperty.Color=accessory.Data.DefaultSafeColor;
                currentProperty.Delay=timeline;
                currentProperty.DestoryAt=2450;
                timeline+=2450;
                
                accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);
                
                currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                currentProperty.Name="Phase5_Guidance_Of_Polarizing_Strikes_In_The_Current_Group_失调打击当前分组指路";
                currentProperty.Scale=new(2);
                currentProperty.Owner=accessory.Data.Me;
                currentProperty.TargetPosition=inTheLeftGroup?positionToStandbyOnTheLeft:positionToStandbyOnTheRight;
                currentProperty.ScaleMode|=ScaleMode.YByDistance;
                currentProperty.Color=accessory.Data.DefaultSafeColor;
                currentProperty.Delay=timeline;
                currentProperty.DestoryAt=2250;
                timeline+=2250;
                
                accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);
                
            }
            
            // ----- -----
            
            // ----- Take hits and swap the group -----
            
            currentProperty=accessory.Data.GetDefaultDrawProperties();
                
            currentProperty.Name="Phase5_Guidance_Of_Polarizing_Strikes_While_Taking_Hits_失调打击挡枪指路";
            currentProperty.Scale=new(2);
            currentProperty.Owner=accessory.Data.Me;
            currentProperty.TargetPosition=inTheLeftGroup?positionToTakeHitsOnTheLeft:positionToTakeHitsOnTheRight;
            currentProperty.ScaleMode|=ScaleMode.YByDistance;
            currentProperty.Color=accessory.Data.DefaultSafeColor;
            currentProperty.Delay=timeline;
            currentProperty.DestoryAt=2450;
            timeline+=2450;
                
            accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);
                
            currentProperty=accessory.Data.GetDefaultDrawProperties();
                
            currentProperty.Name="Phase5_Guidance_Of_Polarizing_Strikes_While_Taking_Hits_失调打击挡枪指路";
            currentProperty.Scale=new(2);
            currentProperty.Owner=accessory.Data.Me;
            currentProperty.TargetPosition=inTheLeftGroup?positionToStandbyOnTheRight:positionToStandbyOnTheLeft;
            currentProperty.ScaleMode|=ScaleMode.YByDistance;
            currentProperty.Color=accessory.Data.DefaultSafeColor;
            currentProperty.Delay=timeline;
            currentProperty.DestoryAt=2250;
            timeline+=2250;
                
            accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);
            
            // ----- -----
            
            // ----- Be covered in the opposite group -----
            
            for(int i=myRoundToTakeHits+1;i<=4;++i) {
                
                currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                currentProperty.Name="Phase5_Guidance_Of_Polarizing_Strikes_In_The_Current_Group_失调打击当前分组指路";
                currentProperty.Scale=new(2);
                currentProperty.Owner=accessory.Data.Me;
                currentProperty.TargetPosition=inTheLeftGroup?positionToBeCoveredOnTheRight:positionToBeCoveredOnTheLeft;
                currentProperty.ScaleMode|=ScaleMode.YByDistance;
                currentProperty.Color=accessory.Data.DefaultSafeColor;
                currentProperty.Delay=timeline;
                currentProperty.DestoryAt=2450;
                timeline+=2450;
                
                accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);
                
                currentProperty=accessory.Data.GetDefaultDrawProperties();
                
                currentProperty.Name="Phase5_Guidance_Of_Polarizing_Strikes_In_The_Current_Group_失调打击当前分组指路";
                currentProperty.Scale=new(2);
                currentProperty.Owner=accessory.Data.Me;
                currentProperty.TargetPosition=inTheLeftGroup?positionToStandbyOnTheRight:positionToStandbyOnTheLeft;
                currentProperty.ScaleMode|=ScaleMode.YByDistance;
                currentProperty.Color=accessory.Data.DefaultSafeColor;
                currentProperty.Delay=timeline;
                currentProperty.DestoryAt=2250;
                timeline+=2250;
                
                accessory.Method.SendDraw(DrawModeEnum.Imgui,DrawTypeEnum.Displacement,currentProperty);
                
            }
            
            // ----- -----

            if(Phase5_Hints_Of_Provoking_挑衅提示) {

                if(myIndex==0) {
                    
                    System.Threading.Thread.Sleep(timeline);
                        
                    accessory.Method.TextInfo("Provoke! 挑衅！",2500);
                    accessory.Method.TTS("Provoke! 挑衅！");
                    
                }

                if(myIndex==1) {
                    
                    System.Threading.Thread.Sleep(timeline);
                        
                    accessory.Method.TextInfo("MT provoke not you! MT挑衅不是你！",2500);
                    accessory.Method.TTS("MT provoke not you! MT挑衅不是你！");
                    
                }
                
            }

        }

        private int getRoundByIndex(int currentIndex) {

            if(Phase5_Orders_During_Polarizing_Strikes_挡枪顺序==Phase5_Orders_During_Polarizing_Strikes.TANKS_MELEES_RANGES_HEALERS_坦克近战远程奶妈) {

                if(currentIndex==0||currentIndex==1) {
                    // Tanks.

                    return 1;

                }
                
                if(currentIndex==4||currentIndex==5) {
                    // Melees.

                    return 2;

                }
                
                if(currentIndex==6||currentIndex==7) {
                    // Ranges.

                    return 3;

                }
                
                if(currentIndex==2||currentIndex==3) {
                    // Healers.

                    return 4;

                }
                
            }
            
            if(Phase5_Orders_During_Polarizing_Strikes_挡枪顺序==Phase5_Orders_During_Polarizing_Strikes.TANKS_HEALERS_MELEES_RANGES_坦克奶妈近战远程) {
                
                if(currentIndex==0||currentIndex==1) {
                    // Tanks.

                    return 1;

                }
                
                if(currentIndex==2||currentIndex==3) {
                    // Healers.

                    return 2;

                }
                
                if(currentIndex==4||currentIndex==5) {
                    // Melees.

                    return 3;

                }
                
                if(currentIndex==6||currentIndex==7) {
                    // Ranges.

                    return 4;

                }
                
            }

            return -1;
            // Just a placeholder and should never be reached.

        }

        #endregion

        private int ParsTargetIcon(string id)
        {
            firstTargetIcon ??= int.Parse(id, System.Globalization.NumberStyles.HexNumber);
            return int.Parse(id, System.Globalization.NumberStyles.HexNumber) - (int)firstTargetIcon;
        }
        private static bool ParseObjectId(string? idStr, out uint id)
        {
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
        /// <summary>
        /// 向近的取
        /// </summary>
        /// <param name="point"></param>
        /// <param name="centre"></param>
        /// <returns></returns>
        private int PositionTo8Dir(Vector3 point, Vector3 centre)
        {
            // Dirs: N = 0, NE = 1, ..., NW = 7
            var r = Math.Round(4 - 4 * Math.Atan2(point.X - centre.X, point.Z - centre.Z) / Math.PI) % 8;
            return (int)r;

        }
        private int PositionTo6Dir(Vector3 point, Vector3 centre)
        {
            var r = Math.Round(3 - 3 * Math.Atan2(point.X - centre.X, point.Z - centre.Z) / Math.PI) % 6;
            return (int)r;

        }
        private Vector3 RotatePoint(Vector3 point, Vector3 centre, float radian)
        {

            Vector2 v2 = new(point.X - centre.X, point.Z - centre.Z);

            var rot = (MathF.PI - MathF.Atan2(v2.X, v2.Y) + radian);
            var lenth = v2.Length();
            return new(centre.X + MathF.Sin(rot) * lenth, centre.Y, centre.Z - MathF.Cos(rot) * lenth);
        }
    }
}

