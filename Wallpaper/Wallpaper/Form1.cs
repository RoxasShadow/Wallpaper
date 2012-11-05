/*
 * Copyright (C) 2012 by Giovanni Capuano <webmaster@giovannicapuano.net>
 *
 * Wallpaper is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Wallpaper is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Wallpaper.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Wallpaper
{
    public partial class Form1 : Form
    {
        private List<PictureBox>    images;
        private PictureBox          selected;

        public Form1()
        {
            InitializeComponent();
            images            =  new List<PictureBox>();
            this.KeyPress     += new KeyPressEventHandler(random_Click);
            this.width.Value  =  Screen.PrimaryScreen.Bounds.Width;
            this.height.Value =  Screen.PrimaryScreen.Bounds.Height;
        }

        private void clear()
        {
            foreach (Control image in images)
            {
                this.Controls.Remove(image);
                image.Dispose();
            }
        }

        private void getRandom(int n, int width, int height, int purity)
        {
            List<Hashtable> urls    = Wallpaper.getRandom(width, height, purity);
            int             count   =   0;

            if (urls == null || urls.Count == 0)
                getRandom(n, width, height, purity);

            foreach (Hashtable hash in urls)
            {
                int x;

                if (count == 0 || count % 3 == 0)
                    x = this.width.Location.X;
                else
                    x                       =   count == 0 ? this.width.Location.X + 10                     : images[count - 1].Location.X + 260;               
                int y                       =   count == 0 ? this.width.Location.Y + this.width.Height + 20 : images[count - 1].Location.Y + (count % 3 == 0 ? 198 : 0);

                string image                =   (string) hash["image"];
                string thumb                =   (string) hash["thumb"];

                PictureBox wallpaper        =   new PictureBox();
                wallpaper.Name              =   image;
                wallpaper.Location          =   new Point(x, y);
                wallpaper.Size              =   new Size(250, 188);
                wallpaper.BackColor         =   Color.White;
                wallpaper.Visible           =   true;
                wallpaper.ImageLocation     =   thumb;
                wallpaper.Click             +=  new EventHandler(setBackground);
                wallpaper.MouseDown         +=  new MouseEventHandler(wallpaperFocus);
                wallpaper.ContextMenuStrip  = contextMenuStrip1;

                images.Add(wallpaper);
                this.Controls.Add(wallpaper);

                ++count;
                if (count == n)
                    break;
            }
        }

        private void setBackground(object sender, EventArgs e)
        {
            this.Cursor =   Cursors.WaitCursor;

            Uri url     =   new Uri((((PictureBox)sender).Name));
            Background.Set(url, Background.Style.Fill);

            this.Cursor =   Cursors.Default;
        }

        private void random_Click(object sender, EventArgs e)
        {
            this.Size   = new Size(820, 510);
            this.Cursor = Cursors.WaitCursor;

            int width   = (int) this.width.Value;
            int height  = (int) this.height.Value;
            int purity  =       this.purity.SelectedText == "SFW" ? 100 : 110;
            int n       = (int) this.thumbs.Value;

            clear();
            getRandom(n, width, height, purity);

            this.Cursor =       Cursors.Default;
        }

        private void wallpaperFocus(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
                selected = (PictureBox)sender;
        }

        private void openWebsite_Click(object sender, EventArgs e)
        {
            string url = "http://wallbase.cc/wallpaper/" + selected.Name.Split(new string[] { "wallpaper-" }, StringSplitOptions.None)[1].Split('.')[0];
            Process.Start(url);
        }

        private void openImage_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            string path = Path.Combine(Path.GetTempPath(), "_wallpaper.jpg");
            Wallpaper.save(selected.Name, path);
            Process.Start(path);

            this.Cursor = Cursors.Default;
        }
    }
}