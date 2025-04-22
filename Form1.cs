using System;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;

namespace PSIP_Project_Adam3
{
    public partial class Form1 : Form
    {
        // Arrays to hold TextBox references for each protocol group.
        private TextBox[] txtLeftIN = new TextBox[8];
        private TextBox[] txtLeftOUT = new TextBox[8];
        private TextBox[] txtRightIN = new TextBox[8];
        private TextBox[] txtRightOUT = new TextBox[8];

        // Timer to periodically update the grid values.
        private System.Windows.Forms.Timer updateTimer;

        // Add a DataGridView to show the MAC table.
        private DataGridView macTableGrid;
        private List<ACLRule> aclRules = new List<ACLRule>(); // List to store ACL rules

        public Form1()
        {
            // Set the default size of the form first
            this.Size = new System.Drawing.Size(800, 600); 

            // Now call InitializeComponent() to set up the form's layout
            InitializeComponent();

            // Initialize other controls
            InitializeGrid();
            InitializeMACTable(); // Initialize the MAC Table DataGridView
            InitializeButtons();
            InitializeTimer();
            InitializeACLControls();
        }


        private void InitializeACLControls()
        {
            // ACL Configuration Panel (similar to MAC Table position)
            Panel aclPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 250 // Adjust height as needed
            };

            // Create a DataGridView to display ACL Rules
            DataGridView aclTableGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = false, // Allow editing ACL rules
                AllowUserToAddRows = false, // Disable automatic row addition
                RowHeadersVisible = false, // Hide row headers
            };

            // Define columns for the ACL table (matching your fields)
            aclTableGrid.Columns.Add("LocalPort", "Local Port");
            aclTableGrid.Columns.Add("Direction", "Direction");
            aclTableGrid.Columns.Add("Protocol", "Protocol");
            aclTableGrid.Columns.Add("SourceIP", "Source IP");
            aclTableGrid.Columns.Add("SourceMAC", "Source MAC");
            aclTableGrid.Columns.Add("DestIP", "Dest IP");
            aclTableGrid.Columns.Add("DestMAC", "Dest MAC");
            aclTableGrid.Columns.Add("DestPort", "Dest Port");
            aclTableGrid.Columns.Add("FilterType", "Filter Type");

            // Add the grid to the ACL panel
            aclPanel.Controls.Add(aclTableGrid);

