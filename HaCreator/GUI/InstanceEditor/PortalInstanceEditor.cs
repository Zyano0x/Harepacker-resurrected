﻿/* Copyright (C) 2015 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HaCreator.MapEditor;
using MapleLib.WzLib.WzStructure.Data;
using System.Collections;
using HaCreator.MapEditor.Instance;
using HaCreator.MapEditor.UndoRedo;
using HaCreator.Wz;
using HaCreator.Properties;

namespace HaCreator.GUI.InstanceEditor
{
    public partial class PortalInstanceEditor : EditorBase
    {
        public PortalInstance item;
        private ControlRowManager rowMan;

        public PortalInstanceEditor(PortalInstance item)
        {
            InitializeComponent();
            int portalTypes = Program.InfoManager.PortalEditor_TypeById.Count;
            ArrayList portals = new ArrayList();
            for (int i = 0; i < portalTypes; i++)
            {
                try
                {
                    portals.Add(PortalTypeExtensions.GetFriendlyName(Program.InfoManager.PortalEditor_TypeById[i]));
                }
                catch(KeyNotFoundException) 
                { 
                    continue; 
                }
            }
            
            ptComboBox.Items.AddRange(portals.ToArray());
            this.item = item;

            rowMan = new ControlRowManager(new ControlRow[] { 
            new ControlRow(new Control[] { pnLabel, pnBox }, 26, "pn"),
            new ControlRow(new Control[] { tmLabel, tmBox, btnBrowseMap, thisMap }, 26, "tm"),
            new ControlRow(new Control[] { tnLabel, tnBox, btnBrowseTn, leftBlankLabel }, 26, "tn"),
            new ControlRow(new Control[] { scriptLabel, scriptBox }, 26, "script"),
            new ControlRow(new Control[] { delayEnable, delayBox }, 26, "delay"),
            new ControlRow(new Control[] { rangeEnable, xRangeLabel, hRangeBox, yRangeLabel, vRangeBox }, 26, "range"),
            new ControlRow(new Control[] { impactLabel, hImpactEnable, hImpactBox, vImpactEnable, vImpactBox }, 26, "impact"),
            new ControlRow(new Control[] { hideTooltip, onlyOnce }, 26, "bool"),
            new ControlRow(new Control[] { imageLabel, portalImageList, portalImageBox }, okButton.Top - portalImageList.Top, "image"),
            new ControlRow(new Control[] { okButton, cancelButton }, 26, "buttons")
        }, this);

            delayEnable.Tag = delayBox;
            hImpactEnable.Tag = hImpactBox;
            vImpactEnable.Tag = vImpactBox;

            xInput.Value = item.X;
            yInput.Value = item.Y;
            ptComboBox.SelectedIndex = Program.InfoManager.PortalIdByType[item.pt];
            pnBox.Text = item.pn;
            if (item.tm == item.Board.MapInfo.id) thisMap.Checked = true;
            else tmBox.Value = item.tm;
            tnBox.Text = item.tn;
            if (item.script != null) scriptBox.Text = item.script;
            SetOptionalInt(item.delay, delayEnable, delayBox);
            SetOptionalInt(item.hRange, rangeEnable, hRangeBox);
            SetOptionalInt(item.vRange, rangeEnable, vRangeBox);
            SetOptionalInt(item.horizontalImpact, hImpactEnable, hImpactBox);
            if (item.verticalImpact != null) vImpactBox.Value = (int)item.verticalImpact;
            onlyOnce.Checked = item.onlyOnce;
            hideTooltip.Checked = item.hideTooltip;
            if (item.image != null)
            {
                portalImageList.SelectedItem = item.image;
            }
        }

        private void SetOptionalInt(int? value, CheckBox enabler, NumericUpDown input)
        {
            if (value == null) enabler.Checked = false;
            else { enabler.Checked = true; input.Value = (int)value; }
        }

        private int? GetOptionalInt(CheckBox enabler, NumericUpDown input)
        {
            return enabler.Checked ? (int?)input.Value : null;
        }

        protected override void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void okButton_Click(object sender, EventArgs e)
        {
            lock (item.Board.ParentControl)
            {
                List<UndoRedoAction> actions = new List<UndoRedoAction>();
                if (xInput.Value != item.X || yInput.Value != item.Y)
                {
                    actions.Add(UndoRedoManager.ItemMoved(item, new Microsoft.Xna.Framework.Point(item.X, item.Y), new Microsoft.Xna.Framework.Point((int)xInput.Value, (int)yInput.Value)));
                    item.Move((int)xInput.Value, (int)yInput.Value);
                }
                if (actions.Count > 0)
                    item.Board.UndoRedoMan.AddUndoBatch(actions);

                item.pt = Program.InfoManager.PortalEditor_TypeById[ptComboBox.SelectedIndex];
                switch (item.pt)
                {
                    case PortalType.StartPoint:
                        {
                            item.pn = "sp";
                            item.tm = MapConstants.MaxMap;
                            item.tn = "";
                            item.hRange = null;
                            item.vRange = null;
                            item.horizontalImpact = null;
                            item.verticalImpact = null;
                            item.delay = null;
                            item.script = null;
                            item.onlyOnce = null;
                            item.hideTooltip = null;
                            break;
                        }
                    case PortalType.Invisible:
                        {
                            item.pn = pnBox.Text;
                            item.tm = thisMap.Checked ? item.Board.MapInfo.id : (int)tmBox.Value;
                            item.tn = tnBox.Text;
                            item.hRange = null;
                            item.vRange = null;
                            item.horizontalImpact = null;
                            item.verticalImpact = null;
                            item.delay = GetOptionalInt(delayEnable, delayBox);
                            item.script = null;
                            item.onlyOnce = onlyOnce.Checked;
                            item.hideTooltip = hideTooltip.Checked;
                            break;
                        }
                    case PortalType.Visible:
                        {
                            item.pn = pnBox.Text;
                            item.tm = thisMap.Checked ? item.Board.MapInfo.id : (int)tmBox.Value;
                            item.tn = tnBox.Text;
                            item.hRange = null;
                            item.vRange = null;
                            item.horizontalImpact = null;
                            item.verticalImpact = null;
                            item.delay = GetOptionalInt(delayEnable, delayBox);
                            item.script = null;
                            item.onlyOnce = onlyOnce.Checked;
                            item.hideTooltip = hideTooltip.Checked;
                            break;
                        }
                    case PortalType.Collision:
                        {
                            item.pn = pnBox.Text;
                            item.tm = thisMap.Checked ? item.Board.MapInfo.id : (int)tmBox.Value;
                            item.tn = tnBox.Text;
                            item.hRange = GetOptionalInt(rangeEnable, hRangeBox);
                            item.vRange = GetOptionalInt(rangeEnable, vRangeBox);
                            item.horizontalImpact = null;
                            item.verticalImpact = null;
                            item.delay = GetOptionalInt(delayEnable, delayBox);
                            item.script = null;
                            item.onlyOnce = onlyOnce.Checked;
                            item.hideTooltip = hideTooltip.Checked;
                            break;
                        }
                    case PortalType.Changeable:
                        {
                            item.pn = pnBox.Text;
                            item.tm = thisMap.Checked ? item.Board.MapInfo.id : (int)tmBox.Value;
                            item.tn = tnBox.Text;
                            item.hRange = null;
                            item.vRange = null;
                            item.horizontalImpact = null;
                            item.verticalImpact = null;
                            item.delay = GetOptionalInt(delayEnable, delayBox);
                            item.script = null;
                            item.onlyOnce = onlyOnce.Checked;
                            item.hideTooltip = hideTooltip.Checked;
                            break;
                        }
                    case PortalType.ChangeableInvisible:
                        {
                            item.pn = pnBox.Text;
                            item.tm = thisMap.Checked ? item.Board.MapInfo.id : (int)tmBox.Value;
                            item.tn = tnBox.Text;
                            item.hRange = null;
                            item.vRange = null;
                            item.horizontalImpact = null;
                            item.verticalImpact = null;
                            item.delay = GetOptionalInt(delayEnable, delayBox);
                            item.script = scriptBox.Text;
                            item.onlyOnce = onlyOnce.Checked;
                            item.hideTooltip = hideTooltip.Checked;
                            break;
                        }
                    case PortalType.TownPortalPoint:
                        {
                            item.pn = "tp";
                            item.tm = MapConstants.MaxMap;
                            item.tn = "";
                            item.hRange = null;
                            item.vRange = null;
                            item.horizontalImpact = null;
                            item.verticalImpact = null;
                            item.delay = null;
                            item.script = null;
                            item.onlyOnce = null;
                            item.hideTooltip = null;
                            break;
                        }
                    case PortalType.Script:
                        {
                            item.pn = pnBox.Text;
                            item.tm = MapConstants.MaxMap;
                            item.tn = "";
                            item.hRange = null;
                            item.vRange = null;
                            item.horizontalImpact = null;
                            item.verticalImpact = null;
                            item.delay = null;
                            item.script = scriptBox.Text;
                            item.onlyOnce = onlyOnce.Checked;
                            item.hideTooltip = hideTooltip.Checked;
                            break;
                        }
                    case PortalType.ScriptInvisible:
                        {
                            item.pn = pnBox.Text;
                            item.tm = MapConstants.MaxMap;
                            item.tn = "";
                            item.hRange = null;
                            item.vRange = null;
                            item.horizontalImpact = null;
                            item.verticalImpact = null;
                            item.delay = null;
                            item.script = null;
                            item.onlyOnce = onlyOnce.Checked;
                            item.hideTooltip = hideTooltip.Checked;
                            break;
                        }
                    case PortalType.CollisionScript:
                        {
                            item.pn = pnBox.Text;
                            item.tm = MapConstants.MaxMap;
                            item.tn = "";
                            item.hRange = null;
                            item.vRange = null;
                            item.horizontalImpact = null;
                            item.verticalImpact = null;
                            item.delay = GetOptionalInt(delayEnable, delayBox);
                            item.script = scriptBox.Text;
                            item.onlyOnce = onlyOnce.Checked;
                            item.hideTooltip = hideTooltip.Checked;
                            break;
                        }
                    case PortalType.Hidden:
                        {
                            item.pn = pnBox.Text;
                            item.tm = thisMap.Checked ? item.Board.MapInfo.id : (int)tmBox.Value;
                            item.tn = tnBox.Text;
                            item.hRange = null;
                            item.vRange = null;
                            item.horizontalImpact = null;
                            item.verticalImpact = null;
                            item.delay = GetOptionalInt(delayEnable, delayBox);
                            item.script = null;
                            item.onlyOnce = onlyOnce.Checked;
                            item.hideTooltip = hideTooltip.Checked;
                            break;
                        }
                    case PortalType.ScriptHidden:
                        {
                            item.pn = pnBox.Text;
                            item.tm = MapConstants.MaxMap;
                            item.tn = "";
                            item.hRange = null;
                            item.vRange = null;
                            item.horizontalImpact = null;
                            item.verticalImpact = null;
                            item.delay = GetOptionalInt(delayEnable, delayBox);
                            item.script = null;
                            item.onlyOnce = onlyOnce.Checked;
                            item.hideTooltip = hideTooltip.Checked;
                            break;
                        }
                    case PortalType.CollisionVerticalJump:
                        {
                            item.pn = pnBox.Text;
                            item.tm = MapConstants.MaxMap;
                            item.tn = tnBox.Text;
                            item.hRange = null;
                            item.vRange = null;
                            item.horizontalImpact = null;
                            item.verticalImpact = null;
                            item.delay = GetOptionalInt(delayEnable, delayBox);
                            item.script = null;
                            item.onlyOnce = onlyOnce.Checked;
                            item.hideTooltip = hideTooltip.Checked;
                            break;
                        }
                    case PortalType.CollisionCustomImpact:
                        {
                            item.pn = pnBox.Text;
                            item.tm = MapConstants.MaxMap;
                            item.tn = "";
                            item.hRange = null;
                            item.vRange = null;
                            item.horizontalImpact = GetOptionalInt(hImpactEnable, hImpactBox);
                            item.verticalImpact = (int)vImpactBox.Value;
                            item.delay = GetOptionalInt(delayEnable, delayBox);
                            item.script = null;
                            item.onlyOnce = onlyOnce.Checked;
                            item.hideTooltip = hideTooltip.Checked;
                            break;
                        }
                    case PortalType.CollisionUnknownPcig:
                        {
                            item.pn = pnBox.Text;
                            item.tm = thisMap.Checked ? item.Board.MapInfo.id : (int)tmBox.Value;
                            item.tn = tnBox.Text;
                            item.hRange = null;
                            item.vRange = null;
                            item.horizontalImpact = GetOptionalInt(hImpactEnable, hImpactBox);
                            item.verticalImpact = GetOptionalInt(vImpactEnable, vImpactBox);
                            item.delay = GetOptionalInt(delayEnable, delayBox);
                            item.script = null;
                            item.onlyOnce = onlyOnce.Checked;
                            item.hideTooltip = hideTooltip.Checked;
                            break;
                        }
                    default:
                        break; // error is thrown at PortalTypeExtensions
                }

                if (portalImageList.SelectedItem != null && Program.InfoManager.PortalGame.ContainsKey(Program.InfoManager.PortalEditor_TypeById[ptComboBox.SelectedIndex]))
                {
                    item.image = (string)portalImageList.SelectedItem;
                }
            }
            Close();
        }

        private void thisMap_CheckedChanged(object sender, EventArgs e)
        {
            tmBox.Enabled = !thisMap.Checked;
            btnBrowseMap.Enabled = !thisMap.Checked;
            btnBrowseTn.Enabled = thisMap.Checked;
        }

        private void EnablingCheckBoxCheckChanged(object sender, EventArgs e)
        {
            ((Control)((CheckBox)sender).Tag).Enabled = ((CheckBox)sender).Checked;
        }

        private void ptComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnBrowseTn.Enabled = thisMap.Checked;

            PortalType portalType = Program.InfoManager.PortalEditor_TypeById[ptComboBox.SelectedIndex];
            switch (portalType)
            {
                case PortalType.StartPoint:
                    {
                        rowMan.SetInvisible("pn");
                        rowMan.SetInvisible("tm");
                        rowMan.SetInvisible("tn");
                        rowMan.SetInvisible("script");
                        rowMan.SetInvisible("delay");
                        rowMan.SetInvisible("range");
                        rowMan.SetInvisible("impact");
                        rowMan.SetInvisible("bool");
                        break;
                    }
                case PortalType.Invisible:
                    {
                        rowMan.SetVisible("pn");
                        rowMan.SetVisible("tm");
                        rowMan.SetVisible("tn");
                        rowMan.SetInvisible("script");
                        rowMan.SetVisible("delay");
                        rowMan.SetInvisible("range");
                        rowMan.SetInvisible("impact");
                        rowMan.SetVisible("bool");
                        break;
                    }
                case PortalType.Visible:
                    {
                        rowMan.SetVisible("pn");
                        rowMan.SetVisible("tm");
                        rowMan.SetVisible("tn");
                        rowMan.SetInvisible("script");
                        rowMan.SetVisible("delay");
                        rowMan.SetInvisible("range");
                        rowMan.SetInvisible("impact");
                        rowMan.SetVisible("bool");
                        break;
                    }
                case PortalType.Collision:
                    {
                        rowMan.SetVisible("pn");
                        rowMan.SetVisible("tm");
                        rowMan.SetVisible("tn");
                        rowMan.SetInvisible("script");
                        rowMan.SetVisible("delay");
                        rowMan.SetVisible("range");
                        rowMan.SetInvisible("impact");
                        rowMan.SetVisible("bool");
                        break;
                    }
                case PortalType.Changeable:
                    {
                        rowMan.SetVisible("pn");
                        rowMan.SetVisible("tm");
                        rowMan.SetVisible("tn");
                        rowMan.SetInvisible("script");
                        rowMan.SetVisible("delay");
                        rowMan.SetInvisible("range");
                        rowMan.SetInvisible("impact");
                        rowMan.SetVisible("bool");
                        break;
                    }
                case PortalType.ChangeableInvisible:
                    {
                        rowMan.SetVisible("pn");
                        rowMan.SetVisible("tm");
                        rowMan.SetVisible("tn");
                        //rowMan.SetInvisible("script");
                        rowMan.SetVisible("script"); // this could also be a script in early pre-bb MapleStory. i.e 1930000000, 1940000000, 1950000000, 1960000000 Internet Cafe maps
                        rowMan.SetVisible("delay");
                        rowMan.SetInvisible("range");
                        rowMan.SetInvisible("impact");
                        rowMan.SetVisible("bool");
                        break;
                    }
                case PortalType.TownPortalPoint:
                    {
                        rowMan.SetInvisible("pn");
                        rowMan.SetInvisible("tm");
                        rowMan.SetInvisible("tn");
                        rowMan.SetInvisible("script");
                        rowMan.SetInvisible("delay");
                        rowMan.SetInvisible("range");
                        rowMan.SetInvisible("impact");
                        rowMan.SetInvisible("bool");
                        break;
                    }
                case PortalType.Script:
                    {
                        rowMan.SetVisible("pn");
                        rowMan.SetInvisible("tm");
                        rowMan.SetInvisible("tn");
                        rowMan.SetVisible("script");
                        rowMan.SetVisible("delay");
                        rowMan.SetInvisible("range");
                        rowMan.SetInvisible("impact");
                        rowMan.SetVisible("bool"); // the portal image selector
                        break;
                    }
                case PortalType.ScriptInvisible:
                    {
                        rowMan.SetVisible("pn");
                        rowMan.SetInvisible("tm");
                        rowMan.SetInvisible("tn");
                        rowMan.SetVisible("script");
                        rowMan.SetVisible("delay");
                        rowMan.SetInvisible("range");
                        rowMan.SetInvisible("impact");
                        rowMan.SetVisible("bool"); // the portal image selector
                        break;
                    }
                case PortalType.CollisionScript:
                    {
                        rowMan.SetVisible("pn");
                        rowMan.SetInvisible("tm");
                        rowMan.SetInvisible("tn");
                        rowMan.SetVisible("script");
                        rowMan.SetVisible("delay");
                        rowMan.SetVisible("range");
                        rowMan.SetInvisible("impact");
                        rowMan.SetVisible("bool");
                        break;
                    }
                case PortalType.Hidden:
                    {
                        rowMan.SetVisible("pn");
                        rowMan.SetVisible("tm");
                        rowMan.SetVisible("tn");
                        rowMan.SetInvisible("script");
                        rowMan.SetVisible("delay");
                        rowMan.SetInvisible("range");
                        rowMan.SetInvisible("impact");
                        rowMan.SetVisible("bool");
                        break;
                    }
                case PortalType.ScriptHidden:
                    {
                        rowMan.SetVisible("pn");
                        rowMan.SetInvisible("tm");
                        rowMan.SetInvisible("tn");
                        rowMan.SetVisible("script");
                        rowMan.SetVisible("delay");
                        rowMan.SetInvisible("range");
                        rowMan.SetInvisible("impact");
                        rowMan.SetVisible("bool");
                        break;
                    }
                case PortalType.CollisionVerticalJump:
                    {
                        rowMan.SetVisible("pn");
                        rowMan.SetInvisible("tm");
                        rowMan.SetVisible("tn");
                        rowMan.SetInvisible("script");
                        rowMan.SetVisible("delay");
                        rowMan.SetInvisible("range");
                        rowMan.SetInvisible("impact");
                        rowMan.SetVisible("bool");
                        break;
                    }
                case PortalType.CollisionCustomImpact:
                    {
                        rowMan.SetVisible("pn");
                        rowMan.SetInvisible("tm");
                        rowMan.SetVisible("tn");
                        rowMan.SetInvisible("script");
                        rowMan.SetVisible("delay");
                        rowMan.SetInvisible("range");
                        rowMan.SetVisible("impact");
                        rowMan.SetVisible("bool");
                        break;
                    }
                case PortalType.CollisionUnknownPcig:
                    {
                        rowMan.SetVisible("pn");
                        rowMan.SetVisible("tm");
                        rowMan.SetVisible("tn");
                        rowMan.SetInvisible("script");
                        rowMan.SetVisible("delay");
                        rowMan.SetInvisible("range");
                        rowMan.SetVisible("impact");
                        rowMan.SetVisible("bool");
                        break;
                    }
            }
            leftBlankLabel.Visible = portalType == PortalType.CollisionVerticalJump;

            if (portalType == PortalType.CollisionVerticalJump)
            {
                btnBrowseTn.Enabled = true;
            }
            if (!Program.InfoManager.PortalGame.ContainsKey(portalType))
            {
                rowMan.SetInvisible("image");
            }
            else
            {
                portalImageList.Items.Clear();

                portalImageBox.Image = null;
                rowMan.SetVisible("image");

                // allows the user to select the different image template of the PortalType
                PortalGameImageInfo portalGameImgInfo = Program.InfoManager.PortalGame[portalType];
                IDictionaryEnumerator enumerator = portalGameImgInfo.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    string key = (string)enumerator.Key;
                    List<Bitmap> value = (List<Bitmap>)enumerator.Value;

                    portalImageList.Items.Add(key);
                }
                portalImageList.SelectedIndex = 0;
            }
        }

        private void portalImageList_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock (item.Board.ParentControl)
            {
                if (portalImageList.SelectedItem == null)
                    return;
                //else if ((string)portalImageList.SelectedItem == "default")
                //    return;

                else
                {
                    string portalName = (string)portalImageList.SelectedItem;
                    PortalType portalType = Program.InfoManager.PortalEditor_TypeById[ptComboBox.SelectedIndex];

                    if (Program.InfoManager.PortalGame.ContainsKey(portalType) && Program.InfoManager.PortalGame[portalType]?[portalName] != null && Program.InfoManager.PortalGame[portalType]?[portalName].Count > 0)
                    {
                        portalImageBox.Image = new Bitmap(Program.InfoManager.PortalGame[portalType][portalName][0]);
                    }
                    else
                    {
                        portalImageBox.Image = Resources.placeholder;
                    }
                }
            }
        }

        private void rangeEnable_CheckedChanged(object sender, EventArgs e)
        {
            hRangeBox.Enabled = rangeEnable.Checked;
            vRangeBox.Enabled = rangeEnable.Checked;
        }

        private void btnBrowseMap_Click(object sender, EventArgs e)
        {
            LoadMapSelector selector = new LoadMapSelector(tmBox);
            selector.ShowDialog();
        }

        private void btnBrowseTn_Click(object sender, EventArgs e)
        {
            string tn = TnSelector.Show(item.Board);
            if (tn != null) 
                tnBox.Text = tn;
        }
    }

    public class ControlRow
    {
        public Control[] controls;
        public bool invisible = false;
        public int rowSize;
        public string rowName;

        public ControlRow(Control[] controls, int rowSize, string rowName)
        {
            this.controls = controls;
            this.rowSize = rowSize;
            this.rowName = rowName;
        }
    }

    public class ControlRowManager
    {
        ControlRow[] rows;
        Hashtable names = new Hashtable();
        Form form;

        public ControlRowManager(ControlRow[] rows, Form form)
        {
            this.form = form;
            this.rows = rows;
            int index = 0;
            foreach (ControlRow row in rows)
                names[row.rowName] = index++;
        }

        public void SetInvisible(string name)
        {
            SetInvisible((int)names[name]);
        }

        public void SetInvisible(int index)
        {
            ControlRow row = rows[index];
            if (row.invisible) return;
            row.invisible = true;
            foreach (Control c in row.controls)
                c.Visible = false;
            int size = row.rowSize;
            for (int i = index + 1; i < rows.Length; i++)
            {
                row = rows[i];
                foreach (Control c in row.controls)
                    c.Location = new Point(c.Location.X, c.Location.Y - size);
            }
            form.Height -= size;
        }

        public void SetVisible(string name)
        {
            SetVisible((int)names[name]);
        }

        public void SetVisible(int index)
        {
            ControlRow row = rows[index];
            if (!row.invisible) return;
            row.invisible = false;
            foreach (Control c in row.controls)
                c.Visible = true;
            int size = row.rowSize;
            for (int i = index + 1; i < rows.Length; i++)
            {
                row = rows[i];
                foreach (Control c in row.controls)
                    c.Location = new Point(c.Location.X, c.Location.Y + size);
            }
            form.Height += size;
        }
    }
}
