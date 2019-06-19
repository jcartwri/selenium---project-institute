using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Threading;
using System.Text.RegularExpressions;

namespace WindowsFormsApp1
{
	public partial class Form1 : Form
	{
		static Mutex mutext = new Mutex(false, @"Global\mutext");
		static Mutex mufeed = new Mutex(false, @"Global\mufeed");
		static Mutex mupic = new Mutex(false, @"Global\mupic");

		//static Mutex mutext = new Mutex();
		//static Mutex mufeed = new Mutex();
		//static Mutex mupic = new Mutex();

		public Form1()
		{
			InitializeComponent();
		}

		[DataContract]
		public class Id_Text
		{
			[DataMember]
			public string id;
			[DataMember]
			public string tex;
			public Id_Text(string id1, string text1)
			{
				id = id1;
				tex = text1;
			}
		}

		[DataContract]
		public class Id_Feed
		{
			[DataMember]
			public string id;
			[DataMember]
			public List<string> str;
			public Id_Feed(string id1, List<string> s)
			{
				id = id1;
				str = new List<string>();
				str = s;
			}
		}

		[DataContract]
		class Id_Picture
		{
			[DataMember]
			public string id;
			[DataMember]
			public List<string> pic = new List<string>();
			public Id_Picture(string id1, List<string> s)
			{
				id = id1;
				pic = new List<string>();
				pic = s;
			}
		}

		private void EnterData(String name, String value, ChromeDriver driver)
		{
			List<IWebElement> webElements = driver.FindElementsById(name).ToList();
			if (!webElements.Any())
			{
				return;
			}
			webElements[0].SendKeys(value);
		}

		private void Click_button(string name, ChromeDriver chrome)
		{
			List<IWebElement> webElements = chrome.FindElementsById(name).ToList();
			if (!webElements.Any())
			{
				return;
			}
			webElements[0].Click();
		}

		private void Ft_Set_Password(ChromeDriver chrome)
		{
			chrome.Navigate().GoToUrl("https://vk.com");
			EnterData("index_email", “твой телефон от вк”, chrome);
			EnterData("index_pass", “твой пароль от вк“, chrome);
			Click_button("index_login_button", chrome);
			Thread.Sleep(10000);
		}

		static bool Ft_Check_Equals_Text(List<Id_Text> mas, Id_Text tex)
		{
			foreach (Id_Text item in mas)
			{
				if (String.Equals(item.id, tex.id) == true)
					return (false);
			}
			return (true);
		}

		static bool Ft_Check_Equals_Feed(List<Id_Feed> mas, Id_Feed obj)
		{
			foreach (Id_Feed item in mas)
			{
				if (String.Equals(item.id, obj.id) == true)
					return (false);
			}
			return (true);
		}

		static bool Ft_Check_Equals_Pic(List<Id_Picture> mas, Id_Picture obj)
		{
			foreach (Id_Picture item in mas)
			{
				if (String.Equals(item.id, obj.id) == true)
					return (false);
			}
			return (true);
		}

		private int Ft_Record_Fail_Text(List<IWebElement> webElements, List<string> list_id, int col, int i, bool flag)
		{
			foreach (IWebElement iteam in webElements)
			{
				if (i < col && iteam.FindElements(By.ClassName("wall_post_text")).Count > 0 && (list_id.Contains(iteam.FindElement(By.CssSelector("._post.post")).GetAttribute("id")) == false))
				{
					i = Ft_Final_Rec_Text(webElements, iteam, list_id, col, i, flag);
				}
			}
			return (i);
		}


		static int Ft_Final_Rec_Text(List<IWebElement> webElements, IWebElement iteam,
			List<string> list_id, int col, int i, bool flag)
		{
			string id;
			id = iteam.FindElement(By.CssSelector("._post.post")).GetAttribute("id");
			list_id.Add(id);
			List<Id_Text> mas = new List<Id_Text>();
			Id_Text txt = new Id_Text(id, iteam.FindElement(By.ClassName("wall_post_text")).Text);
			DataContractJsonSerializer jsonFormatter1 = new DataContractJsonSerializer(typeof(List<Id_Text>));
			const string FileName = @"E:\institute\operating-system\Chrome\WindowsFormsApp1\WindowsFormsApp1\bin\Debug\ID_Text.json";
			FileInfo fileInfo = new FileInfo(FileName);
			if (flag == true)
			{
				mutext.WaitOne();
			}
			FileStream fs1 = new FileStream(FileName, FileMode.OpenOrCreate);
			if (fileInfo.Length > 0)
			{
				mas = jsonFormatter1.ReadObject(fs1) as List<Id_Text>;
			}
			if (Ft_Check_Equals_Text(mas, txt) == true)
			{
				mas.Add(txt);
				fs1.Position = 0;
				jsonFormatter1.WriteObject(fs1, mas);
				i++;
			}
			fs1.Close();
			if (flag == true)
			{
				mutext.ReleaseMutex();
			}
			return (i);
		}

