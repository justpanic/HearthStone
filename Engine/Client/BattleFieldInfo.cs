﻿using Engine.Card;
using Engine.Utility;
using System;
using System.Collections.Generic;

namespace Engine.Client
{
    /// <summary>
    /// 战场信息（炉石专用）
    /// </summary>
    public class BattleFieldInfo
    {
        /// <summary>
        /// 最多7个随从的位置
        /// </summary>
        public const int MaxMinionCount = 7;
        /// <summary>
        /// 未知
        /// </summary>
        public const int UnknowPos = -1;
        /// <summary>
        /// 英雄
        /// </summary>
        public const int HeroPos = 0;
        /// <summary>
        /// 全体随从
        /// </summary>
        public const int AllMinionPos = 8;
        /// <summary>
        /// 全体角色
        /// </summary>
        public const int AllRolePos = 9;
        /// <summary>
        /// 法术消耗
        /// </summary>
        public int AbilityCost = 0;
        /// <summary>
        /// 随从消耗
        /// </summary>
        public int MinionCost = 0;
        /// <summary>
        /// 法术效果
        /// 多次伤害法术，则施法次数 + 1
        /// 单次伤害法术，则施法强度 + 1
        /// </summary>
        public int AbilityDamagePlus = 0;
        /// <summary>
        /// 当前随从数量
        /// </summary>
        public int MinionCount
        {
            get
            {
                int t = 0;
                for (int i = 0; i < MaxMinionCount; i++)
                {
                    if (BattleMinions[i] != null) t++;
                }
                return t;
            }
        }
        /// <summary>
        /// 随从位置
        /// </summary>
        ///<remarks>
        ///7个位置的注意事项
        ///[0][1][2][3][4][5][6]
        ///有些卡牌涉及到对于左右位置的加成问题，所以，位置是很敏感的
        ///</remarks>
        public MinionCard[] BattleMinions = new MinionCard[MaxMinionCount];
        /// <summary>
        /// 卡牌入战场
        /// </summary>
        /// <param name="CardSn"></param>
        public void AppendToBattle(String CardSn)
        {
            int Position = MinionCount + 1;
            PutToBattle(Position, CardSn);
        }
        /// <summary>
        /// 卡牌入战场
        /// </summary>
        /// <param name="CardSn"></param>
        public void AppendToBattle(MinionCard Minion)
        {
            int Position = MinionCount + 1;
            PutToBattle(Position, Minion);
        }
        /// <summary>
        /// 卡牌入战场
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="CardSn"></param>
        public void PutToBattle(int Position, String CardSn)
        {
            Engine.Card.CardBasicInfo card = Engine.Utility.CardUtility.GetCardInfoBySN(CardSn);
            PutToBattle(Position, (MinionCard)card);
        }
        /// <summary>
        /// 卡牌入战场
        /// </summary>
        /// <param name="Position">从1开始的位置</param>
        /// <param name="Minion">随从</param>
        /// <remarks>不涉及到战吼等计算</remarks>
        public void PutToBattle(int Position, MinionCard Minion)
        {
            //战场满了
            if (MinionCount == MaxMinionCount) return;
            //无效的位置
            if ((Position < 1) || (Position > MinionCount + 1) || Position > MaxMinionCount) return;
            //插入操作
            if (BattleMinions[Position - 1] == null)
            {
                //添加到最右边
                BattleMinions[Position - 1] = Minion;
            }
            else
            {
                //Position右边的全体移位，腾出地方
                for (int i = MaxMinionCount - 1; i >= Position; i--)
                {
                    BattleMinions[i] = BattleMinions[i - 1];
                }
                BattleMinions[Position - 1] = Minion;
            }
            for (int i = 0; i < MinionCount; i++)
            {
                BattleMinions[i].战场位置 = i + 1;
            }
        }
        /// <summary>
        /// 发动战吼
        /// 自身/相邻
        /// </summary>
        /// <param name="MinionPos"></param>
        public List<String> 发动战吼(int MinionPos)
        {
            List<String> ActionCodeLst = new List<string>();
            if (!String.IsNullOrEmpty(BattleMinions[MinionPos - 1].战吼效果))
            {
                List<int> PosList = new List<int>();
                if (BattleMinions[MinionPos - 1].战吼类型 == MinionCard.战吼类型列表.相邻)
                {
                    //相邻
                    //左边随从存在？
                    if (MinionPos != 1) PosList.Add(MinionPos - 1);
                    if (MinionPos != MinionCount) PosList.Add(MinionPos + 1);
                }
                else
                {
                    //自身
                    PosList.Add(MinionPos);
                }
                //处理状态和数值变化
                var 战吼 = (Engine.Card.AbilityCard)Engine.Utility.CardUtility.GetCardInfoBySN(BattleMinions[MinionPos - 1].战吼效果);

                foreach (int PosInfo in PosList)
                {
                    switch (战吼.FirstAbilityDefine.MainAbilityDefine.TrueAtomicEffect.AtomicEffectType)
                    {
                        case Engine.Effect.AtomicEffectDefine.AtomicEffectEnum.增益:
                            ((Engine.Effect.PointEffect)战吼.FirstAbilityDefine.MainAbilityDefine.TrueAtomicEffect).RunPointEffect(BattleMinions[PosInfo - 1]);
                            ActionCodeLst.Add(Engine.Server.ActionCode.strPoint + Engine.Utility.CardUtility.strSplitMark + Engine.Utility.CardUtility.strMe + Engine.Utility.CardUtility.strSplitMark +
                            PosInfo + Engine.Utility.CardUtility.strSplitMark + 战吼.FirstAbilityDefine.MainAbilityDefine);
                            break;
                        case Engine.Effect.AtomicEffectDefine.AtomicEffectEnum.状态:
                            ((Engine.Effect.StatusEffect)战吼.FirstAbilityDefine.MainAbilityDefine.TrueAtomicEffect).RunStatusEffect(BattleMinions[PosInfo - 1]);
                            ActionCodeLst.Add(Engine.Server.ActionCode.strStatus + Engine.Utility.CardUtility.strSplitMark + Engine.Utility.CardUtility.strMe + Engine.Utility.CardUtility.strSplitMark +
                            PosInfo + Engine.Utility.CardUtility.strSplitMark + 战吼.FirstAbilityDefine.MainAbilityDefine);
                            break;
                    }
                    switch (战吼.SecondAbilityDefine.MainAbilityDefine.TrueAtomicEffect.AtomicEffectType)
                    {
                        case Engine.Effect.AtomicEffectDefine.AtomicEffectEnum.增益:
                            ((Engine.Effect.PointEffect)战吼.SecondAbilityDefine.MainAbilityDefine.TrueAtomicEffect).RunPointEffect(BattleMinions[PosInfo - 1]);
                            ActionCodeLst.Add(Engine.Server.ActionCode.strPoint + Engine.Utility.CardUtility.strSplitMark + Engine.Utility.CardUtility.strMe + Engine.Utility.CardUtility.strSplitMark +
                            PosInfo + Engine.Utility.CardUtility.strSplitMark + 战吼.SecondAbilityDefine.MainAbilityDefine);
                            break;
                        case Engine.Effect.AtomicEffectDefine.AtomicEffectEnum.状态:
                            ((Engine.Effect.StatusEffect)战吼.SecondAbilityDefine.MainAbilityDefine.TrueAtomicEffect).RunStatusEffect(BattleMinions[PosInfo - 1]);
                            ActionCodeLst.Add(Engine.Server.ActionCode.strStatus + Engine.Utility.CardUtility.strSplitMark + Engine.Utility.CardUtility.strMe + Engine.Utility.CardUtility.strSplitMark +
                            PosInfo + Engine.Utility.CardUtility.strSplitMark + 战吼.SecondAbilityDefine.MainAbilityDefine);
                            break;
                    }
                }
            }
            return ActionCodeLst;
        }
        /// <summary>
        /// 从战场移除单位
        /// </summary>
        /// <param name="Position">从1开始的位置</param>
        /// <remarks>不涉及到亡语等计算</remarks>
        public void GetOutFromBattle(int Position)
        {
            for (int i = Position - 1; i < MaxMinionCount - 1; i++)
            {
                BattleMinions[i] = BattleMinions[i + 1];
            }
            BattleMinions[MaxMinionCount - 1] = null;
            for (int i = 0; i < MinionCount; i++)
            {
                BattleMinions[i].战场位置 = i + 1;
            }
        }
        /// <summary>
        /// Buff的设置
        /// </summary>
        public void ResetBuff()
        {
            //去除所有光环效果
            for (int i = 0; i < BattleMinions.Length; i++)
            {
                if (BattleMinions[i] != null) BattleMinions[i].受战场效果.Clear();
            }
            AbilityCost = 0;
            AbilityDamagePlus = 0;
            MinionCost = 0;
            //设置光环效果
            for (int i = 0; i < BattleMinions.Length; i++)
            {
                var minion = BattleMinions[i];
                if (minion != null)
                {
                    if (!String.IsNullOrEmpty(minion.光环效果.BuffInfo))
                    {
                        switch (minion.光环效果.EffectType)
                        {
                            case MinionCard.光环类型.增加攻防:
                                switch (minion.光环效果.Scope)
                                {
                                    case MinionCard.光环范围.随从全体:
                                        for (int j = 0; j < BattleMinions.Length; j++)
                                        {
                                            if (BattleMinions[j] != null) BattleMinions[j].受战场效果.Add(minion.光环效果);
                                        }
                                        break;
                                    case MinionCard.光环范围.相邻随从:
                                        break;
                                }
                                break;
                            case MinionCard.光环类型.施法成本:
                                AbilityCost += int.Parse(minion.光环效果.BuffInfo);
                                break;
                            case MinionCard.光环类型.法术效果:
                                AbilityDamagePlus += int.Parse(minion.光环效果.BuffInfo);
                                break;
                            case MinionCard.光环类型.随从成本:
                                MinionCost += int.Parse(minion.光环效果.BuffInfo);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 设置是否能成为当前动作的对象
        /// </summary>
        /// <param name="SelectOption"></param>
        /// <param name="game"></param>
        public static void SetTargetSelectEnable(CardUtility.PositionSelectOption SelectOption, GameManager game)
        {
            switch (SelectOption.EffectTargetSelectDirect)
            {
                case CardUtility.TargetSelectDirectEnum.本方:
                    switch (SelectOption.EffectTargetSelectRole)
                    {
                        case CardUtility.TargetSelectRoleEnum.随从:
                            for (int i = 0; i < game.MyInfo.BattleField.MinionCount; i++)
                            {
                                if (Engine.Utility.CardUtility.符合选择条件(game.MyInfo.BattleField.BattleMinions[i], SelectOption))
                                    game.MyInfo.BattleField.BattleMinions[i].能否成为动作对象 = true;
                            }
                            break;
                        case CardUtility.TargetSelectRoleEnum.英雄:
                            game.MyInfo.能否成为动作对象 = true;
                            break;
                        case CardUtility.TargetSelectRoleEnum.所有角色:
                            game.MyInfo.能否成为动作对象 = true;
                            for (int i = 0; i < game.MyInfo.BattleField.MinionCount; i++)
                            {
                                if (Engine.Utility.CardUtility.符合选择条件(game.MyInfo.BattleField.BattleMinions[i], SelectOption))
                                    game.MyInfo.BattleField.BattleMinions[i].能否成为动作对象 = true;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case CardUtility.TargetSelectDirectEnum.对方:
                    switch (SelectOption.EffectTargetSelectRole)
                    {
                        case CardUtility.TargetSelectRoleEnum.随从:
                            for (int i = 0; i < game.YourInfo.BattleField.MinionCount; i++)
                            {
                                if (Engine.Utility.CardUtility.符合选择条件(game.YourInfo.BattleField.BattleMinions[i], SelectOption))
                                    game.YourInfo.BattleField.BattleMinions[i].能否成为动作对象 = true;
                            }
                            break;
                        case CardUtility.TargetSelectRoleEnum.英雄:
                            game.YourInfo.能否成为动作对象 = true;
                            break;
                        case CardUtility.TargetSelectRoleEnum.所有角色:

                            Boolean Has嘲讽 = false;
                            for (int i = 0; i < game.YourInfo.BattleField.MinionCount; i++)
                            {
                                if (game.YourInfo.BattleField.BattleMinions[i].嘲讽特性)
                                {
                                    Has嘲讽 = true;
                                    break;
                                }
                            }
                            if (SelectOption.嘲讽限制 && Has嘲讽)
                            {
                                game.YourInfo.能否成为动作对象 = false;
                                for (int i = 0; i < game.YourInfo.BattleField.MinionCount; i++)
                                {
                                    //只能选择嘲讽对象
                                    if (game.YourInfo.BattleField.BattleMinions[i].嘲讽特性)
                                    {
                                        game.YourInfo.BattleField.BattleMinions[i].能否成为动作对象 = true;
                                    }
                                }
                            }
                            else
                            {
                                game.YourInfo.能否成为动作对象 = true;
                                for (int i = 0; i < game.YourInfo.BattleField.MinionCount; i++)
                                {
                                    if (Engine.Utility.CardUtility.符合选择条件(game.YourInfo.BattleField.BattleMinions[i], SelectOption))
                                        game.YourInfo.BattleField.BattleMinions[i].能否成为动作对象 = true;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case CardUtility.TargetSelectDirectEnum.双方:
                    switch (SelectOption.EffectTargetSelectRole)
                    {
                        case CardUtility.TargetSelectRoleEnum.随从:
                            for (int i = 0; i < game.MyInfo.BattleField.MinionCount; i++)
                            {
                                if (Engine.Utility.CardUtility.符合选择条件(game.MyInfo.BattleField.BattleMinions[i], SelectOption))
                                    game.MyInfo.BattleField.BattleMinions[i].能否成为动作对象 = true;
                            }
                            for (int i = 0; i < game.YourInfo.BattleField.MinionCount; i++)
                            {
                                if (Engine.Utility.CardUtility.符合选择条件(game.YourInfo.BattleField.BattleMinions[i], SelectOption))
                                    game.YourInfo.BattleField.BattleMinions[i].能否成为动作对象 = true;
                            }
                            break;
                        case CardUtility.TargetSelectRoleEnum.英雄:
                            game.MyInfo.能否成为动作对象 = true;
                            game.YourInfo.能否成为动作对象 = true;
                            break;
                        case CardUtility.TargetSelectRoleEnum.所有角色:
                            game.MyInfo.能否成为动作对象 = true;
                            game.YourInfo.能否成为动作对象 = true;
                            for (int i = 0; i < game.MyInfo.BattleField.MinionCount; i++)
                            {
                                if (Engine.Utility.CardUtility.符合选择条件(game.MyInfo.BattleField.BattleMinions[i], SelectOption))
                                    game.MyInfo.BattleField.BattleMinions[i].能否成为动作对象 = true;
                            }
                            for (int i = 0; i < game.YourInfo.BattleField.MinionCount; i++)
                            {
                                if (Engine.Utility.CardUtility.符合选择条件(game.YourInfo.BattleField.BattleMinions[i], SelectOption))
                                    game.YourInfo.BattleField.BattleMinions[i].能否成为动作对象 = true;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 去除死去随从
        /// </summary>
        /// <param name="game"></param>
        /// <param name="MeOrYou"></param>
        /// <returns></returns>
        public List<MinionCard> ClearDead(GameManager game, Boolean MeOrYou)
        {
            //必须是当前的随从，不能使编号
            //如果是沉默状态的随从，无亡语效果！
            List<MinionCard> DeadList = new List<MinionCard>();
            var CloneMinions = new MinionCard[BattleFieldInfo.MaxMinionCount];
            int ALive = 0;
            for (int i = 0; i < BattleFieldInfo.MaxMinionCount; i++)
            {
                if (BattleMinions[i] != null)
                {
                    if (BattleMinions[i].生命值 > 0)
                    {
                        CloneMinions[ALive] = BattleMinions[i];
                        ALive++;
                    }
                    else
                    {
                        DeadList.Add(BattleMinions[i]);
                        game.事件处理组件.事件池.Add(new Engine.Utility.CardUtility.全局事件()
                        {
                            事件类型 = CardUtility.事件类型列表.死亡,
                            触发位置 = i + 1,
                            触发方向 = MeOrYou ? CardUtility.TargetSelectDirectEnum.本方 : CardUtility.TargetSelectDirectEnum.对方,
                            附加信息 = BattleMinions[i].种族.ToString()
                        });
                    }
                }
            }
            BattleMinions = CloneMinions;
            for (int i = 0; i < MinionCount; i++)
            {
                BattleMinions[i].战场位置 = i + 1;
            }
            return DeadList;
        }
    }
}
