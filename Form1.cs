using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
namespace _8085
{

    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        public Form1()
        {
            a = new Assembler85();  // create an object for assembler
            InitializeComponent();
            showMemoryPanel(0x2000);    // draw memory panel from location 0x2000
            showPortPanel(0x00);        // draw port panel from location 0x00
            programBackup = richTextBox1.Text;
        }
        Assembler85 a;  // declare an object for class Assembler85

        //
        //  we will access assembler and it's function from the object "a"
        //  created from class Assembler85  
        //

        Label[] memoryAddressLabels = new Label[0x10];      // Rows of memory panel   
        Label[] memoryAddressIndexLabels = new Label[0x8];  // columns of memory panel
        Label[,] memoryTableLabels = new Label[0x10, 0x8];  // contents of memory panel table

        Label[] portAddressLabels = new Label[0x10];      // Rows of memory panel   
        Label[] portAddressIndexLabels = new Label[0x8];  // columns of memory panel
        Label[,] portTableLabels = new Label[0x10, 0x8];  // contents of memory panel table

        string chosen_file = "";    // file selected for saving 
                                    // this file is also the current opened file
        string programBackup;

        int usedRamSize = 0;        // the RAM occupied by assembly program
        int nextInstrAddress = 0;   // next instruction address, will be used for debugging

        //Label[] programLineLabel = new Label[1000];

        //
        //
        // Updating and Drawing the labels of Registers, Flags, Memory, PORT
        //
        //
        private void updateRegisters()  // this updates the registers in the form 
        {
            // example- change 10 to "A" then change "A" to "0A", note "A" is in HEX
            labelBRegister.Text = a.register[0].ToString("X").PadLeft(2, '0');
            labelCRegister.Text = a.register[1].ToString("X").PadLeft(2, '0');
            labelDRegister.Text = a.register[2].ToString("X").PadLeft(2, '0');
            labelERegister.Text = a.register[3].ToString("X").PadLeft(2, '0');
            labelHRegister.Text = a.register[4].ToString("X").PadLeft(2, '0');
            labelLRegister.Text = a.register[5].ToString("X").PadLeft(2, '0');
            //
            labelARegister.Text = a.register[7].ToString("X").PadLeft(2, '0');

            labelPCRegister.Text = a.registerPC.ToString("X").PadLeft(4, '0');
            labelSPRegister.Text = a.registerSP.ToString("X").PadLeft(4, '0');
        }

        private void updateFlags()
        {
            labelCFlag.Text = a.flag[0].ToString().Replace("True", "1").Replace("False", "0");
            //
            labelPFlag.Text = a.flag[2].ToString().Replace("True", "1").Replace("False", "0");
            //
            labelACFlag.Text = a.flag[4].ToString().Replace("True", "1").Replace("False", "0");
            //
            labelZFlag.Text = a.flag[6].ToString().Replace("True", "1").Replace("False", "0");
            labelSFlag.Text = a.flag[7].ToString().Replace("True", "1").Replace("False", "0");
        }