		private void Ft_Id_Text(ChromeDriver chrome, int col, bool flag)
		{
			int i;
			List<string> list_id = new List<string>();
			List<IWebElement> webElements = (from item in chrome.FindElementsByClassName("feed_row")
											 where item.Displayed
											 select item).ToList();
			i = 0;
			while (i < col)
			{
				i = Ft_Record_Fail_Text(webElements, list_id, col, i, flag);
				chrome.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
				Thread.Sleep(10000);
				webElements = (from item in chrome.FindElementsByClassName("feed_row")
							   where item.Displayed
							   select item).ToList();
			}
		}

		static int Ft_Record_Fail_Feed(List<IWebElement> webElements, List<string>
			list_id, int i, int col, bool flag)
		{
			foreach (IWebElement iteam in webElements)
			{
				if (i < col && iteam.FindElements(By.ClassName("wall_post_text")).Count > 0
					&& (list_id.Contains(iteam.FindElement(By.CssSelector("._post.post")).GetAttribute("id")) == false))
				{
					i = Ft_Final_Rec_Feed(webElements, iteam, list_id, i, col, flag);
				}
			}
			return (i);
		}

		static int Ft_Final_Rec_Feed(List<IWebElement> webElements,
			IWebElement iteam, List<string> list_id, int i, int col, bool flag)
		{
			string id;
			id = iteam.FindElement(By.CssSelector("._post.post")).GetAttribute("id");
			list_id.Add(id);
			List<string> str = new List<string>();
			List<IWebElement> Brouser = iteam.FindElements(By.TagName("a")).ToList();
			foreach (IWebElement web in Brouser)
			{
				if (web.GetAttribute("href") != null && web.GetAttribute("href") != "")
					str.Add(web.GetAttribute("href"));
			}
			Id_Feed feed = new Id_Feed(id, str);
			List<Id_Feed> mas = new List<Id_Feed>();
			DataContractJsonSerializer jsonFormatter1 = new DataContractJsonSerializer(typeof(List<Id_Feed>));
			FileInfo fileInfo = new FileInfo(@"E:\institute\operating-system\Chrome\WindowsFormsApp1\WindowsFormsApp1\bin\Debug\ID_Feed.json");
			if (flag == true)
			{
				mufeed.WaitOne();
			}
			FileStream fs1 = new FileStream(@"E:\institute\operating-system\Chrome\WindowsFormsApp1\WindowsFormsApp1\bin\Debug\ID_Feed.json", FileMode.OpenOrCreate);
			if (fileInfo.Length > 0)
				mas = jsonFormatter1.ReadObject(fs1) as List<Id_Feed>;
			if (Ft_Check_Equals_Feed(mas, feed) == true)
			{
				mas.Add(feed);
				fs1.Position = 0;
				jsonFormatter1.WriteObject(fs1, mas);
				i++;
			}
			fs1.Close();
			if (flag == true)
			{
				mufeed.ReleaseMutex();
			}
			return (i);
		}

		private void Ft_Id_Feed(ChromeDriver chrome, int col, bool flag)
		{
			int i;
			List<string> list_id = new List<string>();
			List<IWebElement> webElements = (from item in chrome.FindElementsByClassName("feed_row")
											 where item.Displayed
											 select item).ToList();
			i = 0;
			while (i < col)
			{
				i = Ft_Record_Fail_Feed(webElements, list_id, i, col, flag);
				webElements = (from item in chrome.FindElementsByClassName("feed_row")
							   where item.Displayed
							   select item).ToList();
			}
		}



