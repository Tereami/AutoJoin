using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoJoin
{
    public partial class FormSetJoinOrder : Form
    {
        public List<MyCategory> Cats;
        public FormSetJoinOrder(List<MyCategory> cats)
        {
            InitializeComponent();
            
            foreach (MyCategory mc in cats)
            {
                listBox1.Items.Add(mc);
            }
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            listBox1.DoDragDrop(listBox1.SelectedItem, DragDropEffects.Move);
        }

        private void listBox1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            Point p = listBox1.PointToClient(new Point(e.X, e.Y));
            int index = listBox1.IndexFromPoint(p);
            if (index < 0)
                index = listBox1.Items.Count - 1;

            //var data = e.Data.GetData(typeof(MyCategory));
            var data = listBox1.SelectedItem;
            listBox1.Items.Remove(data);
            listBox1.Items.Insert(index, data);
            listBox1.SelectedIndex = index;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Cats = new List<MyCategory>();
            for(int i = 0; i < listBox1.Items.Count; i++)
            {
                MyCategory mc = listBox1.Items[i] as MyCategory;
                mc.priority = i;
                Cats.Add(mc);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