        private void showMemoryPanel(int startAddress)  // draw memory panel starting from 
        {                                               // address startAddress
            int i = 0;
            int j;

            panelMemory.Controls.Clear();   // clear the memory panel

            //memory panel update
            // we can view 128 Byte of memory at a time
            // it will be in form of 16 X 8


            //memoryAddressLabels
            // Display initial memory address labels from 0x2000 to 0x20ff 
            // 
            // example
            //          
            //  2000
            //  2008
            //  2010
            //  ..
            //  ..
            //  2078
            //
            //  

            for (i = 0; i <= 0xF; i++)
            {
                memoryAddressLabels[i] = new Label();
            }
            i = 0;
            j = startAddress;
            foreach (Label lbl in memoryAddressLabels)
            {
                lbl.Name = "memoryAddressLabel" + i.ToString("X");
                lbl.Text = j.ToString("X").PadLeft(4, '0');
                lbl.Visible = true;
                lbl.Size = new System.Drawing.Size(40, 15);
                lbl.Location = new Point(10, 20 + 20 * i);
                panelMemory.Controls.Add(lbl);
                i++;
                j += 0x8;
            }

            //memoryAddressIndexLabels
            // Display the top row required for the memory table
            //
            // example
            //
            //  0 1 2 3 4 5 6 7 
            //
            //

            for (i = 0; i <= 0x7; i++)
            {
                memoryAddressIndexLabels[i] = new Label();
            }
            i = 0;
            j = 0x0;
            foreach (Label lbl in memoryAddressIndexLabels)
            {
                lbl.Name = "memoryAddressIndexLabel" + i.ToString("X");
                lbl.Text = j.ToString("X");
                lbl.Visible = true;
                lbl.Size = new System.Drawing.Size(20, 15);
                lbl.Location = new Point(60 + 30 * i, 0);
                panelMemory.Controls.Add(lbl);
                i++;
                j += 1;
            }

            //memoryTableLabels
            // Display the memory contents
            //
            // example
            //
            //       0   1   2   3   4   5   6   7
            // 2000  00  00  00  00  00  00  00  00
            // 2008  01  03  FA  3F  BC  DD  23  48
            //

            for (i = 0; i <= 0xf; i++)
            {
                for (j = 0; j <= 0x7; j++)
                {
                    memoryTableLabels[i, j] = new Label();
                }
            }
            i = 0;
            j = 0x0;
            foreach (Label lbl in memoryTableLabels)
            {
                lbl.Name = "memoryTableLabel" + i.ToString("X");
                lbl.Text = a.RAM[startAddress + (8 * i) + j].ToString("X").PadLeft(2, '0');
                lbl.Visible = true;
                lbl.Size = new System.Drawing.Size(30, 15);
                lbl.Location = new Point(60 + 30 * j, 20 + 20 * i);
                panelMemory.Controls.Add(lbl);
                j++;
                if (j == 8)
                {
                    j = 0;
                    i++;
                }
            }
        }