		static int Ft_Record_Fail_Pic(List<IWebElement> webElements,
			List<string> list_id, int col, int i, bool flag)
		{
			foreach (IWebElement iteam in webElements)
			{
				if (i < col && iteam.FindElements(By.ClassName("wall_post_text")).Count > 0 &&
					(list_id.Contains(iteam.FindElement(By.CssSelector("._post.post")).GetAttribute("id")) == false))
				{
					i = Ft_Final_Rec_Pic(webElements, iteam, list_id, col, i, flag);
				}
			}
			return (i);
		}

		static int Ft_Final_Rec_Pic(List<IWebElement> webElements, IWebElement iteam,
			List<string> list_id, int col, int i, bool flag)
		{
			string id;
			id = iteam.FindElement(By.CssSelector("._post.post")).GetAttribute("id");
			list_id.Add(id);
			List<string> str = new List<string>();
			List<IWebElement> Exploer = iteam.FindElements(By.TagName("a")).ToList();
			foreach (IWebElement web in Exploer)
			{
				if (web.GetAttribute("style") != null && web.GetAttribute("style") != "")
					str.Add(web.GetAttribute("style"));
			}
			Id_Picture pic = new Id_Picture(id, str);
			DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Id_Picture>));
			List<Id_Picture> mas = new List<Id_Picture>();
			FileInfo fileInfo = new FileInfo(@"E:\institute\operating-system\Chrome\WindowsFormsApp1\WindowsFormsApp1\bin\Debug\ID_Picture.json");
			if (flag == true)
			{
				mupic.WaitOne();
			}
			FileStream fs1 = new FileStream(@"E:\institute\operating-system\Chrome\WindowsFormsApp1\WindowsFormsApp1\bin\Debug\ID_Picture.json", FileMode.OpenOrCreate);
			if (fileInfo.Length > 0)
				mas = jsonFormatter.ReadObject(fs1) as List<Id_Picture>;
			if (Ft_Check_Equals_Pic(mas, pic) == true)
			{
				mas.Add(pic);
				fs1.Position = 0;
				jsonFormatter.WriteObject(fs1, mas);
				i++;
			}
			fs1.Close();
			if (flag == true)
			{
				mupic.ReleaseMutex();
			}
			return (i);
		}

		private void Ft_Id_Pic(ChromeDriver chrome, int col, bool flag)
		{

			int i;
			List<string> list_id = new List<string>();
			List<IWebElement> webElements = (from item in chrome.FindElementsByClassName("feed_row")
											 where item.Displayed
											 select item).ToList();
			i = 0;
			while (i < col)
			{
				i = Ft_Record_Fail_Pic(webElements, list_id, col, i, flag);
				webElements = (from item in chrome.FindElementsByClassName("feed_row")
							   where item.Displayed
							   select item).ToList();
			}
		}

		private void Ft_Read_All_File(bool flag, int col)
		{
			int i;
			i = 0;
			while (i < col)
			{
				if (flag == true)
				{
					mutext.WaitOne();
				}
				FileStream fs1 = new FileStream("ID_Text.json", FileMode.Append);
				Thread.Sleep(15000);
				fs1.Close();
				if (flag == true)
				{
					mutext.ReleaseMutex();
				}
				if (flag == true)
				{
					mufeed.WaitOne();
				}
				FileStream fs2 = new FileStream("ID_Feed.json", FileMode.Append);
				Thread.Sleep(15000);
				fs2.Close();
				if (flag == true)
				{
					mufeed.ReleaseMutex();
				}
				if (flag == true)
				{
					mupic.WaitOne();
				}
				FileStream fs3 = new FileStream("ID_Picture.json", FileMode.Append);
				Thread.Sleep(15000);
				fs3.Close();
				if (flag == true)
				{
					mupic.ReleaseMutex();
				}
				i++;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			bool flag = true;
			int col = 10;
			ChromeDriver chrome = new ChromeDriver();
			Ft_Set_Password(chrome);
			Thread thread1 = new Thread(() => Ft_Id_Text(chrome, col, flag));
			Thread thread2 = new Thread(() => Ft_Id_Feed(chrome, col, flag));
			Thread thread3 = new Thread(() => Ft_Id_Pic(chrome, col, flag));
			Thread thread4 = new Thread(() => Ft_Read_All_File(flag, col));
			thread1.Start();
			thread2.Start();
			thread3.Start();
			thread4.Start();
		}
	}
}
