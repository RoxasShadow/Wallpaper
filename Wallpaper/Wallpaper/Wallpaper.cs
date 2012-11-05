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
using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Wallpaper
{
    public class Wallpaper
    {
        public static string download(string url)
        {
            return new WebClient().DownloadString(url);
        }

        public static void save(string url, string path)
        {
            new WebClient().DownloadFile(url, path);
        }

        public static string base64Decode(string data)
        {
            Decoder utf8Decode      = new UTF8Encoding().GetDecoder();

            byte[]  todecode_byte   = Convert.FromBase64String(data);
            int     charCount       = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[]  decoded_char    = new char[charCount];

            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);

            return new String(decoded_char);
        }

        public static string getImage(string url)
        {
            string                  data        =   download(url);
            HtmlDocument            document    =   new HtmlDocument();
                                                    document.LoadHtml(data);
            IEnumerable<HtmlNode>   collection  =   document.DocumentNode.Descendants("script");

            bool skip = true;
            foreach (HtmlNode link in collection)
            {
                if (skip)
                    skip = false;
                else
                {
                    string a = link.InnerText.Split(new string[] { "src=\"'+B('"  }, StringSplitOptions.None)[1];
                    string b = a.Split(             new string[] { "')+'\" />');" }, StringSplitOptions.None)[0];
                    return  base64Decode(b);
                }
            }

            return "";
        }

        public static List<Hashtable> get(string url)
        {
            List<Hashtable>     wallpapers         = new List<Hashtable>();
            string              data               = download(url);
            HtmlDocument        document           = new HtmlDocument();
            document.LoadHtml(data);
            HtmlNodeCollection  collection         = document.DocumentNode.SelectNodes("//a[@class='thdraggable thlink']");

            if (collection == null)
                return null;

            Hashtable wallpaper;
            string image, thumb;
            foreach (HtmlNode link in collection)
            {
                if (!link.Attributes.Contains("href"))
                    continue;

                if (link.SelectNodes("img").Count != 1)
                    continue;

                wallpaper   = new Hashtable();

                image       = getImage(link.Attributes["href"].Value);
                if(image == "")
                    continue;
                wallpaper.Add("image", image);

                thumb       = link.SelectNodes("img")[0].Attributes["src"].Value;
                if (!thumb.StartsWith("http://wallbase"))
                    continue;
                wallpaper.Add("thumb", thumb);

                wallpapers.Add(wallpaper);
            }

            return wallpapers;
        }

        public static List<Hashtable> getRandom(int width, int height, int purity)
        {
            return get("http://wallbase.cc/random/12/eqeq/" + width + 'x' + height + "/0/" + purity + "/20");
        }
    }
}