        private void showPortPanel(int startAddress)
        {
            int i = 0;
            int j;

            panelPort.Controls.Clear();

            //port panel update
            // we can view 128 Byte of memory at a time
            // it will be in form of 16 X 8


            //portAddressLabels
            // Display initial memory address labels from 0x2000 to 0x20ff 
            // 
            // example
            //          
            //  00
            //  08
            //  10
            //  ..
            //  ..
            //  78
            //
            //  

            for (i = 0; i <= 0xF; i++)
            {
                portAddressLabels[i] = new Label();
            }
            i = 0;
            j = startAddress;
            foreach (Label lbl in portAddressLabels)
            {
                lbl.Name = "portAddressLabel" + i.ToString("X");
                lbl.Text = j.ToString("X").PadLeft(2, '0');
                lbl.Visible = true;
                lbl.Size = new System.Drawing.Size(40, 15);
                lbl.Location = new Point(10, 20 + 20 * i);
                panelPort.Controls.Add(lbl);
                i++;
                j += 0x8;
            }

            //portAddressIndexLabels
            // Display the top row required for the port table
            //
            // example
            //
            //  0 1 2 3 4 5 6 7 
            //
            //

            for (i = 0; i <= 0x7; i++)
            {
                portAddressIndexLabels[i] = new Label();
            }
            i = 0;
            j = 0x0;
            foreach (Label lbl in portAddressIndexLabels)
            {
                lbl.Name = "portAddressIndexLabel" + i.ToString("X");
                lbl.Text = j.ToString("X");
                lbl.Visible = true;
                lbl.Size = new System.Drawing.Size(20, 15);
                lbl.Location = new Point(60 + 30 * i, 0);
                panelPort.Controls.Add(lbl);
                i++;
                j += 1;
            }

            //portTableLabels
            // Display the port contents
            //
            // example
            //
            //       0   1   2   3   4   5   6   7
            // 2000  00  00  00  00  00  00  00  00
            // 2008  01  03  FA  3F  BC  DD  23  48
            //

            for (i = 0; i <= 0xf; i++)
            {
                for (j = 0; j <= 0x7; j++)
                {
                    portTableLabels[i, j] = new Label();
                }
            }
            i = 0;
            j = 0x0;
            foreach (Label lbl in portTableLabels)
            {
                lbl.Name = "portTableLabel" + i.ToString("X");
                lbl.Text = a.PORT[startAddress + (8 * i) + j].ToString("X").PadLeft(2, '0');
                lbl.Visible = true;
                lbl.Size = new System.Drawing.Size(30, 15);
                lbl.Location = new Point(60 + 30 * j, 20 + 20 * i);
                panelPort.Controls.Add(lbl);
                j++;
                if (j == 8)
                {
                    j = 0;
                    i++;
                }
            }
        }
        private int getTextBoxMemoryStartAddress(string str)
        {                       // to get the memory start address from text box
            string txtval = textBoxMemoryStartAddress.Text;
            int n = 0;

            n = Convert.ToInt32(txtval, 16);    // convert HEX to INT

            if (n >= 0xFF80)
            {
                return (0xFF80);
            }
            else
            {
                return n;
            }
        }
        private void changeColorRTBLine(int line)
        {
            richTextBox1.SelectionStart = 0;
            richTextBox1.SelectionLength = richTextBox1.Text.Length;
            richTextBox1.SelectionBackColor = System.Drawing.Color.White;
            int firstcharindex = richTextBox1.GetFirstCharIndexFromLine(line);
            int currentline = richTextBox1.GetLineFromCharIndex(firstcharindex);
            string currentlinetext = richTextBox1.Lines[currentline];
            richTextBox1.SelectionStart = firstcharindex;
            richTextBox1.SelectionLength = currentlinetext.Length;
            richTextBox1.SelectionBackColor = System.Drawing.Color.Red;
            //richTextBox1.Select(firstcharindex, currentlinetext.Length);
        }
        private void registerHoverBinary(Label l)
        {
            string binaryval;
            binaryval = Convert.ToString(Convert.ToInt32(l.Text, 16), 2);
            // change the HEX string to BINARY string
            binaryval = binaryval.PadLeft(8, '0');
            toolTipRegisterBinary.SetToolTip(l, binaryval);
            // show tooltip with string binaryval when we hover mouse over label l
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFD.Title = "Select Assembly File";
            openFD.InitialDirectory =
                System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFD.FileName = "";
            openFD.Filter = "8085 assembly|*.asm|All Files|*.*";

            if (openFD.ShowDialog() != DialogResult.Cancel)
            {
                chosen_file = openFD.FileName;
                //MessageBox.Show(chosen_file);
                System.IO.StreamReader asmprogramReader;
                asmprogramReader = new System.IO.StreamReader(chosen_file);
                richTextBox1.Text = asmprogramReader.ReadToEnd();
                asmprogramReader.Close();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (chosen_file == "")
            {
                saveAsFD.Title = "Save File As";
                saveAsFD.InitialDirectory =
                    System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                saveAsFD.FileName = "";
                saveAsFD.Filter = "8085 assembly|*.asm|All Files|*.*";

                if (saveAsFD.ShowDialog() != DialogResult.Cancel)
                {
                    chosen_file = saveAsFD.FileName;
                    System.IO.StreamWriter asmprogramWriter;
                    asmprogramWriter = new System.IO.StreamWriter(chosen_file);
                    asmprogramWriter.Write(richTextBox1.Text);
                    asmprogramWriter.Close();
                }
            }

            else
            {
                System.IO.StreamWriter asmprogramWriter;
                asmprogramWriter = new System.IO.StreamWriter(chosen_file);
                asmprogramWriter.Write(richTextBox1.Text);
                asmprogramWriter.Close();
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveAsFD.Title = "Save File As";
            saveAsFD.InitialDirectory =
                System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveAsFD.FileName = "";
            saveAsFD.Filter = "8085 assembly|*.asm|All Files|*.*";

            if (saveAsFD.ShowDialog() != DialogResult.Cancel)
            {
                chosen_file = saveAsFD.FileName;
                System.IO.StreamWriter asmprogramWriter;
                asmprogramWriter = new System.IO.StreamWriter(chosen_file);
                asmprogramWriter.Write(richTextBox1.Text);
                asmprogramWriter.Close();
            }
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void resetRAMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            a.clearRAM();
            showMemoryPanel(getTextBoxMemoryStartAddress(textBoxMemoryStartAddress.Text));
        }

        private void buttonAssemble_Click(object sender, EventArgs e)
        {
  
        }

        private void resetPortsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            a.clearPORT();
            showPortPanel(0x00);
        }

        private void resetSimulatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            programBackup = richTextBox1.Text;
            a = new Assembler85();
            showMemoryPanel(0x2000);
            showPortPanel(0x00);
            updateRegisters();
            updateFlags();
            toolStripButtonStartDebug.Visible = false;
            toolStripButtonStepIn.Visible = false;
            richTextBox1.Text = programBackup;
        }