            // Add ACL Configuration Fields (Textboxes and Comboboxes)
            FlowLayoutPanel aclControlPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 60, // Compact height for controls
                FlowDirection = FlowDirection.LeftToRight, // Align controls horizontally
                WrapContents = true, // Allow wrapping if controls exceed width
                AutoSize = true
            };

            // Create controls for adding a new ACL rule
            TextBox txtLocalPort = new TextBox() { Width = 60, Margin = new Padding(5) };
            ComboBox cmbDirection = new ComboBox
                { Width = 60, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(5) };
            cmbDirection.Items.AddRange(new string[] { "IN", "OUT" });
            ComboBox cmbProtocol = new ComboBox
                { Width = 80, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(5) };
            cmbProtocol.Items.AddRange(new string[] { "TCP", "UDP", "ICMP", "ARP", "IP" });
            TextBox txtSourceIP = new TextBox() { Width = 80, Margin = new Padding(5) };
            TextBox txtSourceMAC = new TextBox() { Width = 100, Margin = new Padding(5) };
            TextBox txtDestIP = new TextBox() { Width = 80, Margin = new Padding(5) };
            TextBox txtDestMAC = new TextBox() { Width = 100, Margin = new Padding(5) };
            TextBox txtDestPort = new TextBox() { Width = 60, Margin = new Padding(5) };
            ComboBox cmbFilterType = new ComboBox
                { Width = 80, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(5) };
            cmbFilterType.Items.AddRange(new string[] { "Permit", "Deny" });

            // Button to add new ACL Rule
            Button btnAddACL = new Button
            {
                Text = "Add ACL",
                Width = 80,
                Height = 30,
                Margin = new Padding(5)
            };

            // Click event for adding a new ACL rule
            btnAddACL.Click += (s, e) =>
            {
                // Collect all inputs
                string localPort = txtLocalPort.Text;
                string direction = cmbDirection.SelectedItem?.ToString();
                string protocol = cmbProtocol.SelectedItem?.ToString();
                string sourceIP = txtSourceIP.Text;
                string sourceMAC = txtSourceMAC.Text;
                string destIP = txtDestIP.Text;
                string destMAC = txtDestMAC.Text;
                string destPort = txtDestPort.Text;
                string filterType = cmbFilterType.SelectedItem?.ToString();

                // Validate fields
                if (string.IsNullOrEmpty(localPort) || string.IsNullOrEmpty(direction) ||
                    string.IsNullOrEmpty(protocol) ||
                    string.IsNullOrEmpty(sourceIP) || string.IsNullOrEmpty(sourceMAC) || string.IsNullOrEmpty(destIP) ||
                    string.IsNullOrEmpty(destMAC) || string.IsNullOrEmpty(destPort) || string.IsNullOrEmpty(filterType))
                {
                    MessageBox.Show("Please fill in all fields.");
                    return;
                }

                // Add a new row to the ACL table with the collected data
                aclTableGrid.Rows.Add(localPort, direction, protocol, sourceIP, sourceMAC, destIP, destMAC, destPort,
                    filterType);

                // Optionally clear the fields after adding the rule
                txtLocalPort.Clear();
                cmbDirection.SelectedIndex = -1;
                cmbProtocol.SelectedIndex = -1;
                txtSourceIP.Clear();
                txtSourceMAC.Clear();
                txtDestIP.Clear();
                txtDestMAC.Clear();
                txtDestPort.Clear();
                cmbFilterType.SelectedIndex = -1;
            };

            // Add controls to the aclControlPanel
            aclControlPanel.Controls.Add(new Label
                { Text = "Local Port", Width = 60, TextAlign = ContentAlignment.MiddleCenter });
            aclControlPanel.Controls.Add(txtLocalPort);
            aclControlPanel.Controls.Add(new Label
                { Text = "Direction", Width = 60, TextAlign = ContentAlignment.MiddleCenter });
            aclControlPanel.Controls.Add(cmbDirection);
            aclControlPanel.Controls.Add(new Label
                { Text = "Protocol", Width = 80, TextAlign = ContentAlignment.MiddleCenter });
            aclControlPanel.Controls.Add(cmbProtocol);
            aclControlPanel.Controls.Add(new Label
                { Text = "Source IP", Width = 80, TextAlign = ContentAlignment.MiddleCenter });
            aclControlPanel.Controls.Add(txtSourceIP);
            aclControlPanel.Controls.Add(new Label
                { Text = "Source MAC", Width = 100, TextAlign = ContentAlignment.MiddleCenter });
            aclControlPanel.Controls.Add(txtSourceMAC);
            aclControlPanel.Controls.Add(new Label
                { Text = "Dest IP", Width = 80, TextAlign = ContentAlignment.MiddleCenter });
            aclControlPanel.Controls.Add(txtDestIP);
            aclControlPanel.Controls.Add(new Label
                { Text = "Dest MAC", Width = 100, TextAlign = ContentAlignment.MiddleCenter });
            aclControlPanel.Controls.Add(txtDestMAC);
            aclControlPanel.Controls.Add(new Label
                { Text = "Dest Port", Width = 60, TextAlign = ContentAlignment.MiddleCenter });
            aclControlPanel.Controls.Add(txtDestPort);
            aclControlPanel.Controls.Add(new Label
                { Text = "Filter Type", Width = 80, TextAlign = ContentAlignment.MiddleCenter });
            aclControlPanel.Controls.Add(cmbFilterType);
            aclControlPanel.Controls.Add(btnAddACL);

            // Add the ACL control panel above the table
            aclPanel.Controls.Add(aclControlPanel);

            // Add the ACL panel to the form
            this.Controls.Add(aclPanel);
        }


        private void InitializeGrid()
        {
            // Create a TableLayoutPanel with 8 columns and 9 rows (1 header row + 8 protocol rows)
            TableLayoutPanel grid = new TableLayoutPanel
            {
                ColumnCount = 8,
                RowCount = 9,
                Dock = DockStyle.Fill,
                AutoSize = true
            };

            // Set equal column widths (each 12.5% of the width)
            for (int i = 0; i < 8; i++)
            {
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
            }

            // Add header row with a fixed height
            grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            // Add protocol rows (one for each protocol)
            for (int i = 0; i < 8; i++)
            {
                grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / 8));
            }

            // Create header labels for each column
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

            // The order of protocols must match the order of your variable declarations.
            // (For left side: Ethernet, IP, ARP, TCP, UDP, ICMP, HTTP, HTTPS)
            string[] protocols = new string[] { "Ethernet", "IP", "ARP", "TCP", "UDP", "ICMP", "HTTP", "HTTPS" };

            // Create a row for each protocol.
            for (int i = 0; i < protocols.Length; i++)
            {
                int row = i + 1; // Row 0 is used for headers.

                // Column 0: protocol name label for Left IN
                Label lblLeftIN = new Label
                {
                    Text = protocols[i],
                    TextAlign = System.Drawing.ContentAlignment.MiddleRight,
                    Dock = DockStyle.Fill
                };
                grid.Controls.Add(lblLeftIN, 0, row);

                // Column 1: Left IN value TextBox
                TextBox txtValLeftIN = new TextBox
                {
                    Name = "txt" + protocols[i] + "LeftIN",
                    Dock = DockStyle.Fill,
                    ReadOnly = true
                };
                txtLeftIN[i] = txtValLeftIN;
                grid.Controls.Add(txtValLeftIN, 1, row);

                // Column 2: protocol name label for Left OUT
                Label lblLeftOUT = new Label
                {
                    Text = protocols[i],
                    TextAlign = System.Drawing.ContentAlignment.MiddleRight,
                    Dock = DockStyle.Fill
                };
                grid.Controls.Add(lblLeftOUT, 2, row);

                // Column 3: Left OUT value TextBox
                TextBox txtValLeftOUT = new TextBox
                {
                    Name = "txt" + protocols[i] + "LeftOUT",
                    Dock = DockStyle.Fill,
                    ReadOnly = true
                };
                txtLeftOUT[i] = txtValLeftOUT;
                grid.Controls.Add(txtValLeftOUT, 3, row);

                // Column 4: protocol name label for Right IN
                Label lblRightIN = new Label
                {
                    Text = protocols[i],
                    TextAlign = System.Drawing.ContentAlignment.MiddleRight,
                    Dock = DockStyle.Fill
                };
                grid.Controls.Add(lblRightIN, 4, row);

                // Column 5: Right IN value TextBox
                TextBox txtValRightIN = new TextBox
                {
                    Name = "txt" + protocols[i] + "RightIN",
                    Dock = DockStyle.Fill,
                    ReadOnly = true
                };
                txtRightIN[i] = txtValRightIN;
                grid.Controls.Add(txtValRightIN, 5, row);

                // Column 6: protocol name label for Right OUT
                Label lblRightOUT = new Label
                {
                    Text = protocols[i],
                    TextAlign = System.Drawing.ContentAlignment.MiddleRight,
                    Dock = DockStyle.Fill
                };
                grid.Controls.Add(lblRightOUT, 6, row);

                // Column 7: Right OUT value TextBox
                TextBox txtValRightOUT = new TextBox
                {
                    Name = "txt" + protocols[i] + "RightOUT",
                    Dock = DockStyle.Fill,
                    ReadOnly = true
                };
                txtRightOUT[i] = txtValRightOUT;
                grid.Controls.Add(txtValRightOUT, 7, row);
            }

            // Add the grid to the Form's controls.
            this.Controls.Add(grid);
        }

        // Initialize the MAC Table DataGridView
        private void InitializeMACTable()
        {
            macTableGrid = new DataGridView
            {
                Dock = DockStyle.Bottom,
                Height = 200, // Adjust height based on how much data you want to show
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true
            };

            // Add columns for MAC address, TTL, and Port
            macTableGrid.Columns.Add("MacAddress", "MAC Address");
            macTableGrid.Columns.Add("TTL", "TTL");
            macTableGrid.Columns.Add("Port", "Port");

            // Add the DataGridView to the form
            this.Controls.Add(macTableGrid);
        }

        private void InitializeButtons()
        {
            // Create a panel to hold the buttons at the bottom of the form.
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40
            };

            // Start button
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
                // Calls the method to list devices.
                DeviceManager.ListDevices();
                //DeviceManager.MonitorDevices();
                DeviceManager.decrementTime();
            };

            // End button
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
                // Stops capture on device1.
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
                // Calls the method to clear the MAC table.
                DeviceManager.clearMacTable();

                // Optionally, clear the rows from the DataGridView as well.
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
                // Calls the method to clear the MAC table.
                DeviceManager.clearStats();
            };
            // TTL Label
            Label lblTTL = new Label
            {
                Text = "Set TTL:",
                Width = 60,
                Height = 30,
                Left = btnClearStat.Right + 10,
                Top = 5,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

// TTL Input TextBox
            TextBox txtTTLInput = new TextBox
            {
                Width = 60,
                Height = 30,
                Left = lblTTL.Right + 5,
                Top = 5
            };

// Apply TTL Button
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


// Add TTL components to the panel
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
            updateTimer.Interval = 100; // Interval in milliseconds (1 second)
            updateTimer.Tick += (s, e) => UpdateGridValues();
            updateTimer.Start();
        }

        // Call this method to update the TextBox values based on your DeviceManager variables.
        public void UpdateGridValues()
        {
            // Make sure the order in your protocols array matches the order of your variable declarations.
            // Left IN (Column 2 in one-indexed terms)
            txtLeftIN[0].Text = DeviceManager.EthernetPacketCountIN_Left.ToString();
            txtLeftIN[1].Text = DeviceManager.IPPacketCountIN_Left.ToString();
            txtLeftIN[2].Text = DeviceManager.ARPpacketCountIN_Left.ToString();
            txtLeftIN[3].Text = DeviceManager.TCPpacketCountIN_Left.ToString();
            txtLeftIN[4].Text = DeviceManager.UDPpacketCountIN_Left.ToString();
            txtLeftIN[5].Text = DeviceManager.ICMPpacketCountIN_Left.ToString();
            txtLeftIN[6].Text = DeviceManager.HTTPpacketCountIN_Left.ToString();
            txtLeftIN[7].Text = DeviceManager.HTTPSPacketCountIN_Left.ToString();

            // Left OUT (Column 4 in one-indexed terms)
            txtLeftOUT[0].Text = DeviceManager.EthernetPacketCountOUT_Left.ToString();
            txtLeftOUT[1].Text = DeviceManager.IPPacketCountOUT_Left.ToString();
            txtLeftOUT[2].Text = DeviceManager.ARPpacketCountOUT_Left.ToString();
            txtLeftOUT[3].Text = DeviceManager.TCPpacketCountOUT_Left.ToString();
            txtLeftOUT[4].Text = DeviceManager.UDPpacketCountOUT_Left.ToString();
            txtLeftOUT[5].Text = DeviceManager.ICMPpacketCountOUT_Left.ToString();
            txtLeftOUT[6].Text = DeviceManager.HTTPpacketCountOUT_Left.ToString();
            txtLeftOUT[7].Text = DeviceManager.HTTPSPacketCountOUT_Left.ToString();

            // Right IN (Column 6 in one-indexed terms)
            txtRightIN[0].Text = DeviceManager.EthernetPacketCountIN_Right.ToString();
            txtRightIN[1].Text = DeviceManager.IPPacketCountIN_Right.ToString();
            txtRightIN[2].Text = DeviceManager.ARPpacketCountIN_Right.ToString();
            txtRightIN[3].Text = DeviceManager.TCPPacketCountIN_Right.ToString();
            txtRightIN[4].Text = DeviceManager.UDPPacketCountIN_Right.ToString();
            txtRightIN[5].Text = DeviceManager.ICMPPacketCountIN_Right.ToString();
            txtRightIN[6].Text = DeviceManager.HTTPPacketCountIN_Right.ToString();
            txtRightIN[7].Text = DeviceManager.HTTPSPacketCountIN_Right.ToString();

            // Right OUT (Column 8 in one-indexed terms)
            txtRightOUT[0].Text = DeviceManager.EthernetPacketCountOUT_Right.ToString();
            txtRightOUT[1].Text = DeviceManager.IPPacketCountOUT_Right.ToString();
            txtRightOUT[2].Text = DeviceManager.ARPPacketCountOUT_Right.ToString();
            txtRightOUT[3].Text = DeviceManager.TCPPacketCountOUT_Right.ToString();
            txtRightOUT[4].Text = DeviceManager.UDPPacketCountOUT_Right.ToString();
            txtRightOUT[5].Text = DeviceManager.ICMPPacketCountOUT_Right.ToString();
            txtRightOUT[6].Text = DeviceManager.HTTPPacketCountOUT_Right.ToString();
            txtRightOUT[7].Text = DeviceManager.HTTPSPacketCountOUT_Right.ToString();

            // Update MAC table (the new part)
            UpdateMACTable();
        }

        // Method to update the MAC table in the DataGridView
        private void UpdateMACTable()
        {
            // Clear the previous rows
            macTableGrid.Rows.Clear();

            // Locking the dictionary to avoid concurrent modification
            lock (DeviceManager.packetDictionary)
            {
                foreach (var entry in DeviceManager.packetDictionary)
                {
                    // Assuming 'Port' is part of the value in the packetDictionary
                    // Add a new row for each dictionary entry including the Port
                    macTableGrid.Rows.Add(entry.Value.SourceMacAddress, entry.Value.TTL, entry.Value.SourcePort);
                }
            }
        }
    }
}