﻿using ManageSystem.Model;
using ManageSystem.Server;
using ManageSystem.ViewModel.DeviceViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ManageSystem.ViewModel
{
   public  class EndorsementQueryViewModel : NotificationObject
    {
        public QueryTableCallBackDelegate               _querytablecallbackdelegate = null;
        public ObservableCollection<QIANZHUSHUJUModel> _tableListTemp = new ObservableCollection<QIANZHUSHUJUModel>();

        public DelegateCommand<object>                  QueryCommand { get; set; }
        public DelegateCommand<object> FirstPageCommand { get; set; }
        public DelegateCommand<object> PrePageCommand { get; set; }
        public DelegateCommand<object> NextPageCommand { get; set; }
        public DelegateCommand<object> GotoPageCommand { get; set; }

        private int _numofpage;
        public int numofpage
        {
            get
            {
                return _numofpage;
            }
            set
            {
                _numofpage = value;
                this.RaisePropertyChanged("numofpage");
            }
        }

        private string _pagePercent;
        public string pagePercent
        {
            get
            {
                return _pagePercent;
            }
            set
            {
                _pagePercent = value;
                this.RaisePropertyChanged("pagePercent");
            }
        }
        private string _cardNumber;
        public string cardNumber
        {
            get { return _cardNumber; }
            set
            {
                _cardNumber = value;
                this.RaisePropertyChanged("cardNumber");
            }
        }
        private string _businessNumber;
        public string businessNumber
        {
            get { return _businessNumber; }
            set
            {
                _businessNumber = value;
                this.RaisePropertyChanged("businessNumber");
            }
        }
        private string _startTime;
        public string startTime
        {
            get { return _startTime; }
            set
            {
                _startTime = value;
                this.RaisePropertyChanged("startTime");
            }
        }

        private string _endTime;
        public string endTime
        {
            get { return _endTime; }
            set
            {
                _endTime = value;
                this.RaisePropertyChanged("endTime");
            }
        }

        private string _groupInsetText;
        public string groupInsetText
        {
            get { return _groupInsetText; }
            set
            {
                _groupInsetText = value;
                this.RaisePropertyChanged("groupInsetText");
            }
        }

        private string _businessTypeText;
        public string businessTypeText
        {
            get { return _businessTypeText; }
            set
            {
                _businessTypeText = value;
                this.RaisePropertyChanged("businessTypeText");
            }
        }

        private string _originalCertificateTypeText;
        public string originalCertificateTypeText
        {
            get { return _originalCertificateTypeText; }
            set
            {
                _originalCertificateTypeText = value;
                this.RaisePropertyChanged("originalCertificateTypeText");
            }
        }

        private ObservableCollection<QIANZHUSHUJUModel> _tableList;
        public ObservableCollection<QIANZHUSHUJUModel> tableList
        {
            get
            {
                return _tableList;
            }
            set
            {
                _tableList = value;
                this.RaisePropertyChanged("tableList");
            }
        }

        public EndorsementQueryViewModel()
        {
            _querytablecallbackdelegate                 = new QueryTableCallBackDelegate(QueryTableCallBack);
            QueryCommand                                = new DelegateCommand<object>(new Action<object>(this.Query));

            FirstPageCommand                            = new DelegateCommand<object>(FirstPage);
            PrePageCommand                              = new DelegateCommand<object>(PrePage);
            NextPageCommand                             = new DelegateCommand<object>(NextPage);
            GotoPageCommand                             = new DelegateCommand<object>(GotoPage);

            _tableList                                  = new ObservableCollection<QIANZHUSHUJUModel>();
            _pagePercent                                = "0/0";

            startTime                                   = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss");
            endTime                                     = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd HH:mm:ss");
        }

        public void ShowPage(int pageindex)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (numofpage > 0)
                {
                    int count = _tableListTemp.Count;            //获取记录总数  
                    int pageSize = 0;                       //pageSize表示总页数  
                    if (count % numofpage == 0)
                        pageSize = count / numofpage;
                    else
                        pageSize = count / numofpage + 1;

                    if (pageindex < 1 || pageindex > pageSize)
                        return;

                    tableList = new ObservableCollection<QIANZHUSHUJUModel>(_tableListTemp.Take(numofpage * pageindex).Skip(numofpage * (pageindex - 1)).ToList());   //刷选第currentSize页要显示的记录集  
                    pagePercent = pageindex + "/" + pageSize;
                }
            }));
        }

        private void GotoPage(object obj)
        {
            try
            {
                int Number = Convert.ToInt32(obj);
                ShowPage(Number);
            }
            catch { }
        }

        private void NextPage(object obj)
        {
            try
            {
                string[] str = pagePercent.Split('/');
                ShowPage(Convert.ToInt32(str[0]) + 1);
            }
            catch { }
        }

        private void PrePage(object obj)
        {
            try
            {
                string[] str = pagePercent.Split('/');
                ShowPage(Convert.ToInt32(str[0]) - 1);
            }
            catch { }
        }

        private void FirstPage(object obj)
        {
            try
            {
                ShowPage(1);
            }
            catch { }
        }

        public void QueryTableCallBack(string resultStr, string errorStr)
        {
            System.Reflection.PropertyInfo[] properties = typeof(QIANZHUSHUJUModel).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            string[] rows = resultStr.Split(';');
            foreach (string row in rows)
            {
                if (row.Length > 0)
                {
                    QIANZHUSHUJUModel model     = new QIANZHUSHUJUModel();

                    string[] cells = row.Split(',');
                    foreach (string cell in cells)
                    {
                        string[] keyvalue = cell.Split(':');
                        if (keyvalue.Length != 2 || keyvalue[1] == null || keyvalue[1].Length == 0)
                            continue;

                        foreach (System.Reflection.PropertyInfo item in properties)
                        {
                            if (item.Name == keyvalue[0])
                            {
                                if (item.PropertyType.Name.StartsWith("Int32"))
                                {
                                    item.SetValue(model, Convert.ToInt32(keyvalue[1]), null);
                                }
                                else if (item.PropertyType.Name.StartsWith("Int64"))
                                {
                                    item.SetValue(model, Convert.ToInt64(keyvalue[1]), null);
                                }
                                else if (item.PropertyType.Name.StartsWith("String"))
                                {
                                    switch (item.Name)
                                    {
                                        case "Chengshibianhao":
                                        case "Jubianhao":
                                        case "Shiyongdanweibianhao":
                                        case "Shebeibaifangweizhi":
                                        case "Qianzhuzhonglei":
                                        case "ZhikaZhuangtai":
                                        case "Zhengjianleixing":
                                        case "Xingbie":
                                        case "Yewuleixing":
                                            if (MainWindowViewModel._yingshelList.Keys.Contains(Convert.ToInt32(keyvalue[1])))
                                                item.SetValue(model, MainWindowViewModel._yingshelList[Convert.ToInt32(keyvalue[1])], null);
                                            break;
                                        case "Riqi":
                                        case "Chushengriqi":
                                        case "Jiaoyiriqi":
                                            DateTime datetime = Common.ConvertIntDateTime(Convert.ToInt64(keyvalue[1]));
                                            item.SetValue(model, datetime.ToShortDateString(), null);
                                            break;
                                        case "IP":
                                            item.SetValue(model, Common.IntToIp(IPAddress.NetworkToHostOrder((Int32)Convert.ToInt64(keyvalue[1]))), null);
                                            break;
                                        default:
                                            item.SetValue(model, keyvalue[1], null);
                                            break;
                                    }
                                }
                                else if (item.PropertyType.Name.StartsWith("Boolean"))
                                {
                                    item.SetValue(model, Convert.ToBoolean(Convert.ToInt32(keyvalue[1])), null);
                                }
                                else
                                {
                                    ;
                                }
                                break;
                            }
                        }
                    }
                    _tableListTemp.Add(model);
                }
            }
            Application.Current.Dispatcher.Invoke(
             new Action(() =>
             {
                 ShowPage(1);
             }));
        }
       
        public void Query(object obj)
        {
            tableList.Clear();
            _tableListTemp.Clear();
            WorkServer.QueryTable(MakeQuerySql(obj), Marshal.GetFunctionPointerForDelegate(_querytablecallbackdelegate));
        }
        string MakeQuerySql(object obj)
        {
            string str = "select * from Qianzhushuju where Xuhao>=-1";

            string tableName = "Qianzhushuju";
            bool bSel0 = false;
            foreach (DeviceModel model0 in DevicemaViewModel._deviceList)
            {
                if (model0.isSel)
                {
                    foreach (KeyValuePair<int, string> kvp0 in MainWindowViewModel._yingshelList)
                    {
                        if (kvp0.Value == model0.text)
                        {
                            if (bSel0)
                                str += " or "+ tableName + ".[Chengshibianhao]=" + kvp0.Key.ToString();
                            else
                                str += " and ("+ tableName + ".[Chengshibianhao]=" + kvp0.Key.ToString();
                            bSel0 = true;
                        }
                    }
                }

                bool bSel1 = false;
                foreach (DeviceModel model1 in model0.Children)
                {
                    if (model1.isSel)
                    {
                        foreach (KeyValuePair<int, string> kvp0 in MainWindowViewModel._yingshelList)
                        {
                            if (kvp0.Value == model1.text)
                            {
                                if (bSel1)
                                    str += " or "+ tableName + ".[Jubianhao]=" + kvp0.Key.ToString();
                                else
                                    str += " and ("+ tableName + ".[Jubianhao]=" + kvp0.Key.ToString();
                                bSel1 = true;
                            }
                        }
                    }

                    bool bSel2 = false;
                    foreach (DeviceModel model2 in model1.Children)
                    {
                        if (model2.isSel)
                        {
                            foreach (KeyValuePair<int, string> kvp0 in MainWindowViewModel._yingshelList)
                            {
                                if (kvp0.Value == model2.text)
                                {
                                    if (bSel2)
                                        str += " or "+ tableName + ".[Shiyongdanweibianhao]=" + kvp0.Key.ToString();
                                    else
                                        str += " and ("+ tableName + ".[Shiyongdanweibianhao]=" + kvp0.Key.ToString();
                                    bSel2 = true;
                                }
                            }
                        }

                        bool bSel3 = false;    //  是否已有先中项
                        foreach (DeviceModel model3 in model2.Children)
                        {
                            if (model3.isSel)
                            {
                                if (bSel3)
                                    str += " or "+ tableName + ".[IP]=" + (UInt32)(IPAddress.HostToNetworkOrder((Int32)Common.IpToInt(model3.text)));
                                else
                                    str += " and ("+ tableName + ".[IP]=" + (UInt32)(IPAddress.HostToNetworkOrder((Int32)Common.IpToInt(model3.text)));
                                bSel3 = true;
                            }
                        }

                        if (bSel3)
                            str += ")";
                    }
                    if (bSel2)
                        str += ")";
                }
                if (bSel1)
                    str += ")";
            }
            if (bSel0)
                str += ")";

            if (cardNumber!=null && cardNumber.Length != 0)
                str += " and Qianzhushuju.[Zhengjianhaoma]=" + "'" + cardNumber + "'";
            if (businessNumber != null && businessNumber.Length != 0)
                str += " and Qianzhushuju.[Yewubianhao]=" + "'" + businessNumber + "'";
            if (startTime != null && startTime.Length != 0)
                str += " and Qianzhushuju.[Riqi]>=" + Common.ConvertDateTimeInt(DateTime.Parse(startTime));
            if (endTime != null && endTime.Length != 0)
                str += " and Qianzhushuju.[Riqi]<=" + Common.ConvertDateTimeInt(DateTime.Parse(endTime));
            if (groupInsetText != null && groupInsetText.Length != 0 && groupInsetText != "全部")
            {
                if (groupInsetText == "是")
                    str += " and Qianzhushuju.[Shifoucharudajizhong]=1";
                else
                    str += " and Qianzhushuju.[Shifoucharudajizhong]=0";
            }

            if (originalCertificateTypeText != null && originalCertificateTypeText.Length != 0 && originalCertificateTypeText != "全部")
            {
                foreach (KeyValuePair<int, string> kvp0 in MainWindowViewModel._yingshelList)
                {
                    if (kvp0.Value == originalCertificateTypeText)
                    {
                        str += " and Qianzhushuju.[Zhengjianleixing]=" + kvp0.Key.ToString();
                        break;
                    }
                }
            }

            if (businessTypeText != null && businessTypeText.Length != 0 && businessTypeText != "全部")
            {
                if (businessTypeText == "本地业务")
                    str += " and Qianzhushuju.[Bendiyewu]=" + "1";
                else
                    str += " and Qianzhushuju.[Bendiyewu]=" + "0";
            }

            return str;
        }
    }
}
