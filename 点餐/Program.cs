using System;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace 点餐
{
    class Program
    {
        public static int tableCount = 10;
        public static string cfgpath = "./config.ini";
        [DllImport("kernel32")]
        public static extern long GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string value, string filePath);
       //程序入口点
        static void Main(string[] args)
        {
            Init();
            menu();
        }
        //主菜单
        public static void menu()
        {
            Console.Clear();
            ConsolePrint("选择用户类型", "Yellow", true);
            ConsolePrint("[1]", "Cyan", false);
            Console.WriteLine("食客");
            ConsolePrint("[2]", "Cyan", false);
            Console.WriteLine("厨师长");
            ConsolePrint("[3]", "Cyan", false);
            Console.WriteLine("饭店经理");
            ConsolePrint("[其他]", "Cyan", false);
            Console.WriteLine("退出");
            switch (Console.ReadLine())
            {
                case "1":
                    User_order();
                    break;
                case "2":
                    User_cook();
                    break;
                case "3":
                    User_manager();
                    break;
               default:
                    break;
            }
        }
        //初始化临时数据库配置文件
            public static void Init() {
            Console.Clear();
            if (!File.Exists(cfgpath)) {
                for (int i = 1; i <= tableCount; i++)
                {
                    WritePrivateProfileString("table", i.ToString(), "0", cfgpath);
                    WritePrivateProfileString("tableinfo", i.ToString(), "0", cfgpath);
                }
                WritePrivateProfileString("dishes", "count", "0", cfgpath);
                WritePrivateProfileString("cook", "account", "a1", cfgpath);
                WritePrivateProfileString("cook", "passwd", "a1", cfgpath);
                WritePrivateProfileString("manager", "account", "b1", cfgpath);
                WritePrivateProfileString("manager", "passwd", "b1", cfgpath);
                WritePrivateProfileString("card", "count", "0", cfgpath);
            }
        }
        //控制台彩色文本渲染组件
        public static void ConsolePrint(String text, String color,bool isline) {
            ConsoleColor colortoprint = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), color);
            Console.ForegroundColor = colortoprint;
            if (isline)
            {
                Console.WriteLine(text);
            }
            else {
                Console.Write(text);
            }
            Console.ResetColor();
        }
        //食客点餐开始
        public static void User_order() {
            Console.Clear();
            ConsolePrint("选择餐桌号:", "Yellow", true);
            for (int j = 1; j <= 10; j++)
            {
                StringBuilder sb = new StringBuilder(255);
                GetPrivateProfileString("table", j.ToString(),"0", sb, 255, cfgpath);
                if (sb.ToString() == "0")
                {
                    Console.Write("餐桌" + j.ToString() + " ");
                    ConsolePrint("空余", "Green", true);
                }
                else {
                    Console.Write("餐桌" + j.ToString() + " ");
                    ConsolePrint("已点餐", "Cyan", true);
                }
            }
            Console.WriteLine("======输入对应餐桌号选择 输入0返回======");
            string input = Console.ReadLine();
            if (input == "0")
            {
                //如果输入0 则返回主页面
                menu();
            }
            else {
                //点餐或查看状态
                looktable(input);
            }
        }
        //查看当前桌位的状态是否空余或已经点餐
        public static void looktable(string tableid) {
                StringBuilder sb = new StringBuilder(255);
                GetPrivateProfileString("table", tableid, "0", sb, 255, cfgpath);
                string isnull = sb.ToString();
            Console.Write("当前餐桌号为");
            ConsolePrint(tableid, "Yellow", true);
            if (isnull == "0")
            {
                Console.Write(" 欢迎光临，请输入您的用餐人数");
                string person = Console.ReadLine();
                if (person == "0") {
                    looktable(tableid);
                }
                order(tableid,person);
            }
            else {
                Console.Write("已点菜品:");
                StringBuilder sb2 = new StringBuilder(255);
                GetPrivateProfileString("tableinfo", tableid, "0", sb2, 255, cfgpath);
                string[] oded = sb2.ToString().Split(char.Parse(" "));
                for (int i = 1; i < oded.Length; i++) {
                    //蔡
                    StringBuilder sb3 = new StringBuilder(255);
                    GetPrivateProfileString("dishes", oded[i], "0", sb3, 255, cfgpath);
                    string[] dishinfo = sb3.ToString().Split(char.Parse(" "));
                    Console.WriteLine(dishinfo[0]);
                }
                ConsolePrint("输入e催菜 输入q返回", "Yellow", true);
                switch (Console.ReadLine()) {
                    case "q":
                        menu();
                        break;
                    case "e":
                        break;
                }
                menu();
            }
        }
        //菜品打印
        public static void printDishes() {
            StringBuilder sb = new StringBuilder(255);
            GetPrivateProfileString("dishes", "count", "0", sb, 255, cfgpath);
            int count = int.Parse(sb.ToString());
            for (int i = 0; i <= count - 1; i++)
            {
                StringBuilder sb3 = new StringBuilder(255);
                GetPrivateProfileString("dishes", i.ToString(), "0", sb3, 255, cfgpath);
                string dish = sb3.ToString();
                string[] dishinfo = dish.Split(char.Parse(" "));
                ConsolePrint("[" + i + "]", "Yellow", false);
                Console.Write("菜名:" + dishinfo[0]+" 单价:" + dishinfo[1]+ " 食材:");
                for (int k = 2; k <= dishinfo.Length - 1; k++)
                {
                    Console.Write(dishinfo[k]);
                    if (k != dishinfo.Length - 1)
                    {
                        Console.Write(",");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        //菜品搜索与下单
        public static void order(string tableid,string person) {
            Console.Clear();
            Console.WriteLine("菜单:");
            printDishes();
            Console.WriteLine();
            ConsolePrint("输入编号下单，输入s进入食材搜索，输入c完成选择，输入q取消订单", "Yellow", true);
            Console.WriteLine();
            Console.Write("当前已下单:");
            StringBuilder sb4 = new StringBuilder(255);
            GetPrivateProfileString("tableinfo", tableid, "0", sb4, 255, cfgpath);
            string[] tableordered = sb4.ToString().Split(char.Parse(" "));
            int priceall = 0;
            for (int j = 1; j < tableordered.Length; j++) {
                StringBuilder sb5 = new StringBuilder(255);
                GetPrivateProfileString("dishes", tableordered[j], "0", sb5, 255, cfgpath);
                string dish = sb5.ToString();
                string[] dishinfo = dish.Split(char.Parse(" "));
                ConsolePrint(dishinfo[0], "Yellow", false);
                if (j != tableordered.Length - 1) {
                    Console.Write(" ");
                }
                try
                {
                    priceall += int.Parse(dishinfo[1]);
                }
                catch (Exception)
                {
                    Console.WriteLine("参数错误");
                    WritePrivateProfileString("tableinfo", tableid, "0", cfgpath);
                    order(tableid, person);
                }
            }
            Console.WriteLine("共" + priceall + "元");
            string input = Console.ReadLine();
            switch (input) {
                case "c":
                    if (priceall == 0) {
                     order(tableid, person);
                    }
                    pay(tableid,priceall);
                    break;
                case "q":
                    WritePrivateProfileString("tableinfo", tableid, "0", cfgpath);
                    menu();
                    break;
                case "s":
                    ConsolePrint("请输入搜索的菜品名或食材 输入q返回", "Yellow", false);
                    string input2 = Console.ReadLine();
                    if (input2 == "q") {
                        order(tableid, person);
                    }
                    StringBuilder sb = new StringBuilder(255);
                    GetPrivateProfileString("dishes", "count", "0", sb, 255, cfgpath);
                    int count = int.Parse(sb.ToString());
                    Console.WriteLine("搜索结果:");
                    for (int i = 0; i < count; i++)
                    {
                        StringBuilder sb3 = new StringBuilder(255);
                        GetPrivateProfileString("dishes", i.ToString(), "0", sb3, 255, cfgpath);
                        string dish = sb3.ToString();
                        string[] dishinfo = dish.Split(char.Parse(" "));
                        for (int k = 0; k < dishinfo.Length; k++) {
                            if (dishinfo[k].Contains(input2)) {
                                ConsolePrint("[" + i + "]", "Yellow", false);
                                Console.WriteLine(dishinfo[0] + " 单价:" + dishinfo[1]);
                            }
                        }
                    }
                    ConsolePrint("输入编号下单，输入q返回", "Yellow", true);
                    string input3 = Console.ReadLine();
                    if (input3 == "q")
                    {
                        order(tableid, person);
                    }
                    else {
                        StringBuilder sb3 = new StringBuilder(255);
                        GetPrivateProfileString("tableinfo", tableid, "0", sb3, 255, cfgpath);
                        if (sb3.ToString() == "0")
                        {
                            WritePrivateProfileString("tableinfo", tableid, person + " " + input, cfgpath);
                        }
                        else
                        {
                            WritePrivateProfileString("tableinfo", tableid, sb3.ToString() + " " + input, cfgpath);
                        }
                        order(tableid, person);
                    }
                    break;
                default:
                    StringBuilder sb2 = new StringBuilder(255);
                    GetPrivateProfileString("tableinfo", tableid, "0", sb2, 255, cfgpath);
                    if (sb2.ToString() == "0")
                    {
                        WritePrivateProfileString("tableinfo", tableid, person + " " + input, cfgpath);
                    }
                    else {
                        WritePrivateProfileString("tableinfo", tableid, sb2.ToString()+" "+input, cfgpath);
                    }
                    order(tableid, person);
                    break;
            }
        }
        //结账
        public static void pay(string tableid,int price) {
            Console.WriteLine("您消费金额为{0}", price);
            Console.WriteLine("[0]我有本店会员积分卡");
            Console.WriteLine("[1]我没有积分卡，直接支付");
            Console.WriteLine("------------------");
            switch (Console.ReadLine()) {
                case "0":
                    Console.Write("请输入积分卡号");
                    string card = Console.ReadLine();
                    StringBuilder sb = new StringBuilder(255);
                    GetPrivateProfileString("card", "count", "0", sb, 255, cfgpath);
                    int count = int.Parse(sb.ToString());
                    int point = -1;
                    int cardnum = -1;
                    for(int i = 0; i < count; i++)
                    {
                        StringBuilder sb2 = new StringBuilder(255);
                        GetPrivateProfileString("card", i.ToString(), "-1", sb2, 255, cfgpath);
                        string[] cardinfo = sb2.ToString().Split(char.Parse(" "));
                        if (sb2.ToString() != "-1" && cardinfo[0] == card) {
                            point = int.Parse(cardinfo[1]);
                            cardnum = i;
                            break;
                        }
                    }
                    if (point== -1)
                    {
                        Console.WriteLine("该卡不存在或已注销");
                    }
                    else {
                        int rawpoint = point;
                        if (price < point) {
                            rawpoint = price;
                        }
                        Console.WriteLine("卡上当前积分{0} 可减免此餐{1}元", point, rawpoint);
                        int getpoint = price / 10;
                        Console.WriteLine("本次餐饮可获得{0}积分，下次用餐时可使用", getpoint);
                        Console.WriteLine("--------------------");
                        Console.WriteLine("输入y支付 输入q取消");
                        string input = Console.ReadLine();
                        switch (input) {
                            case "q":
                                WritePrivateProfileString("tableinfo", tableid, "0", cfgpath);
                                WritePrivateProfileString("table", tableid, "0", cfgpath);
                                menu();
                                break;
                            case "y":
                                string pointw = (point - rawpoint + getpoint).ToString();
                                WritePrivateProfileString("card", cardnum.ToString(), card +" "+pointw, cfgpath);
                                Console.WriteLine("感谢您的消费，您可以选择桌号查看上菜详情 当前桌号{0} 卡上余额{1}", tableid,pointw);
                                Console.WriteLine("按任意键返回主菜单");
                                WritePrivateProfileString("table", tableid, "1", cfgpath);
                                Console.ReadLine();
                                menu();
                                break;
                        }
                    }
                    break;
                case "1":
                    Console.WriteLine("感谢您的消费，您可以选择桌号查看上菜详情 当前桌号{0}",tableid);
                    Console.WriteLine("按任意键返回主菜单");
                    WritePrivateProfileString("table", tableid, "1", cfgpath);
                    Console.ReadLine();
                    menu();
                    break;
            }
        }
        //厨师长后台的方法
        public static void User_cook()
        {
            Console.Clear();
            Console.WriteLine("请输入账号");
            StringBuilder sb = new StringBuilder(255);
            GetPrivateProfileString("cook", "account", "0", sb, 255, cfgpath);
            if (sb.ToString() != Console.ReadLine())
            {
                ConsolePrint("输入错误，禁止访问", "Red", true);
            }
            else
            {
                Console.WriteLine("请输入密码");
                StringBuilder sb2 = new StringBuilder(255);
                GetPrivateProfileString("cook", "passwd", "0", sb2, 255, cfgpath);
                if (sb2.ToString() != Console.ReadLine())
                {
                    ConsolePrint("输入错误，禁止访问", "Red", true);
                }
                else
                {
                    Console.Clear();
                    cookpanel();
                }
            }
            menu();
        }
        //厨师长的后台界面
        public static void cookpanel() {
            for (int j = 1; j <= 10; j++)
            {
                StringBuilder sb = new StringBuilder(255);
                GetPrivateProfileString("table", j.ToString(), "0", sb, 255, cfgpath);
                if (sb.ToString() == "0")
                {
                    Console.Write("餐桌" + j.ToString() + " ");
                    ConsolePrint("空余", "Green", true);
                }
                else
                {
                    Console.Write("餐桌" + j.ToString() + " ");
                    ConsolePrint("已点餐", "Cyan", true);
                    StringBuilder sb2 = new StringBuilder(255);
                    GetPrivateProfileString("tableinfo", j.ToString(), "0", sb2, 255, cfgpath);
                    string[] oded = sb2.ToString().Split(char.Parse(" "));
                    for (int i = 1; i < oded.Length; i++)
                    {
                        //蔡
                        StringBuilder sb3 = new StringBuilder(255);
                        GetPrivateProfileString("dishes", oded[i], "0", sb3, 255, cfgpath);
                        string[] dishinfo = sb3.ToString().Split(char.Parse(" "));
                        Console.WriteLine(dishinfo[0]);
                    }
                   
                }
            }
            Console.WriteLine("输入对应餐桌上菜 输入r刷新 输入q返回");
            string input = Console.ReadLine();
            if (input == "q")
            {
                menu();
            }
            else if (input == "r")
            {
                cookpanel();
            }
            else {
                Console.WriteLine("{0}餐桌上菜完毕",input);
                WritePrivateProfileString("tableinfo", input, "0", cfgpath);
                WritePrivateProfileString("table", input, "0", cfgpath);
                cookpanel();
            }
        }
        //饭店经理管理的方法
        public static void User_manager()
        {
            Console.Clear();
            Console.WriteLine("请输入账号");
            StringBuilder sb = new StringBuilder(255);
            GetPrivateProfileString("manager", "account", "0", sb, 255, cfgpath);
            if (sb.ToString() != Console.ReadLine())
            {
                ConsolePrint("输入错误，禁止访问", "Red", true);
            }
            else {
                Console.WriteLine("请输入密码");
                StringBuilder sb2 = new StringBuilder(255);
                GetPrivateProfileString("manager", "passwd", "0", sb2, 255, cfgpath);
                if (sb2.ToString() != Console.ReadLine())
                {
                    ConsolePrint("输入错误，禁止访问", "Red", true);
                }
                else {
                    Console.Clear();
                    Console.WriteLine("[0]菜品编辑 [1]积分卡编辑");
                    string input = Console.ReadLine();
                    switch (input) { case "0":dishesedit();break; case "1":cardedit();break; }
                }
            }
            menu();
        }
        //积分卡编辑器
        public static void cardedit() {
            Console.Clear();
            StringBuilder sb = new StringBuilder(255);
            GetPrivateProfileString("card", "count", "0", sb, 255, cfgpath);
            int count = int.Parse(sb.ToString());
            if (count==0)
            {
                Console.WriteLine("当前积分卡列表为空!");
            }
            else {
                for (int i = 0; i < count; i++)
                {
                    StringBuilder sb2 = new StringBuilder(255);
                    GetPrivateProfileString("card", i.ToString(), "0", sb2, 255, cfgpath);
                    Console.WriteLine("[" + i + "]" + sb2.ToString());
                }
            }
            ConsolePrint("[n]开卡 [c]修改卡 [q]返回主菜单", "Yellow", true);
            switch (Console.ReadLine()) {
                case "q":
                    menu();
                    break;
                case "n":
                    Console.WriteLine("输入新卡号:");
                    string cardaccount = Console.ReadLine();
                    Console.WriteLine("输入初始积分:");
                    string point = Console.ReadLine();
                    Console.WriteLine("!!!-确定开卡吗? (y/n)");
                    if (Console.ReadLine() == "y")
                    {
                        WritePrivateProfileString("card",count.ToString(), cardaccount+" "+point, cfgpath);
                        WritePrivateProfileString("card", "count", (count + 1).ToString(), cfgpath);
                    }
                        cardedit();
                    break;
                case "c":
                    Console.WriteLine("输入要修改的卡的编号");
                    string input = Console.ReadLine();
                    StringBuilder sb2 = new StringBuilder(255);
                    GetPrivateProfileString("card",input, "0", sb2, 255, cfgpath);
                    string[] cardinfo = sb2.ToString().Split(char.Parse(" "));
                    Console.Write("当前卡号:{0} 积分:{1}",cardinfo[0],cardinfo[1]);
                    Console.WriteLine("输入新卡号:");
                    string cardac2 = Console.ReadLine();
                    Console.WriteLine("输入积分:");
                    string point2= Console.ReadLine();
                    Console.WriteLine("!!!-确定修改吗? (y/n)");
                    if (Console.ReadLine() == "y")
                    {
                        WritePrivateProfileString("card", input, cardac2 + " " + point2, cfgpath);
                    }
                        cardedit();
                    break;
            }
        }
        //菜品编辑器
        public static void dishesedit()
        {
            Console.Clear();
            StringBuilder sb = new StringBuilder(255);
            GetPrivateProfileString("dishes", "count", "0", sb, 255, cfgpath);
            int count = int.Parse(sb.ToString());
            Console.WriteLine("当前菜品总览:");
            if (count == 0)
            {
                Console.WriteLine("当前菜单为空!");
            }
            else
            {
                for (int i = 0; i <= count - 1; i++)
                {
                    StringBuilder sb2 = new StringBuilder(255);
                    GetPrivateProfileString("dishes", i.ToString(), "0", sb2, 255, cfgpath);
                    Console.WriteLine("[" + i + "]" + sb2.ToString());
                }
            }
            Console.WriteLine("输入add增加菜品 输入菜品号修改菜品 输入q返回");
            string input = Console.ReadLine();
            if (input == "add")
            {
                Console.WriteLine("请输入菜品名称");
                string name = Console.ReadLine();
                Console.WriteLine("请输入单价");
                string price = Console.ReadLine();
                Console.WriteLine("请输入本菜用到的食材 用空格分开");
                string food = Console.ReadLine();
                Console.WriteLine("----------");
                Console.WriteLine("菜名:" + name);
                Console.WriteLine("单价:" + price);
                Console.WriteLine("食材:" + food);
                Console.WriteLine("!!!-确定添加该菜品吗？(y/n)");
                if (Console.ReadLine() == "y")
                {
                    WritePrivateProfileString("dishes", count.ToString(), name + " " + price + " " + food, cfgpath);
                    WritePrivateProfileString("dishes", "count", (count + 1).ToString(), cfgpath);
                }
                    dishesedit();
            }
            else if (input == "q")
            {
                menu();
            }
            else
            {
                StringBuilder sb3 = new StringBuilder(255);
                GetPrivateProfileString("dishes", input, "0", sb3, 255, cfgpath);
                string dish = sb3.ToString();
                string[] dishinfo = dish.Split(char.Parse(" "));
                Console.WriteLine("菜名:" + dishinfo[0]);
                Console.WriteLine("单价:" + dishinfo[1]);
                Console.Write("食材:");
                for (int i = 2; i <= dishinfo.Length - 1; i++)
                {
                    Console.Write(dishinfo[i]);
                    if (i != dishinfo.Length - 1)
                    {
                        Console.Write(",");
                    }
                }
                Console.WriteLine();
                Console.WriteLine("!!!-确定修改当前菜品吗? (y/n)");
                if (Console.ReadLine() == "y")
                {
                    Console.WriteLine("请输入菜品名称");
                    string name = Console.ReadLine();
                    Console.WriteLine("请输入单价");
                    string price = Console.ReadLine();
                    Console.WriteLine("请输入本菜用到的食材 用空格分开");
                    string food = Console.ReadLine();
                    Console.WriteLine("----------");
                    Console.WriteLine("菜名:" + name);
                    Console.WriteLine("单价:" + price);
                    Console.WriteLine("食材:" + food);
                    Console.WriteLine("!!!-确定修改该菜品吗？(y/n)");
                    if (Console.ReadLine() == "y")
                    {
                        WritePrivateProfileString("dishes", input, name + " " + price + " " + food, cfgpath);
                    }
                }
                dishesedit();
            }
            dishesedit();
        }
    }
}