        private void toolStripButtonOpen_Click(object sender, EventArgs e)
        {
            openFD.Title = "Select Assembly File";
            openFD.InitialDirectory =
                System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFD.FileName = "";
            openFD.Filter = "8085 assembly|*.asm|All Files|*.*";

            if (openFD.ShowDialog() != DialogResult.Cancel)
            {
                chosen_file = openFD.FileName;
                //MessageBox.Show(chosen_file);
                System.IO.StreamReader asmprogramReader;
                asmprogramReader = new System.IO.StreamReader(chosen_file);
                richTextBox1.Text = asmprogramReader.ReadToEnd();
                asmprogramReader.Close();
            }
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            if (chosen_file == "")
            {
                saveAsFD.Title = "Save File As";
                saveAsFD.InitialDirectory =
                    System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                saveAsFD.FileName = "";
                saveAsFD.Filter = "8085 assembly|*.asm|All Files|*.*";

                if (saveAsFD.ShowDialog() != DialogResult.Cancel)
                {
                    chosen_file = saveAsFD.FileName;
                    System.IO.StreamWriter asmprogramWriter;
                    asmprogramWriter = new System.IO.StreamWriter(chosen_file);
                    asmprogramWriter.Write(richTextBox1.Text);
                    asmprogramWriter.Close();
                }
            }

            else
            {
                System.IO.StreamWriter asmprogramWriter;
                asmprogramWriter = new System.IO.StreamWriter(chosen_file);
                asmprogramWriter.Write(richTextBox1.Text);
                asmprogramWriter.Close();
            }
        }

        private void toolStripButtonSaveAs_Click(object sender, EventArgs e)
        {
            saveAsFD.Title = "Save File As";
            saveAsFD.InitialDirectory =
                System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveAsFD.FileName = "";
            saveAsFD.Filter = "8085 assembly|*.asm|All Files|*.*";

            if (saveAsFD.ShowDialog() != DialogResult.Cancel)
            {
                chosen_file = saveAsFD.FileName;
                System.IO.StreamWriter asmprogramWriter;
                asmprogramWriter = new System.IO.StreamWriter(chosen_file);
                asmprogramWriter.Write(richTextBox1.Text);
                asmprogramWriter.Close();
            }
        }

        private void toolStripButtonRestartSimulator_Click(object sender, EventArgs e)
        {
            programBackup = richTextBox1.Text;
            a = new Assembler85();
            showMemoryPanel(0x2000);
            showPortPanel(0x00);
            updateRegisters();
            updateFlags();
            toolStripButtonStartDebug.Visible = false;
            toolStripButtonStepIn.Visible = false;
            richTextBox1.Clear();
            richTextBox1.Text = programBackup;
        }

        private void toolStripButtonStartDebug_Click(object sender, EventArgs e)
        {
            a = new Assembler85();
            nextInstrAddress = Convert.ToInt32(textBoxMemoryStartAddress.Text, 16);
            try
            {
                a.changeProgramLength = richTextBox1.Lines.Length;// get the number of lines of program
                a.changeStartLocation = Convert.ToInt32(textBoxMemoryStartAddress.Text, 16);
                // change start location of the program in RAM
                a.FirstPass(richTextBox1.Lines);    // run the first Pass of assembler
                usedRamSize = a.SecondPass();       // run second pass and store the used RAM
                showMemoryPanel(getTextBoxMemoryStartAddress(textBoxMemoryStartAddress.Text));
                // show updated memory
            }
            catch (AssemblerErrorException) { }
            nextInstrAddress = Convert.ToInt32(textBoxMemoryStartAddress.Text, 16);
            programBackup = richTextBox1.Text;
           // labelLineNumber.Visible = false;
           // labelColumnNumber.Visible = false;
            toolStripButtonStepIn.Visible = true;
            toolStripButtonStartDebug.Visible = false;
            changeColorRTBLine(a.RAMprogramLine[nextInstrAddress]);
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {

        }

        private void buttonCreateList_Click(object sender, EventArgs e)
        {
        }

        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            int index = richTextBox1.SelectionStart;        // get index of cursor in current program
            int line = richTextBox1.GetLineFromCharIndex(index);// get line number
           // labelLineNumber.Text = (line + 1).ToString();
            // line number is displayed  line [0] is 1ine 1                     

            int column = richTextBox1.SelectionStart - richTextBox1.GetFirstCharIndexFromLine(line);
            // labelColumnNumber.Text = (column + 1).ToString();
      
        }

