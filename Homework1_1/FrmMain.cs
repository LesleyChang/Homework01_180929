using AWModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsControlLibrary1;

namespace Homework1_1
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
            this.tabControl1.SelectedIndex = 1;
        }
        AdventureWorksEntities dbContext = new AdventureWorksEntities();

        #region TabPage1_在flowLayoutPanel間移動
        private void myButton1_Click(object sender, EventArgs e)
        {
            var q = dbContext.ProductPhotoes;
            List<ProductPhoto> productList = q.ToList();
            this.flowLayoutPanel1.Controls.Clear();
            this.flowLayoutPanel2.Controls.Clear();
            for (int i = 0; i < productList.Count; i++)
            {
                MyItemTemplate photoItem = new MyItemTemplate()
                {
                    Desc = productList[i].ModifiedDate.ToString(),
                    ImageByte = productList[i].LargePhoto
                };
                this.flowLayoutPanel1.Controls.Add(photoItem);
                photoItem.Index = this.flowLayoutPanel1.Controls.GetChildIndex(photoItem);
                photoItem.MyCheckedChange += PhotoItem_MyCheckedChange;
                Application.DoEvents();
            }
        }

        private void PhotoItem_MyCheckedChange(object sender, EventArgs e)
        {
            MyItemTemplate item = sender as MyItemTemplate;
            if (item.Checked)
            {
                this.flowLayoutPanel2.Controls.Add(item);
            }
            else
            {
                this.flowLayoutPanel2.Controls.Remove(item);
                this.flowLayoutPanel1.Controls.Add(item);
                this.flowLayoutPanel1.Controls.SetChildIndex(item, item.Index);
            }
        }
        #endregion

        #region TabPage2_將flowLayoutPanel3指定物件'複製'到flowLayoutPanel4
        private void myButton2_Click(object sender, EventArgs e)
        {
            var q = dbContext.ProductPhotoes;
            List<ProductPhoto> productList = q.ToList();
            this.flowLayoutPanel3.Controls.Clear();
            this.flowLayoutPanel4.Controls.Clear();
            for (int i = 0; i < productList.Count; i++)
            {
                MyItemTemplate photoItem = new MyItemTemplate() {
                    Desc = productList[i].ModifiedDate.ToString(),
                    ImageByte = productList[i].LargePhoto
                };
                this.flowLayoutPanel3.Controls.Add(photoItem);
                photoItem.Index = this.flowLayoutPanel3.Controls.GetChildIndex(photoItem);
                photoItem.MyCheckedChange += PhotoItemCopy_MyCheckedChange;
            }
        }

        private void PhotoItemCopy_MyCheckedChange(object sender, EventArgs e)
        {
            MyItemTemplate item = sender as MyItemTemplate;
            if (item.Checked)
            {
                MyCopyItem copyItem = new MyCopyItem(item);
                copyItem.ParentIndex = item.Index;
                copyItem.Checked = true;
                this.flowLayoutPanel4.Controls.Add(copyItem);
                copyItem.Index = this.flowLayoutPanel4.Controls.GetChildIndex(copyItem);
                copyItem.MyCheckedChange += CopyItem_MyCheckedChange;
            }
            else
            {
                if(item is MyItemTemplate)
                {
                    foreach (MyCopyItem copyItem in flowLayoutPanel4.Controls)
                    {
                        if(copyItem.ParentIndex == item.Index)
                        {
                            this.flowLayoutPanel4.Controls.Remove(copyItem);
                        }
                    }
                    
                    return;
                }
                
            }
        }

        private void CopyItem_MyCheckedChange(object sender, EventArgs e)
        {
            MyCopyItem copyItem = sender as MyCopyItem;
            if (!copyItem.Checked)
            {
                this.flowLayoutPanel4.Controls.Remove(copyItem);
                foreach (MyItemTemplate parentItem in flowLayoutPanel3.Controls)
                {
                    if(parentItem.Index == copyItem.ParentIndex)
                    {
                        parentItem.Checked = false;
                    }
                }
            }
        }

        #endregion


    }
    class MyCopyItem : MyItemTemplate
    {
        public MyCopyItem(MyItemTemplate parentItem) : base()
        {
            this.ImageByte = parentItem.ImageByte;
            this.Desc = parentItem.Desc;
        }
        public int ParentIndex { get; set; }
    }
}
