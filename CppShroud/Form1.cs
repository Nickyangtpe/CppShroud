using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CppShroud
{
    public partial class Form1 : Form
    {
        private static Random random = new Random();
        private static StringBuilder defines = new StringBuilder();
        private static Dictionary<string, string> replacementDict = new Dictionary<string, string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            defines.Clear(); // 每次點擊按鈕時清空定義
            replacementDict.Clear(); // 清空替換字典
            string input = textBox1.Text;
            string result = ReplaceStrings(input);
            textBox2.Text = defines.ToString() + result;
        }

        static string ReplaceStrings(string input)
        {
            // 使用正則表達式來處理每一行
            string[] lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                if (!lines[i].TrimStart().StartsWith("#") && !lines[i].Contains("//"))
                {
                    lines[i] = Regex.Replace(lines[i], "\"(?:\\\\\"|[^\"])*\"|\\S+", new MatchEvaluator(match =>
                    {
                        string originalWord = match.Value;
                        // 檢查是否在引號中
                        if (originalWord.StartsWith("\"") && originalWord.EndsWith("\""))
                        {
                            return originalWord;
                        }

                        string replacement;
                        if (replacementDict.TryGetValue(originalWord, out replacement))
                        {
                            // 如果已經有替換的字，則直接返回
                            return " " + replacement;
                        }
                        else
                        {
                            // 否則，生成一個新的替換的字
                            int length = random.Next(5, 16); // 長度在5到15之間
                            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                            StringBuilder result = new StringBuilder(length);

                            // 確保第一個字符不是數字
                            result.Append(chars[random.Next(52)]); // 從字母開始
                            for (int j = 1; j < length; j++)
                            {
                                result.Append(chars[random.Next(chars.Length)]);
                            }

                            // 添加 #define 替換的字 被替換的字
                            defines.AppendLine($"#define {result} {originalWord} ");
                            replacementDict[originalWord] = result.ToString(); // 將替換的字添加到字典中

                            return " " + result;
                        }
                    }));
                }
            }
            return string.Join(Environment.NewLine, lines);
        }
    }
}