        private void labelARegister_MouseHover(object sender, EventArgs e)
        {
            registerHoverBinary(labelARegister);
        }

        private void labelBRegister_MouseHover(object sender, EventArgs e)
        {
            registerHoverBinary(labelBRegister);
        }

        private void labelCRegister_MouseHover(object sender, EventArgs e)
        {
            registerHoverBinary(labelCRegister);
        }

        private void labelDRegister_MouseHover(object sender, EventArgs e)
        {
            registerHoverBinary(labelDRegister);
        }

        private void labelERegister_MouseHover(object sender, EventArgs e)
        {
            registerHoverBinary(labelERegister);
        }

        private void labelHRegister_MouseHover(object sender, EventArgs e)
        {
            registerHoverBinary(labelHRegister);
        }

        private void labelLRegister_MouseHover(object sender, EventArgs e)
        {
            registerHoverBinary(labelLRegister);
        }

        private void labelPCRegister_MouseHover(object sender, EventArgs e)
        {
            registerHoverBinary(labelPCRegister);
        }

        private void labelSPRegister_MouseHover(object sender, EventArgs e)
        {
            registerHoverBinary(labelSPRegister);
        }

        private void buttonMemoryStartAddress_Click(object sender, EventArgs e)
        {
            showMemoryPanel(getTextBoxMemoryStartAddress(textBoxMemoryStartAddress.Text));

        }

        private void buttonClearRAM_Click(object sender, EventArgs e)
        {
            a.clearRAM();
            showMemoryPanel(getTextBoxMemoryStartAddress(textBoxMemoryStartAddress.Text));
        }

        private void buttonPrevPage_Click(object sender, EventArgs e)
        {
            int n = getTextBoxMemoryStartAddress(textBoxMemoryStartAddress.Text);

            if (n - 0x80 < 0x0000)
                return;
            textBoxMemoryStartAddress.Text = (n - 0x80).ToString("X");
            showMemoryPanel(getTextBoxMemoryStartAddress(textBoxMemoryStartAddress.Text));
        }

        private void buttonNextPage_Click(object sender, EventArgs e)
        {
            int n = getTextBoxMemoryStartAddress(textBoxMemoryStartAddress.Text);

            if (n + 0x80 > 0xFFFF)
                return;
            textBoxMemoryStartAddress.Text = (n + 0x80).ToString("X");
            showMemoryPanel(getTextBoxMemoryStartAddress(textBoxMemoryStartAddress.Text));
        }

        private void buttonMemoryUpdate_Click(object sender, EventArgs e)
        {
            a.RAM[(int)numericUpDown1.Value] = Convert.ToByte(textBoxMemoryUpdateByte.Text, 16);
            // convert numericUpDown1(HEX) to int, convert string from textbox  
            // to BYTE
            if ((((int)numericUpDown1.Value)
                >= getTextBoxMemoryStartAddress(textBoxMemoryStartAddress.Text))
                && (((int)numericUpDown1.Value)
                <= getTextBoxMemoryStartAddress(textBoxMemoryStartAddress.Text) + 0x7F))
            {
                showMemoryPanel(getTextBoxMemoryStartAddress(textBoxMemoryStartAddress.Text));
            }
        }

        private void buttonClearPORT_Click(object sender, EventArgs e)
        {
            a.clearPORT();
            showPortPanel(0x00);
        }

        private void buttonPortUpdate_Click(object sender, EventArgs e)
        {
            a.PORT[(int)numericUpDown2.Value] = Convert.ToByte(textBoxPortUpdateByte.Text, 16);

            if (numericUpDown2.Value <= 0x7F)
                showPortPanel(0x00);
            else
                showPortPanel(0x80);
        }

        private void buttonPrevPortPage_Click(object sender, EventArgs e)
        {
            showPortPanel(0x00);
        }

        private void buttonNextPortPage_Click(object sender, EventArgs e)
        {
            showPortPanel(0x80);
        }

