using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPCameraMonitorSettings
{
    public class ScanDeviceManager
    {
        public string IPS = "";

        private bool finish = false;
        private const int threadCount = 8;
        private ScanDevice[] scaner = new ScanDevice[threadCount];

        public void Run(string header, int begin, int end)
        {
            if (string.IsNullOrEmpty(header) || (end - begin)<0)
            {
                finish = true;
                return;
            }
            if((end - begin) < 10)
            {
                scaner[0] = new ScanDevice();
                scaner[0].Run(header, begin, end);
            }
            else
            {
                int count = (end - begin) / threadCount;
                int mod = (end - begin) % threadCount;
                for (int i = 0; i < scaner.Length; i++)
                {
                    int b = begin + i * count;
                    int e = b + count;
                    if (i == scaner.Length - 1)
                        e = end;
                    scaner[i] = new ScanDevice();
                    scaner[i].Run(header, b, e);
                }
            }
        }

        public bool Finish
        {
            get
            {
                int i = 0;
                for(i=0;i< scaner.Length;i++)
                {
                    if (scaner[i] != null)
                    {
                        if (!scaner[i].finish)
                        {
                            finish = false;
                            break;
                        }
                    }
                }
                if (i == scaner.Length)
                {
                    for (int j = 0; j < scaner.Length; j++)
                    {
                        if(scaner[j] != null)
                            IPS += scaner[j].ips;
                    }
                    finish = true;
                }
                return finish;
            }
        }
    }

    public class ScanDevice
    {
        public bool finish = false;
        public string ips = "";

        private string ipHeader = "";
        private int ipBegin = 1;
        private int ipEnd = 254;

        public void Run(string header,int begin,int end)
        {
            if(string.IsNullOrEmpty(header) || begin < 1 || end <= begin)
            {
                finish = true;
                return;
            }
            ipHeader = header;
            ipBegin = begin;
            ipEnd = end;
            Thread worker = new Thread(Worker);
            worker.IsBackground = true;
            worker.Start();
        }

        private void Worker()
        {
            for (int i = ipBegin; i < ipEnd; i++)
            {
                string ip = ipHeader + i;
                if (GeneralMethod.Ping(ip))
                {
                    ips += ip + ";";
                }
            }
            finish = true;
        }
    }
}
