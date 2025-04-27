using System;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;

namespace PSIP_Project_Adam3
{
    public partial class Form1 : Form
    {
        private TextBox[] txtLeftIN = new TextBox[8];
        private TextBox[] txtLeftOUT = new TextBox[8];
        private TextBox[] txtRightIN = new TextBox[8];
        private TextBox[] txtRightOUT = new TextBox[8];

        private System.Windows.Forms.Timer updateTimer;

        private DataGridView macTableGrid;

        public Form1()
        {
            this.Size = new System.Drawing.Size(800, 600);

            InitializeComponent();

            InitializeGrid();
            InitializeMACTable(); 
            InitializeButtons();
            InitializeTimer();
        }

        private void InitializeGrid()
        {
            TableLayoutPanel grid = new TableLayoutPanel
            {
                ColumnCount = 8,
                RowCount = 9,
                Dock = DockStyle.Fill,
                AutoSize = true
            };

            for (int i = 0; i < 8; i++)
            {
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
            }

            grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            for (int i = 0; i < 8; i++)
            {
                grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / 8));
            }

            Label headerProtocol1 = new Label
                { Text = "Protocol", TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
            Label headerLeftIN = new Label
                { Text = "Left IN", TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
            Label headerProtocol2 = new Label
                { Text = "Protocol", TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
            Label headerLeftOUT = new Label
                { Text = "Left OUT", TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
            Label headerProtocol3 = new Label
                { Text = "Protocol", TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
            Label headerRightIN = new Label
                { Text = "Right IN", TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
            Label headerProtocol4 = new Label
                { Text = "Protocol", TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
            Label headerRightOUT = new Label
                { Text = "Right OUT", TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };

            grid.Controls.Add(headerProtocol1, 0, 0);
            grid.Controls.Add(headerLeftIN, 1, 0);
            grid.Controls.Add(headerProtocol2, 2, 0);
            grid.Controls.Add(headerLeftOUT, 3, 0);
            grid.Controls.Add(headerProtocol3, 4, 0);
            grid.Controls.Add(headerRightIN, 5, 0);
            grid.Controls.Add(headerProtocol4, 6, 0);
            grid.Controls.Add(headerRightOUT, 7, 0);

            string[] protocols = new string[] { "Ethernet", "IP", "ARP", "TCP", "UDP", "ICMP", "HTTP", "HTTPS" };

            for (int i = 0; i < protocols.Length; i++)
            {
                int row = i + 1; 

                Label lblLeftIN = new Label
                {
                    Text = protocols[i],
                    TextAlign = System.Drawing.ContentAlignment.MiddleRight,
                    Dock = DockStyle.Fill
                };
                grid.Controls.Add(lblLeftIN, 0, row);

                TextBox txtValLeftIN = new TextBox
                {
                    Name = "txt" + protocols[i] + "LeftIN",
                    Dock = DockStyle.Fill,
                    ReadOnly = true
                };
                txtLeftIN[i] = txtValLeftIN;
                grid.Controls.Add(txtValLeftIN, 1, row);

                Label lblLeftOUT = new Label
                {
                    Text = protocols[i],
                    TextAlign = System.Drawing.ContentAlignment.MiddleRight,
                    Dock = DockStyle.Fill
                };
                grid.Controls.Add(lblLeftOUT, 2, row);

                TextBox txtValLeftOUT = new TextBox
                {
                    Name = "txt" + protocols[i] + "LeftOUT",
                    Dock = DockStyle.Fill,
                    ReadOnly = true
                };
                txtLeftOUT[i] = txtValLeftOUT;
                grid.Controls.Add(txtValLeftOUT, 3, row);

                Label lblRightIN = new Label
                {
                    Text = protocols[i],
                    TextAlign = System.Drawing.ContentAlignment.MiddleRight,
                    Dock = DockStyle.Fill
                };
                grid.Controls.Add(lblRightIN, 4, row);

                TextBox txtValRightIN = new TextBox
                {
                    Name = "txt" + protocols[i] + "RightIN",
                    Dock = DockStyle.Fill,
                    ReadOnly = true
                };
                txtRightIN[i] = txtValRightIN;
                grid.Controls.Add(txtValRightIN, 5, row);

                Label lblRightOUT = new Label
                {
                    Text = protocols[i],
                    TextAlign = System.Drawing.ContentAlignment.MiddleRight,
                    Dock = DockStyle.Fill
                };
                grid.Controls.Add(lblRightOUT, 6, row);

                TextBox txtValRightOUT = new TextBox
                {
                    Name = "txt" + protocols[i] + "RightOUT",
                    Dock = DockStyle.Fill,
                    ReadOnly = true
                };
                txtRightOUT[i] = txtValRightOUT;
                grid.Controls.Add(txtValRightOUT, 7, row);
            }

            this.Controls.Add(grid);
        }

        private void InitializeMACTable()
        {
            macTableGrid = new DataGridView
            {
                Dock = DockStyle.Bottom,
                Height = 200, 
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true
            };

            macTableGrid.Columns.Add("MacAddress", "MAC Address");
            macTableGrid.Columns.Add("TTL", "TTL");
            macTableGrid.Columns.Add("Port", "Port");

            this.Controls.Add(macTableGrid);
        }

        private void InitializeButtons()
        {
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40
            };

            Button btnStart = new Button
            {
                Text = "Start",
                Width = 80,
                Height = 30,
                Left = 10,
                Top = 5
            };
            btnStart.Click += (s, e) =>
            {
                DeviceManager.ListDevices();
                DeviceManager.decrementTime();
            };

            Button btnEnd = new Button
            {
                Text = "End",
                Width = 80,
                Height = 30,
                Left = btnStart.Right + 10,
                Top = 5
            };
            btnEnd.Click += (s, e) =>
            {
                DeviceManager.device1.StopCapture();
                DeviceManager.device2.StopCapture();
            };

            Button btnClearMAC = new Button
            {
                Text = "Clear MAC Table",
                Width = 120,
                Height = 30,
                Left = btnEnd.Right + 10,
                Top = 5
            };
            btnClearMAC.Click += (s, e) =>
            {
                DeviceManager.clearMacTable();

                macTableGrid.Rows.Clear();
            };

            Button btnClearStat = new Button
            {
                Text = "Clear Stats",
                Width = 120,
                Height = 30,
                Left = btnClearMAC.Right + 10,
                Top = 5
            };
            btnClearStat.Click += (s, e) =>
            {
                DeviceManager.clearStats();
            };
            Label lblTTL = new Label
            {
                Text = "Set TTL:",
                Width = 60,
                Height = 30,
                Left = btnClearStat.Right + 10,
                Top = 5,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            TextBox txtTTLInput = new TextBox
            {
                Width = 60,
                Height = 30,
                Left = lblTTL.Right + 5,
                Top = 5
            };

            Button btnSetTTL = new Button
            {
                Text = "Apply TTL",
                Width = 90,
                Height = 30,
                Left = txtTTLInput.Right + 10,
                Top = 5
            };

            btnSetTTL.Click += (s, e) =>
            {
                if (int.TryParse(txtTTLInput.Text, out int newTTL) && newTTL > 0)
                {
                    DeviceManager.CustomTTL = newTTL;
                    MessageBox.Show($"Custom TTL set to {newTTL}");
                }
                else
                {
                    MessageBox.Show("Please enter a valid TTL (number > 0).");
                }
            };


            buttonPanel.Controls.Add(lblTTL);
            buttonPanel.Controls.Add(txtTTLInput);
            buttonPanel.Controls.Add(btnSetTTL);

            buttonPanel.Controls.Add(btnStart);
            buttonPanel.Controls.Add(btnEnd);
            buttonPanel.Controls.Add(btnClearMAC);
            buttonPanel.Controls.Add(btnClearStat);


            this.Controls.Add(buttonPanel);
        }

        private void InitializeTimer()
        {
            updateTimer = new System.Windows.Forms.Timer();
            updateTimer.Interval = 100; 
            updateTimer.Tick += (s, e) => UpdateGridValues();
            updateTimer.Start();
        }

        public void UpdateGridValues()
        {
            txtLeftIN[0].Text = DeviceManager.EthernetPacketCountIN_Left.ToString();
            txtLeftIN[1].Text = DeviceManager.IPPacketCountIN_Left.ToString();
            txtLeftIN[2].Text = DeviceManager.ARPpacketCountIN_Left.ToString();
            txtLeftIN[3].Text = DeviceManager.TCPpacketCountIN_Left.ToString();
            txtLeftIN[4].Text = DeviceManager.UDPpacketCountIN_Left.ToString();
            txtLeftIN[5].Text = DeviceManager.ICMPpacketCountIN_Left.ToString();
            txtLeftIN[6].Text = DeviceManager.HTTPpacketCountIN_Left.ToString();
            txtLeftIN[7].Text = DeviceManager.HTTPSPacketCountIN_Left.ToString();

            txtLeftOUT[0].Text = DeviceManager.EthernetPacketCountOUT_Left.ToString();
            txtLeftOUT[1].Text = DeviceManager.IPPacketCountOUT_Left.ToString();
            txtLeftOUT[2].Text = DeviceManager.ARPpacketCountOUT_Left.ToString();
            txtLeftOUT[3].Text = DeviceManager.TCPpacketCountOUT_Left.ToString();
            txtLeftOUT[4].Text = DeviceManager.UDPpacketCountOUT_Left.ToString();
            txtLeftOUT[5].Text = DeviceManager.ICMPpacketCountOUT_Left.ToString();
            txtLeftOUT[6].Text = DeviceManager.HTTPpacketCountOUT_Left.ToString();
            txtLeftOUT[7].Text = DeviceManager.HTTPSPacketCountOUT_Left.ToString();

            txtRightIN[0].Text = DeviceManager.EthernetPacketCountIN_Right.ToString();
            txtRightIN[1].Text = DeviceManager.IPPacketCountIN_Right.ToString();
            txtRightIN[2].Text = DeviceManager.ARPpacketCountIN_Right.ToString();
            txtRightIN[3].Text = DeviceManager.TCPPacketCountIN_Right.ToString();
            txtRightIN[4].Text = DeviceManager.UDPPacketCountIN_Right.ToString();
            txtRightIN[5].Text = DeviceManager.ICMPPacketCountIN_Right.ToString();
            txtRightIN[6].Text = DeviceManager.HTTPPacketCountIN_Right.ToString();
            txtRightIN[7].Text = DeviceManager.HTTPSPacketCountIN_Right.ToString();

            txtRightOUT[0].Text = DeviceManager.EthernetPacketCountOUT_Right.ToString();
            txtRightOUT[1].Text = DeviceManager.IPPacketCountOUT_Right.ToString();
            txtRightOUT[2].Text = DeviceManager.ARPPacketCountOUT_Right.ToString();
            txtRightOUT[3].Text = DeviceManager.TCPPacketCountOUT_Right.ToString();
            txtRightOUT[4].Text = DeviceManager.UDPPacketCountOUT_Right.ToString();
            txtRightOUT[5].Text = DeviceManager.ICMPPacketCountOUT_Right.ToString();
            txtRightOUT[6].Text = DeviceManager.HTTPPacketCountOUT_Right.ToString();
            txtRightOUT[7].Text = DeviceManager.HTTPSPacketCountOUT_Right.ToString();

            UpdateMACTable();
        }

        private void UpdateMACTable()
        {
            macTableGrid.Rows.Clear();

            lock (DeviceManager.packetDictionary)
            {
                foreach (var entry in DeviceManager.packetDictionary)
                {
                    macTableGrid.Rows.Add(entry.Value.SourceMacAddress, entry.Value.TTL, entry.Value.SourcePort);
                }
            }
        }
    }
}