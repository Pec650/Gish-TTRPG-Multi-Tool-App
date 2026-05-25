using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Gish.Pages.Main_Pages;

namespace Gish.Pages.ToolPages;

public partial class CRCalculatorPage : ContentPage
{
    private string _weaponAbility = "STR";
    private string _spellAbility  = "NONE";

    // Dynamic style lookup targets to maintain absolute resource alignment
    private Color ActiveGreenColor => Application.Current?.Resources.TryGetValue("AccentGreen", out var colorVal) == true ? (Color)colorVal : Color.FromArgb("#2E7D32");
    private Color InactiveClearColor => Colors.Transparent;

    private static readonly List<CREntry> CRTable = new()
    {
        new("0",    0.0,   2, 13, 1,   6,   "1-6",     4,    3,  0,   1,   "0-1",    0.5,  13),
        new("1/8",  0.125, 2, 13, 7,   35,  "7-35",    21,   3,  2,   3,   "2-3",    2.5,  13),
        new("1/4",  0.25,  2, 13, 36,  49,  "36-49",   43,   3,  4,   5,   "4-5",    4.5,  13),
        new("1/2",  0.5,   2, 13, 50,  70,  "50-70",   60,   3,  6,   8,   "6-8",    7.0,  13),
        new("1",    1.0,   2, 13, 71,  85,  "71-85",   78,   3,  9,   14,  "9-14",   11.5, 13),
        new("2",    2.0,   2, 13, 86,  100, "86-100",  93,   3,  15,  20,  "15-20",  17.5, 13),
        new("3",    3.0,   2, 13, 101, 115, "101-115", 108,  4,  21,  26,  "21-26",  23.5, 13),
        new("4",    4.0,   2, 14, 116, 130, "116-130", 123,  5,  27,  32,  "27-32",  29.5, 14),
        new("5",    5.0,   3, 15, 131, 145, "131-145", 138,  6,  33,  38,  "33-38",  35.5, 15),
        new("6",    6.0,   3, 15, 146, 160, "146-160", 153,  6,  39,  44,  "39-44",  41.5, 15),
        new("7",    7.0,   3, 15, 161, 175, "161-175", 168,  6,  45,  50,  "45-50",  47.5, 15),
        new("8",    8.0,   3, 16, 176, 190, "176-190", 183,  7,  51,  56,  "51-56",  53.5, 16),
        new("9",    9.0,   4, 16, 191, 205, "191-205", 198,  7,  57,  62,  "57-62",  59.5, 16),
        new("10",   10.0,  4, 17, 206, 220, "206-220", 213,  7,  63,  68,  "63-68",  65.5, 16),
        new("11",   11.0,  4, 17, 221, 235, "221-235", 228,  8,  69,  74,  "69-74",  71.5, 17),
        new("12",   12.0,  4, 17, 236, 250, "236-250", 243,  8,  75,  80,  "75-80",  77.5, 17),
        new("13",   13.0,  5, 18, 251, 265, "251-265", 258,  8,  81,  86,  "81-86",  83.5, 18),
        new("14",   14.0,  5, 18, 266, 280, "266-280", 273,  8,  87,  92,  "87-92",  89.5, 18),
        new("15",   15.0,  5, 18, 281, 295, "281-295", 288,  8,  93,  98,  "93-98",  95.5, 18),
        new("16",   16.0,  5, 18, 296, 310, "296-310", 303,  9,  99,  104, "99-104", 101.5,18),
        new("17",   17.0,  6, 19, 311, 325, "311-325", 318,  10, 105, 110, "105-110",107.5,19),
        new("18",   18.0,  6, 19, 326, 340, "326-340", 333,  10, 111, 116, "111-116",113.5,19),
        new("19",   19.0,  6, 19, 341, 355, "341-355", 348,  10, 117, 122, "117-122",119.5,19),
        new("20",   20.0,  6, 19, 356, 400, "356-400", 378,  10, 123, 140, "123-140",131.5,19),
        new("21",   21.0,  7, 19, 401, 445, "401-445", 423,  11, 141, 158, "141-158",149.5,20),
        new("22",   22.0,  7, 19, 446, 490, "446-490", 468,  11, 159, 176, "159-176",167.5,20),
        new("23",   23.0,  7, 19, 491, 535, "491-535", 513,  11, 177, 194, "177-194",185.5,20),
        new("24",   24.0,  7, 19, 536, 580, "536-580", 558,  12, 195, 212, "195-212",203.5,21),
        new("25",   25.0,  8, 19, 581, 625, "581-625", 603,  12, 212, 230, "212-230",221.0,21),
        new("26",   26.0,  8, 19, 626, 670, "626-670", 648,  12, 231, 248, "231-248",239.5,21),
        new("27",   27.0,  8, 19, 671, 715, "671-715", 693,  13, 249, 266, "249-266",257.5,22),
        new("28",   28.0,  8, 19, 716, 760, "716-760", 738,  13, 267, 284, "267-284",275.5,22),
        new("29",   29.0,  9, 19, 761, 805, "761-805", 783,  13, 285, 302, "285-302",293.5,22),
        new("30",   30.0,  9, 19, 806, 850, "806-850", 828,  14, 303, 320, "303-320",311.5,23),
    };

