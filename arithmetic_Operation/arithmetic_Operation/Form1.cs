using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Resources;
using System.Threading;
using System.Globalization;

namespace arithmetic_Operation
{
    public partial class Form1 : Form
    {
        private int t; //用于获取秒数
        private int wrong_Number; //错误题数
        private int nums_wrong; //当前错题数
        private int correct_Number; //正确题数
        private int problem_Nums; //总题数
        private int problem_Now;
        private int score = 100; //题目得分
        private string rightAns; //四则运算结果
        private ResourceManager rm; //获取资源文件内容
        //文件路径（默认放在bin目录下）
        private string path="data.txt";
        private static string expression = null;
        private static Fraction[] opNum = new Fraction[20];  
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 程序运行时加载窗体
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            //初始化对错数量
            this.init_Numbers();
            label11.Text = correct_Number.ToString();
            label12.Text = wrong_Number.ToString();
            //初始化时间
            this.timer1.Enabled = false;
            this.timer1.Interval = 1000;//代表每秒改变一次时间
            //初始化按钮
            label8.Text = "";
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
        }
        /// <summary>
        /// 点击开始按钮，产生一定数量的题目并开始计时
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            //计时器清零，并开始计时
            t = 0;
            this.label10.Text = "00:00:00";
            this.timer1.Enabled = true;

