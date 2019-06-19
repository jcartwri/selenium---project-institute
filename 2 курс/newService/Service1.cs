using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace newService
{
	public partial class Service1 : ServiceBase
	{
		static Mutex mutext = new Mutex(false, @"Global\mutext");
		static Mutex mufeed = new Mutex(false, @"Global\mufeed");
		static Mutex mupic = new Mutex(false, @"Global\mupic");


		public Service1()
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

		static int Ft_Check_Equals_Text(Database1Entities tab, Id_Text obj)
		{
			int i;

			i = 0;
			foreach (Text item in tab.Texts)
			{
				i++;
				if (String.Equals(obj.id, item.Id_Text) == true)
					return (-1);
			}
			return (i);
		}

		static void Ft_Record_Text(Database1Entities tab, List<Id_Text> mas)
		{
			int i;

			foreach (Id_Text iteam in mas)
			{
				if ((i = Ft_Check_Equals_Text(tab, iteam)) != -1)
				{
					Text table = new Text() { Id = i, Id_Text = iteam.id, Text1 = iteam.tex };
					tab.Texts.Add(table);
					tab.SaveChanges();
				}
			}
		}

		static void Ft_Set_Text_Database(bool flag)
		{
			string filename = @"E:\institute\operating-system\Chrome\WindowsFormsApp1\WindowsFormsApp1\bin\Debug\ID_Text.json";
			Database1Entities text = new Database1Entities();
			DataContractJsonSerializer fd = new DataContractJsonSerializer(typeof(List<Id_Text>));
			FileInfo fileInfo = new FileInfo(filename);
			List<Id_Text> mas = new List<Id_Text>();
			if (flag == true)
				mutext.WaitOne();
			using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
			{
				if (fileInfo.Length > 0)
				{
					mas = fd.ReadObject(fs) as List<Id_Text>;
					Ft_Record_Text(text, mas);
				}
			}
			if (flag == true)
				mutext.ReleaseMutex();
		}

		static int Ft_Check_Equals_Feed(Database1Entities model, Id_Feed obj)
		{
			int i;

			i = 0;
			foreach (Feed item in model.Feeds)
			{
				i++;
				if (String.Equals(obj.id, item.Id_Feed) == true)
					return (-1);
			}
			return (i);
		}

		static void Ft_Record_Feed(Database1Entities model, List<Id_Feed> mas)
		{
			int i;

			foreach (Id_Feed iteam in mas)
			{
				if ((i = Ft_Check_Equals_Feed(model, iteam)) != -1)
				{
					Feed table = new Feed
					{
						Id = i,
						Id_Feed = iteam.id,
						feed1 = String.Join(", ", iteam.str)
					};
					model.Feeds.Add(table);
					model.SaveChanges();
				}
			}
		}

		static void Ft_Set_Feed_Database(bool flag)
		{
			string filename = @"E:\institute\operating-system\Chrome\WindowsFormsApp1\WindowsFormsApp1\bin\Debug\ID_Feed.json";
			Database1Entities model = new Database1Entities();
			DataContractJsonSerializer fd = new DataContractJsonSerializer(typeof(List<Id_Feed>));
			FileInfo fileInfo = new FileInfo(filename);
			List<Id_Feed> mas = new List<Id_Feed>();
			if (flag == true)
				mufeed.WaitOne();
			using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
			{
				if (fileInfo.Length > 0)
				{
					mas = fd.ReadObject(fs) as List<Id_Feed>;
					Ft_Record_Feed(model, mas);
				}
			}
			if (flag == true)
				mufeed.ReleaseMutex();
		}

		/*static int Ft_Check_Equals_Pic(Database1Entities model, Id_Picture obj)
		{
			int i;

			i = 0;
			foreach (Picture item in model.Pictures)
			{
				i++;
				if (String.Equals(obj.id, item.Id_Pic) == true)
					return (-1);
			}
			return (i);
		}*/

		static void Ft_Record_Pic(Database1Entities model, List<Id_Picture> mas)
		{
			int i;

			i = 0;
			//foreach (Id_Picture iteam in mas)
			//{
				model.Pictures.AddRange(from iteam in mas
										where (model.Pictures.Any(pic => pic.Id_Pic.Equals(iteam.id)))
										select new Picture {Id = model.Pictures.Count(), Id_Pic = iteam.id, Pic = String.Join(", ", iteam.pic) });
//				if ((i = Ft_Check_Equals_Pic(model, iteam)) != -1)
				//if (!model.Pictures(Id_Feed).Contains(item.Id_Pic))
				//{
				//	Picture table = new Picture
				//	{
				//		Id = i,
				//		Id_Pic = iteam.id,
				//		Pic = String.Join(", ", iteam.pic)
				//	};
				//	model.Pictures.Add(table);
					model.SaveChanges();
				//}
			//}
		}

		static void Ft_Set_Pic_Database(bool flag)
		{
			string filename = @"E:\institute\operating-system\Chrome\WindowsFormsApp1\WindowsFormsApp1\bin\Debug\ID_Picture.json";

			Database1Entities model = new Database1Entities();

			DataContractJsonSerializer fd = new DataContractJsonSerializer(typeof(List<Id_Picture>));
			FileInfo fileInfo = new FileInfo(filename);
			List<Id_Picture> mas = new List<Id_Picture>();
			if (flag == true)
				mupic.WaitOne();
			using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
			{
				if (fileInfo.Length > 0)
				{
					mas = fd.ReadObject(fs) as List<Id_Picture>;
					Ft_Record_Pic(model, mas);
				}
			}
			if (flag == true)
				mupic.ReleaseMutex();
		}

		protected override void OnStart(string[] args)
		{
			bool flag = true;

			Thread.Sleep(10000);
			Thread thread1 = new Thread(() => Ft_Set_Text_Database(flag));
			Thread thread2 = new Thread(() => Ft_Set_Feed_Database(flag));
			Thread thread3 = new Thread(() => Ft_Set_Pic_Database(flag));
			thread1.Start();
			thread2.Start();
			thread3.Start();
		}

		protected override void OnStop()
		{
		}
	}
}