    public CRCalculatorPage()
    {
        InitializeComponent();
        UpdateWeaponUI();
        UpdateSpellUI();
    }

    // --- WEAPON ABILITY SELECTION ---

    private void OnWeaponSTR(object sender, EventArgs e) { _weaponAbility = "STR"; UpdateWeaponUI(); }
    private void OnWeaponDEX(object sender, EventArgs e) { _weaponAbility = "DEX"; UpdateWeaponUI(); }

    private void UpdateWeaponUI()
    {
        WeaponSTRBlock.BackgroundColor = _weaponAbility == "STR" ? ActiveGreenColor : InactiveClearColor;
        WeaponDEXBlock.BackgroundColor = _weaponAbility == "DEX" ? ActiveGreenColor : InactiveClearColor;

        SetBlockLabelColor(WeaponSTRBlock, _weaponAbility == "STR");
        SetBlockLabelColor(WeaponDEXBlock, _weaponAbility == "DEX");
    }

    // --- SPELL ABILITY SELECTION ---

    private void OnSpellNone(object sender, EventArgs e) { _spellAbility = "NONE"; UpdateSpellUI(); }
    private void OnSpellINT(object sender, EventArgs e)  { _spellAbility = "INT";  UpdateSpellUI(); }
    private void OnSpellWIS(object sender, EventArgs e)  { _spellAbility = "WIS";  UpdateSpellUI(); }
    private void OnSpellCHA(object sender, EventArgs e)  { _spellAbility = "CHA";  UpdateSpellUI(); }

    private void UpdateSpellUI()
    {
        SpellNoneBlock.BackgroundColor = _spellAbility == "NONE" ? ActiveGreenColor : InactiveClearColor;
        SpellINTBlock.BackgroundColor  = _spellAbility == "INT"  ? ActiveGreenColor : InactiveClearColor;
        SpellWISBlock.BackgroundColor  = _spellAbility == "WIS"  ? ActiveGreenColor : InactiveClearColor;
        SpellCHABlock.BackgroundColor  = _spellAbility == "CHA"  ? ActiveGreenColor : InactiveClearColor;

        SetBlockLabelColor(SpellNoneBlock, _spellAbility == "NONE");
        SetBlockLabelColor(SpellINTBlock,  _spellAbility == "INT");
        SetBlockLabelColor(SpellWISBlock,  _spellAbility == "WIS");
        SetBlockLabelColor(SpellCHABlock,  _spellAbility == "CHA");
    }

    private static void SetBlockLabelColor(Border block, bool active)
    {
        if (block.Content is Label lbl)
            lbl.TextColor = active ? Colors.White : Color.FromArgb("#AAB7B8");
    }

    // --- CALCULATE ---