            button1.Enabled = false;
            button3.Enabled = true;
            button2.Enabled = false;
            //获取总题数
            problem_Nums = int.Parse(textBox1.Text.ToString());
            problem_Now = 1;
            score = 100;
            //初始化运算式
            expression = "";
            nums_wrong = 0;
            //计算四则运算式的正确结果 
            rightAns = new ExpressionHelper().getResOfPostfix();
            //显示第一道题目
            label5.Text = "题目" + problem_Now.ToString() + ":";
            label8.Text = expression;
        }

        /// <summary>
        /// 点击提交按钮，计算正确答案并批改
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            //获取用户输入
            string userAns = textBox2.Text.ToString();
            //如果输入不是数字,要求重输
            if (!isNumberic(userAns))
            {
                MessageBox.Show(rm.GetString("message1"), rm.GetString("message2"),
MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox2.Text = "";
                return;
            }
            button3.Enabled = false;
            button4.Enabled = true;
            //如果已经是最后一题
            if (problem_Now == problem_Nums)
            {
                button4.Enabled = false;
                button2.Enabled = true;
            } 
            //判断对错,并改变对错题数量 
            if (userAns == rightAns){
                label9.Text = rm.GetString("result1");
                this.correct_Number++; //正确题数+1
                label11.Text = correct_Number.ToString();
            }               
            else
            {
                label9.Text = rm.GetString("result2") + rightAns;
                this.wrong_Number++; //错误题数+1
                label12.Text = wrong_Number.ToString();
                //当前错题数+1
                nums_wrong += 1;
            }
        }
        /// <summary>
        /// 点击下一题
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            button4.Enabled = false;
            button3.Enabled = true;
            expression = "";
            //当前题数+1
            problem_Now += 1;
            //计算四则运算式的正确结果 
            rightAns = new ExpressionHelper().getResOfPostfix();
            //显示第N道题目
            label5.Text = "题目" + problem_Now.ToString() + ":";
            label8.Text = expression;
            textBox2.Text = "";
        }
        /// <summary>
        /// 结束答题，计算总得分
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {          
            this.timer1.Enabled = false;//停止计时
            button1.Enabled = true;
            button2.Enabled = false;
            label7.Text = "";
            score = 100 * (problem_Nums - nums_wrong) / problem_Nums;
            label9.Text = "本次得分为：" + score.ToString();
        }
        /// <summary>
        /// 时钟控件事件，动态改变时间
        /// </summary>
        private void timer1_Tick(object sender, EventArgs e)
        {
            t = t + 1;
            this.label10.Text = GetAllTime(t);
        }
        /// <summary>
        /// 关闭当前窗体时保存对错数量
        /// </summary>
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            StreamWriter sw = new StreamWriter(path, false);
            string text = correct_Number.ToString() + "\r\n" + wrong_Number.ToString();
            sw.Write(text);
            sw.Close();
        }
        /// <summary>
        /// 通过正则表达式判断输入是否为数字
        /// </summary>
        protected bool isNumberic(string message)
        {
            System.Text.RegularExpressions.Regex rex =
            new System.Text.RegularExpressions.Regex(@"^(-?[0-9]+[/]?[0-9]*)$");
            if (rex.IsMatch(message))
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 从文件中获取对错数量
        /// </summary>
        private void init_Numbers()
        {
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(path, Encoding.Default);
                this.correct_Number = int.Parse(sr.ReadLine());
                this.wrong_Number = int.Parse(sr.ReadLine());
            }
            catch
            {
                MessageBox.Show("文件打不开或者文件不存在", "文件打开错误",
MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sr.Close();
            }
        }
        /// <summary>
        /// 计时函数,将秒数转成hh:mm:ss的时间格式
        /// </summary>
        private string GetAllTime(int time)
        {
            string hh, mm, ss;

            int s = time % 60; // 转化为秒
            int m = (time / 60) % 60;     // 分
            int h = (time / 3600) % 60;     // 时

            //秒格式00
            if (s < 10)
            {
                ss = "0" + s.ToString();
            }
            else
            {
                ss = s.ToString();
            }

            //分格式00
            if (m < 10)
            {
                mm = "0" + m.ToString();
            }
            else
            {
                mm = m.ToString();
            }

            //时格式00
            if (h < 10)
            {
                hh = "0" + h.ToString();
            }
            else
            {
                hh = h.ToString();
            }

            //返回 hh:mm:ss.ff            
            return hh + ":" + mm + ":" + ss;
        }

        /// <summary>
        /// 随机四则运算式的生成和计算类
        /// </summary>
        public class ExpressionHelper
        {
            Random ra = new Random(unchecked((int)DateTime.Now.Ticks));
            string[] ss = new string[] { "+", "-", "x", "%" }; //操作符


            //得到后缀表达式
            private string[] getPostfix()
            {
                int t = 0, q = 0, p = 0, top = 0;
                string[] expQueue = new string[100];
                string[] operatorStack = new string[200];
                string op;
                opNum[q++] = getNum();
                expQueue[p++] = (q - 1).ToString();
                //加入附加功能，运算符个数随机生成
                int op_Num = ra.Next() % 5 + 1;
                //附加功能，控制括号的生成
                int braket_Max = 2, braket = 0, flag = 0;//分别代表生成括号的个数和当前左括号个数 
                //生成后缀表达式 
                for (t = 0; t < op_Num; t++)
                {
                    op = getOperator();  //生成运算符  
                    if (t == 0)
                    {
                        operatorStack[top++] = op;
                        //随机决定是否生成左括号
                        if (ra.Next() % 3 == 1 && t < op_Num - 1)
                        {
                            expression += "(";//打印括号 
                            braket_Max--;
                            braket++;
                            operatorStack[top++] = "("; //左括号入栈 
                        }
                        opNum[q++] = getNum();
                        expQueue[p++] = (q - 1).ToString();
                        continue;
                    }

                    //当符号栈顶不是左括号，根据优先级判断出栈 
                    if (operatorStack[top - 1] != "(")
                    {
                        while (mPriority(op) <= mPriority(operatorStack[top - 1]) && operatorStack[top - 1] != "(")
                        {
                            top--;
                            expQueue[p++] = operatorStack[top];
                            if (top == 0) break;
                        }
                    }
                    operatorStack[top++] = op;
                    //随机决定是否生成左括号
                    if (ra.Next() % 3 == 1 && t < op_Num - 1)
                    {
                        if (braket_Max < 0) break;//如果已经生成三对括号，就不再生成 
                        expression += "(";//打印括号 
                        flag = t;
                        braket_Max--;
                        braket++;
                        operatorStack[top++] = "("; //左括号入栈 
                    }
                    opNum[q++] = getNum();//产生一个随机数 
                    expQueue[p++] = (q - 1).ToString();
                    //随机决定是否生成右括号
                    if (flag != t && ra.Next() % 3 == 1)
                    {

                        if (braket <= 0) break;
                        expression += ")";//打印右括号
                        braket--;
                        //一直出栈直到遇到左括号
                        while (operatorStack[top - 1] != "(")
                        {
                            top--;
                            expQueue[p++] = operatorStack[top];
                        }
                        top--;
                    }
                }
                //如果还有左括号还未匹配
                while (braket > 0)
                {
                    braket--;
                    expression += ")";
                    while (operatorStack[top - 1] != "(")
                    {
                        top--;
                        expQueue[p++] = operatorStack[top];
                    }
                    top--;
                }
                while (top > 0)
                {
                    top--;
                    expQueue[p++] = operatorStack[top];
                }
                return expQueue;
            }
            //根据后缀表达式计算结果 
            public string getResOfPostfix()
            {
                string[] expQueue = getPostfix();
                Fraction[] numStack = new Fraction[10];
                string rightAns;
                int top = 0;
                string expQue = string.Join("", expQueue);
                for (int i = 0; i < expQue.Length; i++)
                {
                    //Console.WriteLine("{0}",expQue[i]);
                    if (expQue[i] >= '0' && expQue[i] <= '9')
                    {
                        int k = int.Parse(expQue[i].ToString());
                        numStack[top].Element = opNum[k].Element;
                        numStack[top].Denominator = opNum[k].Denominator;
                        top++;
                    }
                    else
                    {
                        top--;
                        numStack[top - 1] = Calculate(numStack[top - 1], numStack[top], expQue[i].ToString());
                    }
                }
                //expression += "=";
                //得到的正确结果并转成字符串 
                if (numStack[top - 1].Denominator != 1)
                {
                    rightAns = numStack[top - 1].Element + "/" + numStack[top - 1].Denominator;
                }
                else
                    rightAns = numStack[top - 1].Element.ToString();
                return rightAns;
            }
            //随机生成一个操作符并打印
            private string getOperator()
            {
                string op = ss[ra.Next() % 4];
                if (op == "%")
                    expression += "÷";
                else if (op == "x")
                    expression += "×";
                else expression += op;
                return op;
            }
            //随机生成1个操作数并打印 
            private Fraction getNum()
            {
                int i, j, f;
                Fraction res = new Fraction();
                if (ra.Next() % 3 == 1)
                {
                    i = ra.Next() % 11 + 1;
                    j = ra.Next() % 11 + 1;
                    if (i > j) { int temp = i; i = j; j = temp; }
                    f = gcd(j, i);
                    i = i / f; j = j / f;
                    expression += i.ToString() + "/" + j.ToString();
                }
                else
                {
                    i = ra.Next() % 11 + 1;
                    j = 1;
                    expression += i.ToString();
                }
                res.Element = i;
                res.Denominator = j;
                return res;
            }
            //根据操作符计算两个操作数
            private Fraction Calculate(Fraction num1, Fraction num2, string op)
            {
                Fraction res = new Fraction();
                int f = 1;
                switch (op)
                {
                    case "+":
                        {
                            res.Element = num1.Element * num2.Denominator + num1.Denominator * num2.Element;
                            res.Denominator = num1.Denominator * num2.Denominator;
                            break;
                        }
                    case "-":
                        {
                            res.Element = num1.Element * num2.Denominator - num1.Denominator * num2.Element;
                            res.Denominator = num1.Denominator * num2.Denominator;
                            break;
                        }
                    case "x":
                        {
                            res.Element = num1.Element * num2.Element;
                            res.Denominator = num1.Denominator * num2.Denominator;
                            break;
                        }
                    case "%":
                        {
                            res.Element = num1.Element * num2.Denominator;
                            res.Denominator = num1.Denominator * num2.Element;
                            break;
                        }

                }
                if (res.Element < res.Denominator)
                    f = gcd(res.Denominator, res.Element);
                else
                    f = gcd(res.Element, res.Denominator);
                if (f != 0)
                {
                    res.Element = res.Element / f;
                    res.Denominator = res.Denominator / f;
                }

                //如果分母为负数，取反
                if (res.Denominator < 0)
                {
                    res.Element = -res.Element;
                    res.Denominator = -res.Denominator;
                }
                return res;
            }
            //求最大公约数
            private int gcd(int a, int b)
            {
                if (b == 0) return a;
                else return gcd(b, a % b);
            }
            //返回操作符的优先级
            private int mPriority(string op)
            {
                if (op == "+") return 1;
                else if (op == "-") return 1;
                else if (op == "x") return 2;
                else if (op == "%") return 2;
                return 0;
            }
        }
        /// <summary>
        /// 分数类
        /// </summary>
        public struct Fraction
        {
            private int x;
            private int y;
            //分母
            public int Denominator
            {
                get
                {
                    return y;
                }
                set
                {
                    y = value;
                }
            }
            //分子
            public int Element
            {
                get
                {
                    return x;
                }
                set
                {
                    x = value;
                }
            }
        }

        private void 英文ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            UpDataMainFormUILanguage();
        }

        //根据当前的语言区域，更新主窗口的语言信息
        private void UpDataMainFormUILanguage()
        {
            rm = new ResourceManager(typeof(Form1));
            UpDataForm1(rm);
        }
        //根据当前的语言区域，更新主窗口的语言
        private void UpDataForm1(ResourceManager rm)
        {
            //菜单项
            文件FToolStripMenuItem.Text = rm.GetString("start");
            查看历史记录ToolStripMenuItem.Text = rm.GetString("history");
            语言LToolStripMenuItem.Text = rm.GetString("language");
            中文简体ToolStripMenuItem.Text = rm.GetString("zh-CHS");
            中文繁体ToolStripMenuItem.Text = rm.GetString("zh-CHT");
            英文ToolStripMenuItem.Text = rm.GetString("English");
            帮助HToolStripMenuItem.Text = rm.GetString("help");
            //按钮以及标签
            label1.Text = rm.GetString("numbers");
            label2.Text = rm.GetString("time_use");
            label3.Text = rm.GetString("correct");
            label4.Text = rm.GetString("wrong");
            label5.Text = rm.GetString("question");
            button3.Text = rm.GetString("submit");
            button4.Text = rm.GetString("next");
            button1.Text = rm.GetString("begin");
            button2.Text = rm.GetString("end");
        }

        private void 中文简体ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CHS");
            UpDataMainFormUILanguage();
        }

        private void 中文繁体ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CHT");
            UpDataMainFormUILanguage();
        }
        //将对错数量清零
        private void button5_Click(object sender, EventArgs e)
        {
            correct_Number = 0;
            wrong_Number = 0;
            //重置
            StreamWriter sw = new StreamWriter(path, false);
            string text = correct_Number.ToString() + "\r\n" + wrong_Number.ToString();
            sw.Write(text);
            sw.Close();
            label11.Text = correct_Number.ToString();
            label12.Text = wrong_Number.ToString();
        }


    }
}