        private void toolStripButtonStepIn_Click(object sender, EventArgs e)
        {
            if (nextInstrAddress != 0xffff)
            {
                nextInstrAddress = a.runProgram(nextInstrAddress, 1);
                updateRegisters();  // update register labels
                updateFlags();      // update flag labels
                if (nextInstrAddress != 0xffff && nextInstrAddress <= 0xffff)
                    changeColorRTBLine(a.RAMprogramLine[nextInstrAddress]);
                if (richTextBox1.SelectionStart == 0)
                {
                    a = new Assembler85();
                    nextInstrAddress = Convert.ToInt32(textBoxMemoryStartAddress.Text, 16);
                    // set the start address for debugger
                    toolStripButtonStartDebug.Visible = true;   // make the Start Debug button Visible
                    try
                    {
                        a.changeProgramLength = richTextBox1.Lines.Length;// get the number of lines of program
                        a.changeStartLocation = Convert.ToInt32(textBoxMemoryStartAddress.Text, 16);
                        // change start location of the program in RAM
                        a.FirstPass(richTextBox1.Lines);    // run the first Pass of assembler
                        usedRamSize = a.SecondPass();       // run second pass and store the used RAM
                        showMemoryPanel(getTextBoxMemoryStartAddress(textBoxMemoryStartAddress.Text));
                        // show updated memory
                    }
                    catch (AssemblerErrorException) { }
                    toolStripButtonStepIn.Visible = false;
                    MessageBox.Show(a.createListProgram(richTextBox1.Lines));

                }
            }
            else
            {
              //  labelLineNumber.Visible = true;
              //  labelColumnNumber.Visible = true;
                MessageBox.Show("Debugging Over");
                richTextBox1.Clear();
                richTextBox1.Text = programBackup;
                toolStripButtonStepIn.Visible = false;
                toolStripButtonStartDebug.Visible = true;
            }
       
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                a.clearRAM();
                a = new Assembler85();
                nextInstrAddress = Convert.ToInt32(textBoxMemoryStartAddress.Text, 16);
                // set the start address for debugger
                toolStripButtonStartDebug.Visible = true;   // make the Start Debug button Visible
                try
                {
                    a.changeProgramLength = richTextBox1.Lines.Length;// get the number of lines of program
                    a.changeStartLocation = Convert.ToInt32(textBoxMemoryStartAddress.Text, 16);
                    // change start location of the program in RAM
                    a.FirstPass(richTextBox1.Lines);    // run the first Pass of assembler
                    usedRamSize = a.SecondPass();       // run second pass and store the used RAM
                    showMemoryPanel(getTextBoxMemoryStartAddress(textBoxMemoryStartAddress.Text));
                    // show updated memory
                }
                catch (AssemblerErrorException) { }
                a.runProgram((Convert.ToInt32(textBoxMemoryStartAddress.Text, 16)), usedRamSize);
                // run the program 
                // program will run correctly because
                // usedRamSize is always greater than instructions in RAM
                showMemoryPanel(Convert.ToInt32(textBoxMemoryStartAddress.Text, 16));
                showPortPanel(0x00);
                updateRegisters();  // update register labels
                updateFlags();      // update falg labels
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (richTextBox1.Text == "" && richTextBox1.SelectionStart == 0)
            {
                toolStripButtonStartDebug.Visible = false;
            }
            else
            toolStripButtonStartDebug.Visible = true;
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            string command = "";
            command =  Interaction.InputBox("Copy from the source (Sc) to the destination(Dt)", "MOV Command", "MOV A,B");
            if(command != "")
            {
                if(richTextBox1.SelectionStart == 0)
                richTextBox1.AppendText(command);
                else
                richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "MOV Command");
            }
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            string command = "";
            command = Interaction.InputBox("Move immediate 8-bit", "MVI Command", "MVI A,05H");
            if (command != "")
            {
                if (richTextBox1.SelectionStart == 0)
                    richTextBox1.AppendText(command);
                else
                    richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "MOV Command");
            }
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            string command = "";
            command = Interaction.InputBox("Load the accumulato", "LDA Command", "LDA 30FH");
            if (command != "")
            {
                if (richTextBox1.SelectionStart == 0)
                    richTextBox1.AppendText(command);
                else
                    richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "LDA Command");
            }
        }

        private void metroButton4_Click(object sender, EventArgs e)
        {
            string command = "";
            command = Interaction.InputBox("Load the accumulator indirect", "LDAX Command", "LDAX B");
            if (command != "")
            {
                if (richTextBox1.SelectionStart == 0)
                    richTextBox1.AppendText(command);
                else
                    richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "LDA Command");
            }
        }

        private void metroButton5_Click(object sender, EventArgs e)
        {
            string command = "";
            command = Interaction.InputBox("Load the register pair immediate", "LXI Command", "LXI B,25H");
            if (command != "")
            {
                if (richTextBox1.SelectionStart == 0)
                    richTextBox1.AppendText(command);
                else
                    richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "LXI Command");
            }
        }

        private void metroButton6_Click(object sender, EventArgs e)
        {
            string command = "";
            command = Interaction.InputBox("Load H and L registers direct", "LHLD Command", "LHLD 35H");
            if (command != "")
            {
                if (richTextBox1.SelectionStart == 0)
                    richTextBox1.AppendText(command);
                else
                    richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "LXI Command");
            }
        }

        private void metroButton7_Click(object sender, EventArgs e)
        {
            string command = "";
            command = Interaction.InputBox("16-bit address", "STA Command", "STA 35H");
            if (command != "")
            {
                if (richTextBox1.SelectionStart == 0)
                    richTextBox1.AppendText(command);
                else
                    richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "STA Command");
            }
        }

        private void metroButton8_Click(object sender, EventArgs e)
        {
            string command = "";
            command = Interaction.InputBox("Store the accumulator indirect", "STAX Command", "STAX B");
            if (command != "")
            {
                if (richTextBox1.SelectionStart == 0)
                    richTextBox1.AppendText(command);
                else
                    richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "STAX Command");
            }
        }

        private void metroButton9_Click(object sender, EventArgs e)
        {
            string command = "";
            command = Interaction.InputBox("Store H and L registers direct", "SHLD Command", "SHLD 35H");
            if (command != "")
            {
                if (richTextBox1.SelectionStart == 0)
                    richTextBox1.AppendText(command);
                else
                    richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "SHLD Command");
            }
        }

        private void metroButton10_Click(object sender, EventArgs e)
        {
            string command = "";
            command = Interaction.InputBox("Exchange H and L with D and E", "XCHG Command", "XCHG");
            if (command != "")
            {
                if (richTextBox1.SelectionStart == 0)
                    richTextBox1.AppendText(command);
                else
                    richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "XCHG Command");
            }
        }

        private void metroButton11_Click(object sender, EventArgs e)
        {
            string command = "";
            command = Interaction.InputBox("Copy H and L registers to the stack pointer", "SPHL Command", "SPHL");
            if (command != "")
            {
                if (richTextBox1.SelectionStart == 0)
                    richTextBox1.AppendText(command);
                else
                    richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "SPHL");
            }
        }

        private void metroButton12_Click(object sender, EventArgs e)
        {
            string command = "";
            command = Interaction.InputBox("Exchange H and L with top of stack", "XTHL Command", "XTHL");
            if (command != "")
            {
                if (richTextBox1.SelectionStart == 0)
                    richTextBox1.AppendText(command);
                else
                    richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "XTHL");
            }
        }

        private void metroButton13_Click(object sender, EventArgs e)
        {
            string command = "";
            command = Interaction.InputBox("Push the register pair onto the stack", "PUSH Command", "PUSH B");
            if (command != "")
            {
                if (richTextBox1.SelectionStart == 0)
                    richTextBox1.AppendText(command);
                else
                    richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "PUSH");
            }
        }

        private void metroButton14_Click(object sender, EventArgs e)
        {
            string command = "";
            command = Interaction.InputBox("Pop off stack to the register pair", "POP Command", "POP B");
            if (command != "")
            {
                if (richTextBox1.SelectionStart == 0)
                    richTextBox1.AppendText(command);
                else
                    richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "POP");
            }
        }

        private void metroButton15_Click(object sender, EventArgs e)
        {
            string command = "";
            command = Interaction.InputBox("Output the data from the accumulator to a port with 8bit address", "OUT Command", "OUT 01H");
            if (command != "")
            {
                if (richTextBox1.SelectionStart == 0)
                    richTextBox1.AppendText(command);
                else
                    richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "OUT");
            }
        }

        private void metroButton16_Click(object sender, EventArgs e)
        {
            string command = "";
            command = Interaction.InputBox("Input data to accumulator from a port with 8-bit address", "IN Command", "IN 02H");
            if (command != "")
            {
                if (richTextBox1.SelectionStart == 0)
                    richTextBox1.AppendText(command);
                else
                    richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "IN");
            }
        }

        private void metroButton17_Click(object sender, EventArgs e)
        {
            string command = "";
            command = Interaction.InputBox("Add to Accumulator", "ADD Command", "ADD B");
            if (command != "")
            {
                if (richTextBox1.SelectionStart == 0)
                    richTextBox1.AppendText(command);
                else
                    richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "ADD");
            }
        }

        private void metroButton18_Click(object sender, EventArgs e)
        {
            string command = "";
            command = Interaction.InputBox("Exit program", "HLT Command", "HLT");
            if (command != "")
            {
                if (richTextBox1.SelectionStart == 0)
                    richTextBox1.AppendText(command);
                else
                    richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "HLT");
            }
        }

        private void metroButton19_Click(object sender, EventArgs e)
        {
            string command = "";
            command = Interaction.InputBox("INX is a mnemonic that stands for “INcrementeXtended register” and rp stands for register pair", "INX  Command", "INX B");
            if (command != "")
            {
                if (richTextBox1.SelectionStart == 0)
                    richTextBox1.AppendText(command);
                else
                    richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "INX");
            }
        }

        private void metroButton20_Click(object sender, EventArgs e)
        {
            string command = "";
            command = Interaction.InputBox("Compare number with next number", "CMP  Command", "CMP B");
            if (command != "")
            {
                if (richTextBox1.SelectionStart == 0)
                    richTextBox1.AppendText(command);
                else
                    richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "CMP");
            }
        }

        private void metroButton21_Click(object sender, EventArgs e)
        {
            string command = "";
            command = Interaction.InputBox("Add a 8-bit number 32H to Accumulator", "ADI Command", "ADI 32H");
            if (command != "")
            {
                if (richTextBox1.SelectionStart == 0)
                    richTextBox1.AppendText(command);
                else
                    richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "ADI");
            }
        }

        private void metroButton22_Click(object sender, EventArgs e)
        {
            string command = "";
            command = Interaction.InputBox("Subtract a 8-bit number 32H from Accumulator", "SUI Command", "SUI 32H");
            if (command != "")
            {
                if (richTextBox1.SelectionStart == 0)
                    richTextBox1.AppendText(command);
                else
                    richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "SUI");
            }
        }

        private void metroButton23_Click(object sender, EventArgs e)
        {
            string command = "";
            command = Interaction.InputBox("Subtract contents of Register  from Accumulator", "SUB Command", "SUB C");
            if (command != "")
            {
                if (richTextBox1.SelectionStart == 0)
                    richTextBox1.AppendText(command);
                else
                    richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "SUB");
            }
        }

        private void metroButton24_Click(object sender, EventArgs e)
        {
            string command = "";
            command = Interaction.InputBox("Increment the contents of Register  by 1", "INR Command", "INR D");
            if (command != "")
            {
                if (richTextBox1.SelectionStart == 0)
                    richTextBox1.AppendText(command);
                else
                    richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "INR");
            }
        }

        private void metroButton25_Click(object sender, EventArgs e)
        {
            string command = "";
            command = Interaction.InputBox("Decrement the contents of Register by 1", "DCR Command", "DCR E");
            if (command != "")
            {
                if (richTextBox1.SelectionStart == 0)
                    richTextBox1.AppendText(command);
                else
                    richTextBox1.AppendText(Environment.NewLine + command);
                Interaction.MsgBox("Added " + command.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "DCR");
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            a = new Assembler85();
            showMemoryPanel(0x2000);
            showPortPanel(0x00);
            updateRegisters();
            updateFlags();
            toolStripButtonStartDebug.Visible = false;
            toolStripButtonStepIn.Visible = false;
            richTextBox1.Clear();
        }

        private void newFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            a = new Assembler85();
            showMemoryPanel(0x2000);
            showPortPanel(0x00);
            updateRegisters();
            updateFlags();
            toolStripButtonStartDebug.Visible = false;
            toolStripButtonStepIn.Visible = false;
            richTextBox1.Clear();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
    }
}
