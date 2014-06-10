﻿using Card;
using Card.Effect;
using Microsoft.VisualBasic;
//using MongoDB.Driver;
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace 炉边传说
{
    public partial class frmExport : Form
    {
        public frmExport()
        {
            InitializeComponent();
        }
        //private static MongoServer innerServer;
        //private static MongoDatabase innerDatabase;
        //private static MongoCollection innerCollection;
        /// <summary>
        /// 导出到MongoDB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportMongoDB_Click(object sender, EventArgs e)
        {
            //innerServer = MongoServer.Create(@"mongodb://localhost:28030");
            //innerServer.Connect();
            //innerDatabase = innerServer.GetDatabase("HearthStone");
            //innerCollection = innerDatabase.GetCollection("Card");
            if (String.IsNullOrEmpty(ExcelPicker.SelectedPathOrFileName)) return;
            Export(TargetType.MongoDB);
            GC.Collect();
            //innerServer.Disconnect();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportXml_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(ExcelPicker.SelectedPathOrFileName)) return;
            if (String.IsNullOrEmpty(XmlFolderPicker.SelectedPathOrFileName)) return;
            Export(TargetType.Xml);
        }
        /// <summary>
        /// 导出类型
        /// </summary>
        private enum TargetType
        {
            /// <summary>
            /// MongoDataBase
            /// </summary>
            MongoDB,
            /// <summary>
            /// XmlFile
            /// </summary>
            Xml
        }
        /// <summary>
        /// 导入
        /// </summary>
        private void Export(TargetType target)
        {
            dynamic excelObj = Interaction.CreateObject("Excel.Application");
            excelObj.Visible = true;
            dynamic workbook;
            workbook = excelObj.Workbooks.Open(ExcelPicker.SelectedPathOrFileName);
            //Minion(target, workbook);
            //Ability(target, workbook);
            AbilityNewFormat(target, workbook);
            //Weapon(target, workbook);
            //Secret(target, workbook);
            workbook.Close();
            excelObj.Quit();
            excelObj = null;
            MessageBox.Show("导出结束");
        }
        /// <summary>
        /// 奥秘
        /// </summary>
        /// <param name="target"></param>
        /// <param name="workbook"></param>
        private void Secret(TargetType target, dynamic workbook)
        {
            if (Directory.Exists(XmlFolderPicker.SelectedPathOrFileName + "\\Secret\\"))
            {
                Directory.Delete(XmlFolderPicker.SelectedPathOrFileName + "\\Secret\\", true);
            }
            Directory.CreateDirectory(XmlFolderPicker.SelectedPathOrFileName + "\\Secret\\");
            //奥秘的导入
            dynamic worksheet = workbook.Sheets(4);
            int rowCount = 4;
            while (!String.IsNullOrEmpty(worksheet.Cells(rowCount, 2).Text))
            {
                Card.SecretCard Secret = new Card.SecretCard();
                Secret.SN = worksheet.Cells(rowCount, 2).Text;
                Secret.Name = worksheet.Cells(rowCount, 3).Text;
                Secret.Description = worksheet.Cells(rowCount, 4).Text;
                Secret.Class = CardUtility.GetEnum<Card.CardUtility.ClassEnum>(worksheet.Cells(rowCount, 5).Text, Card.CardUtility.ClassEnum.中立);
                Secret.StandardCostPoint = CardUtility.GetInt(worksheet.Cells(rowCount, 7).Text);
                Secret.ActualCostPoint = Secret.StandardCostPoint;
                Secret.Rare = CardUtility.GetEnum<Card.CardBasicInfo.稀有程度>(worksheet.Cells(rowCount, 12).Text, CardBasicInfo.稀有程度.白色);
                Secret.IsCardReady = !String.IsNullOrEmpty(worksheet.Cells(rowCount, 13).Text);
                Secret.Condition = CardUtility.GetEnum<Card.SecretCard.SecretCondition>(worksheet.Cells(rowCount, 14).Text, SecretCard.SecretCondition.对方召唤随从);
                Secret.AdditionInfo = worksheet.Cells(rowCount, 15).Text;
                switch (target)
                {
                    case TargetType.MongoDB:
                        //innerCollection.Insert<Card.SecretCard>(Secret);
                        break;
                    case TargetType.Xml:
                        XmlSerializer xml = new XmlSerializer(typeof(Card.SecretCard));
                        String XmlFilename = XmlFolderPicker.SelectedPathOrFileName + "\\Secret\\" + Secret.SN + ".xml";
                        xml.Serialize(new StreamWriter(XmlFilename), Secret);
                        break;
                    default:
                        break;
                }
                rowCount++;
            }
        }
        /// <summary>
        /// 随从的导入
        /// </summary>
        /// <param name="target"></param>
        /// <param name="workbook"></param>
        private void Minion(TargetType target, dynamic workbook)
        {
            if (Directory.Exists(XmlFolderPicker.SelectedPathOrFileName + "\\Minion\\"))
            {
                Directory.Delete(XmlFolderPicker.SelectedPathOrFileName + "\\Minion\\", true);
            }
            Directory.CreateDirectory(XmlFolderPicker.SelectedPathOrFileName + "\\Minion\\");
            //随从的导入
            dynamic worksheet = workbook.Sheets(1);
            int rowCount = 4;
            while (!String.IsNullOrEmpty(worksheet.Cells(rowCount, 2).Text))
            {
                Card.MinionCard Minion = new Card.MinionCard();
                Minion.SN = worksheet.Cells(rowCount, 2).Text;
                Minion.Name = worksheet.Cells(rowCount, 3).Text;
                Minion.Description = worksheet.Cells(rowCount, 4).Text;
                Minion.Class = CardUtility.GetEnum<Card.CardUtility.ClassEnum>(worksheet.Cells(rowCount, 5).Text, Card.CardUtility.ClassEnum.中立);
                Minion.种族 = CardUtility.GetEnum<Card.CardUtility.种族Enum>(worksheet.Cells(rowCount, 6).Text, Card.CardUtility.种族Enum.无);
                Minion.StandardCostPoint = CardUtility.GetInt(worksheet.Cells(rowCount, 7).Text);

                Minion.标准攻击力 = CardUtility.GetInt(worksheet.Cells(rowCount, 8).Text);
                Minion.标准生命值上限 = CardUtility.GetInt(worksheet.Cells(rowCount, 9).Text);
                Minion.Rare = CardUtility.GetEnum<Card.CardBasicInfo.稀有程度>(worksheet.Cells(rowCount, 12).Text, CardBasicInfo.稀有程度.白色);
                Minion.IsCardReady = !String.IsNullOrEmpty(worksheet.Cells(rowCount, 13).Text);

                Minion.Standard嘲讽 = !String.IsNullOrEmpty(worksheet.Cells(rowCount, 14).Text);
                Minion.Standard冲锋 = !String.IsNullOrEmpty(worksheet.Cells(rowCount, 15).Text);
                Minion.Standard不能攻击 = !String.IsNullOrEmpty(worksheet.Cells(rowCount, 16).Text);
                Minion.Standard风怒 = !String.IsNullOrEmpty(worksheet.Cells(rowCount, 17).Text);
                Minion.潜行特性 = !String.IsNullOrEmpty(worksheet.Cells(rowCount, 18).Text);
                Minion.圣盾特性 = !String.IsNullOrEmpty(worksheet.Cells(rowCount, 19).Text);
                Minion.法术免疫特性 = !String.IsNullOrEmpty(worksheet.Cells(rowCount, 20).Text);
                Minion.英雄技能免疫特性 = !String.IsNullOrEmpty(worksheet.Cells(rowCount, 21).Text);

                Boolean HasBuff = false;
                for (int i = 22; i < 25; i++)
                {
                    if (!String.IsNullOrEmpty(worksheet.Cells(rowCount, i).Text))
                    {
                        HasBuff = true;
                        break;
                    }
                }
                if (HasBuff)
                {
                    Minion.光环效果.Name = Minion.Name;
                    Minion.光环效果.Scope = CardUtility.GetEnum<Card.MinionCard.光环范围>(worksheet.Cells(rowCount, 22).Text, Card.MinionCard.光环范围.随从全体);
                    Minion.光环效果.EffectType = CardUtility.GetEnum<Card.MinionCard.光环类型>(worksheet.Cells(rowCount, 23).Text, Card.MinionCard.光环类型.增加攻防);
                    Minion.光环效果.BuffInfo = worksheet.Cells(rowCount, 24).Text;
                }
                Minion.战吼效果 = worksheet.Cells(rowCount, 25).Text;
                Minion.战吼类型 = CardUtility.GetEnum<Card.MinionCard.战吼类型列表>(worksheet.Cells(rowCount, 26).Text, Card.MinionCard.战吼类型列表.默认);

                Minion.亡语效果 = worksheet.Cells(rowCount, 27).Text;
                Minion.激怒效果 = worksheet.Cells(rowCount, 28).Text;
                Minion.连击效果 = worksheet.Cells(rowCount, 29).Text;
                Minion.回合开始效果 = worksheet.Cells(rowCount, 30).Text;
                Minion.回合结束效果 = worksheet.Cells(rowCount, 31).Text;
                Minion.Overload = CardUtility.GetInt(worksheet.Cells(rowCount, 32).Text);
                Minion.自身事件.事件类型 = CardUtility.GetEnum<Card.CardUtility.事件类型列表>(worksheet.Cells(rowCount, 33).Text, Card.CardUtility.事件类型列表.无);
                Minion.自身事件.事件效果 = worksheet.Cells(rowCount, 34).Text;
                Minion.自身事件.触发方向 = CardUtility.GetEnum<Card.CardUtility.TargetSelectDirectEnum>(worksheet.Cells(rowCount, 35).Text, Card.CardUtility.TargetSelectDirectEnum.本方);
                Minion.自身事件.附加信息 = worksheet.Cells(rowCount, 36).Text;
                Minion.特殊效果 = CardUtility.GetEnum<Card.MinionCard.特殊效果列表>(worksheet.Cells(rowCount, 37).Text, Card.MinionCard.特殊效果列表.无效果);

                switch (target)
                {
                    case TargetType.MongoDB:
                        //innerCollection.Insert<Card.MinionCard>(Minion);
                        break;
                    case TargetType.Xml:
                        XmlSerializer xml = new XmlSerializer(typeof(Card.MinionCard));
                        String XmlFilename = XmlFolderPicker.SelectedPathOrFileName + "\\Minion\\" + Minion.SN + ".xml";
                        xml.Serialize(new StreamWriter(XmlFilename), Minion);
                        break;
                    default:
                        break;
                }
                rowCount++;
            }
        }
        /// <summary>
        /// 法术的导入
        /// </summary>
        /// <param name="target"></param>
        /// <param name="workbook"></param>
        private void Ability(TargetType target, dynamic workbook)
        {
            if (Directory.Exists(XmlFolderPicker.SelectedPathOrFileName + "\\Ability\\"))
            {
                Directory.Delete(XmlFolderPicker.SelectedPathOrFileName + "\\Ability\\", true);
            }
            Directory.CreateDirectory(XmlFolderPicker.SelectedPathOrFileName + "\\Ability\\");
            //法术的导入
            dynamic worksheet = workbook.Sheets(2);
            int rowCount = 4;
            while (!String.IsNullOrEmpty(worksheet.Cells(rowCount, 2).Text))
            {
                Card.AbilityCard Ability = new Card.AbilityCard();
                Ability.SN = worksheet.Cells(rowCount, 2).Text;
                Ability.Name = worksheet.Cells(rowCount, 3).Text;
                Ability.Description = worksheet.Cells(rowCount, 4).Text;
                Ability.Class = CardUtility.GetEnum<Card.CardUtility.ClassEnum>(worksheet.Cells(rowCount, 5).Text, Card.CardUtility.ClassEnum.中立);
                Ability.StandardCostPoint = CardUtility.GetInt(worksheet.Cells(rowCount, 7).Text);

                Ability.Rare = CardUtility.GetEnum<Card.CardBasicInfo.稀有程度>(worksheet.Cells(rowCount, 12).Text, CardBasicInfo.稀有程度.白色);
                Ability.IsCardReady = !String.IsNullOrEmpty(worksheet.Cells(rowCount, 13).Text);

                Card.Effect.AtomicEffectDefine effect = new Card.Effect.AtomicEffectDefine();
                effect.Description = String.IsNullOrEmpty(worksheet.Cells(rowCount, 14).Text) ? String.Empty : worksheet.Cells(rowCount, 14).Text;
                effect.AbilityEffectType = CardUtility.GetEnum<Card.Effect.CardEffect.AbilityEffectEnum>(worksheet.Cells(rowCount, 15).Text, Card.Effect.CardEffect.AbilityEffectEnum.未定义);
                effect.SelectOpt.EffictTargetSelectMode = CardUtility.GetEnum<Card.CardUtility.TargetSelectModeEnum>(worksheet.Cells(rowCount, 16).Text, CardUtility.TargetSelectModeEnum.不用选择);
                effect.SelectOpt.EffectTargetSelectDirect = CardUtility.GetEnum<Card.CardUtility.TargetSelectDirectEnum>(worksheet.Cells(rowCount, 17).Text, CardUtility.TargetSelectDirectEnum.双方);
                effect.SelectOpt.EffectTargetSelectRole = CardUtility.GetEnum<Card.CardUtility.TargetSelectRoleEnum>(worksheet.Cells(rowCount, 18).Text, CardUtility.TargetSelectRoleEnum.随从);
                effect.SelectOpt.EffectTargetSelectCondition = String.IsNullOrEmpty(worksheet.Cells(rowCount, 19).Text) ? String.Empty : worksheet.Cells(rowCount, 19).Text;
                effect.StandardEffectPoint = worksheet.Cells(rowCount, 20).Text;
                effect.StandardEffectCount = CardUtility.GetInt(worksheet.Cells(rowCount, 21).Text);
                if (effect.StandardEffectCount == 0) effect.StandardEffectCount = 1;
                effect.AdditionInfo = worksheet.Cells(rowCount, 22).Text;
                Ability.CardAbility.FirstAbilityDefine.MainAbilityDefine = effect;
                Ability.CardAbility.效果选择类型 = CardUtility.GetEnum<Card.Effect.Ability.效果选择类型枚举>(worksheet.Cells(rowCount, 23).Text, Card.Effect.Ability.效果选择类型枚举.无需选择);
                Boolean HasSecond = false;
                for (int i = 24; i < 32; i++)
                {
                    if (!String.IsNullOrEmpty(worksheet.Cells(rowCount, i).Text))
                    {
                        HasSecond = true;
                        break;
                    }
                }
                if (HasSecond)
                {
                    Card.Effect.AtomicEffectDefine effect2 = new Card.Effect.AtomicEffectDefine();
                    effect2.Description = String.IsNullOrEmpty(worksheet.Cells(rowCount, 24).Text) ? String.Empty : worksheet.Cells(rowCount, 24).Text;
                    effect2.AbilityEffectType = CardUtility.GetEnum<Card.Effect.CardEffect.AbilityEffectEnum>(worksheet.Cells(rowCount, 25).Text, Card.Effect.CardEffect.AbilityEffectEnum.未定义);
                    effect2.SelectOpt.EffictTargetSelectMode = CardUtility.GetEnum<Card.CardUtility.TargetSelectModeEnum>(worksheet.Cells(rowCount, 26).Text, CardUtility.TargetSelectModeEnum.不用选择);
                    effect2.SelectOpt.EffectTargetSelectDirect = CardUtility.GetEnum<Card.CardUtility.TargetSelectDirectEnum>(worksheet.Cells(rowCount, 27).Text, CardUtility.TargetSelectDirectEnum.双方);
                    effect2.SelectOpt.EffectTargetSelectRole = CardUtility.GetEnum<Card.CardUtility.TargetSelectRoleEnum>(worksheet.Cells(rowCount, 28).Text, CardUtility.TargetSelectRoleEnum.随从);
                    effect2.SelectOpt.EffectTargetSelectCondition = String.IsNullOrEmpty(worksheet.Cells(rowCount, 29).Text) ? String.Empty : worksheet.Cells(rowCount, 29).Text;
                    effect2.StandardEffectPoint = worksheet.Cells(rowCount, 30).Text;
                    effect2.StandardEffectCount = CardUtility.GetInt(worksheet.Cells(rowCount, 31).Text);
                    if (effect2.StandardEffectCount == 0) effect2.StandardEffectCount = 1;
                    effect2.AdditionInfo = worksheet.Cells(rowCount, 32).Text;
                    Ability.CardAbility.SecondAbilityDefine.MainAbilityDefine = effect2;
                }
                Ability.Overload = CardUtility.GetInt(worksheet.Cells(rowCount, 33).Text);
                Ability.连击效果 = worksheet.Cells(rowCount, 34).Text;
                switch (target)
                {
                    case TargetType.MongoDB:
                        //innerCollection.Insert<Card.AbilityCard>(Ability);
                        break;
                    case TargetType.Xml:
                        XmlSerializer xml = new XmlSerializer(typeof(Card.AbilityCard));
                        String XmlFilename = XmlFolderPicker.SelectedPathOrFileName + "\\Ability\\" + Ability.SN + ".xml";
                        xml.Serialize(new StreamWriter(XmlFilename), Ability);
                        break;
                    default:
                        break;
                }
                rowCount++;
            }
        }


        /// <summary>
        /// 法术的导入
        /// </summary>
        /// <param name="target"></param>
        /// <param name="workbook"></param>
        private void AbilityNewFormat(TargetType target, dynamic workbook)
        {
            if (Directory.Exists(XmlFolderPicker.SelectedPathOrFileName + "\\Ability\\"))
            {
                Directory.Delete(XmlFolderPicker.SelectedPathOrFileName + "\\Ability\\", true);
            }
            Directory.CreateDirectory(XmlFolderPicker.SelectedPathOrFileName + "\\Ability\\");
            //法术的导入
            dynamic worksheet = workbook.Sheets(7);
            int rowCount = 4;
            Card.AbilityCard Ability;
            while (!String.IsNullOrEmpty(worksheet.Cells(rowCount, 2).Text))
            {
                //这行肯定是卡牌基本情报
                Ability = new AbilityCard();
                Ability.SN = worksheet.Cells(rowCount, 2).Text;
                Ability.Name = worksheet.Cells(rowCount, 3).Text;
                Ability.Description = worksheet.Cells(rowCount, 4).Text;
                Ability.Class = CardUtility.GetEnum<Card.CardUtility.ClassEnum>(worksheet.Cells(rowCount, 5).Text, Card.CardUtility.ClassEnum.中立);
                Ability.StandardCostPoint = CardUtility.GetInt(worksheet.Cells(rowCount, 6).Text);
                Ability.Rare = CardUtility.GetEnum<Card.CardBasicInfo.稀有程度>(worksheet.Cells(rowCount, 9).Text, CardBasicInfo.稀有程度.白色);
                Ability.Overload = CardUtility.GetInt(worksheet.Cells(rowCount, 10).Text);
                rowCount++;
                //这行肯定是选择条件
                Ability.CardAbility.效果选择类型 = CardUtility.GetEnum<Ability.效果选择类型枚举>(worksheet.Cells(rowCount, 3).Text,
                    Card.Effect.Ability.效果选择类型枚举.无需选择);
                rowCount++;
                GetEffectDifine(worksheet, ref rowCount);
                if (Ability.CardAbility.效果选择类型 != Card.Effect.Ability.效果选择类型枚举.无需选择)
                {
                    GetEffectDifine(worksheet, ref rowCount);
                }
                XmlSerializer xml = new XmlSerializer(typeof(Card.AbilityCard));
                String XmlFilename = XmlFolderPicker.SelectedPathOrFileName + "\\Ability\\" + Ability.SN + ".xml";
                xml.Serialize(new StreamWriter(XmlFilename), Ability);
                rowCount++;
            }
        }

        private static void GetEffectDifine(dynamic worksheet, ref int rowCount)
        {
            //这行是选择1的标题
            rowCount++;
            //这行是选择1的内容
            //TODO:根据效果读入
            rowCount++;
            //这行可能是[连击状态]或者是[追加条件]或者是下一张卡牌
            String LineType = worksheet.Cells(rowCount, 2).Text;
            if (LineType.StartsWith("A"))
            {
                rowCount--;
            }
            else
            {
                if (LineType == "连击状态")
                {
                    //TODO:根据效果读入
                }
                if (LineType == "追加条件")
                {
                    //TODO:追加条件的读入
                    rowCount++;
                    //这行是选择追加的标题
                    rowCount++;
                    //这行是选择追加的内容
                    //TODO:根据效果读入
                }
            }
        }

        /// <summary>
        /// 武器的导入
        /// </summary>
        /// <param name="target"></param>
        /// <param name="workbook"></param>
        private void Weapon(TargetType target, dynamic workbook)
        {
            if (Directory.Exists(XmlFolderPicker.SelectedPathOrFileName + "\\Weapon\\"))
            {
                Directory.Delete(XmlFolderPicker.SelectedPathOrFileName + "\\Weapon\\", true);
            }
            Directory.CreateDirectory(XmlFolderPicker.SelectedPathOrFileName + "\\Weapon\\");
            //武器的导入
            dynamic worksheet = workbook.Sheets(3);
            int rowCount = 4;
            while (!String.IsNullOrEmpty(worksheet.Cells(rowCount, 2).Text))
            {
                Card.WeaponCard Weapon = new Card.WeaponCard();
                Weapon.SN = worksheet.Cells(rowCount, 2).Text;
                Weapon.Name = worksheet.Cells(rowCount, 3).Text;
                Weapon.Description = worksheet.Cells(rowCount, 4).Text;
                Weapon.Class = CardUtility.GetEnum<Card.CardUtility.ClassEnum>(worksheet.Cells(rowCount, 5).Text, Card.CardUtility.ClassEnum.中立);
                Weapon.StandardCostPoint = CardUtility.GetInt(worksheet.Cells(rowCount, 7).Text);
                Weapon.ActualCostPoint = Weapon.StandardCostPoint;

                Weapon.StandardAttackPoint = CardUtility.GetInt(worksheet.Cells(rowCount, 8).Text);
                Weapon.标准耐久度 = CardUtility.GetInt(worksheet.Cells(rowCount, 9).Text);
                Weapon.Rare = CardUtility.GetEnum<Card.CardBasicInfo.稀有程度>(worksheet.Cells(rowCount, 12).Text, CardBasicInfo.稀有程度.白色);
                Weapon.IsCardReady = !String.IsNullOrEmpty(worksheet.Cells(rowCount, 13).Text);

                switch (target)
                {
                    case TargetType.MongoDB:
                        //innerCollection.Insert<Card.WeaponCard>(Weapon);
                        break;
                    case TargetType.Xml:
                        XmlSerializer xml = new XmlSerializer(typeof(Card.WeaponCard));
                        String XmlFilename = XmlFolderPicker.SelectedPathOrFileName + "\\Weapon\\" + Weapon.SN + ".xml";
                        xml.Serialize(new StreamWriter(XmlFilename), Weapon);
                        break;
                    default:
                        break;
                }
                rowCount++;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportXML_Click(object sender, EventArgs e)
        {
            Card.CardUtility.Init(@"C:\炉石Git\CardHelper\CardXML");
        }
        private void frmExport_Load(object sender, EventArgs e)
        {
            Card.CardUtility.CardXmlFolder = @"C:\炉石Git\炉石设计\Card";
            XmlFolderPicker.SelectedPathOrFileName = Card.CardUtility.CardXmlFolder;
            ExcelPicker.SelectedPathOrFileName = @"C:\炉石Git\炉石设计\卡牌整理版本.xls";
        }
    }
}
