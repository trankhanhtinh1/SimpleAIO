using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Utility;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;
using EnsoulSharp.SDK.Rendering;

namespace SimpleAIO.Champions
{
    internal class Trundle
    {
        private static Spell Q,W,E,R;
        private static Menu mainMenu;
        public static void OnGameLoad(){
            if(GameObjects.Player.CharacterName != "Trundle") return;
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W,900f);
            E = new Spell(SpellSlot.E,1000f);
            R = new Spell(SpellSlot.R,650);
            R.SetTargetted(0.25f,float.MaxValue);
            E.SetSkillshot(.5f, 187.5f, 1600f, false,SpellType.Circle);

            mainMenu = new Menu("Trundle", "Trundle", true);
            var Combo = new Menu("Combo","Combo Settings");
            Combo.Add(new MenuBool("Quse","Use Q",true));
            Combo.Add(new MenuBool("Wuse","Use W",true));
            Combo.Add(new MenuBool("Euse","Use E",true));
            Combo.Add(new MenuBool("Ruse","Use R",true));
            mainMenu.Add(Combo);
            var Harass = new Menu("Harass","Harass Settings");
            Harass.Add(new MenuBool("Quse","Use Q",true));
            Harass.Add(new MenuBool("Wuse","Use W",true));
            Harass.Add(new MenuBool("Euse","Use E",true));
            Harass.Add(new MenuSlider("mana%","Mana percent",50,0,100));
            mainMenu.Add(Harass);
        
            var Draw = new Menu("Draw","Draw Settings");
            Draw.Add(new MenuBool("qRange","Draw Q range",true));
            Draw.Add(new MenuBool("wRange","Draw W range",true));
            Draw.Add(new MenuBool("eRange","Draw E range",true));
            Draw.Add(new MenuBool("rRange","Draw R range",true));
            Draw.Add(new MenuBool("lista","Draw only if spell is ready",true));
            mainMenu.Add(Draw);

            mainMenu.Attach();
            GameEvent.OnGameTick += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
            Orbwalker.OnAfterAttack += OnAfterAttack;

        }
        private static void ComboLogic()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            var inputE = E.GetPrediction(target);
            if (mainMenu["Combo"].GetValue<MenuBool>("Euse").Enabled && W.IsReady() && target.IsValidTarget())
            {
                W.Cast(target.Position);
            }
                if (mainMenu["Combo"].GetValue<MenuBool>("Euse").Enabled)
            {
               if(E.IsReady() && target.IsValidTarget() && inputE.Hitchance >= HitChance.VeryHigh && E.IsInRange(target))
                {
                        E.Cast(inputE.CastPosition);
                }
            }
        }
        public static void OnAfterAttack(object sender, AfterAttackEventArgs args)
        {
            if (args.Target == null || !args.Target.IsValidTarget()) return;
            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo && args.Target is AIHeroClient && mainMenu["Combo"].GetValue<MenuBool>("Quse").Enabled) if (Q.Cast()) Orbwalker.ResetAutoAttackTimer();
        }

        private static void HarassLogic()
        {
            var target = TargetSelector.GetTarget(E.Range,DamageType.Physical);
            var inputE = E.GetPrediction(target);
            if(mainMenu["Harass"].GetValue<MenuSlider>("mana%").Value <= GameObjects.Player.ManaPercent)
            {
            }
        }
      

        private static void OnGameUpdate(EventArgs args){
            if(GameObjects.Player.IsDead) return;
            
            switch (Orbwalker.ActiveMode){
                case OrbwalkerMode.Combo:
                    ComboLogic();
                    break;
                    case OrbwalkerMode.Harass:
                   // HarassLogic();
                    break;
                case OrbwalkerMode.LaneClear:
                   
                    break;
            }
        }
        private static void OnDraw(EventArgs args)
        {
            var PlayerPos = GameObjects.Player.Position;
            if (mainMenu["Draw"].GetValue<MenuBool>("lista").Enabled)
            {
                if (mainMenu["Draw"].GetValue<MenuBool>("qRange").Enabled) if (Q.IsReady()) CircleRender.Draw(PlayerPos, Q.Range, Color.Cyan, 1);
                if (mainMenu["Draw"].GetValue<MenuBool>("wRange").Enabled) if (W.IsReady()) CircleRender.Draw(PlayerPos, W.Range, Color.Silver, 1);
                if (mainMenu["Draw"].GetValue<MenuBool>("eRange").Enabled) if (E.IsReady()) CircleRender.Draw(PlayerPos, E.Range, Color.Yellow, 1);
                if (mainMenu["Draw"].GetValue<MenuBool>("rRange").Enabled) if (R.IsReady()) CircleRender.Draw(PlayerPos, R.Range, Color.Blue, 1);
            }
            if (!mainMenu["Draw"].GetValue<MenuBool>("lista").Enabled)
            {
                if (mainMenu["Draw"].GetValue<MenuBool>("qRange").Enabled) if (Q.IsReady()) CircleRender.Draw(PlayerPos, Q.Range, Color.Cyan, 1);
                if (mainMenu["Draw"].GetValue<MenuBool>("wRange").Enabled) if (W.IsReady()) CircleRender.Draw(PlayerPos, W.Range, Color.Silver, 1);
                if (mainMenu["Draw"].GetValue<MenuBool>("eRange").Enabled) if (E.IsReady()) CircleRender.Draw(PlayerPos, E.Range, Color.Yellow, 1);
                if (mainMenu["Draw"].GetValue<MenuBool>("rRange").Enabled) if (R.IsReady()) CircleRender.Draw(PlayerPos, R.Range, Color.Blue, 1);

            }
        }
    }
}