    private void OnCalculateClicked(object sender, EventArgs e)
    {
        if (!int.TryParse(HPEntry.Text,  out int hp)  || hp  < 0) return;
        if (!int.TryParse(DPREntry.Text, out int dpr) || dpr < 0) return;
        if (!int.TryParse(ACEntry.Text,  out int ac)  || ac  < 0) return;

        int hpIndex  = GetCRIndexByHP(hp);
        int dprIndex = GetCRIndexByDPR(dpr);
        int avgIndex = Math.Clamp((int)Math.Round((hpIndex + dprIndex) / 2.0), 0, CRTable.Count - 1);

        CREntry hpEntry  = CRTable[hpIndex];
        CREntry dprEntry = CRTable[dprIndex];
        CREntry avgEntry = CRTable[avgIndex];

        double hpCR  = hpEntry.CRNumeric;
        double dprCR = dprEntry.CRNumeric;
        double avgCR = avgEntry.CRNumeric;

        // --- CR stat block labels ---
        CRLabel.Text      = $"CR {avgEntry.CR}";
        CRBadgeLabel.Text = avgEntry.CR;
        HPCRLabel.Text    = hpEntry.CR;
        DPRCRLabel.Text   = dprEntry.CR;

        StatProfBonus.Text   = $"+{avgEntry.ProfBonus}";
        StatAC.Text          = avgEntry.AC.ToString();
        StatHP.Text          = avgEntry.HPRange;
        StatAttackBonus.Text = $"+{avgEntry.AttackBonus}";
        StatDPR.Text         = avgEntry.DPRRange;
        StatSaveDC.Text      = avgEntry.SaveDC.ToString();
        StatAvgHP.Text       = avgEntry.AvgHP.ToString();

        // --- Ability score calculations ---
        int conScore = FloorStat(0.5 * hpCR  + 10.5);
        int dprStat  = FloorStat(0.5 * dprCR + 10.5);
        int avgStat  = FloorStat(0.5 * avgCR + 10.5);

        // DEX from AC
        int dexFromAC  = FloorStat(2.0 * ac - 10.0);
        int dexFromDPR = dprStat;
        int dexScore   = _weaponAbility == "DEX"
            ? Math.Max(dexFromAC, dexFromDPR)
            : dexFromAC;

        // STR
        int strScore = _weaponAbility == "STR" ? dprStat : avgStat;

        // Spell stats
        int intScore = _spellAbility == "INT" ? dprStat : avgStat;
        int wisScore = _spellAbility == "WIS" ? dprStat : avgStat;
        int chaScore = _spellAbility == "CHA" ? dprStat : avgStat;

        // --- Display ability scores ---
        SetAbilityScore(STRScore, STRMod, strScore);
        SetAbilityScore(DEXScore, DEXMod, dexScore);
        SetAbilityScore(CONScore, CONMod, conScore);
        SetAbilityScore(INTScore, INTMod, intScore);
        SetAbilityScore(WISScore, WISMod, wisScore);
        SetAbilityScore(CHAScore, CHAMod, chaScore);

        // Highlight weapon and spell ability scorecards
        STRLabel.TextColor = _weaponAbility == "STR"
            ? Color.FromArgb("#2E7D32") : Color.FromArgb("#AAB7B8");
        DEXLabel.TextColor = _weaponAbility == "DEX"
            ? Color.FromArgb("#2E7D32") : Color.FromArgb("#AAB7B8");

        INTMod.TextColor = _spellAbility == "INT"
            ? Color.FromArgb("#4A148C") : Color.FromArgb("#AAB7B8");
        WISMod.TextColor = _spellAbility == "WIS"
            ? Color.FromArgb("#4A148C") : Color.FromArgb("#AAB7B8");
        CHAMod.TextColor = _spellAbility == "CHA"
            ? Color.FromArgb("#4A148C") : Color.FromArgb("#AAB7B8");

        ResultCard.IsVisible = true;
    }

    private static void SetAbilityScore(Label scoreLabel, Label modLabel, int score)
    {
        scoreLabel.Text = score.ToString();
        int mod = (int)Math.Floor((score - 10) / 2.0);
        modLabel.Text = mod >= 0 ? $"(+{mod})" : $"({mod})";
    }

    private static int FloorStat(double value) => (int)Math.Floor(value);

    private static int GetCRIndexByHP(int hp)
    {
        for (int i = 0; i < CRTable.Count; i++)
            if (hp <= CRTable[i].MaxHP) return i;
        return CRTable.Count - 1;
    }

    private static int GetCRIndexByDPR(int dpr)
    {
        for (int i = 0; i < CRTable.Count; i++)
            if (dpr <= CRTable[i].MaxDPR) return i;
        return CRTable.Count - 1;
    }

    private record CREntry(
        string CR,
        double CRNumeric,
        int ProfBonus,
        int AC,
        int MinHP,
        int MaxHP,
        string HPRange,
        double AvgHP,
        int AttackBonus,
        int MinDPR,
        int MaxDPR,
        string DPRRange,
        double AvgDPR,
        int SaveDC
    );
    
    private void ReturnPage(object? sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }
}